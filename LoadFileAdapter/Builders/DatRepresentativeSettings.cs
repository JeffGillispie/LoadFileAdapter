
namespace LoadFileAdapter.Builders
{
    /// <summary>
    /// Represents the settings used to identify a document representative from a DAT file.
    /// </summary>
    public class DatRepresentativeSettings
    {
        private string column;
        private Representative.FileType fileType;

        /// <summary>
        /// The name of the column containing the representative file path.
        /// </summary>
        public string ColumnName { get { return this.column; } }

        /// <summary>
        /// The file type of the document representative.
        /// </summary>
        public Representative.FileType Type { get { return this.fileType; } }

        /// <summary>
        /// Initializes a new instance of <see cref="DatRepresentativeSettings"/>.
        /// </summary>
        /// <param name="column">The name of the field containing the representative file path.</param>
        /// <param name="type">The file type of the representative.</param>
        public DatRepresentativeSettings(string column, Representative.FileType type)
        {
            this.column = column;
            this.fileType = type;
        }
    }
}
