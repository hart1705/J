using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PORTAL.DAL.EF.Migrations
{
    public partial class ApplicationPermissionView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE VIEW [dbo].[ApplicationPermissionView] " +
                "AS SELECT[Id],[ApplicationPermission_Id],[CreatedBy],[CreatedOn]," +
                "[Description],[ModifiedBy],[ModifiedOn],[Status] , " +
                "CASE WHEN[Status] = 0 THEN 'Active' ELSE 'Inactive' END as [StatusName] " +
                "FROM ApplicationPermission"
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW [dbo].[ApplicationPermissionView]");
        }
    }
}
