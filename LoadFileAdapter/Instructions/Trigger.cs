using System;
using LoadFileAdapter.Exporters;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// Defines a trigger which causes value in an XREF to change.
    /// </summary>
    public class Trigger : IEquatable<Trigger>
    {
        /// <summary>
        /// Defines which trigger type will be used.
        /// </summary>
        public Switch.SwitchType Type = Switch.SwitchType.None;

        /// <summary>
        /// The type of field change event to test for.
        /// </summary>
        public Switch.ValueChangeOption FieldChangeOption = Switch.ValueChangeOption.None;

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
        /// <param name="trigger">The <see cref="Switch"/> used to initialize the object.</param>
        public Trigger(Switch trigger)
        {
            this.Type = trigger.Type;
            this.FieldName = trigger.FieldName;
            this.RegexPattern = trigger.RegexPattern;
            this.SegmentCount = trigger.SegmentCount;
            this.SegmentDelimiter = trigger.SegmentDelimiter;
            this.FieldChangeOption = trigger.ChangeOption;
        }

        /// <summary>
        /// Gets a <see cref="Switch"/>.
        /// </summary>
        /// <returns>Returns a <see cref="Switch"/>.</returns>
        public Switch ToSwitch()
        {
            return Switch.Builder
                .Start(Type)
                .SetRegexPattern(RegexPattern)
                .SetFieldName(FieldName)
                .SetSegmentCount(SegmentCount)
                .SetSegmentDelimiter(SegmentDelimiter)
                .SetChangeOption(FieldChangeOption)
                .Build();            
        }

        public bool Equals(Trigger trigger)
        {
            if (trigger == null) return false;

            return this.Type == trigger.Type &&
                this.FieldChangeOption == trigger.FieldChangeOption &&
                (this.FieldName == trigger.FieldName || this.FieldName.Equals(trigger.FieldName)) &&
                (this.RegexPattern == trigger.RegexPattern || this.RegexPattern.Equals(trigger.RegexPattern)) &&
                (this.SegmentDelimiter == trigger.SegmentDelimiter || this.SegmentDelimiter.Equals(trigger.SegmentDelimiter)) &&
                this.SegmentCount == trigger.SegmentCount;                
        }
    }
}
