using System.Collections.Generic;

namespace LoadFileAdapter.Parsers
{
    public interface IParser<T, S, R> 
        where T: ParseFileSettings
        where S: ParseReaderSettings
        where R: ParseLineSettings
    {
        List<string[]> Parse(T args);
        List<string[]> Parse(S args);
        string[] ParseLine(R args);
    }
}
