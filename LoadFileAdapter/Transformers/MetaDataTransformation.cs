using System;
using System.IO;
using System.Text.RegularExpressions;

namespace LoadFileAdapter.Transformers
{
    /// <summary>
    /// Contains the settings to perform a transformation on the metadata 
    /// of a <see cref="Document"/>.
    /// </summary>
    public class MetaDataTransformation : Transformation
    {
        private string fieldName = String.Empty;        
        private string alternateDestinationField = String.Empty;
        private string prependField = String.Empty;
        private string appendField = String.Empty;
        private string joinDelimiter = String.Empty;        
        private DirectoryInfo prependDirectory = null;
        
        /// <summary>
        /// The target field name.
        /// </summary>
        public string FieldName { get { return fieldName; } }

        /// <summary>
        /// The field name of an optional alternate destination for the 
        /// modified value.
        /// </summary>
        public string AlternateDestinationField { get { return alternateDestinationField; } }

        /// <summary>
        /// The field name of a field that should be prepended to the 
        /// target field value.
        /// </summary>
        public string PrependField { get { return prependField; } }

        /// <summary>
        /// The field name of a field that should be appended to the 
        /// target field value.
        /// </summary>
        public string AppendField { get { return appendField; } }

        /// <summary>
        /// The delimiter used to join a preprend or append value to the 
        /// target field.
        /// </summary>
        public string JoinDelimiter { get { return joinDelimiter; } }

        /// <summary>
        /// A root path that is prepended to a relational path in order to 
        /// convert it to an absolute path.
        /// </summary>
        public DirectoryInfo PrependDirectory { get { return prependDirectory; } }

        /// <summary>
        /// Initializes a new instance of <see cref="MetaDataTransformation"/>.
        /// </summary>
        /// <param name="fieldName">The target field.</param>
        /// <param name="findText">The regex for a find / replace operation.</param>
        /// <param name="replaceText">The replacement text used with the find text regex.</param>
        /// <param name="alternateDestinationField">An optional alt destination for the modified value.</param>
        /// <param name="prependField">The field containing a value to be prepended to the value of the target field.</param>
        /// <param name="appendField">The field contains a value to be appended to hte value of the target field.</param>
        /// <param name="joinDelimiter">The delimiter used to join the preprend or append data.</param>
        /// <param name="filterField">The filter field used to determine if a document should be edited.</param>
        /// <param name="filterText">The filter regex used to determine if a filter field is is match and should be edited.</param>
        /// <param name="prependDirectory">Used to prepend an existing relational path in order to convert it to an absolute path.</param>
        public MetaDataTransformation( string fieldName,
            Regex findText, string replaceText,
            string alternateDestinationField, string prependField, 
            string appendField, string joinDelimiter,
            string filterField, Regex filterText, 
            DirectoryInfo prependDirectory) :
            base(findText, replaceText, filterField, filterText)
        {
            this.fieldName = fieldName;            
            this.alternateDestinationField = alternateDestinationField;
            this.prependField = prependField;
            this.appendField = appendField;
            this.joinDelimiter = joinDelimiter;            
            this.prependDirectory = prependDirectory;
        }
         
        /// <summary>
        /// Modifies the supplied <see cref="Document"/>. If the doc does not 
        /// contains the target field name that field is added to the doc's
        /// metadata. The find/replace is performed, followed by append, 
        /// preprend, preprend directory, The the destination is set with the 
        /// new value.
        /// </summary>
        /// <param name="doc">the document to be modified.</param>                               
        public override void Transform(Document doc)
        {
            if (!doc.Metadata.ContainsKey(this.fieldName))
            {
                doc.Metadata.Add(this.fieldName, String.Empty);
            }

            if (!String.IsNullOrWhiteSpace(this.alternateDestinationField) && 
                !doc.Metadata.ContainsKey(this.alternateDestinationField))
            {
                doc.Metadata.Add(this.alternateDestinationField, String.Empty);
            }

            if (base.hasEdit(doc))
            {
                // get orig value
                string value = doc.Metadata[FieldName];

                // perform find / replace
                value = base.Replace(value);

                // append if set
                if (!String.IsNullOrWhiteSpace(AppendField))
                {
                    value = value + JoinDelimiter + doc.Metadata[AppendField];
                }

                // prepend if set
                if (!String.IsNullOrWhiteSpace(PrependField))
                {
                    value = doc.Metadata[PrependField] + JoinDelimiter + value;
                }

                // preprend with file path if set
                if (PrependDirectory != null)
                {
                    value = Path.Combine(PrependDirectory.FullName, value.TrimStart('\\'));
                }

                // update destination
                if (!String.IsNullOrWhiteSpace(AlternateDestinationField))
                {
                    doc.Metadata[AlternateDestinationField] = value;
                }
                else
                {
                    doc.Metadata[fieldName] = value;
                }
            }
        }        
    }
}
