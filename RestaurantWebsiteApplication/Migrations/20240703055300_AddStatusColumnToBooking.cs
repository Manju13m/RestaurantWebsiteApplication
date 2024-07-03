using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantWebsiteApplication.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusColumnToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Bookingdata",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Bookingdata");
        }
    }
}
