
namespace LoadFileAdapter.Parsers
{
    public class ParseLineSetting
    {
        private string line;

        public string Line { get { return line; } }

        public ParseLineSetting(string line)
        {
            this.line = line;
        }
    }
}
