using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModelDevOpsExtractor.Tests.TestHelpers
{
    /// <summary>
    /// Implementazione fake di IOrganizationService per testing
    /// </summary>
    public class FakeOrganizationService : IOrganizationService
    {
        private readonly Dictionary<string, List<Entity>> _entities = new Dictionary<string, List<Entity>>();

        public Guid Create(Entity entity)
        {
            var id = Guid.NewGuid();
            entity.Id = id;

            if (!_entities.ContainsKey(entity.LogicalName))
            {
                _entities[entity.LogicalName] = new List<Entity>();
            }

            _entities[entity.LogicalName].Add(entity);
            return id;
        }

        public void Update(Entity entity)
        {
            if (!_entities.ContainsKey(entity.LogicalName))
            {
                throw new InvalidOperationException($"Entity {entity.LogicalName} not found");
            }

            var existing = _entities[entity.LogicalName].FirstOrDefault(e => e.Id == entity.Id);
            if (existing == null)
            {
                throw new InvalidOperationException($"Entity with ID {entity.Id} not found");
            }

            foreach (var attribute in entity.Attributes)
            {
                existing[attribute.Key] = attribute.Value;
            }
        }

        public void Delete(string entityName, Guid id)
        {
            if (!_entities.ContainsKey(entityName))
            {
                throw new InvalidOperationException($"Entity {entityName} not found");
            }

            var entity = _entities[entityName].FirstOrDefault(e => e.Id == id);
            if (entity != null)
            {
                _entities[entityName].Remove(entity);
            }
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            if (!_entities.ContainsKey(entityName))
            {
                throw new InvalidOperationException($"Entity {entityName} not found");
            }

            var entity = _entities[entityName].FirstOrDefault(e => e.Id == id);
            if (entity == null)
            {
                throw new InvalidOperationException($"Entity with ID {id} not found");
            }

            return entity;
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            var queryExpression = query as QueryExpression;
            if (queryExpression == null)
            {
                return new EntityCollection();
            }

            if (!_entities.ContainsKey(queryExpression.EntityName))
            {
                return new EntityCollection();
            }

            var results = _entities[queryExpression.EntityName].AsEnumerable();

            foreach (var condition in queryExpression.Criteria.Conditions)
            {
                results = results.Where(e =>
                {
                    if (!e.Contains(condition.AttributeName))
                    {
                        return false;
                    }

                    var value = e[condition.AttributeName];
                    if (condition.Operator == ConditionOperator.Equal)
                    {
                        return value?.ToString() == condition.Values[0]?.ToString();
                    }

                    return false;
                });
            }

            var collection = new EntityCollection();
            collection.Entities.AddRange(results.ToList());
            return collection;
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            throw new NotImplementedException("Execute not implemented in FakeOrganizationService");
        }

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            throw new NotImplementedException();
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            throw new NotImplementedException();
        }

        public void AddEntity(Entity entity)
        {
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }

            if (!_entities.ContainsKey(entity.LogicalName))
            {
                _entities[entity.LogicalName] = new List<Entity>();
            }

            _entities[entity.LogicalName].Add(entity);
        }

        public void Clear()
        {
            _entities.Clear();
        }
    }
}
