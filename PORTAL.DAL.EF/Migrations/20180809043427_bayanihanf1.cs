using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PORTAL.DAL.EF.Migrations
{
    public partial class bayanihanf1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bayanihan_User_Id",
                table: "Bayanihan");

            migrationBuilder.AlterColumn<string>(
                name: "BayanihanId",
                table: "User",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_BayanihanId",
                table: "User",
                column: "BayanihanId",
                unique: true,
                filter: "[BayanihanId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Bayanihan_BayanihanId",
                table: "User",
                column: "BayanihanId",
                principalTable: "Bayanihan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Bayanihan_BayanihanId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_BayanihanId",
                table: "User");

            migrationBuilder.AlterColumn<string>(
                name: "BayanihanId",
                table: "User",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bayanihan_User_Id",
                table: "Bayanihan",
                column: "Id",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
