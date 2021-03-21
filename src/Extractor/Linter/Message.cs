namespace Extractor.Linter
{
    public sealed class Message
    {
        public Message(MessageSeverity messageSeverity, File file, int line, int column, string text)
        {
            MessageSeverity = messageSeverity;
            File = file;
            Line = line;
            Column = column;
            Text = text;
        }

        public MessageSeverity MessageSeverity { get; }
        public File File { get; }
        public int Line { get; }
        public int Column { get; }
        public string Text { get; }
    }
}