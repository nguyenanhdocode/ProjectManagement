using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class _0011 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_AppTasks_CK_Task_Status",
                table: "AppTasks");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 4, 0, 15, 0, 399, DateTimeKind.Utc).AddTicks(3949),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 10, 3, 23, 59, 6, 707, DateTimeKind.Utc).AddTicks(7099));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Assets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 4, 0, 15, 0, 402, DateTimeKind.Utc).AddTicks(2707),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 10, 3, 23, 59, 6, 709, DateTimeKind.Utc).AddTicks(5760));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Assets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("e6bcf975-6349-4a70-8c2e-4818d8e1c43c"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("2750b500-599a-4ce0-82b2-ef4f3a7fa73c"));

            migrationBuilder.AddCheckConstraint(
                name: "CK_AppTasks_CK_Task_Status",
                table: "AppTasks",
                sql: "Status >= 0 AND Status <= 4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_AppTasks_CK_Task_Status",
                table: "AppTasks");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 3, 23, 59, 6, 707, DateTimeKind.Utc).AddTicks(7099),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 10, 4, 0, 15, 0, 399, DateTimeKind.Utc).AddTicks(3949));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Assets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 3, 23, 59, 6, 709, DateTimeKind.Utc).AddTicks(5760),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 10, 4, 0, 15, 0, 402, DateTimeKind.Utc).AddTicks(2707));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Assets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("2750b500-599a-4ce0-82b2-ef4f3a7fa73c"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("e6bcf975-6349-4a70-8c2e-4818d8e1c43c"));

            migrationBuilder.AddCheckConstraint(
                name: "CK_AppTasks_CK_Task_Status",
                table: "AppTasks",
                sql: "Status >= 0 AND Status <= 3");
        }
    }
}
