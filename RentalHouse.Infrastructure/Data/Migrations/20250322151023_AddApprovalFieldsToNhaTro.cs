using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalHouse.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalFieldsToNhaTro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "NhaTros",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "NhaTros",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "NhaTros",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "NhaTros");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "NhaTros");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "NhaTros");
        }
    }
}
