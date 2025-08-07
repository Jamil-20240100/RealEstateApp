using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseWithJamilChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Offers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "PropertyId1",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FavoriteProperties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    PropertyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteProperties_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_PropertyId1",
                table: "Messages",
                column: "PropertyId1");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteProperties_ClientId_PropertyId",
                table: "FavoriteProperties",
                columns: new[] { "ClientId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteProperties_PropertyId",
                table: "FavoriteProperties",
                column: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Properties_PropertyId1",
                table: "Messages",
                column: "PropertyId1",
                principalTable: "Properties",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Properties_PropertyId1",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "FavoriteProperties");

            migrationBuilder.DropIndex(
                name: "IX_Messages_PropertyId1",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "PropertyId1",
                table: "Messages");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Offers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);
        }
    }
}
