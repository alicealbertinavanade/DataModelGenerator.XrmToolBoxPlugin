using DataModelDevOpsExtractor.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.IdentityModel.Tokens.SecurityTokenHandlerCollectionManager;
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
            var recordQuery = new QueryExpression(prefixEnv + "table");
            recordQuery.ColumnSet.AllColumns = true;
            recordQuery.NoLock = true;
            recordQuery.Criteria.AddCondition(prefixEnv + "name", ConditionOperator.Equal, tableName);
            return service.RetrieveMultiple(recordQuery);
        }
        public Entity GetOrCreateTable(string tableName, string system, string nameEn, string nameIt)
        {
            var results = getTableByName(tableName);
            if (results.Entities.Count > 1)
            {
                throw new Exception($"More than one record found for entity {prefixEnv + "table"} with the specified key values {tableName}.");
            }
            var entity = results.Entities.FirstOrDefault();
            if(entity == null)
            {
                entity = new Entity(prefixEnv + "table");
                entity[prefixEnv + "name"] = tableName;
                entity[prefixEnv + "system"] = system;
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
            var recordQuery = new QueryExpression(prefixEnv + "column");
            recordQuery.ColumnSet.AllColumns = true;
            recordQuery.NoLock = true;
            recordQuery.Criteria.AddCondition(prefixEnv + "schemaname", ConditionOperator.Equal, columnName);
            recordQuery.Criteria.AddCondition(prefixEnv + "tableid", ConditionOperator.Equal, tableEn.Id);
            var results = service.RetrieveMultiple(recordQuery);

            if (results.Entities.Count > 1)
            {
                throw new Exception($"More than one record found for entity {prefixEnv + "column"} with the specified key values {columnName}, {tableEn.Id}.");
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
                    var resultsLookupTable = getTableByName(lookupTable);
                    if (resultsLookupTable == null)
                    {
                        throw new Exception($"Lookup table not found {lookupTable}.");
                    }
                    entity[prefixEnv + "lookuptableid"] = resultsLookupTable.Entities.FirstOrDefault()?.ToEntityReference();
                }
                object reqLevelVal = null;
                if (Enum.TryParse<RequirementLevelCode>(requiredLevel, true, out var requiredLevelEnum))
                    reqLevelVal = (int)requiredLevelEnum;
                object usageVal = null;
                if (Enum.TryParse<UsageCode>(usage, true, out var usageEnum))
                    usageVal = (int)usageEnum;

                entity[prefixEnv + "tableid"] = tableEn.ToEntityReference();
                entity[prefixEnv + "schemaname"] = columnName;
                entity[prefixEnv + "columntypecode"] = new OptionSetValue((int)colTypeVal);
                entity[prefixEnv + "additionaldata"] = additionalData;
                entity[prefixEnv + "displayname_it"] = displayNameIt;
                entity[prefixEnv + "displayname_en"] = displayNameEn;
                entity[prefixEnv + "requirementlevelcode"] = new OptionSetValue((int)reqLevelVal);
                entity[prefixEnv + "description"] = description;
                entity[prefixEnv + "usagecode"] = new OptionSetValue((int)usageVal);
                
                var id = service.Create(entity);
                entity.Id = id;
            }

            return entity;
        }
    }
}