using DataModelDevOpsExtractor.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace DataModelDevOpsExtractor.Repository
{
    public class DataModelRepository
    {
        private readonly IOrganizationService service;
        private readonly string prefixEnv;

        public DataModelRepository(IOrganizationService service, string prefixEnv)
        {
            this.service = service;
            this.prefixEnv = prefixEnv;
        }

        public EntityCollection getTableByName(string tableName)
        {
            var normalizedTableName = NormalizeSchemaName(tableName);
            var recordQuery = new QueryExpression(prefixEnv + "table");
            recordQuery.ColumnSet.AllColumns = true;
            recordQuery.NoLock = true;
            recordQuery.Criteria.AddCondition(prefixEnv + "name", ConditionOperator.Equal, normalizedTableName);
            return service.RetrieveMultiple(recordQuery);
        }

        public bool ColumnExists(string columnName, Guid tableId)
        {
            var normalizedColumnName = NormalizeSchemaName(columnName);
            var recordQuery = new QueryExpression(prefixEnv + "column");
            recordQuery.ColumnSet = new ColumnSet(false);
            recordQuery.NoLock = true;
            recordQuery.Criteria.AddCondition(prefixEnv + "schemaname", ConditionOperator.Equal, normalizedColumnName);
            recordQuery.Criteria.AddCondition(prefixEnv + "tableid", ConditionOperator.Equal, tableId);
            var results = service.RetrieveMultiple(recordQuery);
            return results.Entities.Count > 0;
        }

        public Entity GetOrCreateTable(string tableName, string system, string nameEn, string nameIt)
        {
            var normalizedTableName = NormalizeSchemaName(tableName);
            var results = getTableByName(normalizedTableName);
            if (results.Entities.Count > 1)
            {
                throw new Exception($"More than one record found for entity {prefixEnv + "table"} with the specified key values {normalizedTableName}.");
            }
            var entity = results.Entities.FirstOrDefault();
            if(entity == null)
            {
                entity = new Entity(prefixEnv + "table");
                entity[prefixEnv + "name"] = normalizedTableName;
                entity[prefixEnv + "systemid"] = system;
                entity[prefixEnv + "label_en"] = nameEn;
                entity[prefixEnv + "label_it"] = nameIt;
                var id = service.Create(entity);
                entity.Id = id;
            }
            return entity;
        }

        public Entity GetOrCreateColumn(
            string columnName, 
            Entity tableEn, 
            string additionalData,
            string displayNameIt,
            string displayNameEn,
            string description,
            string columnType,
            string lookupTable,
            string requiredLevel,
            string usage
            )
        {
            var normalizedColumnName = NormalizeSchemaName(columnName);
            var normalizedLookupTable = NormalizeSchemaName(lookupTable);

            var recordQuery = new QueryExpression(prefixEnv + "column");
            recordQuery.ColumnSet.AllColumns = true;
            recordQuery.NoLock = true;
            recordQuery.Criteria.AddCondition(prefixEnv + "schemaname", ConditionOperator.Equal, normalizedColumnName);
            recordQuery.Criteria.AddCondition(prefixEnv + "tableid", ConditionOperator.Equal, tableEn.Id);
            var results = service.RetrieveMultiple(recordQuery);

            if (results.Entities.Count > 1)
            {
                throw new Exception($"More than one record found for entity {prefixEnv + "column"} with the specified key values {normalizedColumnName}, {tableEn.Id}.");
            }
            var entity = results.Entities.FirstOrDefault();
            if (entity == null)
            {
                object colTypeVal = null;
                if (Enum.TryParse<ColumnTypeCode>(columnType, true, out var colTypeEnum))
                    colTypeVal = (int)colTypeEnum;
                entity = new Entity(prefixEnv + "column");
                if (!string.IsNullOrEmpty(lookupTable) && colTypeEnum == ColumnTypeCode.Lookup)
                {
                    var resultsLookupTable = getTableByName(normalizedLookupTable);
                    if (resultsLookupTable == null)
                    {
                        throw new Exception($"Lookup table not found {normalizedLookupTable}.");
                    }
                    entity[prefixEnv + "lookuptableid"] = resultsLookupTable.Entities.FirstOrDefault()?.ToEntityReference();
                }
                object reqLevelVal = null;
                if (Enum.TryParse<RequirementLevelCode>(requiredLevel, true, out var requiredLevelEnum))
                    reqLevelVal = (int)requiredLevelEnum;
                object usageVal = null;
                var normalizedUsage = NormalizeEnumToken(usage);
                if (Enum.TryParse<UsageCode>(normalizedUsage, true, out var usageEnum))
                    usageVal = (int)usageEnum;

                entity[prefixEnv + "tableid"] = tableEn.ToEntityReference();
                entity[prefixEnv + "schemaname"] = normalizedColumnName;
                entity[prefixEnv + "columntypecode"] = new OptionSetValue((int)colTypeVal);
                entity[prefixEnv + "additionaldata"] = additionalData;
                entity[prefixEnv + "displayname_it"] = displayNameIt;
                entity[prefixEnv + "displayname_en"] = displayNameEn;
                entity[prefixEnv + "requirementlevelcode"] = new OptionSetValue((int)reqLevelVal);
                entity[prefixEnv + "description"] = description;
                entity[prefixEnv + "usagecode"] = usageVal == null? new OptionSetValue((int)UsageCode.IN_USE) : new OptionSetValue((int)usageVal);
                
                var id = service.Create(entity);
                entity.Id = id;
            }

            return entity;
        }

        private static string NormalizeSchemaName(string value)
        {
            return (value ?? string.Empty).Trim().ToLowerInvariant();
        }

        private static string NormalizeEnumToken(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var normalized = value.Trim().ToUpperInvariant();
            normalized = Regex.Replace(normalized, "[-\\s]+", "_");
            normalized = Regex.Replace(normalized, "_+", "_");
            return normalized;
        }
    }
}