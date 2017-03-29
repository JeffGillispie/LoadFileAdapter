using System.IO;
using System.Text;

namespace LoadFileAdapter.Parsers
{
    /// <summary>
    /// The settings used to parse a DAT file.
    /// </summary>
    public class ParseFileDatSettings : ParseFileSettings
    {
        private Delimiters delimiters;

        /// <summary>
        /// The delimiters used to read the text delimited file.
        /// </summary>
        public Delimiters Delimiters { get { return delimiters; } }

        /// <summary>
        /// Initializes a new instance of <see cref="ParseFileDatSettings"/>.
        /// </summary>
        /// <param name="file">The file to be parsed.</param>
        /// <param name="encoding">The encoding used to read the file.</param>
        /// <param name="delimiters">The delimiters used to read the file.</param>
        public ParseFileDatSettings(
            FileInfo file, Encoding encoding, Delimiters delimiters) : 
            base(file, encoding)
        {
            this.delimiters = delimiters;
        }
    }
}
