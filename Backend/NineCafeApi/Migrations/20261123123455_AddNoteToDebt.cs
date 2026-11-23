using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NineCafeApi.Migrations
{
    /// <inheritdoc />
    public partial class AddNoteToDebt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Debts",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "Debts");
        }
    }
}
