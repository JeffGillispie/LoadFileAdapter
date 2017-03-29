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
        public new MetaDataTransformation GetEdit()
        {
            return new MetaDataTransformation(
                this.FieldName, base.FindText, base.ReplaceText,
                this.AlternateDestinationField, this.PrependField, 
                this.AppendField, this.JoinDelimiter,
                base.FilterField, base.FilterText, this.PrependDirectory);
        }
    }
}
