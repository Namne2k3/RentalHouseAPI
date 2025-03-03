using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalHouse.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class add_manyToMany_User_NhaTro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NhaTroId1",
                table: "Favorites",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_NhaTroId1",
                table: "Favorites",
                column: "NhaTroId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_NhaTros_NhaTroId1",
                table: "Favorites",
                column: "NhaTroId1",
                principalTable: "NhaTros",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_NhaTros_NhaTroId1",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_NhaTroId1",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "NhaTroId1",
                table: "Favorites");
        }
    }
}
