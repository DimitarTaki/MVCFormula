using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MVCFormula.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Driver",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(maxLength: 30, nullable: false),
                    LastName = table.Column<string>(maxLength: 30, nullable: false),
                    BirthDate = table.Column<DateTime>(nullable: false),
                    Podiums = table.Column<int>(nullable: false),
                    Country = table.Column<string>(nullable: true),
                    DriverPicture = table.Column<string>(nullable: true),
                    Team = table.Column<string>(nullable: true),
                    Points = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Driver", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Country = table.Column<string>(maxLength: 40, nullable: true),
                    Headquarters = table.Column<string>(maxLength: 100, nullable: true),
                    Founded = table.Column<DateTime>(nullable: true),
                    ManufacturerPicture = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Formula",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Model = table.Column<string>(maxLength: 40, nullable: false),
                    ModelYear = table.Column<int>(nullable: true),
                    Tyres = table.Column<string>(maxLength: 40, nullable: true),
                    FormulaPicture = table.Column<string>(nullable: true),
                    ManufacturerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formula", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Formula_Manufacturer_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormulaDriver",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormulaId = table.Column<int>(nullable: false),
                    DriverId = table.Column<int>(nullable: false),
                    Fuel = table.Column<string>(nullable: true),
                    Chassis = table.Column<string>(nullable: true),
                    Pts = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormulaDriver", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormulaDriver_Driver_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Driver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormulaDriver_Formula_FormulaId",
                        column: x => x.FormulaId,
                        principalTable: "Formula",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Formula_ManufacturerId",
                table: "Formula",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_FormulaDriver_DriverId",
                table: "FormulaDriver",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_FormulaDriver_FormulaId",
                table: "FormulaDriver",
                column: "FormulaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormulaDriver");

            migrationBuilder.DropTable(
                name: "Driver");

            migrationBuilder.DropTable(
                name: "Formula");

            migrationBuilder.DropTable(
                name: "Manufacturer");
        }
    }
}
