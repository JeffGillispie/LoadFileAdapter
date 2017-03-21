
namespace LoadFileAdapter.Parsers
{
    public class ParseLineDatSettings : ParseLineSettings
    {
        private Delimiters delimiters;

        public Delimiters Delimiters { get { return delimiters; } }

        public ParseLineDatSettings(string line, Delimiters delimiters) : base(line)
        {
            this.delimiters = delimiters;
        }
    }
}
