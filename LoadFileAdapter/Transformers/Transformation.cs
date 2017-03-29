using System;
using System.Text.RegularExpressions;

namespace LoadFileAdapter.Transformers
{
    /// <summary>
    /// Contains the generic settings used to modify a <see cref="Document"/>.
    /// </summary>
    public abstract class Transformation
    {
        private Regex findText = null;
        private string replaceText = String.Empty;        
        private string filterField = String.Empty;
        private Regex filterText = null;
 
        /// <summary>
        /// The regex used for a find / replace operation.
        /// </summary>
        public Regex FindText { get { return findText; } }

        /// <summary>
        /// The replacement value used in the find text regex replace operation.
        /// </summary>
        public string ReplaceText { get { return replaceText; } }        

        /// <summary>
        /// The field name of a field whose value will be checked against the
        /// filter text regex for filtering out documents that should not be
        /// modified. If the filter field is null no filter will be performed.
        /// </summary>
        public string FilterField { get { return filterField; } }

        /// <summary>
        /// A regex used with the field field to determine if a document should
        /// be modified. If the filter text is null no filter will be performed.
        /// </summary>
        public Regex FilterText { get { return filterText; } }        

        /// <summary>
        /// Initializes a new instance of <see cref="Transformation"/>.
        /// </summary>
        /// <param name="findText">The regex for a find / replace operation.</param>
        /// <param name="replaceText">The replacement value used with the find text regex.</param>
        /// <param name="filterField">The field name of the field to be filtered.</param>
        /// <param name="filterText">A regex used to determine if a document should be modified.</param>
        public Transformation(Regex findText, string replaceText, string filterField, Regex filterText)
        {
            this.findText = findText;
            this.replaceText = replaceText;            
            this.filterField = filterField;
            this.filterText = filterText;            
        }

        /// <summary>
        /// If the find text regex is not null then a regex
        /// find / replace is performed. Otherwise, the 
        /// original value is returned.
        /// </summary>
        /// <param name="value">The value to replace.</param>
        /// <returns>The result of the replace operation.</returns>
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

        /// <summary>
        /// Modifies a <see cref="Document"/>.
        /// </summary>
        /// <param name="doc"></param>
        public abstract void Transform(Document doc);

        /// <summary>
        /// Determines if a <see cref="Document"/> should be edited.
        /// The filter text regex match is only checked if it is not
        /// null, the filter field is not null, and the document
        /// contains the filter field in it's metadata. Otherwise
        /// the document will not be excluded.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
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
