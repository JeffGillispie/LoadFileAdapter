using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter.Builders;

namespace LoadFileAdapterTests.Builders
{
    [TestClass]
    public class TU_BuildDocCollectionDatSettings
    {
        [TestMethod]
        public void Builders_BuildDocCollectionDatSettings_Properties()
        {
            List<string[]> records = new List<string[]>() { new string[] { "blah" } };            
            string pathPrefix = "one";
            bool hasHeader = true;
            string keyColName = "two";
            string parentColName = "three";
            string childColName = "four";
            string childColDelim = "five";
            List<DatRepresentativeSettings> repColInfo = new List<DatRepresentativeSettings>();

            BuildDocCollectionDatSettings settings = new BuildDocCollectionDatSettings(
                records, 
                pathPrefix, 
                hasHeader, 
                keyColName, 
                parentColName, 
                childColName, 
                childColDelim, 
                repColInfo);

            Assert.AreEqual(records, settings.Records);
            Assert.AreEqual(pathPrefix, settings.PathPrefix);
            Assert.AreEqual(hasHeader, settings.HasHeader);
            Assert.AreEqual(keyColName, settings.KeyColumnName);
            Assert.AreEqual(parentColName, settings.ParentColumnName);
            Assert.AreEqual(childColName, settings.ChildColumnName);
            Assert.AreEqual(childColDelim, settings.ChildColumnDelimiter);
            Assert.AreEqual(repColInfo, settings.RepresentativeColumnInfo);
        }
    }
}
