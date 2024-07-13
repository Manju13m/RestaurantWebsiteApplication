using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantWebsiteApplication.Migrations
{
    /// <inheritdoc />
    public partial class newdatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Customerdata");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Admindata");

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "Customerdata",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "Customerdata",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "Admindata",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "Admindata",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Customerdata");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Customerdata");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Admindata");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Admindata");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Customerdata",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Admindata",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");
        }
    }
}
