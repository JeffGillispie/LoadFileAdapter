
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using LoadFileAdapter.Transformers;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// This class is a wrapper for a <see cref="Transformation"/> in order to support 
    /// serialization to XML. It contains the instructions to perform an <see cref="Edit"/>
    /// or a <see cref="Transformation"/> on a <see cref="Document"/>.
    /// </summary>
    public abstract class Edit
    {
        /// <summary>
        /// Contains the pattern and <see cref="RegexOptions"/> for a find / replace operation
        /// in conjunction with the ReplaceText field that is appled to documents in a 
        /// <see cref="DocumentCollection"/>. This field is ignored for serialization to XML
        /// and replaced with the FindTextPattern, FindTextIgnoreCase and FindTextIsRightToLeft
        /// properties.
        /// </summary>
        [XmlIgnore]
        public Regex FindText = null;

        /// <summary>
        /// The replacement text that is used with the FindText regex.
        /// </summary>
        public string ReplaceText = null;

        /// <summary>
        /// Determines which field if any the FilterText regex is applied to in order to determine
        /// which records in a document collection the edit is applied to.
        /// </summary>
        public string FilterField = null;

        /// <summary>
        /// Determines which records in a document collection are edited in conjunction with the FilterField.
        /// This field is ignored for serialization and replaced with the FilterTextPattern, 
        /// FilterTextIgnoreCase, and FilterTextIsRightToLeft properties.
        /// </summary>
        [XmlIgnore]
        public Regex FilterText = null;

        /// <summary>
        /// Initializes a new instance of <see cref="Edit"/>.
        /// </summary>
        public Edit()
        {

        }

        /// <summary>
        /// A serializable property for the find text pattern. It also is used to
        /// manage the FindText <see cref="Regex"/>.
        /// </summary>
        public string FindTextPattern
        {
            get
            {
                if (this.FindText != null)
                    return this.FindText.ToString();
                else
                    return null;
            }

            set
            {
                RegexOptions options = getRegexOptions(FindTextIgnoreCase, FindTextIsRightToLeft);
                this.FindText = new Regex(value, options);
            }
        }

        /// <summary>
        /// A serializable property indicating if the find text <see cref="Regex"/> 
        /// should ignore case.
        /// </summary>
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
                if (FindTextPattern != null)
                {
                    bool ignoreCase = value;
                    bool rtl = FindTextIsRightToLeft;
                    RegexOptions options = getRegexOptions(ignoreCase, rtl);
                    this.FindText = new Regex(FindTextPattern, options);
                }
            }
        }

        /// <summary>
        /// A serializable property indicating if the find text is right to left.
        /// It is also used in managing the <see cref="RegexOptions"/> for the 
        /// FindText <see cref="Regex"/>.
        /// </summary>
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
                if (FindTextPattern != null)
                {
                    bool ignoreCase = FindTextIgnoreCase;
                    bool rtl = false;
                    RegexOptions options = getRegexOptions(ignoreCase, rtl);
                    this.FindText = new Regex(FindTextPattern, options);
                }
            }
        }

        /// <summary>
        /// A serializable property containing the filter text pattern
        /// and used to manage the FilterText <see cref="Regex"/>.
        /// </summary>
        public string FilterTextPattern
        {
            get
            {
                if (this.FilterText != null)
                    return this.FilterText.ToString();
                else
                    return null;
            }

            set
            {
                RegexOptions options = getRegexOptions(FilterTextIgnoreCase, FilterTextRightToLeft);
                this.FilterText = new Regex(value, options);
            }
        }

        /// <summary>
        /// A serializable property indicating if the filter text pattern
        /// in the edit instructions should ignore case. It is also used
        /// to manage the <see cref="RegexOptions"/> for the FilterText 
        /// <see cref="Regex"/>.
        /// </summary>
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
                if (FilterTextPattern != null)
                {
                    bool ignoreCase = value;
                    bool rtl = FilterTextRightToLeft;
                    RegexOptions options = getRegexOptions(ignoreCase, rtl);
                    this.FilterText = new Regex(FilterTextPattern, options);
                }
            }
        }

        /// <summary>
        /// A serializable property indicating if the filter text pattern
        /// in the edit instructions is right to left. It is also used to 
        /// manage the <see cref="RegexOptions"/> for the FilterText
        /// <see cref="Regex"/>.
        /// </summary>
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
                if (FilterTextPattern != null)
                {
                    bool ignoreCase = FilterTextIgnoreCase;
                    bool rtl = value;
                    RegexOptions options = getRegexOptions(ignoreCase, rtl);
                    this.FilterText = new Regex(FilterTextPattern, options);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="RegexOptions"/> used in the edit instructions.
        /// </summary>
        /// <param name="ignoreCase">Indicates if case should be ignored.</param>
        /// <param name="rightToLeft">Indicates if the match should be made from right to left.</param>
        /// <returns></returns>
        protected RegexOptions getRegexOptions(bool ignoreCase, bool rightToLeft)
        {
            RegexOptions options = RegexOptions.None;
            options = (ignoreCase) ? options | RegexOptions.IgnoreCase : options;
            options = (rightToLeft) ? options | RegexOptions.RightToLeft : options;
            return options;
        }

        /// <summary>
        /// Gets the <see cref="Transformation"/> version of the edit instructions.
        /// </summary>
        /// <returns>Returns a <see cref="Transformation"/>.</returns>
        public Transformation GetEdit()
        {
            if (this.GetType().Equals(typeof(MetaDataEdit)))
            {
                return ((MetaDataEdit)this).GetEdit();
            }
            else
            {
                return ((RepresentativeEdit)this).GetEdit();
            }
        }
    }
}
