using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class _0005 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectAsset_Assets_AssetId",
                table: "ProjectAsset");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectAsset_Projects_ProjectId",
                table: "ProjectAsset");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectAsset",
                table: "ProjectAsset");

            migrationBuilder.RenameTable(
                name: "ProjectAsset",
                newName: "ProjectAssets");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectAsset_AssetId",
                table: "ProjectAssets",
                newName: "IX_ProjectAssets_AssetId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 10, 8, 6, 49, 722, DateTimeKind.Utc).AddTicks(2535),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 10, 4, 36, 12, 915, DateTimeKind.Utc).AddTicks(8154));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Assets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 10, 8, 6, 49, 724, DateTimeKind.Utc).AddTicks(1487),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 10, 4, 36, 12, 917, DateTimeKind.Utc).AddTicks(5442));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Assets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("b7b6d2a1-dd11-412d-83e3-2df6630f917a"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("5e2f7661-08f8-4ec6-b42e-090e470ee744"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectAssets",
                table: "ProjectAssets",
                columns: new[] { "ProjectId", "AssetId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectAssets_Assets_AssetId",
                table: "ProjectAssets",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectAssets_Projects_ProjectId",
                table: "ProjectAssets",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectAssets_Assets_AssetId",
                table: "ProjectAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectAssets_Projects_ProjectId",
                table: "ProjectAssets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectAssets",
                table: "ProjectAssets");

            migrationBuilder.RenameTable(
                name: "ProjectAssets",
                newName: "ProjectAsset");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectAssets_AssetId",
                table: "ProjectAsset",
                newName: "IX_ProjectAsset_AssetId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 10, 4, 36, 12, 915, DateTimeKind.Utc).AddTicks(8154),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 10, 8, 6, 49, 722, DateTimeKind.Utc).AddTicks(2535));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Assets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 9, 10, 4, 36, 12, 917, DateTimeKind.Utc).AddTicks(5442),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 9, 10, 8, 6, 49, 724, DateTimeKind.Utc).AddTicks(1487));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Assets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("5e2f7661-08f8-4ec6-b42e-090e470ee744"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("b7b6d2a1-dd11-412d-83e3-2df6630f917a"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectAsset",
                table: "ProjectAsset",
                columns: new[] { "ProjectId", "AssetId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectAsset_Assets_AssetId",
                table: "ProjectAsset",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectAsset_Projects_ProjectId",
                table: "ProjectAsset",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
