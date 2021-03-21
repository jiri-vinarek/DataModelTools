using System;
using System.Collections.Generic;
using System.Linq;
using Extractor.Dto;

namespace Extractor
{
    internal static class Extractor
    {
        public static IEnumerable<Extract> GetExtracts(DataModelSchema schema)
        {
            var tables = schema.FilterTables();
            
            var measures = tables.Where(t => t.Measures != null).SelectMany(t => GetMeasures(t));
            var columns = tables.Where(t => t.Columns != null).SelectMany(t => GetColumns(t));
            var partitions = tables.Where(t => t.Partitions != null).SelectMany(t => GetPartitions(t));

            return measures.Concat(columns).Concat(partitions);
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
        
        private static string ExpandEscaped(string sourceExpression)
        {
            return sourceExpression.Replace("\n", Environment.NewLine)
                .Replace("#(lf)", Environment.NewLine)
                .Replace("#(tab)", "\t")
                .Replace("\\\"", "\"");
        }
    }
}