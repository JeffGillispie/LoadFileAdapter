using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using LoadFileAdapter.Builders;

namespace LoadFileAdapter.Instructions
{
    public class TextFileSettingsBuilder
    {
        public TextFileSettings.TextLevel FileLevel = TextFileSettings.TextLevel.None;
        public TextFileSettings.TextLocation FileLocation = TextFileSettings.TextLocation.None;
        [XmlIgnore]
        public Regex PathFind = null;
        public string PathReplace = String.Empty;

        public TextFileSettingsBuilder()
        {

        }

        public TextFileSettingsBuilder(TextFileSettings textSettings)
        {
            this.FileLevel = textSettings.FileLevel;
            this.FileLocation = textSettings.FileLocation;
            this.PathFind = textSettings.PathFind;
            this.PathReplace = textSettings.PathReplace;
        }

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

        public TextFileSettings GetSettings()
        {
            return new TextFileSettings(this.FileLevel, this.FileLocation, this.PathFind, this.PathReplace);
        }

        protected RegexOptions getRegexOptions(bool ignoreCase, bool rightToLeft)
        {
            RegexOptions options = RegexOptions.None;
            options = (ignoreCase) ? options | RegexOptions.IgnoreCase : options;
            options = (rightToLeft) ? options | RegexOptions.RightToLeft : options;
            return options;
        }
    }
}
