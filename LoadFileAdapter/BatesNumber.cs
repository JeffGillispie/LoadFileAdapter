using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LoadFileAdapter
{
    public class BatesNumber
    {
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

        public string Value { get { return value; } }
        public string Prefix { get { if (hasPrefix) return prefix; else return String.Empty; } }
        public int Number { get { return number; } }
        public string SuffixDelimiter { get { if (hasDelim) return suffixDelimiter; else return String.Empty; } }
        public int Suffix { get { if (hasSuffix) return suffix; else return 0; } }
        public bool HasSuffix { get { return hasSuffix; } }
        public bool HasPrefix { get { return hasPrefix; } }
        public bool HasDelim { get { return hasDelim; } }
        public string NumberAsString { get { return number.ToString().PadLeft(numberLength, '0'); } }
        public string SuffixAsString { get { return suffixDelimiter + suffix.ToString().PadLeft(suffixLength, '0'); } }

        public BatesNumber(string batesNumber)
        {
            List<string> numericSequences = new List<string>();
            MatchCollection matches = Regex.Matches(batesNumber, @"\d+", RegexOptions.None);
            numericSequences = matches.Cast<Match>().Select(match => match.Value).ToList();

            if (numericSequences.Count == 1)
            {
                if (batesNumber.Substring(
                        batesNumber.Length - numericSequences[0].Length, 
                        numericSequences[0].Length
                    ) == numericSequences[0])
                {
                    value = batesNumber;
                    number = int.Parse(numericSequences[0]);
                    numberLength = numericSequences[0].Length;
                    hasDelim = false;
                    hasSuffix = false;

                    if (batesNumber == numericSequences[0])
                    {
                        hasPrefix = false;
                    }
                    else
                    {
                        hasPrefix = true;
                        prefix = batesNumber.Substring(0, batesNumber.Length - numericSequences[0].Length);
                    }
                }
                else
                {
                    throw new Exception(String.Format(
                        "BatesNumber Exception: The numberic sequence {0} in the bates number {1} is not in the correct position, " +
                        "it should be at the end of the value.",
                        numericSequences[0], batesNumber));
                }
            }
            else if (numericSequences.Count >= 2)
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
                    (suspectedDelim == "." || suspectedDelim == "-" || suspectedDelim == "_"))
                {
                    value = batesNumber;
                    number = int.Parse(suspectedNumber);
                    numberLength = suspectedNumber.Length;
                    suffix = int.Parse(suspectedSuffix);
                    suffixLength = suspectedSuffix.Length;
                    suffixDelimiter = suspectedDelim;
                    hasDelim = true;
                    hasSuffix = true;

                    if (!String.IsNullOrWhiteSpace(suspectedPrefix))
                    {
                        hasPrefix = true;
                        prefix = suspectedPrefix;
                    }
                    else
                    {
                        hasPrefix = false;
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
                throw new Exception(String.Format(
                    "BatesNumber Exception: No numberic sequences were detected in the bates number {1}.", 
                    batesNumber));
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
            number++;
            hasSuffix = false;
            value = String.Format("{0}{1}", Prefix, Number.ToString().PadLeft(numberLength, '0'));
        }

        public void DecreaseNumber()
        {
            number--;
            hasSuffix = false;
            value = String.Format("{0}{1}", Prefix, Number.ToString().PadLeft(numberLength, '0'));
        }
    }
}
