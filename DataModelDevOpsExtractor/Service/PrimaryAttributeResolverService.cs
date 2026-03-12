using DataModelDevOpsExtractor.Model;
using DataModelDevOpsExtractor.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModelDevOpsExtractor.Service
{
    public static class PrimaryAttributeResolverService
    {
        public static Dictionary<string, EnvironmentRepository.PrimaryAttributeDefinition> BuildPrimaryAttributeMap(List<DataModelTaskRow> allRows)
        {
            var result = new Dictionary<string, EnvironmentRepository.PrimaryAttributeDefinition>(StringComparer.OrdinalIgnoreCase);
            if (allRows == null)
            {
                return result;
            }

            foreach (var preScanRow in allRows)
            {
                var row = preScanRow.Row;
                if (row == null)
                {
                    continue;
                }

                var tableName = row.ElementAtOrDefault(1)?.Trim();
                var schemaNameCandidate = row.ElementAtOrDefault(2)?.Trim();
                var primaryFlag = row.ElementAtOrDefault(10)?.Trim();

                if (string.IsNullOrWhiteSpace(tableName) || string.IsNullOrWhiteSpace(schemaNameCandidate))
                {
                    continue;
                }

                if (!string.Equals(primaryFlag, "Y", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (result.TryGetValue(tableName, out var existingPrimary)
                    && !string.Equals(existingPrimary?.SchemaName, schemaNameCandidate, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException($"La tabella '{tableName}' ha piu colonne marcate come Primary=Y ({existingPrimary?.SchemaName}, {schemaNameCandidate}). Usa una sola colonna Primary=Y per tabella.");
                }

                result[tableName] = new EnvironmentRepository.PrimaryAttributeDefinition
                {
                    SchemaName = schemaNameCandidate,
                    DisplayNameIt = row.ElementAtOrDefault(3)?.Trim(),
                    DisplayNameEn = row.ElementAtOrDefault(4)?.Trim(),
                    Description = row.ElementAtOrDefault(5)?.Trim(),
                    ColumnType = row.ElementAtOrDefault(6)?.Trim(),
                    AdditionalData = row.ElementAtOrDefault(8)?.Trim(),
                    RequirementLevel = row.ElementAtOrDefault(9)?.Trim()
                };
            }

            return result;
        }
    }
}
