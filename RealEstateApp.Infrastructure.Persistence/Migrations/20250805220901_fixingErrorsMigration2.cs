using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class fixingErrorsMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyFeatures_Properties_PropertyId",
                table: "PropertyFeatures");

            migrationBuilder.RenameColumn(
                name: "PropertyId",
                table: "PropertyFeatures",
                newName: "PropertiesId");

            migrationBuilder.RenameIndex(
                name: "IX_PropertyFeatures_PropertyId",
                table: "PropertyFeatures",
                newName: "IX_PropertyFeatures_PropertiesId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SalesTypes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SalesTypes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PropertyTypes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PropertyTypes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Features",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Features",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyFeatures_Properties_PropertiesId",
                table: "PropertyFeatures",
                column: "PropertiesId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyFeatures_Properties_PropertiesId",
                table: "PropertyFeatures");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "SalesTypes");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SalesTypes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PropertyTypes");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "PropertyTypes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Features");

            migrationBuilder.RenameColumn(
                name: "PropertiesId",
                table: "PropertyFeatures",
                newName: "PropertyId");

            migrationBuilder.RenameIndex(
                name: "IX_PropertyFeatures_PropertiesId",
                table: "PropertyFeatures",
                newName: "IX_PropertyFeatures_PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyFeatures_Properties_PropertyId",
                table: "PropertyFeatures",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
