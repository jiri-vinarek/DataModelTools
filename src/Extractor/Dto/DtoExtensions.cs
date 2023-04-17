using System.Collections.Generic;
using System.Linq;
using Extractor.Dto;
using Newtonsoft.Json;

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

        public static IEnumerable<TablePermission> FilterPermissions(this Role table)
        {
            return table.TablePermissions.Where(p => p.FilterExpression != null);
        }

        public static IEnumerable<Filter> ConvertToFilters(this string filters)
        {
            return string.IsNullOrEmpty(filters) ? 
                Enumerable.Empty<Filter>() : JsonConvert.DeserializeObject<IEnumerable<Filter>>(filters).Where(f => f.Expression != null);
        }

        public static Config ConvertToConfig(this string config)
        {
            return JsonConvert.DeserializeObject<Config>(config);
        }

        public static Query ConvertToQuery(this string query)
        {
            return JsonConvert.DeserializeObject<Query>(query);
        }

        public static DataTransforms ConvertToDataTransforms(this string dataTransforms)
        {
            return JsonConvert.DeserializeObject<DataTransforms>(dataTransforms);
        }
    }
}