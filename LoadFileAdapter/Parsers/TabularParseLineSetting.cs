
namespace LoadFileAdapter.Parsers
{
    public class TabularParseLineSetting : ParseLineSetting
    {
        private Delimiters delimiters;

        public Delimiters Delimiters { get { return delimiters; } }

        public TabularParseLineSetting(string line, Delimiters delimiters) : base(line)
        {
            this.delimiters = delimiters;
        }
    }
}
