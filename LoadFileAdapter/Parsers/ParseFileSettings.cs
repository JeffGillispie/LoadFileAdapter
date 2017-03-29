using System.IO;
using System.Text;

namespace LoadFileAdapter.Parsers
{
    /// <summary>
    /// The settings used to parse a generic file.
    /// </summary>
    public class ParseFileSettings
    {
        private FileInfo file;
        private Encoding encoding;

        /// <summary>
        /// The file to be parsed.
        /// </summary>
        public FileInfo File { get { return file; } }

        /// <summary>
        /// The encoding used to read the input file.
        /// </summary>
        public Encoding Encoding { get { return encoding; } }

        /// <summary>
        /// Initializes a new instance of <see cref="ParseFileSettings"/>.
        /// </summary>
        /// <param name="file">The file to be parsed.</param>
        /// <param name="encoding">The encoding used to read the file.</param>
        public ParseFileSettings(FileInfo file, Encoding encoding)
        {
            this.file = file;
            this.encoding = encoding;
        }
    }
}
