using LoadFileAdapter.Exporters;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// The settings used to create a hyperlink in an excel export.
    /// </summary>
    public class Hyperlink
    {
        /// <summary>
        /// The representative type to link.
        /// </summary>
        public Representative.FileType FileType;

        /// <summary>
        /// The display text of the link.
        /// </summary>
        public string DisplayText;

        /// <summary>
        /// The position of hyperlink.
        /// </summary>
        public int ColumnIndex;

        /// <summary>
        /// Initializes a new instance of <see cref="Hyperlink"/>.
        /// </summary>
        public Hyperlink()
        {
            // do nothing here
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Hyperlink"/>.
        /// </summary>
        /// <param name="link">The settings used to build this object.</param>
        public Hyperlink(HyperLinkInfo link)
        {
            this.FileType = link.GetFileType();
            this.DisplayText = link.GetDisplayText();
            this.ColumnIndex = link.GetColumnIndex();
        }

        /// <summary>
        /// Gets a <see cref="HyperLinkInfo"/> value of this object.
        /// </summary>
        /// <returns>Returns <see cref="HyperLinkInfo"/>.</returns>
        public HyperLinkInfo GetLinkSettings()
        {
            return new HyperLinkInfo(
                this.FileType, this.DisplayText, this.ColumnIndex);
        }

        public override string ToString()
        {
            return string.Format("{0} Link - {1} {2}", FileType, ColumnIndex, DisplayText);
        }
    }
}
