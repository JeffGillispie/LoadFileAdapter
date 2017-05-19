using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter;
using LoadFileAdapter.Transformers;

namespace LoadFileAdapterTests.Transformers
{
    [TestClass]
    public class TU_DateFormatTransformation
    {
        class TestTransformer : DateFormatTransformation
        {
            public TestTransformer(DateTime start, DateTime end) : 
                base("", null, "", "", null, "", "", null, null, start, end, FailAction.DoNothing)
            {
                // do nothing here
            }

            public new bool isInRange(DateTime date)
            {
                return base.isInRange(date);
            }
        }

        [TestMethod]
        public void Transformers_DateFormatTransformation_isInRange()
        {
            DateTime start = new DateTime(1999, 1, 1);
            DateTime end = new DateTime(1999, 12, 31);
            TestTransformer t = new TestTransformer(start, end);
            DateTime before = new DateTime(1998, 12, 31);
            DateTime after = new DateTime(2000, 1, 1);
            DateTime during = new DateTime(1999, 5, 3);                        
            bool isInRange = t.isInRange(before);
            Assert.IsFalse(isInRange);
            isInRange = t.isInRange(during);
            Assert.IsTrue(isInRange);
            isInRange = t.isInRange(after);
            Assert.IsFalse(isInRange);
            isInRange = t.isInRange(start);
            Assert.IsTrue(isInRange);
            isInRange = t.isInRange(end);
            Assert.IsTrue(isInRange);
        }

        public string getTransformedDate(string input, string format)
        {
            return getTransformedDate(input, null, format);
        }

        public string getTransformedDate(string input, string inputformat, string outputformat)
        {
            return getTransformedDate(input, inputformat, outputformat, null, null);
        }

        public string getTransformedDate (string value, string inFormat, string outFormat, TimeZoneInfo inZone, TimeZoneInfo outZone)
        {
            string fieldName = "date";
            Regex findText = new Regex("");
            string replaceText = "";
            string filterField = "";
            Regex filterText = new Regex("");
            DateTime rangeStart = new DateTime(2000, 1, 1);
            DateTime rangeEnd = DateTime.Today;
            DateFormatTransformation.FailAction onFailure = DateFormatTransformation.FailAction.DoNothing;
            DateFormatTransformation t = new DateFormatTransformation(
                fieldName, findText, replaceText, filterField, filterText,
                outFormat, inFormat, outZone, inZone, rangeStart, rangeEnd, onFailure);
            string key = "DOC000001";
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add("date", value);
            Document doc = new Document(key, null, null, metadata, null);
            t.Transform(doc);
            return doc.Metadata[fieldName];
        }

        [TestMethod]
        public void Transformers_DateFormatTransformation_Transform()
        {
            // test output formats            
            Assert.AreEqual("10/1/2012", getTransformedDate("10/1/2012 1:25:30 PM", "d"));
            Assert.AreEqual("Monday, October 1, 2012", getTransformedDate("10/1/2012 1:25:30 PM", "D"));
            Assert.AreEqual("Monday, October 1, 2012 1:25 PM", getTransformedDate("10/1/2012 1:25:30 PM", "f"));
            Assert.AreEqual("Monday, October 1, 2012 1:25:30 PM", getTransformedDate("10/1/2012 1:25:30 PM", "F"));
            Assert.AreEqual("10/1/2012 1:25 PM", getTransformedDate("10/1/2012 1:25:30 PM", "g"));
            Assert.AreEqual("10/1/2012 1:25:30 PM", getTransformedDate("10/1/2012 1:25:30 PM", "G"));
            Assert.AreEqual("October 1", getTransformedDate("10/1/2012 1:25:30 PM", "M"));
            Assert.AreEqual("October 1", getTransformedDate("10/1/2012 1:25:30 PM", "m"));
            Assert.AreEqual("2012-10-01T13:25:30.0000000", getTransformedDate("10/1/2012 1:25:30 PM", "O"));
            Assert.AreEqual("2012-10-01T13:25:30.0000000", getTransformedDate("10/1/2012 1:25:30 PM", "o"));
            Assert.AreEqual("Mon, 01 Oct 2012 13:25:30 GMT", getTransformedDate("10/1/2012 1:25:30 PM", "R"));
            Assert.AreEqual("Mon, 01 Oct 2012 13:25:30 GMT", getTransformedDate("10/1/2012 1:25:30 PM", "r"));
            Assert.AreEqual("2012-10-01T13:25:30", getTransformedDate("10/1/2012 1:25:30 PM", "s"));
            Assert.AreEqual("1:25 PM", getTransformedDate("10/1/2012 1:25:30 PM", "t"));
            Assert.AreEqual("1:25:30 PM", getTransformedDate("10/1/2012 1:25:30 PM", "T"));
            Assert.AreEqual("2012-10-01 13:25:30Z", getTransformedDate("10/1/2012 1:25:30 PM", "u"));
            Assert.AreEqual("Monday, October 1, 2012 6:25:30 PM", getTransformedDate("10/1/2012 1:25:30 PM", "U"));
            Assert.AreEqual("October 2012", getTransformedDate("10/1/2012 1:25:30 PM", "y"));
            Assert.AreEqual("October 2012", getTransformedDate("10/1/2012 1:25:30 PM", "Y"));
            Assert.AreEqual("10-01-12", getTransformedDate("10/1/2012", "MM-dd-yy"));
            // test input format
            Assert.AreEqual("01:25PM", getTransformedDate("13:25:30", "HH:mm:ss", "hh:mmtt"));
            // test time zone abbreviation replacement
            Assert.AreEqual("9/9/2015 7:15:52 PM", getTransformedDate("2015/09/09 19:15:52.000 UTC", "G"));
            Assert.AreEqual("9/10/2015 12:15:52 AM", getTransformedDate("2015/09/09 19:15:52.000 CDT", "G"));
            // test time zone out
            TimeZoneInfo outTZ = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            TimeZoneInfo inTZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            Assert.AreEqual("10/1/2012 6:25 AM", getTransformedDate("10/1/2012 1:25:30 PM", "", "g", null, outTZ));
            Assert.AreEqual("12/9/2015 4:15 PM", getTransformedDate("2015/12/09 19:15:52.000 EST", "", "g", null, outTZ));
            // test time zone in
            Assert.AreEqual("12/1/2012 10:25 AM", getTransformedDate("12/1/2012 1:25:30 PM", "", "g", inTZ, outTZ));
            Assert.AreEqual("5/19/2017 5:25 PM", getTransformedDate("5/19/2017 1:25:30 PM", "", "g", inTZ, null));
            Assert.AreEqual("12/9/2015 5:15 PM", getTransformedDate("2015/12/09 19:15:52.000 CST", "", "g", inTZ, outTZ));
            Assert.AreEqual("12/10/2015 1:15 AM", getTransformedDate("2015/12/09 19:15:52.000 CST", "", "g", inTZ, null));
        }
    }
}
