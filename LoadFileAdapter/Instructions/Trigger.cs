using LoadFileAdapter.Exporters;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// Defines a trigger which causes value in an XREF to change.
    /// </summary>
    public class Trigger
    {
        /// <summary>
        /// Defines which trigger type will be used.
        /// </summary>
        public XrefTrigger.TriggerType Type = XrefTrigger.TriggerType.None;

        /// <summary>
        /// The type of field change event to test for.
        /// </summary>
        public XrefTrigger.FieldValueChangeOption FieldChangeOption = XrefTrigger.FieldValueChangeOption.None;

        /// <summary>
        /// The pattern to used for match testing.
        /// </summary>
        public string RegexPattern;

        /// <summary>
        /// The name of the metadata field that contains the value for comparison.
        /// </summary>
        public string FieldName;

        /// <summary>
        /// The number of segments to take for comparison.
        /// </summary>
        public int SegmentCount = 0;

        /// <summary>
        /// The delimiter used to split the target value for evaluation.
        /// </summary>
        public string SegmentDelimiter;

        /// <summary>
        /// Initializes a new instance of <see cref="Trigger"/>.
        /// </summary>
        public Trigger()
        {
            // do nothing here
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Trigger"/>.
        /// </summary>
        /// <param name="trigger">The <see cref="XrefTrigger"/> used to initialize the object.</param>
        public Trigger(XrefTrigger trigger)
        {
            this.Type = trigger.Type;
            this.FieldName = trigger.FieldName;
            this.RegexPattern = trigger.RegexPattern;
            this.SegmentCount = trigger.SegmentCount;
            this.SegmentDelimiter = trigger.SegmentDelimiter;
            this.FieldChangeOption = trigger.ChangeOption;
        }

        /// <summary>
        /// Gets a <see cref="XrefTrigger"/>.
        /// </summary>
        /// <returns>Returns a <see cref="XrefTrigger"/>.</returns>
        public XrefTrigger GetXrefTrigger()
        {
            return new XrefTrigger(this.Type, this.RegexPattern, this.FieldName, 
                this.SegmentCount, this.SegmentDelimiter, this.FieldChangeOption);
        }
    }
}
