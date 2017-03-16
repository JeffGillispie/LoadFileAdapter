using System.Collections.Generic;

namespace LoadFileAdapter.Parsers
{
    public interface Parser<T, S, R> 
        where T: ParseFileSetting
        where S: ParseReaderSetting
        where R: ParseLineSetting
    {
        List<string[]> Parse(T args);
        List<string[]> Parse(S args);
        string[] ParseLine(R args);
    }
}
