using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using PowerBiTools.Dto;

namespace ConsoleApplication1
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Provide *.pbit or *.pbix file name as the first argument and output directory as the second argument.");
                return;
            }
            
            var inputFile = args[0];
            var outputDir = args[1];
            
            var schema = Read(inputFile);
            Write(schema, outputDir);
        }

        private static DataModelSchema Read(string path)
        {
            using (var archive = ZipFile.OpenRead(path))
            using (var stream = archive.GetEntry("DataModelSchema").Open())
            using (var file = new StreamReader(stream, Encoding.Unicode))
            {
                var serializer = new JsonSerializer();
                return (DataModelSchema)serializer.Deserialize(file, typeof(DataModelSchema));
            }
        }

        private static void Write(DataModelSchema schema, string outputDir)
        {
            foreach (var table in schema.Model.Tables.Where(t => t.Measures != null))
            {
                var path = $"{outputDir}/tables/{table.Name}/measures";
                Directory.CreateDirectory(path);
                
                foreach (var measure in table.Measures)
                {
                    File.WriteAllText($"{path}/{measure.Name}", measure.Expression);
                }
            }
        }
    }
}