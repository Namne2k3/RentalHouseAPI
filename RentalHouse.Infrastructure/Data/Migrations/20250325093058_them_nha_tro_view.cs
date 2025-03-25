using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalHouse.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class them_nha_tro_view : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "NhaTros",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "NhaTros",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "NhaTros",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "NhaTroViews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NhaTroId = table.Column<int>(type: "int", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ViewerIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ViewerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaTroViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NhaTroViews_NhaTros_NhaTroId",
                        column: x => x.NhaTroId,
                        principalTable: "NhaTros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NhaTroViews_Users_ViewerId",
                        column: x => x.ViewerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NhaTroViews_NhaTroId",
                table: "NhaTroViews",
                column: "NhaTroId");

            migrationBuilder.CreateIndex(
                name: "IX_NhaTroViews_ViewerId",
                table: "NhaTroViews",
                column: "ViewerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NhaTroViews");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "NhaTros");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "NhaTros");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "NhaTros");
        }
    }
}
