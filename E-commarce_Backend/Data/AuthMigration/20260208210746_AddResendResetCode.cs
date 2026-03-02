using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_commarce_Backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddResendResetCode : Migration
    {
        /// <inheritdoc /> 
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastResetCodeSentAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastResetCodeSentAt",
                table: "AspNetUsers");
        }
    }
}
