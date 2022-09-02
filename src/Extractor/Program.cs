using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using Extractor.Dto;

namespace Extractor
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: dotnet Extractor.dll report_file.pbi[xt] output_dir");
                return;
            }

            var inputFile = args[0];
            var outputDir = args[1];

            var (schema, layout) = Read(inputFile);

            if (schema != null)
            {
                var extracts = Extractor.GetSchemaExtracts(schema);
                Write(extracts, outputDir);
            }

            if (layout != null)
            {
                var extract = Extractor.GetLayoutExtract(layout);
                Write(new[] { extract }, outputDir);
            }
        }

        public static (DataModelSchema dataModelSchema, object layout) Read(string path)
        {
            DataModelSchema dataModelSchema = null;
            object layout = null;

            using var archive = ZipFile.OpenRead(path);

            var schemaEntry = archive.GetEntry("DataModelSchema");
            if (schemaEntry != null)
            {
                using var stream = schemaEntry.Open();
                using var reader = new StreamReader(stream, Encoding.Unicode);
                dataModelSchema = GetSchema(reader);
            }

            var layoutEntry = archive.GetEntry("Report/Layout");
            if (layoutEntry != null)
            {
                using var stream = layoutEntry.Open();
                using var reader = new StreamReader(stream, Encoding.Unicode);
                layout = GetLayout(reader);
            }

            return (dataModelSchema, layout);
        }

        public static DataModelSchema GetSchema(StreamReader reader)
        {
            var serializer = new JsonSerializer();
            return (DataModelSchema)serializer.Deserialize(reader, typeof(DataModelSchema));
        }

        public static object GetLayout(StreamReader reader)
        {
            return JsonConvert.DeserializeObject(reader.ReadToEnd());
        }

        public static void Write(IEnumerable<Extract> extracts, string outputDir)
        {
            foreach (var extract in extracts)
            {
                var directory = $"{outputDir}/{extract.File.RelativePath}";
                var contents = extract.Content + Environment.NewLine;
                Directory.CreateDirectory(directory);
                System.IO.File.WriteAllText($"{directory}/{extract.File.FileName}", contents);
            }
        }
    }
}