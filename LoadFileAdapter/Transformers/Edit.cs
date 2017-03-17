using System;
using System.Text.RegularExpressions;

namespace LoadFileAdapter.Transformers
{
    public abstract class Edit
    {
        private Regex findText = null;
        private string replaceText = String.Empty;        
        private string filterField = String.Empty;
        private Regex filterText = null;
        
        public Regex FindText { get { return findText; } }
        public string ReplaceText { get { return replaceText; } }        
        public string FilterField { get { return filterField; } }
        public Regex FilterText { get { return filterText; } }        

        public Edit(Regex findText, string replaceText, string filterField, Regex filterText)
        {
            this.findText = findText;
            this.replaceText = replaceText;            
            this.filterField = filterField;
            this.filterText = filterText;            
        }

        public string Replace(string value)
        {
            if (findText != null)
            {
                return findText.Replace(value, replaceText);
            }
            else
            {
                return value;
            }
        }

        public abstract void Transform(Document doc);

        protected bool hasEdit(Document doc)
        {
            if (String.IsNullOrWhiteSpace(FilterField) == false &&
                doc.Metadata.ContainsKey(FilterField) &&
                FilterText != null)
            {
                string value = doc.Metadata[filterField];

                if (filterText.IsMatch(value) == false)
                {
                    return false;
                }                
            }

            return true;
        }
    }
}
