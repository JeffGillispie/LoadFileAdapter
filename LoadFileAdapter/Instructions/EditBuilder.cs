using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using LoadFileAdapter.Transformers;

namespace LoadFileAdapter.Instructions
{
    public abstract class EditBuilder
    {
        [XmlIgnore]
        public Regex FindText = null;
        public string ReplaceText = String.Empty;
        public string FilterField = String.Empty;
        [XmlIgnore]
        public Regex FilterText = null;

        public EditBuilder()
        {

        }

        public string FindTextPattern
        {
            get
            {
                if (this.FindText != null)
                    return this.FindText.ToString();
                else
                    return String.Empty;
            }

            set
            {
                RegexOptions options = getRegexOptions(FindTextIgnoreCase, FindTextIsRightToLeft);
                this.FindText = new Regex(value, options);
            }
        }

        public bool FindTextIgnoreCase
        {
            get
            {
                if (this.FindText != null)
                    return (this.FindText.Options & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase;
                else
                    return false;
            }

            set
            {
                bool ignoreCase = value;
                bool rtl = FindTextIsRightToLeft;
                RegexOptions options = getRegexOptions(ignoreCase, rtl);
                this.FindText = new Regex(FindTextPattern, options);
            }
        }

        public bool FindTextIsRightToLeft
        {
            get
            {
                if (this.FindText != null)
                    return (this.FindText.Options & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase;
                else
                    return false;
            }

            set
            {
                bool ignoreCase = FindTextIgnoreCase;
                bool rtl = false;
                RegexOptions options = getRegexOptions(ignoreCase, rtl);
                this.FindText = new Regex(FindTextPattern, options);
            }
        }

        public string FilterTextPattern
        {
            get
            {
                if (this.FilterText != null)
                    return this.FilterText.ToString();
                else
                    return String.Empty;
            }

            set
            {
                RegexOptions options = getRegexOptions(FilterTextIgnoreCase, FilterTextRightToLeft);
                this.FilterText = new Regex(value, options);
            }
        }

        public bool FilterTextIgnoreCase
        {
            get
            {
                if (this.FilterText != null)
                    return (this.FilterText.Options & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase;
                else
                    return false;
            }

            set
            {
                bool ignoreCase = value;
                bool rtl = FilterTextRightToLeft;
                RegexOptions options = getRegexOptions(ignoreCase, rtl);
                this.FilterText = new Regex(FilterTextPattern, options);
            }
        }

        public bool FilterTextRightToLeft
        {
            get
            {
                if (this.FilterText != null)
                    return (this.FilterText.Options & RegexOptions.RightToLeft) == RegexOptions.RightToLeft;
                else
                    return false;
            }

            set
            {
                bool ignoreCase = FilterTextIgnoreCase;
                bool rtl = value;
                RegexOptions options = getRegexOptions(ignoreCase, rtl);
                this.FilterText = new Regex(this.FilterText.ToString(), options);
            }
        }

        protected RegexOptions getRegexOptions(bool ignoreCase, bool rightToLeft)
        {
            RegexOptions options = RegexOptions.None;
            options = (ignoreCase) ? options | RegexOptions.IgnoreCase : options;
            options = (rightToLeft) ? options | RegexOptions.RightToLeft : options;
            return options;
        }

        public Edit GetEdit()
        {
            if (this.GetType().Equals(typeof(MetaDataEditBuilder)))
            {
                return ((MetaDataEditBuilder)this).GetEdit();
            }
            else
            {
                return ((LinkedFileEditBuilder)this).GetEdit();
            }
        }
    }
}
