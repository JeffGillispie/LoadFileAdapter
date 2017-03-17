using System;
using System.IO;
using System.Text.RegularExpressions;

namespace LoadFileAdapter.Transformers
{
    public class MetaDataEdit : Edit
    {
        private string fieldName = String.Empty;        
        private string alternateDestinationField = String.Empty;
        private string prependField = String.Empty;
        private string appendField = String.Empty;
        private string joinDelimiter = String.Empty;        
        private DirectoryInfo prependDirectory = null;

        public string FieldName { get { return fieldName; } }        
        public string AlternateDestinationField { get { return alternateDestinationField; } }
        public string PrependField { get { return prependField; } }
        public string AppendField { get { return appendField; } }
        public string JoinDelimiter { get { return joinDelimiter; } }        
        public DirectoryInfo PrependDirectory { get { return prependDirectory; } }

        public MetaDataEdit( string fieldName,
            Regex findText, string replaceText,
            string alternateDestinationField, string prependField, string appendField, string joinDelimiter,
            string filterField, Regex filterText, DirectoryInfo prependDirectory) :
            base(findText, replaceText, filterField, filterText)
        {
            this.fieldName = fieldName;            
            this.alternateDestinationField = alternateDestinationField;
            this.prependField = prependField;
            this.appendField = appendField;
            this.joinDelimiter = joinDelimiter;            
            this.prependDirectory = prependDirectory;
        }
                        
        public override void Transform(Document doc)
        {
            if (!doc.Metadata.ContainsKey(this.fieldName))
            {
                doc.Metadata.Add(this.fieldName, String.Empty);
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
