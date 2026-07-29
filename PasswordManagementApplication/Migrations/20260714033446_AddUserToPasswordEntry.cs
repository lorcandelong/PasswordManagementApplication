using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecurePasswordApplication.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToPasswordEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "PasswordEntries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordEntries_UserId",
                table: "PasswordEntries",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordEntries_AspNetUsers_UserId",
                table: "PasswordEntries",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PasswordEntries_AspNetUsers_UserId",
                table: "PasswordEntries");

            migrationBuilder.DropIndex(
                name: "IX_PasswordEntries_UserId",
                table: "PasswordEntries");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PasswordEntries");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");
        }
    }
}
