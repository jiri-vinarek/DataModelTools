using System.IO;
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
            var tableName = measure.DisplayFolder == null ? table.Name : $"{table.Name}/{measure.DisplayFolder}";
            var path = Sanitize($"tables/{tableName}/measures");
            var fileName = Sanitize($"{measure.Name}.dax");

            return new File(path, fileName);
        }

        public static File FromColumn(Table table, Column column)
        {
            var tableName = column.DisplayFolder == null ? table.Name : $"{table.Name}/{column.DisplayFolder}";
            var path = Sanitize($"tables/{tableName}/columns");
            var fileName = Sanitize($"{column.Name}.dax");

            return new File(path, fileName);
        }

        public static File FromPartition(Table table, Partition partition)
        {
            var path = Sanitize($"tables/{table.Name}/partitions");
            var fileName = Sanitize($"{partition.Name}.{partition.Source.Type}");

            return new File(path, fileName);
        }

        public static File FromExpression(Expression expression)
        {
            var fileName = Sanitize($"{expression.Name}.{expression.Kind}");

            return new File("expressions", fileName);
        }

        private static string Sanitize(string path, string replacement = "_")
        {
            foreach (var badChar in Path.GetInvalidFileNameChars())
            {
                path = path.Replace(badChar.ToString(), replacement);
            }

            return path;
        }
    }
}