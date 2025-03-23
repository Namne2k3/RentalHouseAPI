using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalHouse.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class them_phan_hoi_images : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Evidence",
                table: "Reports");

            migrationBuilder.CreateTable(
                name: "ReportImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportImages_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportImages_ReportId",
                table: "ReportImages",
                column: "ReportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportImages");

            migrationBuilder.AddColumn<string>(
                name: "Evidence",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
