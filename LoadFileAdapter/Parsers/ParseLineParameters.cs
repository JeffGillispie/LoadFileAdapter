
namespace LoadFileAdapter.Parsers
{
    public class ParseLineParameters
    {
        private string line;

        public string Line { get { return line; } }

        public ParseLineParameters(string line)
        {
            this.line = line;
        }
    }
}
