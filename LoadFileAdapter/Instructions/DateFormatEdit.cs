using System;
using System.Xml.Serialization;
using LoadFileAdapter.Transformers;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// Contains the instructions for modifying a metadata date time value to a
    /// specific date or time format. 
    /// It is a wrapper for <see cref="DateFormatTransformation"/>.
    /// It is intended to serialsize instructions and deserialize instructions to XML.
    /// </summary>
    public class DateFormatEdit : Edit
    {
        /// <summary>
        /// The name of the metadata field to edit.
        /// </summary>
        public string FieldName = null;

        /// <summary>
        /// The output format of the new date time value.
        /// https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx
        /// https://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.110).aspx
        /// </summary>
        public string OutputFormat = null;

        /// <summary>
        /// The exact input format of the date time value.
        /// If this value is null or empty then DateTime.Parse
        /// is used.
        /// </summary>
        public string InputFormat = null;

        [XmlIgnore]
        public TimeZoneInfo OutputTimeZone = null;

        [XmlIgnore]
        public TimeZoneInfo InputTimeZone = null;

        /// <summary>
        /// The start value of the valid date time range.
        /// </summary>
        [XmlIgnore]
        public DateTime RangeStart = new DateTime(1946, 2, 14);

        /// <summary>
        /// The end value of the valid date time range.
        /// </summary>
        [XmlIgnore]
        public DateTime RangeEnd = DateTime.Today;

        /// <summary>
        /// The action to take when the metadata value is not null, a valid
        /// date, or in the valid date range.
        /// </summary>
        public DateFormatTransformation.FailAction OnFailure = DateFormatTransformation.FailAction.ReplaceWithNull;
        
        /// <summary>
        /// Initializes a new instance of <see cref="DateFormatEdit"/>.
        /// </summary>
        public DateFormatEdit() : base()
        {            
            // do nothing here
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DateFormatEdit"/>.
        /// </summary>
        /// <param name="transformation">The edit initialization settings.</param>
        public DateFormatEdit(DateFormatTransformation transformation)
        {
            this.FieldName = transformation.FieldName;
            this.OutputFormat = transformation.OutputFormat;
            this.InputFormat = transformation.InputFormat;
            this.OutputTimeZone = transformation.OutputTimeZone;
            this.InputTimeZone = transformation.InputTimeZone;
            this.RangeStart = transformation.RangeStart;
            this.RangeEnd = transformation.RangeEnd;
            this.OnFailure = transformation.OnFailure;
        }

        /// <summary>
        /// The starting value in the valid date range.
        /// </summary>
        public string StartDateRange
        {
            get
            {
                return RangeStart.ToShortDateString();
            }

            set
            {
                try
                {
                    RangeStart = DateTime.Parse(value);
                }
                catch (Exception)
                {
                    RangeStart = new DateTime(1946, 2, 14);
                }
            }
        }

        /// <summary>
        /// The ending value in the valid date range.
        /// </summary>
        public string EndDateRange
        {
            get
            {
                return RangeEnd.ToShortDateString();
            }

            set
            {
                try
                {
                    RangeEnd = DateTime.Parse(value);
                }
                catch (Exception)
                {
                    RangeEnd = DateTime.Today;
                }
            }
        }

        public string TimeZoneBefore
        {
            get
            {
                return InputTimeZone.ToString();
            }

            set
            {
                InputTimeZone = (String.IsNullOrWhiteSpace(value)) 
                    ? null : TimeZoneInfo.FindSystemTimeZoneById(value);
            }
        }

        public string TimeZoneAfter
        {
            get
            {
                return OutputTimeZone.ToString();
            }

            set
            {                
                OutputTimeZone = (String.IsNullOrWhiteSpace(value))
                    ? null : TimeZoneInfo.FindSystemTimeZoneById(value);
            }
        }

        /// <summary>
        /// Gets the <see cref="DateFormatTransformation"/> value of the edit.
        /// </summary>
        /// <returns>Returns a <see cref="DateFormatTransformation"/>.</returns>
        public override Transformation GetTransformation()
        {
            return new DateFormatTransformation(FieldName, 
                FindText, ReplaceText, FilterField, FilterText, 
                OutputFormat, InputFormat, OutputTimeZone, InputTimeZone, 
                RangeStart, RangeEnd, OnFailure);
        }
    }
}
