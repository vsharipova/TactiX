using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TactiX.Migrations
{
    /// <inheritdoc />
    public partial class S : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_comparison_created_at",
                schema: "public",
                table: "comparison");

            migrationBuilder.DropColumn(
                name: "created_at",
                schema: "public",
                table: "comparison");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                schema: "public",
                table: "comparison",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_comparison_created_at",
                schema: "public",
                table: "comparison",
                column: "created_at");
        }
    }
}
