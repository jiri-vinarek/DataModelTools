using Extractor.Dto;
using Extractor.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

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

        public static Query ConvertToQuery(this string query)
        {
            return JsonConvert.DeserializeObject<Query>(query);
        }

        public static DataTransforms ConvertToDataTransforms(this string dataTransforms)
        {
            return JsonConvert.DeserializeObject<DataTransforms>(dataTransforms);
        }

        public static string GetVisualContainerTitle(this Visualcontainer visualcontainer)
        {
            JObject config = JObject.Parse(visualcontainer.Config);

            var title = config["singleVisual"]?["vcObjects"]?["title"]?[0]?["properties"]?["text"]?["expr"]?["Literal"]?["Value"].ToString();

            if (title != null)
            {
                return title.ToCamelCase();
            }

            return null;
        }
    }
}