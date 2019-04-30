using System.IO;
using System.Xml.Serialization;
using LoadFileAdapter.Transformers;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// Contains the instructions for editing metadata.
    /// It is a wrapper for <see cref="MetaDataTransformation"/>.
    /// It is intended to be used to serialize instructions and
    /// deserialize instruction from XML.
    /// </summary>
    public class MetaDataEdit : Edit
    {
        /// <summary>
        /// The name of the metadata field that is the subject
        /// of this edit.
        /// </summary>
        public string FieldName = null;

        /// <summary>
        /// The name of a metadata field that should be the 
        /// destination of the value which results from the 
        /// metadata edit.
        /// </summary>
        public string AlternateDestinationField = null;

        /// <summary>
        /// The name of a metadata field that contains a value
        /// which should be preprended to the value in the metadata
        /// field identified by the FieldName value.
        /// </summary>
        public string PrependField = null;

        /// <summary>
        /// The name of a metadata field that contains a value
        /// which should be appended to the value in the metadata
        /// field identified by the FieldName value.
        /// </summary>
        public string AppendField = null;

        /// <summary>
        /// The delimiter used in conjuction with the preprend field or
        /// append field to combine values from two metadata fields.
        /// </summary>
        public string JoinDelimiter = null;

        /// <summary>
        /// Used to prepend a relative path with a root path
        /// in order to convert it to an absolute path.
        /// </summary>
        [XmlIgnore]
        public DirectoryInfo PrependDirectory = null;

        /// <summary>
        /// Initializes a new instance of <see cref="MetaDataEdit"/>.
        /// </summary>
        public MetaDataEdit() : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="MetaDataEdit"/>.
        /// </summary>
        /// <param name="edit">The edit initialization settings.</param>
        public MetaDataEdit(MetaDataTransformation edit)
        {            
            base.FilterField = edit.FilterField;
            base.FilterText = edit.FilterText;
            base.FindText = edit.FindText;
            base.ReplaceText = edit.ReplaceText;
            this.FieldName = edit.FieldName;
            this.AlternateDestinationField = edit.AlternateDestinationField;
            this.PrependField = edit.PrependField;
            this.AppendField = edit.AppendField;
            this.JoinDelimiter = edit.JoinDelimiter;
            this.PrependDirectory = edit.PrependDirectory;
        }

        /// <summary>
        /// Used to prepend a relative path with a root path to 
        /// convert the path to an absolute path.
        /// </summary>
        public string PrependDirectoryPath
        {
            get
            {
                if (this.PrependDirectory == null)
                    return null;
                else
                    return this.PrependDirectory.FullName;
            }

            set
            {
                if (value == null)
                    this.PrependDirectory = null;
                else
                    this.PrependDirectory = new DirectoryInfo(value);
            }
        }

        /// <summary>
        /// Get the <see cref="MetaDataTransformation"/> value of the edit.
        /// </summary>
        /// <returns>Returns a <see cref="MetaDataTransformation"/>.</returns>
        public override Transformation ToTransformation()
        {
            return MetaDataTransformation.Builder
                .Start(FieldName, FindText, ReplaceText, FilterField, FilterText)
                .SetAltDestinationField(AlternateDestinationField)
                .SetAppendField(AppendField)
                .SetPrependField(PrependField)
                .SetJoinDelimiter(JoinDelimiter)
                .SetPrependDir(PrependDirectory)
                .Build();            
        }

        public override string ToString()
        {
            string s = $"MetaDataEdit for \"{this.FieldName}\", [{this.FindTextPattern}] >> [{this.ReplaceText}]";

            if (!string.IsNullOrEmpty(this.FilterTextPattern))
                s = s + $" on ({this.FilterTextPattern})";

            if (!string.IsNullOrEmpty(this.FilterField))
                s = s + $" in the \"{this.FilterField}\" field";

            if (!string.IsNullOrEmpty(this.AlternateDestinationField))
                s = s + $", to the \"{this.AlternateDestinationField}\" field";

            if (!string.IsNullOrEmpty(this.PrependDirectoryPath))
                s = s + $", prepended by [{this.PrependDirectoryPath}]";

            if (!string.IsNullOrEmpty(this.PrependField))
                s = s + $", prepended by the \"{this.PrependField}\" field";

            if (!string.IsNullOrEmpty(this.AppendField))
                s = s + $", appended by the \"{this.AppendField}\" field";

            if (!string.IsNullOrEmpty(this.JoinDelimiter))
                s = s + $", joined by [{this.JoinDelimiter}]";

            return s;
        }
    }
}
