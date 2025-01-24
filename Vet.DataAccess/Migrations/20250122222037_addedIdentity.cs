using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vet.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addedIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "VetOwners");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "VetOwners");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "VetOwners");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "VetOwners",
                type: "varchar(95)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "VetOwners",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "VetOwners",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "VetOwners",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "VetOwners",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "VetOwners",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "VetOwners",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "VetOwners",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "VetOwners",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "VetOwners",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "VetOwners",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "VetOwners",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "VetOwners",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "VetOwners",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "VetOwners",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "VetOwners");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "VetOwners");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "VetOwners");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "VetOwners");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "VetOwners");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "VetOwners");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "VetOwners");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "VetOwners");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "VetOwners");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "VetOwners");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "VetOwners");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "VetOwners");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "VetOwners");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "VetOwners");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "VetOwners",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(95)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "VetOwners",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "VetOwners",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "VetOwners",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
