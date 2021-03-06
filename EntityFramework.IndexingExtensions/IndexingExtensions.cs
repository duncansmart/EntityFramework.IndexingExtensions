﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace System.Data.Entity
{
    public static class IndexingExtensions
    {
        /// <summary>
        /// Configures a non-clustered index for this entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entityTypeConfiguration">The entity type configuration.</param>
        /// <param name="indexName">The name of the index.</param>
        /// <param name="propertySelector">Selects the property to be indexed.</param>
        /// <param name="additionalPropertySelectors">Selects additional properties to be added to the index.</param>
        /// <returns>The entity type configuration, for fluent chaining.</returns>
        public static EntityTypeConfiguration<TEntity> HasIndex<TEntity>(
            this EntityTypeConfiguration<TEntity> entityTypeConfiguration,
            string indexName,
            Func<EntityTypeConfiguration<TEntity>, PrimitivePropertyConfiguration> propertySelector,
            params Func<EntityTypeConfiguration<TEntity>, PrimitivePropertyConfiguration>[] additionalPropertySelectors)
            where TEntity : class
        {
            return entityTypeConfiguration.HasIndex(indexName, IndexOptions.Nonclustered,
                propertySelector, additionalPropertySelectors);
        }

        /// <summary>
        /// Configures an index for this entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entityTypeConfiguration">The entity type configuration.</param>
        /// <param name="indexName">The name of the index.</param>
        /// <param name="indexOptions">One or more options used when creating the index.</param>
        /// <param name="propertySelector">Selects the property to be indexed.</param>
        /// <param name="additionalPropertySelectors">Selects additional properties to be added to the index.</param>
        /// <returns>The entity type configuration, for fluent chaining.</returns>
        public static EntityTypeConfiguration<TEntity> HasIndex<TEntity>(
            this EntityTypeConfiguration<TEntity> entityTypeConfiguration,
            string indexName, IndexOptions indexOptions,
            Func<EntityTypeConfiguration<TEntity>, PrimitivePropertyConfiguration> propertySelector,
            params Func<EntityTypeConfiguration<TEntity>, PrimitivePropertyConfiguration>[] additionalPropertySelectors)
            where TEntity : class
        {
            AddIndexColumn(indexName, indexOptions, 1, propertySelector(entityTypeConfiguration));
            for (int i = 0; i < additionalPropertySelectors.Length; i++)
            {
                AddIndexColumn(indexName, indexOptions, i + 2, additionalPropertySelectors[i](entityTypeConfiguration));
            }

            return entityTypeConfiguration;
        }

        private static void AddIndexColumn(
            string indexName,
            IndexOptions indexOptions,
            int column,
            PrimitivePropertyConfiguration propertyConfiguration)
        {
            var indexAttribute = new IndexAttribute(indexName, column)
            {
                IsClustered = indexOptions.HasFlag(IndexOptions.Clustered),
                IsUnique = indexOptions.HasFlag(IndexOptions.Unique)
            };

            propertyConfiguration.HasColumnAnnotation("Index", new IndexAnnotation(indexAttribute));
        }
    }
}
