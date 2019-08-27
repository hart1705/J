using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PORTAL.DAL.EF.Migrations
{
    public partial class RoleView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE VIEW [dbo].[RoleView] AS " +
                "SELECT [Id]" +
                ",[Name]" +
                ",[CreatedBy]" +
                ",[CreatedOn]" +
                ",[Description]" +
                ",[IsSysAdmin]" +
                ",CASE WHEN[IsSysAdmin] = 0 THEN 'No' ELSE 'Yes' END as [IsSysAdminName]" +
                ",[ModifiedBy]" +
                ",[ModifiedOn]" +
                ",[Status]" +
                ",CASE WHEN[Status] = 0 THEN 'Active' ELSE 'Inactive' END as [StatusName]" +
                " FROM [Role]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW [dbo].[RoleView]");
        }
    }
}
