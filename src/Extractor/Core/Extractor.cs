using System;
using System.Collections.Generic;
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
            var expressions = schema.Model.Expressions != null ? schema.Model.Expressions.Select(e => GetExtractFromExpression(e)) : Enumerable.Empty<Extract>();

            return measures.Concat(columns).Concat(partitions).Concat(expressions);
        }

        public static Extract GetLayoutExtract(object layout)
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

        private static string ExpandEscaped(string sourceExpression)
        {
            return sourceExpression.Replace("\n", Environment.NewLine)
                .Replace("#(lf)", Environment.NewLine)
                .Replace("#(tab)", "\t")
                .Replace("\\\"", "\"");
        }
    }
}