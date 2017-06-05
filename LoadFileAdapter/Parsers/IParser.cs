using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LoadFileAdapter.Parsers
{
    /// <summary>
    /// Parses data from a delimited text file.
    /// </summary>
    public interface IParser        
    {
        /// <summary>
        /// Parses a file.
        /// </summary>
        /// <param name="file">The file to parse.</param>
        /// <param name="encoding">The encoding of the file.</param>
        /// <returns>Returns a list of parsed fields.</returns>
        List<string[]> Parse(FileInfo file, Encoding encoding);

        /// <summary>
        /// Parses data from a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="reader">The reader used to read the text to parse.</param>
        /// <returns>Returns a list of parsed fields.</returns>
        List<string[]> Parse(TextReader reader);

        /// <summary>
        /// Parses a line.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>Returns the parsed fields.</returns>
        string[] ParseLine(string line);
    }
}
