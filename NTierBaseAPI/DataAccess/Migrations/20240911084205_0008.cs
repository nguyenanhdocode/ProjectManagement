using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class _0008 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 11, 8, 42, 5, 258, DateTimeKind.Utc).AddTicks(8392),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 11, 7, 1, 6, 416, DateTimeKind.Utc).AddTicks(5893));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Assets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 11, 8, 42, 5, 261, DateTimeKind.Utc).AddTicks(539),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 11, 7, 1, 6, 418, DateTimeKind.Utc).AddTicks(6368));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Assets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("85e3363d-eccd-47e7-851d-f032f762775e"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("5ef88770-c7a7-45fa-bb93-64da6d541ef9"));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AppTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_AppTasks_CK_Task_Status",
                table: "AppTasks",
                sql: "Status >= 0 AND Status <= 3");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_AppTasks_CK_Task_Status",
                table: "AppTasks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AppTasks");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 11, 7, 1, 6, 416, DateTimeKind.Utc).AddTicks(5893),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 11, 8, 42, 5, 258, DateTimeKind.Utc).AddTicks(8392));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Assets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 11, 7, 1, 6, 418, DateTimeKind.Utc).AddTicks(6368),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 11, 8, 42, 5, 261, DateTimeKind.Utc).AddTicks(539));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Assets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("5ef88770-c7a7-45fa-bb93-64da6d541ef9"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("85e3363d-eccd-47e7-851d-f032f762775e"));
        }
    }
}
