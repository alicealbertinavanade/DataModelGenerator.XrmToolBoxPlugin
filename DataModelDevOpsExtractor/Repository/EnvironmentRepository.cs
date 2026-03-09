using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace DataModelDevOpsExtractor.Repository
{
    public class EnvironmentRepository
    {
        private const int SolutionComponentTypeEntity = 1;
        private const int SolutionComponentTypeAttribute = 2;

        private readonly IOrganizationService service;
        private readonly string prefixEnv;

        public EnvironmentRepository(IOrganizationService service, string prefixEnv)
        {
            this.service = service;
            this.prefixEnv = prefixEnv;
        }

        public void CreateTable(string tableName, string system, string nameEn, string nameIt)
        {
            var createReq = new CreateEntityRequest
            {
                Entity = new EntityMetadata
                {
                    SchemaName = tableName,               // es: "new_MyTable"
                    LogicalName = tableName,             // es: "new_mytable"
                    DisplayName = new Label(nameEn, 1033),
                    DisplayCollectionName = new Label(nameEn, 1033),
                    OwnershipType = OwnershipTypes.UserOwned,
                    IsActivity = false,
                    HasNotes = true,
                    HasActivities = false
                },
                PrimaryAttribute = new StringAttributeMetadata
                {
                    SchemaName = $"{prefixEnv}name",       // es: "new_MyTableName"
                    LogicalName = $"{prefixEnv}name",
                    DisplayName = new Label("Name", 1033),
                    MaxLength = 200,
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None)
                }
            };

            service.Execute(createReq);
            // 3) Publish della sola entità (consigliato)
            PublishEntity(service, tableName);
        }


        private static void PublishEntity(IOrganizationService service, string logicalName)
        {
            var publishReq = new PublishXmlRequest
            {
                ParameterXml =
                    $"<importexportxml><entities><entity>{logicalName}</entity></entities></importexportxml>"
            };
            service.Execute(publishReq);
        }


        public bool TableExists(string tableName)
        {
            try
            {
                var req = new RetrieveEntityRequest
                {
                    LogicalName = tableName,
                    EntityFilters = EntityFilters.Entity, // basta Entity, non serve Attributes
                    RetrieveAsIfPublished = true
                };

                var resp = (RetrieveEntityResponse)service.Execute(req);

                return resp?.EntityMetadata?.LogicalName != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SolutionExists(string solutionUniqueName)
        {
            if (string.IsNullOrWhiteSpace(solutionUniqueName))
            {
                return false;
            }

            var query = new QueryExpression("solution")
            {
                ColumnSet = new ColumnSet("solutionid"),
                TopCount = 1
            };
            query.Criteria.AddCondition("uniquename", ConditionOperator.Equal, solutionUniqueName.Trim());

            return service.RetrieveMultiple(query).Entities.Any();
        }

        public void AddTableToSolution(string solutionUniqueName, string tableName)
        {
            var entityReq = new RetrieveEntityRequest
            {
                LogicalName = tableName,
                EntityFilters = EntityFilters.Entity,
                RetrieveAsIfPublished = true
            };
            var entityResp = (RetrieveEntityResponse)service.Execute(entityReq);
            var entityId = entityResp?.EntityMetadata?.MetadataId;

            if (!entityId.HasValue)
            {
                throw new InvalidPluginExecutionException($"Impossibile recuperare MetadataId per la tabella {tableName}.");
            }

            AddSolutionComponent(solutionUniqueName, entityId.Value, SolutionComponentTypeEntity);
        }

        public void AddColumnToSolution(string solutionUniqueName, string tableName, string columnName)
        {
            var attributeMetadata = TryGetAttributeMetadataWithRetry(tableName, columnName, true, 6, 500)
                ?? TryGetAttributeMetadataWithRetry(tableName, columnName, false, 4, 500);

            var attributeId = attributeMetadata?.MetadataId;

            if (!attributeId.HasValue)
            {
                throw new InvalidPluginExecutionException($"Impossibile recuperare MetadataId per la colonna {tableName}.{columnName}.");
            }

            AddSolutionComponent(solutionUniqueName, attributeId.Value, SolutionComponentTypeAttribute);
        }

        private void AddSolutionComponent(string solutionUniqueName, Guid componentId, int componentType)
        {
            if (string.IsNullOrWhiteSpace(solutionUniqueName))
            {
                return;
            }

            try
            {
                var addReq = new AddSolutionComponentRequest
                {
                    SolutionUniqueName = solutionUniqueName.Trim(),
                    ComponentType = componentType,
                    ComponentId = componentId,
                    AddRequiredComponents = false,
                    DoNotIncludeSubcomponents = false
                };

                service.Execute(addReq);
            }
            catch (Exception ex) when (IsAlreadyInSolutionError(ex))
            {
                // Componente gia presente nella solution: non bloccare l'upload.
            }
        }

        private static bool IsAlreadyInSolutionError(Exception ex)
        {
            var message = ex?.Message ?? string.Empty;
            return message.IndexOf("already", StringComparison.OrdinalIgnoreCase) >= 0
                && message.IndexOf("solution", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public bool ColumnExists(string columnName, string tableName)
        {
            try
            {
                var req = new RetrieveAttributeRequest
                {
                    EntityLogicalName = tableName,
                    LogicalName = columnName,
                    RetrieveAsIfPublished = true
                };

                var resp = (RetrieveAttributeResponse)service.Execute(req);
                return resp?.AttributeMetadata != null;
            }
            catch
            {
                return false;
            }

        }

        public AttributeMetadata GetOrCreateColumn(
            string columnName,
            string tableName,
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
            var columnExists = ColumnExists(columnName, tableName);
            if (columnExists)
            {
                var retrieveReq = new RetrieveAttributeRequest
                {
                    EntityLogicalName = tableName,
                    LogicalName = columnName,
                    RetrieveAsIfPublished = true
                };
                var retrieveResp = (RetrieveAttributeResponse)service.Execute(retrieveReq);
                return retrieveResp?.AttributeMetadata;
            }

            var required = ParseRequiredLevel(requiredLevel);
            var normalizedType = NormalizeColumnType(columnType);
            var sanitizedAdditionalData = SanitizeAdditionalData(additionalData);

            if (normalizedType == "LOOKUP")
            {
                return CreateLookupColumn(
                    columnName,
                    tableName,
                    lookupTable,
                    displayNameEn,
                    description,
                    required);
            }

            var metadata = CreateAttributeMetadataByType(
                columnName,
                displayNameEn,
                description,
                columnType,
                lookupTable,
				sanitizedAdditionalData,
                required);

            var createReq = new CreateAttributeRequest
            {
                EntityName = tableName,
                Attribute = metadata
            };

            service.Execute(createReq);
            PublishEntity(service, tableName);

            return metadata;
        }

        private AttributeMetadata CreateLookupColumn(
            string columnName,
            string tableName,
            string lookupTable,
            string displayNameEn,
            string description,
            AttributeRequiredLevel requiredLevel)
        {
            var targetEntity = (lookupTable ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(targetEntity))
            {
                throw new InvalidPluginExecutionException($"Lookup table mancante per la colonna {columnName}.");
            }

            if (!TableExists(targetEntity))
            {
                throw new InvalidPluginExecutionException($"Lookup table non trovata: {targetEntity}.");
            }

            var lookupMetadata = new LookupAttributeMetadata
            {
                SchemaName = columnName,
                LogicalName = columnName,
                DisplayName = new Label(string.IsNullOrWhiteSpace(displayNameEn) ? columnName : displayNameEn, 1033),
                Description = new Label(description ?? string.Empty, 1033),
                RequiredLevel = new AttributeRequiredLevelManagedProperty(requiredLevel)
            };

            var relationSchemaName = BuildRelationshipSchemaName(tableName, columnName, targetEntity);
            var createRelationshipReq = new CreateOneToManyRequest
            {
                Lookup = lookupMetadata,
                OneToManyRelationship = new OneToManyRelationshipMetadata
                {
                    ReferencedEntity = targetEntity,
                    ReferencingEntity = tableName,
                    SchemaName = relationSchemaName,
                    AssociatedMenuConfiguration = new AssociatedMenuConfiguration
                    {
                        Behavior = AssociatedMenuBehavior.UseCollectionName,
                        Group = AssociatedMenuGroup.Details,
                        Label = new Label(string.Empty, 1033),
                        Order = 10000
                    },
                    CascadeConfiguration = new CascadeConfiguration
                    {
                        Assign = CascadeType.NoCascade,
                        Delete = CascadeType.RemoveLink,
                        Merge = CascadeType.NoCascade,
                        Reparent = CascadeType.NoCascade,
                        Share = CascadeType.NoCascade,
                        Unshare = CascadeType.NoCascade
                    }
                }
            };

            service.Execute(createRelationshipReq);
            PublishEntity(service, tableName);

            // Per i lookup il metadata puo diventare disponibile con lieve ritardo.
            return TryGetAttributeMetadataWithRetry(tableName, columnName, true, 6, 500)
                ?? lookupMetadata;
        }

        private static string BuildRelationshipSchemaName(string referencingEntity, string columnName, string referencedEntity)
        {
            var raw = $"{referencingEntity}_{columnName}_{referencedEntity}";
            var sanitized = Regex.Replace(raw ?? string.Empty, "[^A-Za-z0-9_]", "_");
            if (string.IsNullOrWhiteSpace(sanitized))
            {
                sanitized = "rel_lookup";
            }

            return sanitized.Length <= 100 ? sanitized : sanitized.Substring(0, 100);
        }

        private AttributeMetadata TryGetAttributeMetadataWithRetry(
            string tableName,
            string columnName,
            bool retrieveAsIfPublished,
            int attempts,
            int delayMs)
        {
            for (var i = 0; i < attempts; i++)
            {
                try
                {
                    var retrieveReq = new RetrieveAttributeRequest
                    {
                        EntityLogicalName = tableName,
                        LogicalName = columnName,
                        RetrieveAsIfPublished = retrieveAsIfPublished
                    };

                    var retrieveResp = (RetrieveAttributeResponse)service.Execute(retrieveReq);
                    if (retrieveResp?.AttributeMetadata != null)
                    {
                        return retrieveResp.AttributeMetadata;
                    }
                }
                catch
                {
                    // Il metadata potrebbe non essere ancora propagato: riprova.
                }

                if (i < attempts - 1)
                {
                    Thread.Sleep(delayMs);
                }
            }

            return null;
        }

        private AttributeMetadata CreateAttributeMetadataByType(
            string columnName,
            string displayNameEn,
            string description,
            string columnType,
            string lookupTable,
            string additionalData,
            AttributeRequiredLevel requiredLevel)
        {
            var normalizedType = NormalizeColumnType(columnType);
            var displayLabel = new Label(string.IsNullOrWhiteSpace(displayNameEn) ? columnName : displayNameEn, 1033);
            var descriptionLabel = new Label(description ?? string.Empty, 1033);

            switch (normalizedType)
            {
                case "INTEGER":
                    var intMin = ParseIntSetting(additionalData, "Minimum value", int.MinValue);
                    var intMax = ParseIntSetting(additionalData, "Maximum value", int.MaxValue);
                    return new IntegerAttributeMetadata
                    {
                        SchemaName = columnName,
                        LogicalName = columnName,
                        DisplayName = displayLabel,
                        Description = descriptionLabel,
                        MinValue = Math.Min(intMin, intMax),
                        MaxValue = Math.Max(intMin, intMax),
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(requiredLevel)
                    };

                case "MEMO":
                    return new MemoAttributeMetadata
                    {
                        SchemaName = columnName,
                        LogicalName = columnName,
                        DisplayName = displayLabel,
                        Description = descriptionLabel,
                        MaxLength = ParseMaxLength(additionalData, 2000),
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(requiredLevel)
                    };

                case "DECIMAL":
                    var decimalMin = ParseDecimalSetting(additionalData, "Minimum value", -100000000000m);
                    var decimalMax = ParseDecimalSetting(additionalData, "Maximum value", 100000000000m);
                    return new DecimalAttributeMetadata
                    {
                        SchemaName = columnName,
                        LogicalName = columnName,
                        DisplayName = displayLabel,
                        Description = descriptionLabel,
                        MinValue = Math.Min(decimalMin, decimalMax),
                        MaxValue = Math.Max(decimalMin, decimalMax),
                        Precision = 2,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(requiredLevel)
                    };

                case "DOUBLE":
                    var doubleMin = ParseDoubleSetting(additionalData, "Minimum value", -100000000000d);
                    var doubleMax = ParseDoubleSetting(additionalData, "Maximum value", 100000000000d);
                    return new DoubleAttributeMetadata
                    {
                        SchemaName = columnName,
                        LogicalName = columnName,
                        DisplayName = displayLabel,
                        Description = descriptionLabel,
                        MinValue = Math.Min(doubleMin, doubleMax),
                        MaxValue = Math.Max(doubleMin, doubleMax),
                        Precision = 2,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(requiredLevel)
                    };

                case "MONEY":
                    var moneyMin = ParseDoubleSetting(additionalData, "Minimum value", 0d);
                    var moneyMax = ParseDoubleSetting(additionalData, "Maximum value", 1000000000000d);
                    return new MoneyAttributeMetadata
                    {
                        SchemaName = columnName,
                        LogicalName = columnName,
                        DisplayName = displayLabel,
                        Description = descriptionLabel,
                        MinValue = Math.Min(moneyMin, moneyMax),
                        MaxValue = Math.Max(moneyMin, moneyMax),
                        Precision = 2,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(requiredLevel)
                    };

                case "BOOLEAN":
                    var trueLabel = ParseAdditionalDataValue(additionalData, "True") ?? "Yes";
                    var falseLabel = ParseAdditionalDataValue(additionalData, "False") ?? "No";
                    var defaultBool = ParseBooleanSetting(additionalData, "Default Value");
                    return new BooleanAttributeMetadata
                    {
                        SchemaName = columnName,
                        LogicalName = columnName,
                        DisplayName = displayLabel,
                        Description = descriptionLabel,
                        OptionSet = new BooleanOptionSetMetadata(
                            new OptionMetadata(new Label(falseLabel, 1033), 0),
                            new OptionMetadata(new Label(trueLabel, 1033), 1)),
                        DefaultValue = defaultBool,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(requiredLevel)
                    };

                case "DATETIME":
                    var dateFormat = ParseDateTimeFormat(additionalData);
                    return new DateTimeAttributeMetadata
                    {
                        SchemaName = columnName,
                        LogicalName = columnName,
                        DisplayName = displayLabel,
                        Description = descriptionLabel,
                        Format = dateFormat,
                        ImeMode = ImeMode.Disabled,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(requiredLevel)
                    };

                case "PICKLIST":
                case "OPTIONSET":
                case "OPTIONS":
                    return CreatePicklistMetadata(columnName, displayLabel, descriptionLabel, additionalData, requiredLevel);

                case "LOOKUP":
                    if (string.IsNullOrWhiteSpace(lookupTable))
                    {
                        throw new InvalidPluginExecutionException($"Lookup table mancante per la colonna {columnName}.");
                    }
                    return new LookupAttributeMetadata
                    {
                        SchemaName = columnName,
                        LogicalName = columnName,
                        DisplayName = displayLabel,
                        Description = descriptionLabel,
                        Targets = new[] { lookupTable.Trim().ToLowerInvariant() },
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(requiredLevel)
                    };

                default:
                    return new StringAttributeMetadata
                    {
                        SchemaName = columnName,
                        LogicalName = columnName,
                        DisplayName = displayLabel,
                        Description = descriptionLabel,
                        MaxLength = ParseMaxLength(additionalData, 200),
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(requiredLevel)
                    };
            }
        }

        private static PicklistAttributeMetadata CreatePicklistMetadata(
            string columnName,
            Label displayLabel,
            Label descriptionLabel,
            string additionalData,
            AttributeRequiredLevel requiredLevel)
        {
            var options = ParseOptionSetOptions(additionalData).ToList();
            if (!options.Any())
            {
                // Fallback minimo per evitare colonne picklist senza opzioni valide.
                options.Add(new OptionMetadata(new Label("N/A", 1033), 100000000));
            }

            var defaultRaw = ParseAdditionalDataValue(additionalData, "Default");
            int? defaultValue = null;
            if (!string.IsNullOrWhiteSpace(defaultRaw)
                && !string.Equals(defaultRaw.Trim(), "N/A", StringComparison.OrdinalIgnoreCase)
                && int.TryParse(defaultRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var numericDefault))
            {
                if (options.Any(o => o.Value == numericDefault))
                {
                    defaultValue = numericDefault;
                }
            }
            else if (!string.IsNullOrWhiteSpace(defaultRaw)
                && !string.Equals(defaultRaw.Trim(), "N/A", StringComparison.OrdinalIgnoreCase))
            {
                var matched = options.FirstOrDefault(o =>
                    string.Equals(o.Label?.UserLocalizedLabel?.Label, defaultRaw, StringComparison.OrdinalIgnoreCase));
                if (matched != null)
                {
                    defaultValue = matched.Value;
                }
            }

            var optionSetMetadata = new OptionSetMetadata
            {
                IsGlobal = false,
                OptionSetType = OptionSetType.Picklist
            };

            foreach (var option in options)
            {
                optionSetMetadata.Options.Add(option);
            }

            return new PicklistAttributeMetadata
            {
                SchemaName = columnName,
                LogicalName = columnName,
                DisplayName = displayLabel,
                Description = descriptionLabel,
                OptionSet = optionSetMetadata,
                DefaultFormValue = defaultValue,
                RequiredLevel = new AttributeRequiredLevelManagedProperty(requiredLevel)
            };
        }

        private static string NormalizeColumnType(string columnType)
        {
            if (string.IsNullOrWhiteSpace(columnType))
            {
                return string.Empty;
            }

            var normalized = columnType.Trim().ToUpperInvariant();
            normalized = normalized.Replace("_", string.Empty).Replace("-", string.Empty).Replace(" ", string.Empty);
            return normalized;
        }

        private static int ParseMaxLength(string additionalData, int defaultValue)
        {
            var fromLabel = ParseIntSetting(additionalData, "Max length", defaultValue);
            if (fromLabel != defaultValue)
            {
                return Math.Max(1, fromLabel);
            }

            if (string.IsNullOrWhiteSpace(additionalData))
            {
                return defaultValue;
            }

            var match = Regex.Match(additionalData, @"\d+");
            if (!match.Success)
            {
                return defaultValue;
            }

            if (!int.TryParse(match.Value, out var maxLength))
            {
                return defaultValue;
            }

            return Math.Max(1, maxLength);
        }

        private static string SanitizeAdditionalData(string additionalData)
        {
            if (string.IsNullOrWhiteSpace(additionalData))
            {
                return string.Empty;
            }

            var decoded = WebUtility.HtmlDecode(additionalData);
            // Preserva le righe principali anche quando arrivano da HTML (es. Options:<br>0: Active...).
            var withLineBreaks = Regex.Replace(decoded, "<\\s*br\\s*/?\\s*>", "\n", RegexOptions.IgnoreCase);
            var withoutTags = Regex.Replace(withLineBreaks, "<[^>]+>", " ");
            var withoutInvisibleChars = Regex.Replace(withoutTags, "[\u00A0\u200B\u200C\u200D\uFEFF]", " ");
            var normalizedLineEndings = withoutInvisibleChars.Replace("\r\n", "\n").Replace('\r', '\n');

            var cleanedLines = Regex.Split(normalizedLineEndings, "\\n")
                .Select(line => Regex.Replace(line, "[ \t]+", " ").Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line));

            return string.Join(Environment.NewLine, cleanedLines);
        }

        private static string ParseAdditionalDataValue(string additionalData, string key)
        {
            if (string.IsNullOrWhiteSpace(additionalData) || string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            var pattern = $@"(?im)^\s*{Regex.Escape(key)}\s*:\s*(.+?)\s*$";
            var match = Regex.Match(additionalData, pattern);
            return match.Success ? match.Groups[1].Value.Trim() : null;
        }

        private static int ParseIntSetting(string additionalData, string key, int defaultValue)
        {
            var raw = ParseAdditionalDataValue(additionalData, key);
            return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
                ? value
                : defaultValue;
        }

        private static decimal ParseDecimalSetting(string additionalData, string key, decimal defaultValue)
        {
            var raw = ParseAdditionalDataValue(additionalData, key);
            return decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)
                ? value
                : defaultValue;
        }

        private static double ParseDoubleSetting(string additionalData, string key, double defaultValue)
        {
            var raw = ParseAdditionalDataValue(additionalData, key);
            return double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)
                ? value
                : defaultValue;
        }

        private static bool? ParseBooleanSetting(string additionalData, string key)
        {
            var raw = ParseAdditionalDataValue(additionalData, key);
            if (string.IsNullOrWhiteSpace(raw))
            {
                return null;
            }

            if (raw.Equals("true", StringComparison.OrdinalIgnoreCase) || raw.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (raw.Equals("false", StringComparison.OrdinalIgnoreCase) || raw.Equals("no", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return null;
        }

        private static DateTimeFormat ParseDateTimeFormat(string additionalData)
        {
            var raw = ParseAdditionalDataValue(additionalData, "Format");
            if (!string.IsNullOrWhiteSpace(raw) && raw.IndexOf("dateonly", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return DateTimeFormat.DateOnly;
            }

            return DateTimeFormat.DateAndTime;
        }

        private static IEnumerable<OptionMetadata> ParseOptionSetOptions(string additionalData)
        {
            if (string.IsNullOrWhiteSpace(additionalData))
            {
                yield break;
            }

            var hasOptionsSection = Regex.IsMatch(additionalData, @"(?im)^\s*Options\s*:\s*.*$");
            if (!hasOptionsSection)
            {
                yield break;
            }

            var lines = Regex.Split(additionalData, "\\r?\\n");
            var optionsStarted = false;

            foreach (var line in lines)
            {
                if (!optionsStarted)
                {
                    var optionsHeaderMatch = Regex.Match(line, @"^\s*Options\s*:\s*(.*)$", RegexOptions.IgnoreCase);
                    if (optionsHeaderMatch.Success)
                    {
                        optionsStarted = true;

                        var optionInline = optionsHeaderMatch.Groups[1].Value?.Trim();
                        if (!string.IsNullOrWhiteSpace(optionInline))
                        {
                            var inlineMatch = Regex.Match(optionInline, @"^(\d+)\s*:\s*(.+?)\s*$");
                            if (inlineMatch.Success
                                && int.TryParse(inlineMatch.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var inlineValue)
                                && !string.IsNullOrWhiteSpace(inlineMatch.Groups[2].Value))
                            {
                                yield return new OptionMetadata(new Label(inlineMatch.Groups[2].Value.Trim(), 1033), inlineValue);
                            }
                        }
                    }

                    continue;
                }

                if (Regex.IsMatch(line, @"^\s*Default\s*:", RegexOptions.IgnoreCase))
                {
                    break;
                }

                var match = Regex.Match(line, @"^\s*(\d+)\s*:\s*(.+?)\s*$");
                if (!match.Success)
                {
                    continue;
                }

                if (!int.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
                {
                    continue;
                }

                var label = match.Groups[2].Value.Trim();
                if (string.IsNullOrWhiteSpace(label))
                {
                    continue;
                }

                yield return new OptionMetadata(new Label(label, 1033), value);
            }
        }

        private static AttributeRequiredLevel ParseRequiredLevel(string requiredLevel)
        {
            if (string.IsNullOrWhiteSpace(requiredLevel))
            {
                return AttributeRequiredLevel.None;
            }

            var normalized = requiredLevel.Trim().ToUpperInvariant();
            if (normalized.Contains("REQUIRED"))
            {
                return AttributeRequiredLevel.ApplicationRequired;
            }

            if (normalized.Contains("RECOMMENDED"))
            {
                return AttributeRequiredLevel.Recommended;
            }

            return AttributeRequiredLevel.None;
        }



    }
}
