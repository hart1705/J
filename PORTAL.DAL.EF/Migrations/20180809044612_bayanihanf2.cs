using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PORTAL.DAL.EF.Migrations
{
    public partial class bayanihanf2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bayanihan_User_UserId",
                table: "Bayanihan");

            migrationBuilder.DropForeignKey(
                name: "FK_Income_User_UserId",
                table: "Income");

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

            migrationBuilder.DropIndex(
                name: "IX_Income_UserId",
                table: "Income");

            migrationBuilder.DropIndex(
                name: "IX_Bayanihan_UserId",
                table: "Bayanihan");

            migrationBuilder.CreateIndex(
                name: "IX_User_BayanihanId",
                table: "User",
                column: "BayanihanId");

            migrationBuilder.CreateIndex(
                name: "IX_User_IncomeId",
                table: "User",
                column: "IncomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Income_UserId",
                table: "Income",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Bayanihan_UserId",
                table: "Bayanihan",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Bayanihan_User_UserId",
                table: "Bayanihan",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Income_User_UserId",
                table: "Income",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bayanihan_User_UserId",
                table: "Bayanihan");

            migrationBuilder.DropForeignKey(
                name: "FK_Income_User_UserId",
                table: "Income");

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

            migrationBuilder.DropIndex(
                name: "IX_Income_UserId",
                table: "Income");

            migrationBuilder.DropIndex(
                name: "IX_Bayanihan_UserId",
                table: "Bayanihan");

            migrationBuilder.CreateIndex(
                name: "IX_User_BayanihanId",
                table: "User",
                column: "BayanihanId",
                unique: true,
                filter: "[BayanihanId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_User_IncomeId",
                table: "User",
                column: "IncomeId",
                unique: true,
                filter: "[IncomeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Income_UserId",
                table: "Income",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bayanihan_UserId",
                table: "Bayanihan",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bayanihan_User_UserId",
                table: "Bayanihan",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Income_User_UserId",
                table: "Income",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Bayanihan_BayanihanId",
                table: "User",
                column: "BayanihanId",
                principalTable: "Bayanihan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
