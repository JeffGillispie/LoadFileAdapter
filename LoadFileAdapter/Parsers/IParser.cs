using System.Collections.Generic;

namespace LoadFileAdapter.Parsers
{
    /// <summary>
    /// The interface for a parser to parse data from a text delimited file.
    /// </summary>
    /// <typeparam name="T">Settings to parse from a file.</typeparam>
    /// <typeparam name="S">Settings to parse from a text reader."/></typeparam>
    /// <typeparam name="R">Settings to parse a single line.</typeparam>
    public interface IParser<T, S, R> 
        where T: ParseFileSettings
        where S: ParseReaderSettings
        where R: ParseLineSettings
    {
        /// <summary>
        /// Parses a list of records from text delimited data.
        /// </summary>
        /// <param name="args">The <see cref="ParseFileSettings"/> used to 
        /// parse text delimited data from a file.</param>
        /// <returns>Returns a list of parsed lines.</returns>
        List<string[]> Parse(T args);

        /// <summary>
        /// Parses a list of records from text delimited data.
        /// </summary>
        /// <param name="args">The <see cref="ParseReaderSettings"/> used to 
        /// parse data.</param>
        /// <returns>Returns a list of parsed lines.</returns>
        List<string[]> Parse(S args);

        /// <summary>
        /// Parses a line into an array of fields.
        /// </summary>
        /// <param name="args">The <see cref="ParseLineSettings"/> used to 
        /// parse data.</param>
        /// <returns>Returns an array of parsed fields.</returns>
        string[] ParseLine(R args);
    }
}
