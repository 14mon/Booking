using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace booking_system.Migrations
{
    /// <inheritdoc />
    public partial class userCredit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCreditHistories_Transactions_TransactionId",
                table: "UserCreditHistories");

            migrationBuilder.DropIndex(
                name: "IX_UserCreditHistories_TransactionId",
                table: "UserCreditHistories");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "UserCreditHistories");

            migrationBuilder.DropColumn(
                name: "IsRefund",
                table: "ClassBookings");

            migrationBuilder.RenameColumn(
                name: "TotalCredit",
                table: "UserCreditHistories",
                newName: "CreditAmount");

            migrationBuilder.AddColumn<int>(
                name: "CreditBalance",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsFull",
                table: "Classes",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditBalance",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsFull",
                table: "Classes");

            migrationBuilder.RenameColumn(
                name: "CreditAmount",
                table: "UserCreditHistories",
                newName: "TotalCredit");

            migrationBuilder.AddColumn<Guid>(
                name: "TransactionId",
                table: "UserCreditHistories",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsRefund",
                table: "ClassBookings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_UserCreditHistories_TransactionId",
                table: "UserCreditHistories",
                column: "TransactionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCreditHistories_Transactions_TransactionId",
                table: "UserCreditHistories",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
