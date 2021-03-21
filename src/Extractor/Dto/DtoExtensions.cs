using System.Collections.Generic;
using System.Linq;
using Extractor.Dto;

namespace Extractor
{
    internal static class DtoExtensions
    {
        public static IEnumerable<Table> FilterTables(this DataModelSchema schema)
        {
            return schema.Model.Tables.Where(t => !t.Name.StartsWith("DateTableTemplate") && !t.Name.StartsWith("LocalDateTable"));
        }
        
        public static IEnumerable<Measure> FilterMeasures(this Table table)
        {
            return table.Measures.Where(m => m.Expression != null && !m.Expression.StartsWith("EXTERNALMEASURE"));
        }
        
        public static IEnumerable<Column> FilterColumns(this Table table)
        {
            return table.Columns.Where(c => c.Type == "calculated");
        }
        
        public static IEnumerable<Partition> FilterPartitions(this Table table)
        {
            return table.Partitions.Where(p => p.Source.Expression != null);
        }
    }
}