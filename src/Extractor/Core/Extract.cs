namespace Extractor
{
    internal sealed class Extract
    {
        public Extract(File file, string content)
        {
            File = file;
            Content = content;
        }

        public File File { get; }
        public string Content { get; }
    }
}