using LoadFileAdapter.Builders;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// Contains instructions for creating a <see cref="Representative"/> 
    /// from a DAT file. This is a wrapper for <see cref="RepresentativeBuilder"/>.
    /// It is also used to serialize instructions and deserialize instructions from XML.
    /// </summary>
    public class RepresentativeInfo
    {
        /// <summary>
        /// The name of the metadata column in a DAT file that contains
        /// the file path to the <see cref="Representative"/> this 
        /// instruction will create.
        /// </summary>
        public string ColumnName = null;

        /// <summary>
        /// The <see cref="RepresentativeInfo.FileType"/> of this 
        /// <see cref="Representative"/> this instruction will create.
        /// </summary>
        public Representative.FileType FileType = Representative.FileType.Native;

        /// <summary>
        /// Initializes a new instance of <see cref="RepresentativeInfo"/>.
        /// </summary>
        public RepresentativeInfo()
        {
            // do nothing here
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RepresentativeInfo"/>.
        /// </summary>
        /// <param name="setting">The settings used to build this object.</param>
        public RepresentativeInfo(RepresentativeBuilder setting)
        {
            this.ColumnName = setting.ColumnName;
            this.FileType = setting.Type;
        }

        /// <summary>
        /// Gets the <see cref="RepresentativeBuilder"/> value of this object.
        /// </summary>
        /// <returns>Returns a <see cref="RepresentativeBuilder"/>.</returns>
        public RepresentativeBuilder GetBuilder()
        {
            return new RepresentativeBuilder(this.ColumnName, this.FileType);
        }
    }
}
