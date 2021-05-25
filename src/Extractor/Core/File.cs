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
            var path = SanitizePath($"tables/{tableName}/measures");
            var fileName = SanitizeFileName($"{measure.Name}.dax");

            return new File(path, fileName);
        }

        public static File FromColumn(Table table, Column column)
        {
            var tableName = column.DisplayFolder == null ? table.Name : $"{table.Name}/{column.DisplayFolder}";
            var path = SanitizePath($"tables/{tableName}/columns");
            var fileName = SanitizeFileName($"{column.Name}.dax");

            return new File(path, fileName);
        }

        public static File FromPartition(Table table, Partition partition)
        {
            var path = SanitizePath($"tables/{table.Name}/partitions");
            var fileName = SanitizeFileName($"{partition.Name}.{partition.Source.Type}");

            return new File(path, fileName);
        }

        public static File FromExpression(Expression expression)
        {
            var fileName = SanitizeFileName($"{expression.Name}.{expression.Kind}");

            return new File("expressions", fileName);
        }
        
        private static string SanitizeFileName(string fileName, string replacement = "_")
        {
            foreach (var badChar in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(badChar.ToString(), replacement);
            }

            return fileName.Replace("\\", replacement);
        }

        private static string SanitizePath(string path, string replacement = "_")
        {
            foreach (var badChar in Path.GetInvalidPathChars())
            {
                path = path.Replace(badChar.ToString(), replacement);
            }

            return path.Replace("\\", "/");
        }
    }
}