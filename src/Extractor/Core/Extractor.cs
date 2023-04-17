using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Extractor.Dto;
using Newtonsoft.Json;

namespace Extractor
{
    public static class Extractor
    {
        public static IEnumerable<Extract> GetSchemaExtracts(DataModelSchema schema)
        {
            var tables = schema.FilterTables().ToList();

            var measures = tables.Where(t => t.Measures != null).SelectMany(t => GetMeasures(t));
            var columns = tables.Where(t => t.Columns != null).SelectMany(t => GetColumns(t));
            var partitions = tables.Where(t => t.Partitions != null).SelectMany(t => GetPartitions(t));
            var roles = schema.Model.Roles != null ? schema.Model.Roles.Select(r => GetExtractFromRoles(r)) : Enumerable.Empty<Extract>();
            var relationships = schema.Model.Relationships != null ? schema.Model.Relationships.Select(r => GetExtractFromRelationships(r)) : Enumerable.Empty<Extract>();
            var expressions = schema.Model.Expressions != null ? schema.Model.Expressions.Select(e => GetExtractFromExpression(e)) : Enumerable.Empty<Extract>();

            return measures.Concat(columns).Concat(partitions).Concat(expressions).Concat(relationships).Concat(roles);
        }

        public static IEnumerable<Extract> GetLayoutExtracts(Layout layout)
        {
            var layoutRaw = GetLayoutExtract(layout);
            var allPagesFilter = layout.Filters != null ? GetAllPagesFilter(layout) : null;

            var pageFilters = layout.Sections.Where(s => s.Filters != null && s.Filters != "[]").Select(s => GetPageFilter(s));
            var pageConfigs = layout.Sections.Select(s => GetPageConfig(s));

            var visualFilters = layout.Sections.SelectMany(s => s.VisualContainers.Where(x => x.Filters != "[]").Select(v => GetVisualFilter(s, v)));
            var visualConfigs = layout.Sections.SelectMany(s => s.VisualContainers.Select(v => GetVisualConfigs(s, v)));
            var visualQuery = layout.Sections.SelectMany(s => s.VisualContainers.Where(x => x.Query != null).Select(v => GetVisualQuery(s, v)));
            var visualDataTransforms = layout.Sections.SelectMany(s => s.VisualContainers.Where(x => x.DataTransforms != null).Select(v => GetVisualDataTransforms(s, v)));

            return new List<Extract> { layoutRaw, allPagesFilter }
                .Concat(pageConfigs)
                .Concat(pageFilters)
                .Concat(visualFilters)
                .Concat(visualConfigs)
                .Concat(visualQuery)
                .Concat(visualDataTransforms);
        }

        public static Extract GetAllPagesFilter(Layout layout)
        {
            return new Extract(
            new File("report", "AllPagesFilter.json"),
                JsonConvert.SerializeObject(layout.Filters.ConvertToFilters(), Formatting.Indented)
            );
        }

        public static Extract GetPageFilter(Section section)
        {
            return new Extract(
                File.FromPage(section, "Filters"),
                JsonConvert.SerializeObject(section.Filters.ConvertToFilters(), Formatting.Indented)
            );
        }

        public static Extract GetPageConfig(Section section)
        {
            return new Extract(
                File.FromPage(section, "Config"),
                JsonConvert.SerializeObject(section.Filters.ConvertToFilters(), Formatting.Indented)
            );
        }

        public static Extract GetVisualFilter(Section section, Visualcontainer container)
        {
            return new Extract(
                File.FromVisualContainer(section, container, "Filters"),
                JsonConvert.SerializeObject(container.Filters.ConvertToFilters(), Formatting.Indented)
            );
        }

        public static Extract GetVisualConfigs(Section section, Visualcontainer container)
        {
            return new Extract(
                File.FromVisualContainer(section, container, "Config"),
                JsonConvert.SerializeObject(container.Config.ConvertToConfig(), Formatting.Indented)
            );
        }

        public static Extract GetVisualQuery(Section section, Visualcontainer container)
        {
            return new Extract(
                File.FromVisualContainer(section, container, "Query"),
                JsonConvert.SerializeObject(container.Query.ConvertToQuery(), Formatting.Indented)
            );
        }

        public static Extract GetVisualDataTransforms(Section section, Visualcontainer container)
        {
            return new Extract(
                File.FromVisualContainer(section, container, "DataTransforms"),
                JsonConvert.SerializeObject(container.DataTransforms.ConvertToDataTransforms(), Formatting.Indented)
            );
        }

        public static Extract GetLayoutExtract(Layout layout)
        {
            return new Extract(
                new File("report", "layout.json"),
                JsonConvert.SerializeObject(layout, Formatting.Indented)
            );
        }

        private static IEnumerable<Extract> GetMeasures(Table table)
        {
            return table.FilterMeasures().Select(m => new Extract(
                File.FromMeasure(table, m),
                m.Expression
            ));
        }

        private static IEnumerable<Extract> GetColumns(Table table)
        {
            return table.FilterColumns().Select(c => new Extract(
                File.FromColumn(table, c),
                c.Expression
            ));
        }

        private static IEnumerable<Extract> GetPartitions(Table table)
        {
            return table.FilterPartitions().Select(p => new Extract(
                File.FromPartition(table, p),
                ExpandEscaped(p.Source.Expression)
            ));
        }

        private static Extract GetExtractFromExpression(Expression expression)
        {
            return new Extract(
                File.FromExpression(expression),
                ExpandEscaped(expression.ExpressionContent)
            );
        }

        private static Extract GetExtractFromRelationships(Relationship relationship)
        {
            return new Extract(
                File.FromRelationship(relationship),
                JsonConvert.SerializeObject(relationship, Formatting.Indented)
            );
        }

        private static Extract GetExtractFromRoles(Role role)
        {
            return new Extract(
                File.FromRole(role),
                string.Join(Environment.NewLine, role.TablePermissions.Select(p => ExpandEscaped(p.FilterExpression))));
        }

        private static string ExpandEscaped(string sourceExpression)
        {
            return sourceExpression.Replace("\n", Environment.NewLine)
                .Replace("#(lf)", Environment.NewLine)
                .Replace("#(tab)", "\t")
                .Replace("\\\"", "\"");
        }
    }
}