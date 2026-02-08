using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_commarce_Backend.Data.AuthMigration
{
    /// <inheritdoc />
    public partial class AddVerificationCooldown : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastVerificationCodeSentAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastVerificationCodeSentAt",
                table: "AspNetUsers");
        }
    }
}
