using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic;

namespace pbkdf_basic_demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Crypto crypto = new Crypto();
        // 20 bytes (160 bits) salt, true random numbers taken from https://www.random.org/
        // Note: PBKDF2 standard recommends a minimum salt of 64 bits
        private byte[] salt = HexStringToByteArray("f2c9e2ef3d7b440faf63613f853701cc7aa205f7");
        private byte[] keyDerivedFromPassword;

        public static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length / 2)
                             .Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16))
                             .ToArray();
        }

        public MainWindow()
        {
            InitializeComponent();

            using (var db = new DbCtx())
            {
                foreach(var p in db.Persons)
                {
                    lb_persons.Items.Add(p);
                }
            }
        }

        private void lb_persons_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lb_persons.SelectedItem == null)
            {
                MessageBox.Show("No person selected.");
                return;
            }

            int w = Convert.ToInt16(System.Windows.SystemParameters.PrimaryScreenWidth);
            int h = Convert.ToInt16(System.Windows.SystemParameters.PrimaryScreenHeight);

            string pwdInput = Interaction.InputBox("Enter password to decrypt", "Title", "", w/2, h/2);

            keyDerivedFromPassword = crypto.GenerateKeyFromPassword(pwdInput, salt);

            using (var db = new DbCtx())
            {
                var p = (Person)lb_persons.SelectedItem;
                byte[] emailBytes;

                try
                {
                    emailBytes = crypto.Decrypt(p.Email, keyDerivedFromPassword, p.Iv);
                }
                catch(Exception)
                {
                    MessageBox.Show("Decryption failed");
                    return;
                }

                string email = Encoding.UTF8.GetString(emailBytes);

                MessageBox.Show($"Decrypted email: {email}");
            }

        }

        private void btn_UsePassword_Click(object sender, RoutedEventArgs e)
        {
            keyDerivedFromPassword = crypto.GenerateKeyFromPassword(tbx_pwd.Text, salt);
            tb_keyhash.Text = Convert.ToHexString(keyDerivedFromPassword);
        }

        private void btn_AddUser_Click(object sender, RoutedEventArgs e)
        {
            if(keyDerivedFromPassword == null)
            {
                MessageBox.Show("Please set a password for encryption/decryption.");
                return;
            }
            // encrypting email-address with the derived key
            (byte[] cipher, byte[] key, byte[] IV) aes = crypto.Encrypt(Encoding.UTF8.GetBytes(tb_email.Text), keyDerivedFromPassword);

            // add person to database and listbox
            using (var db = new DbCtx())
            {
                Person newPerson = new Person(tb_naam.Text, aes.cipher, aes.IV);
                db.Add(newPerson);
                db.SaveChanges();
                lb_persons.Items.Add(newPerson);
            }
        }
    }
}
