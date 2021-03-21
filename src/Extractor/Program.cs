using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using Extractor.Dto;
using Extractor.Linter;

namespace Extractor
{
    internal sealed class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 3 || args[0] != "extract" && args[0] != "lint")
            {
                Console.WriteLine("Usage: dotnet Extractor.dll (extract|lint) report_file.pbi[xt] output_dir");
                return;
            }

            var inputFile = args[1];
            var outputDir = args[2];

            var schema = Read(inputFile);
            
            if (schema != null)
            {
                if (args[0] == "extract")
                {
                    var extracts = Extractor.GetExtracts(schema);
                    Write(extracts, outputDir);
                }

                if (args[0] == "lint")
                {
                    var messages = Linter.Linter.Check(schema);
                    Output(messages, outputDir);
                }
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

        private static void Output(IEnumerable<Message> messages, string outputDir)
        {
            foreach (var message in messages)
            {
                Console.Out.WriteLine($"::{message.MessageSeverity} file={outputDir}/{message.File.RelativePath}/{message.File.FileName},line={message.Line},col={message.Column}::{message.Text}");
            }
        }
    }
}