using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.UserControls.LookupControl
{
    public class LookupEntityTableDef
    {
        public static TableDef AspNetUserRoles
        {
            get
            {
                return new TableDef
                {
                    PrimaryEntity = "User",
                    SecondaryEntity = "RoleView",
                    RelationShipName = "AspNetUserRoles",
                    ColumnSet = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Name", "Role Name"),
                        new KeyValuePair<string, string>("StatusName", "Status")
                    }
                };
            }
        }

        public static TableDef ApplicationActionPermission
        {
            get
            {
                return new TableDef
                {
                    PrimaryEntity = "ApplicationPermission",
                    SecondaryEntity = "ApplicationAction",
                    RelationShipName = "ApplicationAction_ApplicationPermission",
                    ColumnSet = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("ApplicationAction_Id", "Action"),
                        new KeyValuePair<string, string>("Description", "Description")
                    }
                };
            }
        }

        public static TableDef RolePermission
        {
            get
            {
                return new TableDef
                {
                    PrimaryEntity = "Role",
                    SecondaryEntity = "ApplicationPermissionView",
                    RelationShipName = "ApplicationRole_ApplicationPermission",
                    ColumnSet = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("ApplicationPermission_Id", "Permission ID"),
                        new KeyValuePair<string, string>("Description", "Description"),
                        new KeyValuePair<string, string>("StatusName", "Status")
                    }
                };
            }
        }

        public static TableDef IdentityRoleMenuItem
        {
            get
            {
                return new TableDef
                {
                    PrimaryEntity = "MenuItem",
                    SecondaryEntity = "Role",
                    RelationShipName = "IdentityRoleMenuItem",
                    ColumnSet = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Name", "Role Name"),
                        new KeyValuePair<string, string>("Status", "Status")
                    }
                };
            }
        }
    }
}
