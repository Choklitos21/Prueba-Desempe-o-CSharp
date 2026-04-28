using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebaDsesempeño.Migrations
{
    /// <inheritdoc />
    public partial class ThirdMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Reservation",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Reservation",
                newName: "EndTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Reservation",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Reservation");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Reservation",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "Reservation",
                newName: "EndDate");
        }
    }
}
