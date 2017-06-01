using System;
using System.Collections.Generic;
using System.Globalization;
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
        private string outputFormat;
        private string inputFormat;
        private TimeZoneInfo outputTimeZone;
        private TimeZoneInfo inputTimeZone;
        private DateTime rangeStart;
        private DateTime rangeEnd;
        private FailAction onFailure;
        private readonly Dictionary<string, string> TIME_ZONE_MAP = new Dictionary<string, string>() {
            { "UTC", "Z" },
            { "GMT", "Z" },
            { "EST", "-5" },
            { "EDT", "-4" },
            { "CST", "-6" },
            { "CDT", "-5" },
            { "MST", "-7" },
            { "MDT", "-6" },
            { "PST", "-8" },
            { "PDT", "-7" },
            { "AKST", "-9" },
            { "AKDT", "-8" },
            { "HST", "-10" },
            { "HAST", "-10" },
            { "HADT", "-9" },
            { "SST", "-11" },
            { "SDT", "-10" }
        };

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
        public string OutputFormat
        {
            get
            {
                return outputFormat;
            }
        }

        /// <summary>
        /// The date format string used to read the date value.
        /// This is optional and can be left as null or empty.
        /// </summary>
        public string InputFormat
        {
            get
            {
                return inputFormat;
            }
        }

        /// <summary>
        /// Optional field for converting the date value to a
        /// specific time zone.
        /// </summary>
        public TimeZoneInfo OutputTimeZone
        {
            get
            {
                return outputTimeZone;
            }
        }

        /// <summary>
        /// Optional field for parsing the date value in a
        /// specific time zone.
        /// </summary>
        public TimeZoneInfo InputTimeZone
        {
            get
            {
                return inputTimeZone;
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
        /// The action to take on failure.
        /// </summary>
        public FailAction OnFailure
        {
            get
            {
                return onFailure;
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
        /// <param name="outputFormat">The output date format.</param>
        /// <param name="inputFormat">The input date format.</param>
        /// <param name="outputTimeZone">The time zone used for the date output.</param>
        /// <param name="inputTimeZone">The time zone used to parse the input.</param>
        /// <param name="rangeStart">The start date in the valid date range.</param>
        /// <param name="rangeEnd">The end date in the valid date range.</param>
        /// <param name="onFailure">The action to take on failure.</param>
        public DateFormatTransformation(string fieldName,
            Regex findText, string replaceText, string filterField, Regex filterText,
            string outputFormat, string inputFormat, TimeZoneInfo outputTimeZone, 
            TimeZoneInfo inputTimeZone, DateTime rangeStart, DateTime rangeEnd, 
            FailAction onFailure) : 
            base(findText, replaceText, filterField, filterText)
        {
            this.fieldName = fieldName;
            this.outputFormat = outputFormat;
            this.inputFormat = inputFormat;
            this.outputTimeZone = outputTimeZone;
            this.inputTimeZone = inputTimeZone;
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
                    DateTime parsedDate = DateTime.Today;
                    bool isValidDateTime = isDateTime(value, out parsedDate);                                                            
                    bool inRange = isInRange(parsedDate);

                    if (isValidDateTime && inRange)
                    {
                        value = parsedDate.ToString(outputFormat);
                    }
                    else
                    {
                        if (onFailure == FailAction.ReplaceWithNull)
                        {
                            value = String.Empty;
                        }
                        else if (onFailure == FailAction.Fail)
                        {
                            string msg = (!isValidDateTime) ? "Invalid date value." : "Date value not in date range.";
                            throw new Exception(msg);                            
                        }
                    }
                }

                doc.Metadata[fieldName] = value; 
            }
        }

        /// <summary>
        /// Determines if a value is a date.
        /// </summary>
        /// <param name="value">The date value to check.</param>
        /// <param name="parsedDate">The parsed date value.</param>
        /// <returns>Returns true if the value is a date otherwise false.</returns>
        protected bool isDateTime(string value, out DateTime parsedDate)
        {
            bool isDateTime = false;

            if (String.IsNullOrWhiteSpace(inputFormat))
            {
                isDateTime = DateTime.TryParse(value, out parsedDate);
            }
            else
            {
                CultureInfo culture = new CultureInfo("en-US");
                isDateTime = DateTime.TryParseExact(
                    value, inputFormat, culture, DateTimeStyles.None, out parsedDate);
            }

            if (!isDateTime && value != null)
            {
                foreach (KeyValuePair<string, string> kvp in TIME_ZONE_MAP)
                {
                    string timeValue = value.Replace(kvp.Key, kvp.Value);
                    isDateTime = DateTime.TryParse(timeValue, out parsedDate);

                    if (isDateTime)
                    {
                        break;
                    }
                }                
            }

            parsedDate = adjustTimeZone(parsedDate);
            return isDateTime;
        }

        /// <summary>
        /// Adjust the date value based on the input and output time zone
        /// values. If the kind is unspecified we will assume UTC.
        /// </summary>
        /// <param name="parsedDate">The date value to adjust.</param>
        /// <returns>Returns an adjusted date value based on the time zone values.</returns>
        protected DateTime adjustTimeZone(DateTime parsedDate)
        {
            if (outputTimeZone != null && inputTimeZone != null && parsedDate.Kind == DateTimeKind.Unspecified)
            {
                parsedDate = TimeZoneInfo.ConvertTime(parsedDate, inputTimeZone, outputTimeZone);
            }
            else if (inputTimeZone != null && parsedDate.Kind == DateTimeKind.Unspecified)
            {
                var offset = inputTimeZone.GetUtcOffset(parsedDate);
                var dto = new DateTimeOffset(parsedDate, offset);
                parsedDate = dto.UtcDateTime;
            }
            else if (outputTimeZone != null && parsedDate.Kind == DateTimeKind.Unspecified)
            {
                parsedDate = TimeZoneInfo.ConvertTimeFromUtc(parsedDate, outputTimeZone);
            }
            else if (outputTimeZone != null)
            {
                var dto = new DateTimeOffset(parsedDate);
                parsedDate = TimeZoneInfo.ConvertTime(dto, outputTimeZone).DateTime;
            }
            else if (parsedDate.Kind != DateTimeKind.Unspecified)
            {
                parsedDate = TimeZoneInfo.ConvertTimeToUtc(parsedDate);
            }

            return parsedDate;
        }

        /// <summary>
        /// Checks if the provided date is in the valid date range.
        /// </summary>
        /// <param name="date">The date to check.</param>
        /// <returns>Returns true if the date is in the valid range, otherwise false.</returns>
        protected bool isInRange(DateTime date)
        {
            bool pass = true;            
            bool timeOnly = (date.Date == DateTime.Today && date.TimeOfDay.TotalSeconds > 0) 
                ? true : false;

            if (!timeOnly && date < rangeStart)
            {
                pass = false;
            }

            if (!timeOnly && date > rangeEnd)
            {
                pass = false;
            }

            return pass;
        }                
    }
}
