
namespace LoadFileAdapter.Parsers
{
    public class ParseLineSettings
    {
        private string line;

        public string Line { get { return line; } }

        public ParseLineSettings(string line)
        {
            this.line = line;
        }
    }
}
