using System;
using System.IO;
using System.Text.RegularExpressions;

namespace LoadFileAdapter.Transformers
{
    public class MetaDataEdit
    {
        private string fieldName = String.Empty;
        private Regex findText = null;
        private string replaceText = String.Empty;
        private bool useRegex = false;        
        private string alternateDestinationField = String.Empty;
        private string prependField = String.Empty;
        private string appendField = String.Empty;
        private string joinDelimiter = String.Empty;
        private string filterField = String.Empty;
        private Regex filterText = null;
        private bool filterTextIsRegex = false;                        
        private DirectoryInfo prependDirectory = null;

        public string FieldName { get { return fieldName; } }
        public Regex FindText { get { return findText; } }
        public string ReplaceText { get { return replaceText; } }
        public bool UseRegex { get { return useRegex; } }        
        public string AlternateDestinationField { get { return alternateDestinationField; } }
        public string PrependField { get { return prependField; } }
        public string AppendField { get { return appendField; } }
        public string JoinDelimiter { get { return joinDelimiter; } }
        public string FilterField { get { return filterField; } }
        public Regex FilterText { get { return filterText; } }
        public bool FilterTextIsRegex { get { return filterTextIsRegex; } }                
        public DirectoryInfo PrependDirectory { get { return prependDirectory; } }

        public MetaDataEdit( string fieldName,
            Regex findText, string replaceText, bool useRegex,
            string alternateDestinationField, string prependField, string appendField, string joinDelimiter,
            string filterField, Regex filterText, bool filterTextIsRegex, DirectoryInfo prependDirectory)
        {
            this.fieldName = fieldName;
            this.findText = findText;
            this.replaceText = replaceText;
            this.useRegex = useRegex;            
            this.alternateDestinationField = alternateDestinationField;
            this.prependField = prependField;
            this.appendField = appendField;
            this.joinDelimiter = joinDelimiter;
            this.filterText = filterText;
            this.filterTextIsRegex = filterTextIsRegex;
            this.filterField = filterField;            
            this.prependDirectory = prependDirectory;
        }
        
        public string Replace(string value)
        {
            if (useRegex == false && findText != null)
            {
                return value.Replace(findText.ToString(), replaceText);
            }
            else if (findText != null)
            {
                return findText.Replace(value, replaceText);                
            }
            else
            {
                return value;
            }
        }
        
        public void Transform(Document doc)
        {
            if (!doc.Metadata.ContainsKey(this.fieldName))
            {
                doc.Metadata.Add(this.fieldName, String.Empty);
            }

            if (hasEdit(doc))
            {
                // get orig value
                string value = doc.Metadata[FieldName];

                // perform find / replace
                value = Replace(value);

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

        protected bool hasEdit(Document doc)
        {
            if (FilterField != null && !String.IsNullOrWhiteSpace(FilterField) && doc.Metadata.ContainsKey(FilterField) && FilterText != null)
            {                
                if (FilterTextIsRegex)
                {
                    if (FilterText.IsMatch(doc.Metadata[FilterField]) == false)
                    {
                        return false;
                    }
                }
                else
                {
                    if (doc.Metadata[FilterField].Equals(FilterText.ToString()) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
