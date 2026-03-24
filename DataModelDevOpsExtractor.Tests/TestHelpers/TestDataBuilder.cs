using DataModelDevOpsExtractor.Model;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace DataModelDevOpsExtractor.Tests.TestHelpers
{
    /// <summary>
    /// Builder per creare dati di test
    /// </summary>
    public static class TestDataBuilder
    {
        public static Entity CreateTableEntity(string prefix, string tableName, string system = "TestSystem")
        {
            var entity = new Entity(prefix + "table")
            {
                Id = Guid.NewGuid()
            };
            entity[prefix + "name"] = tableName.ToLowerInvariant();
            entity[prefix + "systemid"] = system;
            entity[prefix + "label_en"] = tableName + " EN";
            entity[prefix + "label_it"] = tableName + " IT";
            return entity;
        }

        public static Entity CreateColumnEntity(string prefix, string columnName, Guid tableId, ColumnTypeCode columnType = ColumnTypeCode.String)
        {
            var entity = new Entity(prefix + "column")
            {
                Id = Guid.NewGuid()
            };
            entity[prefix + "schemaname"] = columnName.ToLowerInvariant();
            entity[prefix + "tableid"] = new EntityReference(prefix + "table", tableId);
            entity[prefix + "columntypecode"] = new OptionSetValue((int)columnType);
            entity[prefix + "displayname_en"] = columnName + " EN";
            entity[prefix + "displayname_it"] = columnName + " IT";
            entity[prefix + "requirementlevelcode"] = new OptionSetValue((int)RequirementLevelCode.None);
            entity[prefix + "usagecode"] = new OptionSetValue((int)UsageCode.IN_USE);
            return entity;
        }

        public static DataModelTaskRow CreateDataModelTaskRow(
            string system = "TestSystem",
            string table = "test_table",
            string schemaName = "test_column",
            string displayNameIt = "Test IT",
            string displayNameEn = "Test EN",
            string description = "Test description",
            string columnType = "String",
            string lookupTable = "",
            string additionalData = "",
            string requirementLevel = "None",
            string primary = "N",
            string usage = "IN_USE")
        {
            return new DataModelTaskRow
            {
                Row = new string[]
                {
                    system,
                    table,
                    schemaName,
                    displayNameIt,
                    displayNameEn,
                    description,
                    columnType,
                    lookupTable,
                    additionalData,
                    requirementLevel,
                    primary,
                    usage
                },
                TableName = table,
                TableDisplayNameEn = table + " EN",
                TableDisplayNameIt = table + " IT"
            };
        }

        public static List<DataModelTaskRow> CreateMultipleRows(int count, string tablePrefix = "table")
        {
            var rows = new List<DataModelTaskRow>();
            for (int i = 0; i < count; i++)
            {
                rows.Add(CreateDataModelTaskRow(
                    table: $"{tablePrefix}_{i}",
                    schemaName: $"column_{i}"
                ));
            }
            return rows;
        }
    }
}
