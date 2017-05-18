using System;
using System.Text.RegularExpressions;

namespace LoadFileAdapter.Transformers
{
    /// <summary>
    /// Contains the settings to perform a transformation on a metadata value 
    /// where a date and/or time value is converted to a specified format.
    /// </summary>
    public class DateFormatTransformation : Transformation
    {
        private string fieldName;
        private string format;        
        private DateTime rangeStart;
        private DateTime rangeEnd;
        private FailAction onFailure;
                
        /// <summary>
        /// Types of fail actions.
        /// </summary>
        public enum FailAction
        {
            ReplaceWithNull,
            DoNothing,
            Fail
        }

        /// <summary>
        /// The name of the metadata field to transform.
        /// </summary>
        public string FieldName
        {
            get
            {
                return fieldName;
            }
        }

        /// <summary>
        /// A standard or custom date format string.
        /// https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx
        /// https://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.110).aspx
        /// </summary>
        public string Format
        {
            get
            {
                return format;
            }
        }
                
        /// <summary>
        /// The start value of the valid date range.
        /// </summary>
        public DateTime RangeStart
        {
            get
            {
                return rangeStart;
            }
        }

        /// <summary>
        /// The end value of the valid date range.
        /// </summary>
        public DateTime RangeEnd
        {
            get
            {
                return rangeEnd;
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DateFormatTransformation"/>.
        /// </summary>
        /// <param name="fieldName">The metadata field to transform.</param>
        /// <param name="findText">The regex for a find / replace operation.</param>
        /// <param name="replaceText">The replacement text used with the find text regex.</param>
        /// <param name="filterField">The filter field used to determine if a document should be edited.</param>
        /// <param name="filterText">The filter regex used to determine if a filter field is is match and should be edited.</param>
        /// <param name="format">The output date format.</param>
        /// <param name="rangeStart">The start date in the valid date range.</param>
        /// <param name="rangeEnd">The end date in the valid date range.</param>
        /// <param name="onFailure">The action to take on failure.</param>
        public DateFormatTransformation(string fieldName,
            Regex findText, string replaceText, string filterField, Regex filterText,
            string format, DateTime rangeStart, DateTime rangeEnd, FailAction onFailure) : 
            base(findText, replaceText, filterField, filterText)
        {
            this.fieldName = fieldName;
            this.format = format;            
            this.rangeStart = rangeStart;
            this.rangeEnd = rangeEnd;
            this.onFailure = onFailure;
        }

        /// <summary>
        /// Modifies the supplied <see cref="Document"/>. If the doc does not 
        /// contains the target field name that field is added to the doc's
        /// metadata. The document's filter field is validated against the 
        /// filter text regex if any exists. Then the find / replace is performed 
        /// if any exists. If the target value is not null or whitespace then 
        /// the value is validated as a valid date and the date is in the
        /// valid date range. If either is false then the fail action is performed.
        /// Otherwise the target value is converted to the specified output format.
        /// </summary>
        /// <param name="doc">The <see cref="Document"/> to be modified.</param>
        public override void Transform(Document doc)
        {
            if (!doc.Metadata.ContainsKey(this.fieldName))
            {
                doc.Metadata.Add(this.fieldName, String.Empty);
            }

            if (base.hasEdit(doc))
            {                
                string value = doc.Metadata[fieldName];
                value = base.Replace(value);           

                if (!String.IsNullOrWhiteSpace(value))
                {
                    DateTime parsedDate = new DateTime(1946, 2, 14);
                    bool isDateTime = DateTime.TryParse(value, out parsedDate);
                    bool inRange = isInRange(parsedDate);

                    if (isDateTime && inRange)
                    {
                        value = parsedDate.ToString(format);
                    }
                    else
                    {
                        if (onFailure == FailAction.ReplaceWithNull)
                        {
                            value = String.Empty;
                        }
                        else if (onFailure == FailAction.Fail)
                        {
                            string msg = (!isDateTime) ? "Invalid date value." : "Date value not in date range.";
                            throw new Exception(msg);                            
                        }
                    }
                }

                doc.Metadata[fieldName] = value; 
            }
        }

        /// <summary>
        /// Checks if the provided date is in the valid date range.
        /// </summary>
        /// <param name="date">The date to check.</param>
        /// <returns>Returns true if the date is in the valid range, otherwise false.</returns>
        protected bool isInRange(DateTime date)
        {
            bool pass = true;

            if (rangeStart != null && date < rangeStart)
            {
                pass = false;
            }

            if (rangeEnd != null && date > rangeEnd)
            {
                pass = false;
            }

            return pass;
        }                
    }
}
