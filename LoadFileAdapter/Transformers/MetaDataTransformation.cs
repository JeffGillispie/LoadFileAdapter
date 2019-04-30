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

        private MetaDataTransformation(Regex findText, string replaceText, string filterField, Regex filterText) 
            : base(findText, replaceText, filterField, filterText)
        {
            // do nothing here
        }
        
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
        
        public class Builder
        {
            private MetaDataTransformation instance;

            private Builder(Regex findText, string replaceText, string filterField, Regex filterText)
            {
                instance = new MetaDataTransformation(findText, replaceText, filterField, filterText);
            }

            public static Builder Start(string fieldName, Regex findText, string replaceText, 
                string filterField, Regex filterText)
            {
                Builder builder =  new Builder(findText, replaceText, filterField, filterText);
                builder.instance.fieldName = fieldName;
                return builder;
            }

            public static Builder Start(string fieldName, Regex findText, string replaceText)
            {
                Builder builder = new Builder(findText, replaceText, null, null);
                builder.instance.fieldName = fieldName;
                return builder;
            }

            public Builder SetFilterField(string value)
            {
                var instance = new MetaDataTransformation(
                    this.instance.FindText,
                    this.instance.ReplaceText,
                    value,
                    this.instance.FilterText);
                instance.alternateDestinationField = this.instance.alternateDestinationField;
                instance.appendField = this.instance.appendField;
                instance.fieldName = this.instance.fieldName;
                instance.joinDelimiter = this.instance.joinDelimiter;
                instance.prependDirectory = this.instance.prependDirectory;
                instance.prependField = this.instance.prependField;
                this.instance = instance;
                return this;
            }

            public Builder SetFilterText(Regex value)
            {
                var instance = new MetaDataTransformation(
                    this.instance.FindText,
                    this.instance.ReplaceText,
                    this.instance.FilterField,
                    value);
                instance.alternateDestinationField = this.instance.alternateDestinationField;
                instance.appendField = this.instance.appendField;
                instance.fieldName = this.instance.fieldName;
                instance.joinDelimiter = this.instance.joinDelimiter;
                instance.prependDirectory = this.instance.prependDirectory;
                instance.prependField = this.instance.prependField;
                this.instance = instance;
                return this;
            }
                        
            public Builder SetAltDestinationField(string value)
            {
                instance.alternateDestinationField = value;
                return this;
            }

            public Builder SetPrependField(string value)
            {
                instance.prependField = value;
                return this;
            }

            public Builder SetAppendField(string value)
            {
                instance.appendField = value;
                return this;
            }

            public Builder SetJoinDelimiter(string value)
            {
                instance.joinDelimiter = value;
                return this;
            }

            public Builder SetPrependDir(DirectoryInfo value)
            {
                instance.prependDirectory = value;
                return this;
            }

            public MetaDataTransformation Build()
            {
                MetaDataTransformation instance = this.instance;
                this.instance = null;
                return instance;
            }
        }        
    }
}
