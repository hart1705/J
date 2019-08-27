using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PORTAL.DAL.EF.Migrations
{
    public partial class references : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Income_IncomeId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_IncomeId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_Income_UserId",
                table: "Income");

            migrationBuilder.DropColumn(
                name: "IncomeId",
                table: "User");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpirationDate",
                table: "ReferralCode",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.CreateIndex(
                name: "IX_Income_UserId",
                table: "Income",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bayanihan_User_Id",
                table: "Bayanihan",
                column: "Id",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Income_User_Id",
                table: "Income",
                column: "Id",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bayanihan_User_Id",
                table: "Bayanihan");

            migrationBuilder.DropForeignKey(
                name: "FK_Income_User_Id",
                table: "Income");

            migrationBuilder.DropIndex(
                name: "IX_Income_UserId",
                table: "Income");

            migrationBuilder.AddColumn<string>(
                name: "IncomeId",
                table: "User",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpirationDate",
                table: "ReferralCode",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_IncomeId",
                table: "User",
                column: "IncomeId",
                unique: true,
                filter: "[IncomeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Income_UserId",
                table: "Income",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Income_IncomeId",
                table: "User",
                column: "IncomeId",
                principalTable: "Income",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
