
namespace LoadFileAdapter.Exporters
{
    public class XrefTrigger
    {
        //todo: comments
        private TriggerType type;
        private string regexPattern;
        private string regexField;
        private string fieldValueChangeEventField;
        private int startingSegmentCount;
        private int endingSegmentCount;
        private string startingSegmentDelimiter;
        private string endingSegmentDelimiter;
        private string parentIdField;
        private FieldValueChangeOption changeOption;

        public TriggerType Type { get { return type; } }
        public string RegexPattern { get { return regexPattern; } }
        public string RegexField { get { return regexField; } }
        public string FieldValueChangeEventField { get { return fieldValueChangeEventField; } }
        public int StartingSegmentCount { get { return startingSegmentCount; } }
        public int EndingSegmentCount { get { return endingSegmentCount; } }
        public string StartingSegmentDelimiter { get { return startingSegmentDelimiter; } }
        public string EndingSegmentDelimiter { get { return endingSegmentDelimiter; } }
        public string ParentIdField { get { return parentIdField; } }
        public FieldValueChangeOption ChangeOption { get { return changeOption; } }

        public XrefTrigger(TriggerType type, string regexPattern, string regexField, string fieldValueChangeEventField,
            int startingSegmentCount, int endingSegmentCount, string startingSegmentDelimiter, string endingSegmentDelimiter,
            string parentIdField, FieldValueChangeOption changeOption)
        {
            this.type = type;
            this.regexPattern = regexPattern;
            this.regexField = regexField;
            this.fieldValueChangeEventField = fieldValueChangeEventField;
            this.startingSegmentCount = startingSegmentCount;
            this.endingSegmentCount = endingSegmentCount;
            this.startingSegmentDelimiter = startingSegmentDelimiter;
            this.endingSegmentDelimiter = endingSegmentDelimiter;
            this.parentIdField = parentIdField;
            this.changeOption = changeOption;
        }

        public enum TriggerType
        {
            None, Family, Regex, FieldValueChange
        }

        public enum FieldValueChangeOption
        {
            None, StripFileName, UseStartingSegments, UseEndingSegments
        }
    }
}
