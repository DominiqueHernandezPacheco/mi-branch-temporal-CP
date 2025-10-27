using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Core.Infraestructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class MigracionInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cat_Modulos",
                columns: table => new
                {
                    IdModulo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Icono = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Modulos", x => x.IdModulo);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Roles",
                columns: table => new
                {
                    IdRol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Roles", x => x.IdRol);
                });

            migrationBuilder.CreateTable(
                name: "Cat_Opciones",
                columns: table => new
                {
                    IdOpcion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdModulo = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    Url = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    Icono = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_OpcionesModulo", x => x.IdOpcion);
                    table.ForeignKey(
                        name: "FK_Cat_Opciones_Cat_Modulos",
                        column: x => x.IdModulo,
                        principalTable: "Cat_Modulos",
                        principalColumn: "IdModulo");
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuarioLlave = table.Column<int>(type: "int", nullable: false),
                    IdRol = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuario__5B65BF97E14C2783", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_Usuario_Cat_Roles",
                        column: x => x.IdRol,
                        principalTable: "Cat_Roles",
                        principalColumn: "IdRol");
                });

            migrationBuilder.CreateTable(
                name: "Cat_OpcionesRol",
                columns: table => new
                {
                    IdOpcionRol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRol = table.Column<int>(type: "int", nullable: false),
                    IdOpcion = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpcionesRol", x => x.IdOpcionRol);
                    table.ForeignKey(
                        name: "FK_Cat_OpcionesRol_Cat_Opciones",
                        column: x => x.IdOpcion,
                        principalTable: "Cat_Opciones",
                        principalColumn: "IdOpcion");
                    table.ForeignKey(
                        name: "FK_Cat_OpcionesRol_Cat_Roles",
                        column: x => x.IdRol,
                        principalTable: "Cat_Roles",
                        principalColumn: "IdRol");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cat_OpcionesModulo_IdModulo",
                table: "Cat_Opciones",
                column: "IdModulo");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_OpcionesRol_IdOpcion",
                table: "Cat_OpcionesRol",
                column: "IdOpcion");

            migrationBuilder.CreateIndex(
                name: "IX_OpcionesRol_IdRol",
                table: "Cat_OpcionesRol",
                column: "IdRol");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Rol",
                table: "Usuarios",
                column: "IdRol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cat_OpcionesRol");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Cat_Opciones");

            migrationBuilder.DropTable(
                name: "Cat_Roles");

            migrationBuilder.DropTable(
                name: "Cat_Modulos");
        }
    }
}
