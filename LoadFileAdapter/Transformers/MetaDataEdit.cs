using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LoadFileAdapter.Transformers
{
    public class MetaDataEdit
    {
        private string fieldName = String.Empty;
        private string findText = String.Empty;
        private string replaceText = String.Empty;
        private bool useRegex = false;
        private bool useRegexMatchCase = false;
        private bool useRegexSearchRightToLeft;
        private string alternateDestinationField = String.Empty;
        private string prependField = String.Empty;
        private string appendField = String.Empty;
        private string joinDelimiter = String.Empty;
        private string filterText = String.Empty;
        private bool filterTextIsExactMatch = false;
        private string filterField = String.Empty;        
        private DirectoryInfo prependDirectory = null;

        public string FieldName { get { return fieldName; } }
        public string FindText { get { return findText; } }
        public string ReplaceText { get { return replaceText; } }
        public bool UseRegex { get { return useRegex; } }
        public bool UseRegexMatchCase { get { return useRegexMatchCase; } }
        public bool UseRegexSearchRightToLeft { get { return useRegexSearchRightToLeft; } }
        public string AlternateDestinationField { get { return alternateDestinationField; } }
        public string PrependField { get { return prependField; } }
        public string AppendField { get { return appendField; } }
        public string JoinDelimiter { get { return joinDelimiter; } }
        public string FilterText { get { return filterText; } }
        public bool FilterTextIsExactMatch { get { return filterTextIsExactMatch; } }
        public string FilterField { get { return filterField; } }        
        public DirectoryInfo PrependDirectory { get { return prependDirectory; } }

        public MetaDataEdit( string fieldName,
            string findText, string replaceText, bool useRegex, bool useRegexMatchCase, bool useRegexSearchRightToLeft,
            string alternateDestinationField, string prependField, string appendField, string joinDelimiter,
            string filterText, bool filterTextIsExactMatch, string filterField, DirectoryInfo prependDirectory)
        {
            this.fieldName = fieldName;
            this.findText = findText;
            this.replaceText = replaceText;
            this.useRegex = useRegex;
            this.useRegexMatchCase = useRegexMatchCase;
            this.useRegexSearchRightToLeft = useRegexSearchRightToLeft;
            this.alternateDestinationField = alternateDestinationField;
            this.prependField = prependField;
            this.appendField = appendField;
            this.joinDelimiter = joinDelimiter;
            this.filterText = filterText;
            this.filterTextIsExactMatch = filterTextIsExactMatch;
            this.filterField = filterField;            
            this.prependDirectory = prependDirectory;
        }

        public RegexOptions GetRegexOptions()
        {
            RegexOptions options = RegexOptions.None;

            if (UseRegexMatchCase && UseRegexSearchRightToLeft)
                options = RegexOptions.RightToLeft;
            else if (UseRegexMatchCase && !UseRegexSearchRightToLeft)
                options = RegexOptions.None;
            else if (!UseRegexMatchCase && UseRegexSearchRightToLeft)
                options = RegexOptions.IgnoreCase | RegexOptions.RightToLeft;
            else if (!UseRegexMatchCase && !UseRegexSearchRightToLeft)
                options = RegexOptions.IgnoreCase;

            return options;
        }

        public string Replace(string value)
        {
            if (useRegex == false && !String.IsNullOrEmpty(findText))
            {
                return value.Replace(findText, replaceText);
            }
            else if (!String.IsNullOrEmpty(findText))
            {
                RegexOptions options = GetRegexOptions();
                return Regex.Replace(value, findText, replaceText, options);
            }
            else
            {
                return value;
            }
        }
        
        public Document Transform(Document document)
        {
            Document doc = (Document)document.Clone();

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

            return doc;
        }

        private bool hasEdit(Document doc)
        {
            if (FilterField != null && !String.IsNullOrWhiteSpace(FilterField))
            {
                if (FilterTextIsExactMatch)
                {
                    if (!doc.Metadata[FilterField].Equals(FilterText))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!doc.Metadata[FilterField].ToString().Contains(FilterText))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
