using Microsoft.EntityFrameworkCore;
using PORTAL.DAL.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PORTAL.WEB.UserControls.LookupControl
{
    public static class Lookup
    {
        public static TableRecord LoadTableRecords(ApplicationDbContext context, string primaryRecordId, TableDef tableDef, bool isNewAssociation = false, string search = "")
        {
            DataTable dtRecords = new DataTable();
            TableRecord lookupRecord = new TableRecord();

            List<RecordRow> recordRows = new List<RecordRow>();
            List<string> columnHeaders = new List<string>();
            List<string> ids = GetSelectedRecords(context, primaryRecordId, tableDef);

            bool isSearch = !string.IsNullOrWhiteSpace(search);

            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                string[] columnSet = tableDef.ColumnSet.Select(c => $"{c.Key} [{c.Value}]").ToArray();

                string query = $"SELECT ";
                if (ids.Count <= 0 && !isNewAssociation)
                {
                    query += "TOP 0 ";
                }
                if (isNewAssociation)
                {
                    query += "TOP 1000";
                }

                query += $" Id, {string.Join(",", columnSet)} FROM [{tableDef.SecondaryEntity}]";
                if (ids.Count > 0)
                {
                    query += !query.Contains("WHERE") ? " WHERE " : " AND ";
                    query += "Id " + (isNewAssociation ? "NOT " : "") + "IN ({0})";
                    string[] paramArray = ids.Select((x, i) => "@id" + i).ToArray();
                    query = string.Format(query, string.Join(",", paramArray));

                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        query += $" AND {tableDef.ColumnSet[0].Key} LIKE '%{search}%'";
                    }
                    if (!string.IsNullOrWhiteSpace(search) || isNewAssociation)
                    {
                        query += " AND Status = 0";
                    }
                    query += $" ORDER BY {tableDef.ColumnSet[0].Key} ASC";

                    command.CommandText = query;

                    for (int i = 0; i < ids.Count; ++i)
                    {
                        command.Parameters.Add(new SqlParameter("@id" + i, ids[i]));
                    }
                }
                else
                {
                    if (isSearch)
                    {
                        query += $" WHERE {tableDef.ColumnSet[0].Key} LIKE '%{search}%'";
                    }
                    query += query.Contains("WHERE") ? " AND" : " WHERE";
                    query += " Status = 0";
                    query += $" ORDER BY {tableDef.ColumnSet[0].Key} ASC";
                    command.CommandText = query;
                }

                context.Database.OpenConnection();
                using (var result = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    dtRecords.Load(result);
                }

                for (int i = 1; i < dtRecords.Columns.Count; i++)
                {
                    columnHeaders.Add(dtRecords.Columns[i].ColumnName);
                }

                foreach (DataRow row in dtRecords.Rows)
                {
                    var lookupItem = new RecordRow
                    {
                        RecordId = row["Id"].ToString()
                    };
                    lookupItem.ColumnValues = new List<string>();
                    for (int i = 1; i < dtRecords.Columns.Count; i++)
                    {
                        lookupItem.ColumnValues.Add(row[dtRecords.Columns[i].ColumnName].ToString());
                    }
                    recordRows.Add(lookupItem);
                }
                lookupRecord.ColumnHeaders = columnHeaders;
                lookupRecord.Rows = recordRows;
            }
            return lookupRecord;
        }

        public static TableRecord LoadTableRecords(ApplicationDbContext context, string entityName, string primaryKeyName, string colums, string search = "", string recordId = "")
        {
            DataTable dtRecords = new DataTable();
            TableRecord lookupRecord = new TableRecord();

            List<RecordRow> recordRows = new List<RecordRow>();
            List<string> columnHeaders = new List<string>();


            bool isSearch = !string.IsNullOrWhiteSpace(search);

            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                string query = $"SELECT TOP 1000";
                query += $" Id, {primaryKeyName}, {colums} FROM [{entityName}]";
                if (isSearch)
                {
                    query += $" WHERE {primaryKeyName} LIKE '%{search}%' AND Status = 0";
                }
                else if (!string.IsNullOrWhiteSpace(recordId))
                {
                    query += $" WHERE Id = '{recordId}'";
                }
                else
                {
                    query += $" WHERE Status = 0 ";
                }
                query += $" ORDER BY {primaryKeyName} ASC";
                command.CommandText = query;
                context.Database.OpenConnection();
                using (var result = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    dtRecords.Load(result);
                }

                for (int i = 2; i < dtRecords.Columns.Count; i++)
                {
                    columnHeaders.Add(dtRecords.Columns[i].ColumnName);
                }

                foreach (DataRow row in dtRecords.Rows)
                {
                    var lookupItem = new RecordRow
                    {
                        RecordId = row["Id"].ToString()
                    };
                    lookupItem.ColumnValues = new List<string>();
                    for (int i = 2; i < dtRecords.Columns.Count; i++)
                    {
                        lookupItem.ColumnValues.Add(row[dtRecords.Columns[i].ColumnName].ToString());
                    }
                    recordRows.Add(lookupItem);
                }
                lookupRecord.ColumnHeaders = columnHeaders;
                lookupRecord.Rows = recordRows;
            }
            return lookupRecord;
        }

        public static List<string> GetSelectedRecords(ApplicationDbContext context, string primaryRecordId, TableDef tableDef)
        {
            List<string> ids = new List<string>();
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                var pid = tableDef.SecondaryEntity.Substring(tableDef.SecondaryEntity.Length - 4) == "View" ? tableDef.SecondaryEntity.Substring(0, tableDef.SecondaryEntity.Length - 4) : tableDef.SecondaryEntity;

                string query = $"SELECT {pid}Id FROM [{tableDef.RelationShipName}] WHERE {tableDef.PrimaryEntity}Id = '{primaryRecordId}'";
                command.CommandText = query;
                context.Database.OpenConnection();
                using (var result = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            ids.Add(result.GetValue(0).ToString());
                        }
                    }
                }
            }
            return ids;
        }

        public static bool DisassociateRecord(ApplicationDbContext context, string primaryRecordId, string recordId, TableDef tableDef)
        {
            int result = 0;
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                var pid = tableDef.SecondaryEntity.Substring(tableDef.SecondaryEntity.Length - 4) == "View" ? tableDef.SecondaryEntity.Substring(0, tableDef.SecondaryEntity.Length - 4) : tableDef.SecondaryEntity;

                string query = $"DELETE FROM [{tableDef.RelationShipName}] WHERE {tableDef.PrimaryEntity}Id = '{primaryRecordId}' AND {pid}Id = '{recordId}'";
                command.CommandText = query;
                context.Database.OpenConnection();
                result = command.ExecuteNonQuery();
                context.Database.CloseConnection();
            }
            return result > 0;
        }

        public static bool AssociateRecord(ApplicationDbContext context, string primaryRecordId, string recordId, TableDef tableDef)
        {
            int result = 0;
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                var pid = tableDef.SecondaryEntity.Substring(tableDef.SecondaryEntity.Length - 4) == "View" ? tableDef.SecondaryEntity.Substring(0, tableDef.SecondaryEntity.Length - 4) : tableDef.SecondaryEntity;

                string query = $"INSERT INTO [{tableDef.RelationShipName}] ({tableDef.PrimaryEntity}Id, {pid}Id) VALUES ('{primaryRecordId}', '{recordId}')";
                command.CommandText = query;
                context.Database.OpenConnection();
                result = command.ExecuteNonQuery();
                context.Database.CloseConnection();
            }
            return result > 0;
        }

        public static string GetSelectedRecord(ApplicationDbContext context, string entityName, string recordId, string displayCol)
        {
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                string query = $"SELECT TOP 1 {displayCol} FROM [{entityName}] WHERE Id = '{recordId}'";
                command.CommandText = query;
                context.Database.OpenConnection();
                using (var result = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (result.HasRows)
                    {
                        result.Read();
                        return result.GetValue(0).ToString();
                    }
                }
            }
            return string.Empty;
        }

        public static TableDef ResolveTableDefinition(string relationShipName)
        {
            TableDef tableDef = null;
            Type type = typeof(LookupEntityTableDef);
            PropertyInfo propInfo = type.GetProperty(relationShipName);
            if (propInfo != null)
            {
                tableDef = propInfo.GetValue(type) as TableDef;
            }
            return tableDef;
        }

    }
}
