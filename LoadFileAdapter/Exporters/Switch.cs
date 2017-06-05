﻿using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// Defines a trigger which causes value in an XREF to change.
    /// </summary>
    public class Switch
    {        
        private SwitchType type;
        private string regexPattern;
        private string fieldName;
        private int segmentCount;        
        private string segmentDelimiter;                
        private ValueChangeOption changeOption;

        /// <summary>
        /// The type of trigger.
        /// </summary>
        public SwitchType Type { get { return type; } }

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
        public ValueChangeOption ChangeOption { get { return changeOption; } }

        /// <summary>
        /// Initializes a new instance of <see cref="Switch"/>.
        /// </summary>
        /// <param name="type">The trigger type.</param>
        /// <param name="regexPattern">The regex pattern to use for a regex trigger type.</param>
        /// <param name="fieldName">The target field to use for a regex trigger or the field value change types.</param>        
        /// <param name="segmentCount">The segment count to use for the field value change event type.</param>        
        /// <param name="segmentDelimiter">The segment delimiter to use for the field value change event type.</param>        
        /// <param name="changeOption">The option to use for the field value change event type.</param>
        public Switch(SwitchType type, string regexPattern, string fieldName,
            int segmentCount, string segmentDelimiter, ValueChangeOption changeOption)
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
        public enum SwitchType
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
        public enum ValueChangeOption
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

        /// <summary>
        /// Tests if the switch has been triggered.
        /// </summary>
        /// <param name="doc">The document to test.</param>
        /// <param name="previousDoc">Used to test if a flag is needed if the trigger is a field value change trigger.</param>
        /// <returns>Returns true if the trigger is a family trigger and the doc key matches the parent doc key.
        /// Returns true if the trigger is a regex trigger and target metadata field matches the trigger pattern.
        /// Returns true if the trigger is a field value change trigger and the target metadata field is different 
        /// from the metadata in the previous document. Otherwise it returns false.</returns>
        public bool IsTriggered(Document doc, Document previousDoc)
        {
            bool result = false;
            string docid = doc.Key;
            string parentid = (doc.Parent != null) ? doc.Parent.Key : String.Empty;
                        
            switch (Type)
            {
                case Switch.SwitchType.Family:
                    result = (docid.Equals(parentid) || String.IsNullOrEmpty(parentid));
                    break;
                case Switch.SwitchType.FieldValueChange:
                    result = hasFieldValueChange(doc, previousDoc);
                    break;
                case Switch.SwitchType.None:
                    // do nothing here
                    break;
                case Switch.SwitchType.Regex:
                    result = Regex.IsMatch(doc.Metadata[FieldName], RegexPattern);
                    break;
                default:
                    // do nothing here
                    break;
            }
            
            return result;
        }

        /// <summary>
        /// Tests if the target document has a field value change event.
        /// </summary>
        /// <param name="doc">The document to test.</param>
        /// <param name="previousDoc">The previous document in the collection being exported.</param>
        /// <returns>Returns true if there is no change option and the target metadata of the current
        /// document is different from the previous document. Returns true if the change option is 
        /// strip file name and directory name of the current field value is different from
        /// the previous field value. Returns true if the change option is use ending segments
        /// and the specified ending segments of the current doc are different from the ending
        /// segments of the previous doc. Returns true if the change option is starting segments
        /// and the specified starting segments fo the current doc are different from the starting 
        /// segments of the previous doc. Otherwise returns false.</returns>
        protected bool hasFieldValueChange(Document doc, Document previousDoc)
        {
            bool result = false;
            string changeFieldValue = doc.Metadata[FieldName].ToString();
            string previousFieldValue = (previousDoc != null)
                ? previousDoc.Metadata[FieldName].ToString()
                : String.Empty;

            switch (ChangeOption)
            {
                case Switch.ValueChangeOption.None:
                    result = !changeFieldValue.Equals(previousFieldValue);
                    break;
                case Switch.ValueChangeOption.StripFileName:
                    string currentDir = Path.GetDirectoryName(changeFieldValue);
                    string previousDir = Path.GetDirectoryName(previousFieldValue);
                    result = !currentDir.Equals(previousDir);
                    break;
                case Switch.ValueChangeOption.UseEndingSegments:
                    var currentValueEnd = String.Join(
                        SegmentDelimiter,
                        changeFieldValue
                            .Split(new string[] { SegmentDelimiter }, StringSplitOptions.None)
                            .Reverse().Take(SegmentCount).Reverse());
                    var previousValueEnd = String.Join(
                        SegmentDelimiter,
                        previousFieldValue
                            .Split(new string[] { SegmentDelimiter }, StringSplitOptions.None)
                            .Reverse().Take(SegmentCount).Reverse());
                    result = !currentValueEnd.Equals(previousValueEnd);
                    break;
                case Switch.ValueChangeOption.UseStartingSegments:
                    var currentValueStart = String.Join(
                        SegmentDelimiter,
                        changeFieldValue
                            .Split(new string[] { SegmentDelimiter }, StringSplitOptions.None)
                            .Take(SegmentCount));
                    var previousValueStart = String.Join(
                        SegmentDelimiter,
                        previousFieldValue
                            .Split(new string[] { SegmentDelimiter }, StringSplitOptions.None)
                            .Take(SegmentCount));
                    result = !currentValueStart.Equals(previousValueStart);
                    break;
                default:
                    // do nothing here
                    break;
            }

            return result;
        }
    }
}
