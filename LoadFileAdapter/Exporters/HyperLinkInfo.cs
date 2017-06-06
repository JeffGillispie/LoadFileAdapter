
namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// Contains the settings to create a hyperlink in an excel export
    /// that links to a <see cref="Representative"/>.
    /// </summary>
    public class HyperLinkInfo
    {
        private Representative.FileType fileType;
        private string displayText;
        private int columnIndex;

        /// <summary>
        /// Initializes a new instance of <see cref="HyperLinkInfo"/>.
        /// </summary>
        /// <param name="fileType">The type of the file to link.</param>
        /// <param name="displayText">The display text to insert.</param>
        /// <param name="columnIndex">The column index of the destination link.</param>
        public HyperLinkInfo(Representative.FileType fileType, string displayText, int columnIndex)
        {
            this.fileType = fileType;
            this.displayText = displayText;
            this.columnIndex = columnIndex;
        }

        /// <summary>
        /// Gets the type of the representative to link.
        /// </summary>
        /// <returns>Returns the file type to link.</returns>
        public Representative.FileType GetFileType()
        {
            return this.fileType;
        }

        /// <summary>
        /// Gets the display text of the hyperlink. If there is no text
        /// then the value of the field at the column index is used. If
        /// there is a text value then a display column is inserted.
        /// </summary>
        /// <returns>Returns the display text.</returns>
        public string GetDisplayText()
        {
            return this.displayText;
        }

        /// <summary>
        /// Gets the column index of the hyperlink.
        /// </summary>
        /// <returns>Returns the index of the destination column.</returns>
        public int GetColumnIndex()
        {
            return this.columnIndex;
        }
    }
}
