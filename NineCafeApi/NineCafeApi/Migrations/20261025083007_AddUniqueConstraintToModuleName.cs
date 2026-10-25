using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NineCafeApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToModuleName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Modules_Name",
                table: "Modules",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Modules_Name",
                table: "Modules");
        }
    }
}
