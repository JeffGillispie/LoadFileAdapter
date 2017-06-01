
namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// Defines a trigger which causes value in an XREF to change.
    /// </summary>
    public class XrefTrigger
    {        
        private TriggerType type;
        private string regexPattern;
        private string fieldName;
        private int segmentCount;        
        private string segmentDelimiter;                
        private FieldValueChangeOption changeOption;

        /// <summary>
        /// The type of trigger.
        /// </summary>
        public TriggerType Type { get { return type; } }

        /// <summary>
        /// The pattern to use with the regex trigger type.
        /// </summary>
        public string RegexPattern { get { return regexPattern; } }

        /// <summary>
        /// The field to use with the regex trigger type of the field value change type.
        /// </summary>
        public string FieldName { get { return fieldName; } }
               
        /// <summary>
        /// The segment count used with the starting segment or ending segment field value change option.
        /// </summary>
        public int SegmentCount { get { return segmentCount; } }
        
        /// <summary>
        /// The delimiter used with the starting segment or ending segment field value change option.
        /// </summary>
        public string SegmentDelimiter { get { return segmentDelimiter; } }
        
        /// <summary>
        /// The change option used with the field value change type.
        /// </summary>
        public FieldValueChangeOption ChangeOption { get { return changeOption; } }

        /// <summary>
        /// Initializes a new instance of <see cref="XrefTrigger"/>.
        /// </summary>
        /// <param name="type">The trigger type.</param>
        /// <param name="regexPattern">The regex pattern to use for a regex trigger type.</param>
        /// <param name="fieldName">The target field to use for a regex trigger or the field value change types.</param>        
        /// <param name="segmentCount">The segment count to use for the field value change event type.</param>        
        /// <param name="segmentDelimiter">The segment delimiter to use for the field value change event type.</param>        
        /// <param name="changeOption">The option to use for the field value change event type.</param>
        public XrefTrigger(TriggerType type, string regexPattern, string fieldName,
            int segmentCount, string segmentDelimiter, FieldValueChangeOption changeOption)
        {
            this.type = type;
            this.regexPattern = regexPattern;
            this.fieldName = fieldName;            
            this.segmentCount = segmentCount;            
            this.segmentDelimiter = segmentDelimiter;                        
            this.changeOption = changeOption;
        }

        /// <summary>
        /// The type of trigger.
        /// </summary>
        public enum TriggerType
        {
            /// <summary>
            /// No trigger.
            /// </summary>
            None,
            /// <summary>
            /// Indicates a change in family should activate the trigger.
            /// </summary>
            Family,
            /// <summary>
            /// Indicates a regex pattern match should activate the trigger.
            /// </summary>
            Regex,
            /// <summary>
            /// Indicates a change in field value between documents should
            /// activate the trigger.
            /// </summary>
            FieldValueChange
        }

        /// <summary>
        /// A field value change option.
        /// </summary>
        public enum FieldValueChangeOption
        {
            /// <summary>
            /// No field value change option.
            /// </summary>
            None,
            /// <summary>
            /// Indicates a file name should be stripped from the field value.
            /// </summary>
            StripFileName,
            /// <summary>
            /// Indicates the field value should be split on a delimiter and 
            /// only a specified number of the resulting elements starting 
            /// from the start should be used for comparison.
            /// </summary>
            UseStartingSegments,
            /// <summary>
            /// Indicates the field value should be split on a delimiter and
            /// only a specified number of the resulting elements starting 
            /// from the end should be used for comparison.
            /// </summary>
            UseEndingSegments
        }
    }
}
