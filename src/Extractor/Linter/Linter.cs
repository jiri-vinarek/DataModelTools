using System.Collections.Generic;
using System.Linq;
using Extractor.Dto;

namespace Extractor.Linter
{
    public static class Linter
    {
        public static IEnumerable<Message> Check(DataModelSchema schema)
        {
            var tables = schema.FilterTables();
            
            return tables.Where(t => t.Measures != null).SelectMany(t => GetMeasureMessages(t));
        }

        private static IEnumerable<Message> GetMeasureMessages(Table table)
        {
            return table.FilterMeasures().Where(m => !StartsWithCapital(m.Name)).Select(m => new Message(
                messageSeverity: MessageSeverity.Warning,
                file: File.FromMeasure(table, m),
                line: 1,
                column: 1,
                "A measure name should start with capital."
            ));
        }

        private static bool StartsWithCapital(string name)
        {
            var firstChar = name.Substring(0, 1);
            
            return firstChar.ToUpper() == firstChar;
        }
    }
}