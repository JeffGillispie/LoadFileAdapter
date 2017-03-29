using System;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using LoadFileAdapter.Builders;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// Contains the instructions to create a text <see cref="Representative"/>
    /// from a <see cref="ImgImport"/>. This wraps the <see cref="TextRepresentativeSettings"/> class.
    /// It is used to serialize instructions and deserialize instructions from XML.
    /// </summary>
    public class TextFileSettingsBuilder
    {
        /// <summary>
        /// Defines the level or scope of the text <see cref="Representative"/>.
        /// </summary>
        public TextRepresentativeSettings.TextLevel FileLevel = TextRepresentativeSettings.TextLevel.None;

        /// <summary>
        /// Defines the location the text <see cref="Representative"/> will be found in
        /// relation to the image path.
        /// </summary>
        public TextRepresentativeSettings.TextLocation FileLocation = TextRepresentativeSettings.TextLocation.None;

        /// <summary>
        /// Used in conjunction with the PathReplace field to change the image path from 
        /// a <see cref="ImgImport"/> into a text path when the 
        /// <see cref="TextRepresentativeSettings.TextLocation"/> is set to an alternate
        /// destination from the image path.
        /// </summary>
        [XmlIgnore]
        public Regex PathFind = null;

        /// <summary>
        /// The replace value used in conjunction with the PathFind <see cref="Regex"/> 
        /// to change the image path in a <see cref="ImgImport"/> into a text path to
        /// create a text <see cref="Representative"/>.
        /// </summary>
        public string PathReplace = String.Empty;

        /// <summary>
        /// Initializes a new instance of <see cref="TextFileSettingsBuilder"/>.
        /// </summary>
        public TextFileSettingsBuilder()
        {
            // do nothing here
        }

        /// <summary>
        /// Initializes a new instance of <see cref="TextFileSettingsBuilder"/>.
        /// </summary>
        /// <param name="textSettings">The <see cref="TextRepresentativeSettings"/> used to build this object.</param>
        public TextFileSettingsBuilder(TextRepresentativeSettings textSettings)
        {
            this.FileLevel = textSettings.FileLevel;
            this.FileLocation = textSettings.FileLocation;
            this.PathFind = textSettings.PathFind;
            this.PathReplace = textSettings.PathReplace;
        }

        /// <summary>
        /// The pattern applied to the find operation of the text
        /// representative path. Used to manage the PathFind field.
        /// </summary>
        public string PathFindPattern
        {
            get
            {
                if (this.PathFind != null)
                    return this.PathFind.ToString();
                else
                    return String.Empty;
            }

            set
            {
                this.PathFind = new Regex(value);
            }
        }

        /// <summary>
        /// Indicates if the path find pattern should ignore case.
        /// Used to manage the PathFind field.
        /// </summary>
        public bool PathFindIgnoreCase
        {
            get
            {
                if (this.PathFind != null)
                    return (this.PathFind.Options & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase;
                else
                    return false;
            }

            set
            {
                bool ignoreCase = value;
                bool rtl = PathFindIsRightToLeft;
                RegexOptions options = getRegexOptions(ignoreCase, rtl);
                this.PathFind = new Regex(PathFindPattern, options);
            }
        }

        /// <summary>
        /// Indicates if the path find pattern should be applied right to left.
        /// This property is used to manage the PathFind field.
        /// </summary>
        public bool PathFindIsRightToLeft
        {
            get
            {
                if (this.PathFind != null)
                    return (this.PathFind.Options & RegexOptions.RightToLeft) == RegexOptions.RightToLeft;
                else
                    return false;
            }

            set
            {
                bool ignoreCase = PathFindIgnoreCase;
                bool rtl = value;
                RegexOptions options = getRegexOptions(ignoreCase, rtl);
                this.PathFind = new Regex(PathFindPattern, options);
            }
        }

        /// <summary>
        /// Gets the underlying <see cref="TextRepresentativeSettings"/> value.
        /// </summary>
        /// <returns>Returns a <see cref="TextRepresentativeSettings"/>.</returns>
        public TextRepresentativeSettings GetSettings()
        {
            return new TextRepresentativeSettings(this.FileLevel, this.FileLocation, this.PathFind, this.PathReplace);
        }

        /// <summary>
        /// Get a <see cref="RegexOptions"/>  value based on the ignore case
        /// and right to left parameters.
        /// </summary>
        /// <param name="ignoreCase">Indicates if the case should be ignored.</param>
        /// <param name="rightToLeft">Indicates if the pattern should be applied right to left.</param>
        /// <returns></returns>
        protected RegexOptions getRegexOptions(bool ignoreCase, bool rightToLeft)
        {
            RegexOptions options = RegexOptions.None;
            options = (ignoreCase) ? options | RegexOptions.IgnoreCase : options;
            options = (rightToLeft) ? options | RegexOptions.RightToLeft : options;
            return options;
        }
    }
}
