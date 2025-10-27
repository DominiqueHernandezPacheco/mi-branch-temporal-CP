using System;
using System.Collections.Generic;
using Application.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Core.Infraestructure.Persistance;

public partial class LoginDefaultDbContext : DbContext
{
    public LoginDefaultDbContext()
    {
    }

    public LoginDefaultDbContext(DbContextOptions<LoginDefaultDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Modulo> CatModulos { get; set; }

    public virtual DbSet<Opcion> CatOpciones { get; set; }

    public virtual DbSet<OpcionRol> CatOpcionesRols { get; set; }

    public virtual DbSet<Rol> CatRoles { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }
   


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Modulo>(entity =>
        {
            entity.HasKey(e => e.IdModulo);

            entity.ToTable("Cat_Modulos");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Icono)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Opcion>(entity =>
        {
            entity.HasKey(e => e.IdOpcion).HasName("PK_Cat_OpcionesModulo");

            entity.ToTable("Cat_Opciones");

            entity.HasIndex(e => e.IdModulo, "IX_Cat_OpcionesModulo_IdModulo");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Icono)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.IdModuloNavigation).WithMany(p => p.CatOpciones)
                .HasForeignKey(d => d.IdModulo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cat_Opciones_Cat_Modulos");
        });

        modelBuilder.Entity<OpcionRol>(entity =>
        {
            entity.HasKey(e => e.IdOpcionRol).HasName("PK_OpcionesRol");

            entity.ToTable("Cat_OpcionesRol");

            entity.HasIndex(e => e.IdRol, "IX_OpcionesRol_IdRol");

            entity.HasOne(d => d.IdOpcionNavigation).WithMany(p => p.CatOpcionesRols)
                .HasForeignKey(d => d.IdOpcion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cat_OpcionesRol_Cat_Opciones");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.CatOpcionesRols)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cat_OpcionesRol_Cat_Roles");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol);

            entity.ToTable("Cat_Roles");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });


        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__5B65BF97E14C2783");

            entity.ToTable("Usuarios");
            entity.Ignore(e => e.Token);
            entity.Ignore(e => e.TokenLlave);

            entity.HasIndex(e => e.IdRol, "IX_Usuario_Rol");

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Cat_Roles");

   
        });
       

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}