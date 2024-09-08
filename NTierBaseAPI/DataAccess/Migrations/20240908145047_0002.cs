using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class _0002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 8, 14, 50, 47, 822, DateTimeKind.Utc).AddTicks(2602),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 8, 7, 53, 58, 881, DateTimeKind.Utc).AddTicks(1958));

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpires",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Assets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 8, 14, 50, 47, 823, DateTimeKind.Utc).AddTicks(9406),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 8, 7, 53, 58, 882, DateTimeKind.Utc).AddTicks(6967));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Assets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("f52bbcde-c581-48c8-a6c8-5b2fa88c6dc3"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("570639c3-2ab2-4564-8c75-46f41c746114"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpires",
                table: "Users");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 8, 7, 53, 58, 881, DateTimeKind.Utc).AddTicks(1958),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 8, 14, 50, 47, 822, DateTimeKind.Utc).AddTicks(2602));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Assets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 8, 7, 53, 58, 882, DateTimeKind.Utc).AddTicks(6967),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 8, 14, 50, 47, 823, DateTimeKind.Utc).AddTicks(9406));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Assets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("570639c3-2ab2-4564-8c75-46f41c746114"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("f52bbcde-c581-48c8-a6c8-5b2fa88c6dc3"));
        }
    }
}
