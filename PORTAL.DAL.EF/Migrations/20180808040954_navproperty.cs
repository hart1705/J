﻿using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PORTAL.DAL.EF.Migrations
{
    public partial class navproperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IncomeId",
                table: "User",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_IncomeId",
                table: "User",
                column: "IncomeId",
                unique: true,
                filter: "[IncomeId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Income_IncomeId",
                table: "User",
                column: "IncomeId",
                principalTable: "Income",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Income_IncomeId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_IncomeId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IncomeId",
                table: "User");
        }
    }
}
