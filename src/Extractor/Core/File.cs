using Extractor.Dto;

namespace Extractor
{
    internal sealed class File
    {
        public string RelativePath { get; }
        public string FileName { get; }

        private File(string relativePath, string fileName)
        {
            RelativePath = relativePath;
            FileName = fileName;
        }

        public static File FromMeasure(Table table, Measure measure)
        {
            var tableName = GetSanitizedTableName(measure.DisplayFolder, table.Name);
            var path = $"tables/{tableName}/measures";
            var fileName = Sanitize($"{measure.Name}.dax");

            return new File(path, fileName);
        }

        public static File FromColumn(Table table, Column column)
        {
            var tableName = GetSanitizedTableName(column.DisplayFolder, table.Name);
            var path = $"tables/{tableName}/columns";
            var fileName = Sanitize($"{column.Name}.dax");

            return new File(path, fileName);
        }

        private static string GetSanitizedTableName(string displayFolder, string tableName)
        {
            var sanitizedTableName = Sanitize(tableName);

            return displayFolder == null ? sanitizedTableName : $"{sanitizedTableName}/{Sanitize(displayFolder)}";
        }

        public static File FromPartition(Table table, Partition partition)
        {
            var path = $"tables/{Sanitize(table.Name)}/partitions";
            var fileName = Sanitize($"{partition.Name}.{partition.Source.Type}");

            return new File(path, fileName);
        }

        public static File FromExpression(Expression expression)
        {
            var fileName = Sanitize($"{expression.Name}.{expression.Kind}");

            return new File("expressions", fileName);
        }

        private static string Sanitize(string fileName, string replacement = "_")
        {
            foreach (var badChar in InvalidFileNameChars)
            {
                fileName = fileName.Replace(badChar.ToString(), replacement);
            }

            return fileName;
        }

        private static readonly char[] InvalidFileNameChars =
        {
            '\"', '<', '>', '|', '\0',
            (char) 1, (char) 2, (char) 3, (char) 4, (char) 5, (char) 6, (char) 7, (char) 8, (char) 9, (char) 10,
            (char) 11, (char) 12, (char) 13, (char) 14, (char) 15, (char) 16, (char) 17, (char) 18, (char) 19,
            (char) 20,
            (char) 21, (char) 22, (char) 23, (char) 24, (char) 25, (char) 26, (char) 27, (char) 28, (char) 29,
            (char) 30,
            (char) 31, ':', '*', '?', '\\', '/'
        };
    }
}