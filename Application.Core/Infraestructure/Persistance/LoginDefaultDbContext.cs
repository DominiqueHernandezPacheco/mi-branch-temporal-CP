using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Application.Core.Domain.Entities;
namespace Application.Core.Infraestructure.Persistance;

public partial class LoginDefaultDbContext : DbContext
{
    public LoginDefaultDbContext() { }

    public LoginDefaultDbContext(DbContextOptions<LoginDefaultDbContext> options)
        : base(options) { }

    // =========================================================
    // 1. DBSETS CON LOS NOMBRES QUE TU APLICACIÓN ESPERA
    // =========================================================
    public virtual DbSet<Usuario> Usuarios { get; set; }       // Mapeado a SYS_USUARIOS
    public virtual DbSet<Rol> CatRoles { get; set; }           // Mapeado a SYS_ROLES
    public virtual DbSet<Opcion> CatOpciones { get; set; }     // Mapeado a SYS_PERMISOS
    public virtual DbSet<OpcionRol> CatOpcionesRols { get; set; } // Mapeado a REL_ROLES_PERMISOS
    //public virtual DbSet<Modulo> CatModulos { get; set; }      // Tabla "Dummy" para que el menú funcione

    // Tablas adicionales de tu BD (Geografía, Logs, etc.)
    public virtual DbSet<CatCiudade> CatCiudades { get; set; }
    public virtual DbSet<CatEstado> CatEstados { get; set; }
    public virtual DbSet<CatMunicipio> CatMunicipios { get; set; }
    public virtual DbSet<CatTiposAsentamiento> CatTiposAsentamientos { get; set; }
    public virtual DbSet<CpAsentamiento> CpAsentamientos { get; set; }
    public virtual DbSet<SysRegistroModificacione> SysRegistroModificaciones { get; set; }
    public virtual DbSet<SysRegistroModificacionesJson> SysRegistroModificacionesJsons { get; set; }
    public virtual DbSet<TmpCarga> TmpCargas { get; set; }
    public virtual DbSet<VwPermisosUsuario> VwPermisosUsuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
             // Usa cadena de conexión segura
             optionsBuilder.UseSqlServer("Data Source=DOMINIQUEPC\\SQLEXPRESS;Initial Catalog=GestionCodigosPostales;Integrated Security=True;Encrypt=False;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // =========================================================
        // 2. MAPEO: CLASES DE APP -> TABLAS REALES DE BD
        // =========================================================

        // --- USUARIOS ---
       modelBuilder.Entity<Usuario>(entity =>
{
    entity.ToTable("SYS_USUARIOS");
    entity.HasKey(e => e.IdUsuario);

    entity.Property(e => e.IdUsuario).HasColumnName("ID_Usuarios");
    
    // --- ESTA ES LA LÍNEA QUE TE FALTA ---
    entity.Property(e => e.NombreUsuario).HasColumnName("Nombre_Usuario"); 
    // -------------------------------------

    entity.Property(e => e.IdUsuarioLlave).HasColumnName("Identificador_Externo");
    entity.Property(e => e.IdRol).HasColumnName("FK_Rol");
    entity.Property(e => e.Activo).HasColumnName("Estatus");

    entity.Ignore(e => e.Token);
    entity.Ignore(e => e.TokenLlave);

    entity.HasOne(d => d.Rol)
        .WithMany(p => p.Usuarios)
        .HasForeignKey(d => d.IdRol)
        .HasConstraintName("FK_Usuario_Rol");
});

        // --- ROLES ---
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("SYS_ROLES");
            entity.HasKey(e => e.IdRol); // Tu clase debe tener IdRol

            entity.Property(e => e.IdRol).HasColumnName("PK_ID_Rol");
            entity.Property(e => e.Nombre).HasColumnName("Nombre_Rol");
            entity.Property(e => e.Descripcion).HasColumnName("Descripcion");
            entity.Property(e => e.Activo).HasColumnName("Estatus");
        });

        // --- OPCIONES (PERMISOS) ---
        modelBuilder.Entity<Opcion>(entity =>
        {
            entity.ToTable("SYS_PERMISOS");
            entity.HasKey(e => e.IdOpcion); // Tu clase debe tener IdOpcion

            entity.Property(e => e.IdOpcion).HasColumnName("PK_ID_Permiso");
            entity.Property(e => e.Nombre).HasColumnName("Clave_Permiso"); // O "Descripcion" según prefieras
            entity.Property(e => e.Descripcion).HasColumnName("Descripcion");
            // Mapea otras propiedades si tu clase Opcion las tiene
        });

        // --- OPCION ROL (INTERMEDIA) ---
        modelBuilder.Entity<OpcionRol>(entity =>
        {
            entity.ToTable("REL_ROLES_PERMISOS");
            entity.HasKey(e => new { e.IdRol, e.IdOpcion });

            entity.Property(e => e.IdRol).HasColumnName("FK_ID_Rol");
            entity.Property(e => e.IdOpcion).HasColumnName("FK_ID_Permiso");
            
            entity.Ignore(e => e.IdOpcionRol); // Si la clase lo tiene pero la tabla no
            entity.Ignore(e => e.Activo);      // Si la tabla no tiene columna Activo

            entity.HasOne(d => d.IdRolNavigation)
                .WithMany(p => p.CatOpcionesRols)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK_Rel_Rol");

            entity.HasOne(d => d.IdOpcionNavigation)
                .WithMany(p => p.CatOpcionesRols)
                .HasForeignKey(d => d.IdOpcion)
                .HasConstraintName("FK_Rel_Permiso");
        });

        // =========================================================
        // 3. TABLAS EXISTENTES EN BD PERO NO EN LA APLICACIÓN
        // =========================================================
        
        modelBuilder.Entity<CatCiudade>(entity =>
        {
            entity.HasKey(e => e.IdCiudad).HasName("PK__CAT_CIUD__05C00C1CC83EF652");
            entity.ToTable("CAT_CIUDADES");
            entity.HasIndex(e => e.FkEstado, "IX_Ciudad_Estado");
            entity.Property(e => e.IdCiudad).ValueGeneratedNever().HasColumnName("ID_Ciudad");
            entity.Property(e => e.FkEstado).HasColumnName("FK_Estado");
            entity.Property(e => e.NombreCiudad).HasMaxLength(100).IsUnicode(false).HasColumnName("Nombre_Ciudad");
            entity.Property(e => e.TipoZona).HasMaxLength(10).IsUnicode(false).IsFixedLength().HasColumnName("Tipo_Zona");
            entity.HasOne(d => d.FkEstadoNavigation).WithMany(p => p.CatCiudades).HasForeignKey(d => d.FkEstado).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Ciudad_Estado");
        });

        modelBuilder.Entity<CatEstado>(entity =>
        {
            entity.HasKey(e => e.IdEstado).HasName("PK__CAT_ESTA__9CF493954117F800");
            entity.ToTable("CAT_ESTADO");
            entity.HasIndex(e => e.NombreEstado, "UQ_Nombre_Estado").IsUnique();
            entity.Property(e => e.IdEstado).ValueGeneratedNever().HasColumnName("ID_Estado");
            entity.Property(e => e.NombreEstado).HasMaxLength(50).IsUnicode(false).HasColumnName("Nombre_Estado");
        });

        modelBuilder.Entity<CatMunicipio>(entity =>
        {
            entity.HasKey(e => e.IdMunicipio).HasName("PK__CAT_MUNI__ED00F5B5AAC42007");
            entity.ToTable("CAT_MUNICIPIO");
            entity.HasIndex(e => e.FkEstado, "IX_Municipio_Estado");
            entity.Property(e => e.IdMunicipio).ValueGeneratedNever().HasColumnName("ID_Municipio");
            entity.Property(e => e.FkEstado).HasColumnName("FK_Estado");
            entity.Property(e => e.NombreMunicipio).HasMaxLength(100).IsUnicode(false).HasColumnName("Nombre_Municipio");
            entity.HasOne(d => d.FkEstadoNavigation).WithMany(p => p.CatMunicipios).HasForeignKey(d => d.FkEstado).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Mun_Estado");
        });

        modelBuilder.Entity<CatTiposAsentamiento>(entity =>
        {
            entity.HasKey(e => e.IdTipoAsentamiento).HasName("PK__CAT_TIPO__0DC193B5FED6913D");
            entity.ToTable("CAT_TIPOS_ASENTAMIENTO");
            entity.Property(e => e.IdTipoAsentamiento).ValueGeneratedNever().HasColumnName("ID_TipoAsentamiento");
            entity.Property(e => e.NombreTipoAsentamiento).HasMaxLength(50).IsUnicode(false).HasColumnName("Nombre_TipoAsentamiento");
        });

        modelBuilder.Entity<CpAsentamiento>(entity =>
        {
            entity.HasKey(e => e.IdAsentamiento).HasName("PK__CP_ASENT__3B52B857CAD0BAF0");
            entity.ToTable("CP_ASENTAMIENTOS");
            entity.HasIndex(e => e.CodigoPostal, "IX_CP_Codigo");
            entity.HasIndex(e => e.NombreAsentamiento, "IX_CP_Nombre");
            entity.Property(e => e.IdAsentamiento).HasColumnName("ID_Asentamiento");
            entity.Property(e => e.COficina).HasMaxLength(5).IsUnicode(false).IsFixedLength().HasColumnName("C_Oficina");
            entity.Property(e => e.CodigoPostal).HasMaxLength(5).IsUnicode(false).IsFixedLength().HasColumnName("Codigo_Postal");
            entity.Property(e => e.EstatusRegistro).HasDefaultValueSql("((1))").HasColumnName("Estatus_Registro");
            entity.Property(e => e.FechaUltimaModificacion).HasDefaultValueSql("(getdate())").HasColumnName("Fecha_Ultima_Modificacion");
            entity.Property(e => e.FkCiudad).HasColumnName("FK_Ciudad");
            entity.Property(e => e.FkMunicipio).HasColumnName("FK_Municipio");
            entity.Property(e => e.FkTipoAsentamiento).HasColumnName("FK_TipoAsentamiento");
            entity.Property(e => e.FkUsuarioUltimaMod).HasColumnName("FK_Usuario_Ultima_Mod");
            entity.Property(e => e.NombreAsentamiento).HasMaxLength(255).IsUnicode(false).HasColumnName("Nombre_Asentamiento");

            entity.HasOne(d => d.FkCiudadNavigation).WithMany(p => p.CpAsentamientos).HasForeignKey(d => d.FkCiudad).HasConstraintName("FK_CP_Ciudad");
            entity.HasOne(d => d.FkMunicipioNavigation).WithMany(p => p.CpAsentamientos).HasForeignKey(d => d.FkMunicipio).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CP_Municipio");
            entity.HasOne(d => d.FkTipoAsentamientoNavigation).WithMany(p => p.CpAsentamientos).HasForeignKey(d => d.FkTipoAsentamiento).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_CP_Tipo");
            entity.HasOne(d => d.FkUsuarioUltimaModNavigation).WithMany(p => p.CpAsentamientos).HasForeignKey(d => d.FkUsuarioUltimaMod).HasConstraintName("FK_CP_UsuarioMod");
        });


      modelBuilder.Entity<SysRegistroModificacione>(entity =>
        {
            entity.ToTable("SYS_REGISTRO_MODIFICACIONES");

            // Asegúrate que IdLog sea long si tu BD es BigInt, o int si es Int.
            entity.HasKey(e => e.IdLog);
            entity.Property(e => e.IdLog).HasColumnName("ID_Log"); 

            entity.Property(e => e.Accion).HasMaxLength(int.MaxValue).IsUnicode(false);
            entity.Property(e => e.FechaHora).HasColumnName("Fecha_Hora");
            entity.Property(e => e.FkUsuario).HasColumnName("FK_Usuario");
            entity.Property(e => e.FkAsentamiento).HasColumnName("FK_Asentamiento");

            // RELACIONES UNIDIRECCIONALES
            // Al dejar .WithMany() vacío, ignoramos cualquier lista fantasma que haya quedado.
            
            entity.HasOne(d => d.FkUsuarioNavigation)
                .WithMany() // <--- VACÍO (El truco maestro)
                .HasForeignKey(d => d.FkUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Log");

            entity.HasOne(d => d.FkAsentamientoNavigation)
                .WithMany() // <--- VACÍO TAMBIÉN
                .HasForeignKey(d => d.FkAsentamiento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Asentamiento_Log");
        });
        
    modelBuilder.Entity<SysRegistroModificacionesJson>(entity =>
{
    entity.ToTable("SYS_REGISTRO_MODIFICACIONES_JSON");

    entity.HasKey(e => e.IdLog);

    entity.Property(e => e.IdLog)
          .HasColumnName("ID_Log")
          .ValueGeneratedNever();

    entity.Property(e => e.DatosAnteriores)
          .HasColumnName("Datos_Anteriores");

    // --- AQUÍ ESTÁ EL ARREGLO ---
    entity.HasOne(d => d.IdLogNavigation)
          .WithOne() // <--- DÉJALO VACÍO (Sin parámetros)
          .HasForeignKey<SysRegistroModificacionesJson>(d => d.IdLog)
          .HasConstraintName("FK_SYS_REGISTRO_MODIFICACIONES_JSON_Log");


});
        modelBuilder.Entity<VwPermisosUsuario>(entity =>
        {
            entity.HasNoKey().ToView("VW_PERMISOS_USUARIOS");
            entity.Property(e => e.ClavePermiso).HasMaxLength(50).IsUnicode(false).HasColumnName("Clave_Permiso");
            entity.Property(e => e.IdUsuarios).HasColumnName("ID_Usuarios");
            entity.Property(e => e.IdentificadorExterno).HasMaxLength(50).IsUnicode(false).HasColumnName("Identificador_Externo");
            entity.Property(e => e.NombreRol).HasMaxLength(50).IsUnicode(false).HasColumnName("Nombre_Rol");
        });


        modelBuilder.Entity<Modulo>(entity =>
{
    // Opción A: Si quieres simular que tiene llave (Recomendado para evitar errores de relaciones)
    entity.HasKey(e => e.IdModulo); 
    
    // Opción B: Si solo quieres que deje de molestar (pero podría fallar si intentas usarla)
    // entity.HasNoKey(); 
    
    // IMPORTANTE: Como la tabla NO existe en SQL, le decimos que no la busque todavía
    // o le damos un nombre falso para que EF crea que existe.
    // Pero como tu error es de configuración, con definir la HasKey basta para que arranque.
    entity.Ignore(e => e.CatOpciones); // Rompemos la relación para que no busque datos
});

// =========================================================
            // ARREGLO FINAL: TABLA TEMPORAL SIN LLAVE
            // =========================================================
            modelBuilder.Entity<TmpCarga>(entity =>
            {
                // ESTA LÍNEA ES LA CURA PARA TU ERROR ACTUAL:
                entity.HasNoKey(); 
                
                entity.ToTable("TMP_CARGA");
                
                // Mapeo básico para que no truene si intentas usarla
                entity.Property(e => e.CCp).HasColumnName("c_CP");
            });
            
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}