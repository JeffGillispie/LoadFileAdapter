using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LoadFileAdapter
{
    public class BatesNumber
    {
        /// <summary>
        /// Bates delimiters used to separate a bates number and a bates suffix.
        /// </summary>
        public static HashSet<string> BatesDelimiters = new HashSet<string>() { ".", "-", "_" };

        // private cosntant fields
        private const string NUMERIC_PATTERN = @"\d+";
        private const char PAD_VALUE = '0';
        
        // private member fields
        private string value = String.Empty;
        private string prefix = String.Empty;
        private int number = 0;
        private int numberLength = 0;
        private string suffixDelimiter = String.Empty;
        private int suffix = 0;
        private int suffixLength = 0;
        private bool hasSuffix = false;
        private bool hasPrefix = false;
        private bool hasDelim = false;

        /// <summary>
        /// The value of the bates prefix 
        /// (i.e. 'PREFIX' from 'PREFIX_000002.001').
        /// </summary>
        public string Prefix
        {
            get
            {
                return (this.hasPrefix) ? prefix : String.Empty;                
            }
        }

        /// <summary>
        /// The value of the bates number
        /// (i.e. '2' from 'PREFIX_000002.001').
        /// </summary>
        public int Number
        {
            get
            {
                return number;
            }
        }

        /// <summary>
        /// The value of the suffix delimiter
        /// (i.e. '.' from 'PREFIX_000002.001').
        /// </summary>
        public string SuffixDelimiter
        {
            get
            {
                return (this.hasDelim) ? suffixDelimiter : String.Empty;                
            }
        }

        /// <summary>
        /// The value of the bates suffix or zero if none exists
        /// (i.e. '1' from 'PREFIX_000002.001').
        /// </summary>
        public int Suffix
        {
            get
            {
                return (this.hasSuffix) ? suffix : 0;                
            }
        }

        /// <summary>
        /// Returns true if the bates number has a suffix, otherwise false.
        /// </summary>
        public bool HasSuffix
        {
            get
            {
                return hasSuffix;
            }
        }

        /// <summary>
        /// Returns true if the bates number has a prefix, otherwise false.
        /// </summary>
        public bool HasPrefix
        {
            get
            {
                return hasPrefix;
            }
        }

        /// <summary>
        /// Returns true if the bates number has a suffix, otherwise false.
        /// </summary>
        public bool HasDelim
        {
            get
            {
                return hasDelim;
            }
        }

        /// <summary>
        ///  Returns a padded value of the bates number
        ///  (i.e. '000002' from 'PREFIX000002.001').
        /// </summary>
        public string NumberAsString
        {
            get
            {
                return number.ToString().PadLeft(numberLength, PAD_VALUE);
            }
        }

        /// <summary>
        /// Returns a padded value of the suffix
        /// (i.e. '001' from 'PREFIX000002.001').
        /// </summary>
        public string SuffixAsString
        {
            get
            {
                return suffixDelimiter + suffix.ToString().PadLeft(suffixLength, PAD_VALUE);
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="BatesNumber"/>
        /// </summary>
        /// <param name="batesNumber">The bates value.</param>
        public BatesNumber(string batesNumber)
        {
            List<string> numericSequences = new List<string>();
            MatchCollection matches = Regex.Matches(batesNumber, NUMERIC_PATTERN, RegexOptions.None);
            numericSequences = matches.Cast<Match>().Select(match => match.Value).ToList();

            if (numericSequences.Count == 1)
            {
                buildSingleNumBates(batesNumber, numericSequences);
            }
            else if (numericSequences.Count >= 2)
            {
                buildMultiNumBates(batesNumber, numericSequences);
            }
            else
            {
                string msg = String.Format(
                    "BatesNumber Exception: No numeric sequences were detected in the bates number {1}.", 
                    batesNumber);
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// Supplies a bates number that appears after the given value.
        /// </summary>
        /// <param name="batesNumber">The initial bates value.</param>
        /// <returns>A bates number that has been increased from the given value.</returns>
        public static String NextNumber(String batesNumber)
        {
            BatesNumber bates = new BatesNumber(batesNumber);
            bates.IterateNumber();
            return bates.ToString();
        }

        /// <summary>
        /// Supplies a bates number that appears before the given value.
        /// </summary>
        /// <param name="batesNumber">The initial bates value.</param>
        /// <returns>A bates number that has been decreased from the given value.</returns>
        public static String PreviousNumber(String batesNumber)
        {
            BatesNumber bates = new BatesNumber(batesNumber);
            bates.DecreaseNumber();
            return bates.ToString();
        }

        public override string ToString()
        {
            return this.value;
        }

        /// <summary>
        /// Increases the bates number value by 1. 
        /// Any suffix is ignored.
        /// </summary>
        public void IterateNumber()
        {
            this.number++;
            this.hasSuffix = false;
            string num = Number.ToString().PadLeft(numberLength, '0');
            this.value = String.Format("{0}{1}", Prefix, num);
        }

        /// <summary>
        /// Reduces the bates number value by 1. 
        /// Any suffix is ignored.
        /// </summary>
        public void DecreaseNumber()
        {
            this.number--;
            this.hasSuffix = false;
            string num = Number.ToString().PadLeft(numberLength, '0');
            this.value = String.Format("{0}{1}", Prefix, num);
        }

        /// <summary>
        /// Modifies this instance to build a bates number assuming it looks like this:
        /// PREFIX_000001
        /// </summary>
        /// <param name="batesNumber">The bates number value.</param>
        /// <param name="numericSequences">A list of numeric sequences in the bates value.</param>
        protected void buildSingleNumBates(string batesNumber, List<string> numericSequences)
        {
            int numIndex = batesNumber.Length - numericSequences[0].Length;
            int numLength = numericSequences[0].Length;

            if (batesNumber.Substring(numIndex, numLength) == numericSequences[0])
            {
                this.value = batesNumber;
                this.number = int.Parse(numericSequences[0]);
                this.numberLength = numericSequences[0].Length;
                this.hasDelim = false;
                this.hasSuffix = false;

                if (batesNumber == numericSequences[0])
                {
                    this.hasPrefix = false;
                }
                else
                {
                    int prefixLength = batesNumber.Length - numericSequences[0].Length;
                    this.hasPrefix = true;                    
                    this.prefix = batesNumber.Substring(0, prefixLength);
                }
            }
            else
            {
                string msg = String.Format(
                    "BatesNumber Exception: The numeric sequence {0} in the bates number {1}" +
                    "is not in the correct position, it should be at the end of the value.", 
                    numericSequences[0], batesNumber);
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// Modifies this instance to build a bates number assuming it looks like this:
        /// PREFIX_99_PREFIX-000001.001
        /// </summary>
        /// <param name="batesNumber">The bates number value.</param>
        /// <param name="numericSequences">A list of numeric sequences in the bates value.</param>
        protected void buildMultiNumBates(string batesNumber, List<string> numericSequences)
        {            
            string lastSequence = numericSequences[numericSequences.Count - 1];
            string suspectedSuffix = batesNumber.Substring(
                batesNumber.Length - lastSequence.Length,
                lastSequence.Length);
            string suspectedDelim = batesNumber.Substring(
                batesNumber.Length - lastSequence.Length - 1,
                1);
            string nextToLastSequence = numericSequences[numericSequences.Count - 2];
            string suspectedNumber = batesNumber.Substring(
                batesNumber.Length - lastSequence.Length - 1 - nextToLastSequence.Length,
                nextToLastSequence.Length);
            string suspectedPrefix = batesNumber.Substring(
                0,
                batesNumber.Length - lastSequence.Length - 1 - nextToLastSequence.Length);

            if (lastSequence == suspectedSuffix &&
                nextToLastSequence == suspectedNumber &&
                BatesNumber.BatesDelimiters.Contains(suspectedDelim))
            {
                this.value = batesNumber;
                this.number = int.Parse(suspectedNumber);
                this.numberLength = suspectedNumber.Length;
                this.suffix = int.Parse(suspectedSuffix);
                this.suffixLength = suspectedSuffix.Length;
                this.suffixDelimiter = suspectedDelim;
                this.hasDelim = true;
                this.hasSuffix = true;

                if (!String.IsNullOrWhiteSpace(suspectedPrefix))
                {
                    this.hasPrefix = true;
                    this.prefix = suspectedPrefix;
                }
                else
                {
                    this.hasPrefix = false;
                }
            }
            else
            {
                if (lastSequence != suspectedSuffix)
                {
                    string msg = String.Format(
                        "BatesNumber Exception: The suspcted suffix {0} does not match the last numeric sequence {1} from the value {2}.",
                        suspectedSuffix, lastSequence, batesNumber);
                    throw new Exception(msg);
                }
                else if (nextToLastSequence != suspectedNumber)
                {
                    string msg = String.Format(
                        "BatesNumber Exception: The suspected number {1} does not match the next to last numeric sequence {1} from the value {2}.",
                        suspectedNumber, nextToLastSequence, batesNumber);
                    throw new Exception(msg);
                }
                else
                {
                    string msg = String.Format(
                        "BatesNumber Exception: The suspected delimiter {0} in {1} is not a valid delimiter ({2}).", 
                        suspectedDelim, batesNumber, String.Join(", ", BatesNumber.BatesDelimiters));
                    throw new Exception(msg);
                }
            }
        }
    }
}
