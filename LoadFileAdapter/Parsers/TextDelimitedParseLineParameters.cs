
namespace LoadFileAdapter.Parsers
{
    public class TextDelimitedParseLineParameters : ParseLineParameters
    {
        private Delimiters delimiters;

        public Delimiters Delimiters { get { return delimiters; } }

        public TextDelimitedParseLineParameters(string line, Delimiters delimiters) : base(line)
        {
            this.delimiters = delimiters;
        }
    }
}
