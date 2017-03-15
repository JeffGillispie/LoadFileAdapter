using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Parsers
{
    public interface Parser
    {
        List<string[]> Parse(FileInfo file, Delimiters delimiters, Encoding encoding);

        string[] ParseLine(string line, Delimiters delimiters);
    }
}
