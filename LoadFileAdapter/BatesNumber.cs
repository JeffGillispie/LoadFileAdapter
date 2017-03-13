using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LoadFileAdapter
{
    public class BatesNumber
    {
        private String _value;
        private String _prefix;
        private int _number;
        private int _numberLength;
        private String _suffixDelimiter;
        private int _suffix;
        private int _suffixLength;
        private bool _hasSuffix;
        private bool _hasPrefix;
        private bool _hasDelim;

        public String Value { get { return _value; } }
        public String Prefix { get { if (_hasPrefix) return _prefix; else return String.Empty; } }
        public int Number { get { return _number; } }
        public String SuffixDelimiter { get { if (_hasDelim) return _suffixDelimiter; else return String.Empty; } }
        public int Suffix { get { if (_hasSuffix) return _suffix; else return 0; } }
        public bool HasSuffix { get { return _hasSuffix; } }
        public bool HasPrefix { get { return _hasPrefix; } }
        public bool HasDelim { get { return _hasDelim; } }
        public String NumberAsString { get { return _number.ToString().PadLeft(_numberLength, '0'); } }
        public String SuffixAsString { get { return _suffixDelimiter + _suffix.ToString().PadLeft(_suffixLength, '0'); } }

        public BatesNumber(String batesNumber)
        {
            List<String> numericSequences = new List<String>();
            MatchCollection matches = Regex.Matches(batesNumber, @"\d+", RegexOptions.None);
            numericSequences = matches.Cast<Match>().Select(match => match.Value).ToList();

            if (numericSequences.Count == 1)
            {
                if (batesNumber.Substring(batesNumber.Length - numericSequences[0].Length, numericSequences[0].Length) == numericSequences[0])
                {
                    _value = batesNumber;
                    _number = int.Parse(numericSequences[0]);
                    _numberLength = numericSequences[0].Length;
                    _hasDelim = false;
                    _hasSuffix = false;

                    if (batesNumber == numericSequences[0])
                    {
                        _hasPrefix = false;
                    }
                    else
                    {
                        _hasPrefix = true;
                        _prefix = batesNumber.Substring(0, batesNumber.Length - numericSequences[0].Length);
                    }
                }
                else
                {
                    throw new Exception(String.Format(
                        "BatesNumber Exception: The numberic sequence {0} in the bates number {1} is not in the correct position, it should be at the end of the value.",
                        numericSequences[0], batesNumber));
                }
            }
            else if (numericSequences.Count >= 2)
            {
                String lastSequence = numericSequences[numericSequences.Count - 1];
                String suspectedSuffix = batesNumber.Substring(batesNumber.Length - lastSequence.Length, lastSequence.Length);
                String suspectedDelim = batesNumber.Substring(batesNumber.Length - lastSequence.Length - 1, 1);
                String nextToLastSequence = numericSequences[numericSequences.Count - 2];
                String suspectedNumber = batesNumber.Substring(batesNumber.Length - lastSequence.Length - 1 - nextToLastSequence.Length, nextToLastSequence.Length);
                String suspectedPrefix = batesNumber.Substring(0, batesNumber.Length - lastSequence.Length - 1 - nextToLastSequence.Length);

                if (lastSequence == suspectedSuffix &&
                    nextToLastSequence == suspectedNumber &&
                    (suspectedDelim == "." || suspectedDelim == "-" || suspectedDelim == "_"))
                {
                    _value = batesNumber;
                    _number = int.Parse(suspectedNumber);
                    _numberLength = suspectedNumber.Length;
                    _suffix = int.Parse(suspectedSuffix);
                    _suffixLength = suspectedSuffix.Length;
                    _suffixDelimiter = suspectedDelim;
                    _hasDelim = true;
                    _hasSuffix = true;

                    if (!String.IsNullOrWhiteSpace(suspectedPrefix))
                    {
                        _hasPrefix = true;
                        _prefix = suspectedPrefix;
                    }
                    else
                    {
                        _hasPrefix = false;
                    }
                }
                else
                {
                    if (lastSequence != suspectedSuffix)
                    {
                        throw new Exception(String.Format(
                            "BatesNumber Exception: The suspected suffix {0} does not match the last numeric sequence {1} from the value {2}.",
                            suspectedSuffix,
                            lastSequence,
                            batesNumber));
                    }
                    else if (nextToLastSequence != suspectedNumber)
                    {
                        throw new Exception(String.Format(
                            "BatesNumber Exception: The suspected number {1} does not match the next to last numeric sequence {1} from the value {2}.",
                            suspectedNumber,
                            nextToLastSequence,
                            batesNumber));
                    }
                    else throw new Exception(String.Format(
                        "BatesNumber Exception: The suspected delimiter must be a '.', '_', or a '-' from the value {1}.",
                        suspectedDelim,
                        batesNumber));
                }
            }
            else
            {
                throw new Exception(String.Format("BatesNumber Exception: No numberic sequences were detected in the bates number {1}.", batesNumber));
            }
        }

        public static String NextNumber(String batesNumber)
        {
            BatesNumber bates = new BatesNumber(batesNumber);
            bates.IterateNumber();
            return bates.Value;
        }

        public static String PreviousNumber(String batesNumber)
        {
            BatesNumber bates = new BatesNumber(batesNumber);
            bates.DecreaseNumber();
            return bates.Value;
        }

        public void IterateNumber()
        {
            _number++;
            _hasSuffix = false;
            _value = String.Format("{0}{1}", Prefix, Number.ToString().PadLeft(_numberLength, '0'));
        }

        public void DecreaseNumber()
        {
            _number--;
            _hasSuffix = false;
            _value = String.Format("{0}{1}", Prefix, Number.ToString().PadLeft(_numberLength, '0'));
        }
    }
}
