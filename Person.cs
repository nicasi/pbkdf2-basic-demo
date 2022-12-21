using System;
using System.Collections.Generic;

namespace pbkdf_basic_demo;

public partial class Person
{
    public long Id { get; set; }

    public string? Naam { get; set; }

    public byte[]? Email { get; set; }

    public byte[]? Iv { get; set; }

    public Person(string? naam, byte[]? email, byte[] iv)
    {
        Naam = naam;
        Email = email;
        Iv = iv;
    }

    public override string ToString()
    {
        return $"{Naam}";
    }
}
