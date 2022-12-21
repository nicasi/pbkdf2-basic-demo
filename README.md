# pbkdf2-basic-demo

Basic demonstration of the use of PBKDF2 to generate an AES key. The program uses Rfc2898DeriveBytes.Pbkdf2() to generate the key. The key is then used to AES-encrypt/decrypt the email of a user in an SQLite database.

The AES key is never persistently stored.
