using System.Collections.Generic;

namespace LoadFileAdapter.Parsers
{
    public interface Parser<T, S, R> 
        where T: ParseFileParameters
        where S: ParseReaderParameters
        where R: ParseLineParameters
    {
        List<string[]> Parse(T parameters);
        List<string[]> Parse(S parameters);
        string[] ParseLine(R parameters);
    }
}
