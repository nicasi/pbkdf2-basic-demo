using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace pbkdf_basic_demo;

public partial class DbCtx : DbContext
{
    public DbCtx()
    {
    }

    public DbCtx(DbContextOptions<DbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Person> Persons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("DataSource=db.sqlite");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("persons");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Iv).HasColumnName("IV");
            entity.Property(e => e.Naam).HasColumnName("naam");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
