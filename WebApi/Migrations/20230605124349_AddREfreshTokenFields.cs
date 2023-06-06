using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddREfreshTokenFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "008f1f23-7fa5-44b6-92a3-2b6b72df9119");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1c5c8300-45ea-43d1-96bc-a4441ab107b4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fbabba78-c31b-4ab2-ba0d-f2b65af3d951");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "021c32ae-4fd6-4380-8e6b-f96ebe4c8d50", "618e99fb-2284-4cd0-8728-7c2f9c1a73d8", "Admin", "ADMIN" },
                    { "2670615d-c8b8-48ce-8810-7cb644280d0c", "c75a6bc0-dc75-4147-bf1e-7a5e4febad41", "Editor", "EDITOR" },
                    { "2e1d0313-7682-49ad-accb-c79c7d924b64", "a694ae3c-e612-45b6-ab9b-98e5b218bbfe", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "021c32ae-4fd6-4380-8e6b-f96ebe4c8d50");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2670615d-c8b8-48ce-8810-7cb644280d0c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2e1d0313-7682-49ad-accb-c79c7d924b64");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "008f1f23-7fa5-44b6-92a3-2b6b72df9119", "16ce216e-5a75-42bd-aca6-99bd161e2fab", "User", "USER" },
                    { "1c5c8300-45ea-43d1-96bc-a4441ab107b4", "566ddab0-57ef-4c69-8aa1-50ffd350f5f8", "Editor", "EDITOR" },
                    { "fbabba78-c31b-4ab2-ba0d-f2b65af3d951", "f826d067-12cf-43f6-9f11-de1aada5b798", "Admin", "ADMIN" }
                });
        }
    }
}
