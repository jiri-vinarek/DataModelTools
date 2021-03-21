using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using Extractor.Dto;

namespace Extractor
{
    internal sealed class Program
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

            var schema = Read(inputFile);
            
            if (schema != null)
            {
                var extracts = Extractor.GetExtracts(schema);
                Write(extracts, outputDir);
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

        private static void Write(IEnumerable<Extract> extracts, string outputDir)
        {
            foreach (var extract in extracts)
            {
                var directory = $"{outputDir}/{extract.File.RelativePath}";
                Directory.CreateDirectory(directory);
                System.IO.File.WriteAllText($"{directory}/{extract.File.FileName}", extract.Content);
            }
        }
    }
}