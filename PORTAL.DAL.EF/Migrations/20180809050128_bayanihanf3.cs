using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PORTAL.DAL.EF.Migrations
{
    public partial class bayanihanf3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Bayanihan_BayanihanId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Income_IncomeId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_BayanihanId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_IncomeId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "BayanihanId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IncomeId",
                table: "User");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BayanihanId",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncomeId",
                table: "User",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_BayanihanId",
                table: "User",
                column: "BayanihanId");

            migrationBuilder.CreateIndex(
                name: "IX_User_IncomeId",
                table: "User",
                column: "IncomeId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Bayanihan_BayanihanId",
                table: "User",
                column: "BayanihanId",
                principalTable: "Bayanihan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Income_IncomeId",
                table: "User",
                column: "IncomeId",
                principalTable: "Income",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
