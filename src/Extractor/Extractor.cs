using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using PowerBiTools.Dto;

namespace Extractor
{
    internal sealed class Extractor
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine(
                    "Provide *.pbit or *.pbix file name as the first argument and output directory as the second argument.");
                return;
            }

            var inputFile = args[0];
            var outputDir = args[1];

            var schema = Read(inputFile);
            if (schema != null)
            {
                Write(schema, outputDir);
            }
        }

        private static DataModelSchema Read(string path)
        {
            using var archive = ZipFile.OpenRead(path);
            var schema = archive.GetEntry("DataModelSchema");
            if (schema == null)
            {
                return null;
            }

            using var stream = schema.Open();
            using var file = new StreamReader(stream, Encoding.Unicode);
            var serializer = new JsonSerializer();
            return (DataModelSchema) serializer.Deserialize(file, typeof(DataModelSchema));
        }

        private static void Write(DataModelSchema schema, string outputDir)
        {
            var tables = schema.Model.Tables.Where(t => !t.Name.StartsWith("DateTableTemplate") && !t.Name.StartsWith("LocalDateTable"));
            
            var measures = tables.Where(t => t.Measures != null).SelectMany(t => GetMeasures(t, outputDir));
            var columns = tables.Where(t => t.Columns != null).SelectMany(t => GetColumns(t, outputDir));
            var partitions = tables.Where(t => t.Partitions != null).SelectMany(t => GetPartitions(t, outputDir));

            foreach (var extract in measures.Concat(columns).Concat(partitions))
            {
                var path = SanitizePath(extract.Path);
                Directory.CreateDirectory(path);
                File.WriteAllText($"{path}/{SanitizeFileName(extract.FileName)}", extract.Content);
            }
        }

        private static IEnumerable<Extract> GetMeasures(Table table, string outputDir)
        {
            return table.Measures.Where(m => m.Expression != null && !m.Expression.StartsWith("EXTERNALMEASURE")).Select(m =>
            {
                var tableName = m.DisplayFolder != null ? table.Name : $"{table.Name}/{m.DisplayFolder}";
                
                return new Extract(
                    path: $"{outputDir}/tables/{tableName}/measures",
                    fileName: m.Name,
                    content: m.Expression
                );
            });
        }

        private static IEnumerable<Extract> GetColumns(Table table, string outputDir)
        {
            return table.Columns.Where(c => c.Type == "calculated").Select(c =>
            {
                var tableName = c.DisplayFolder != null ? table.Name : $"{table.Name}/{c.DisplayFolder}";
                
                return new Extract(
                    path: $"{outputDir}/tables/{tableName}/columns",
                    fileName: c.Name,
                    content: c.Expression
                );
            });
        }

        private static IEnumerable<Extract> GetPartitions(Table table, string outputDir)
        {
            return table.Partitions.Where(p => p.Source.Expression != null).Select(p =>
            {
                return new Extract(
                    path: $"{outputDir}/tables/{table.Name}/partitions",
                    fileName: $"{p.Name}.{p.Source.Type}",
                    content: ExpandEscaped(p.Source.Expression)
                );
            });
        }

        private static string SanitizeFileName(string fileName, string replacement = "_")
        {
            foreach (var badChar in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(badChar.ToString(), replacement);
            }

            return fileName;
        }

        private static string SanitizePath(string path, string replacement = "_")
        {
            foreach (var badChar in Path.GetInvalidPathChars())
            {
                path = path.Replace(badChar.ToString(), replacement);
            }

            return path;
        }

        private static string ExpandEscaped(string sourceExpression)
        {
            return sourceExpression.Replace("\n", Environment.NewLine)
                .Replace("#(lf)", Environment.NewLine)
                .Replace("#(tab)", "\t")
                .Replace("\\\"", "\"");
        }
    }

    internal sealed class Extract
    {
        public Extract(string path, string fileName, string content)
        {
            Path = path;
            FileName = fileName;
            Content = content;
        }

        public string Path { get; }
        public  string FileName { get; }
        public  string Content { get; }
    }
}