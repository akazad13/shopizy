using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shopizy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboxDeadLetter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeadLetteredOn",
                table: "OutboxMessages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeadLetterReason",
                table: "OutboxMessages",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_DeadLetteredOn",
                table: "OutboxMessages",
                column: "DeadLetteredOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_DeadLetteredOn",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "DeadLetteredOn",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "DeadLetterReason",
                table: "OutboxMessages");
        }
    }
}
