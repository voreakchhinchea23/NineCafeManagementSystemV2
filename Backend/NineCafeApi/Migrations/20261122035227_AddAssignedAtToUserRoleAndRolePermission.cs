using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NineCafeApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedAtToUserRoleAndRolePermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                table: "UserRoles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                table: "RolePermissions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedAt",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "AssignedAt",
                table: "RolePermissions");
        }
    }
}
