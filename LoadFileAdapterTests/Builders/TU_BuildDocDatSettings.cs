using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter.Builders;

namespace LoadFileAdapterTests.Builders
{
    [TestClass]
    public class TU_BuildDocDatSettings
    {
        [TestMethod]
        public void Builders_BuildDocDatSettings_Properties()
        {
            string[] header = new string[] { "test" };
            string[] record = new string[] { "blah" };
            string keyColName = "name";
            string pathPrefix = "path";
            List<DatRepresentativeSettings> datRep = new List<DatRepresentativeSettings>();
            datRep.Add(new DatRepresentativeSettings("col", LoadFileAdapter.Representative.FileType.Native));
            var settings = new BuildDocDatSettings(record, header, keyColName, datRep, pathPrefix);

            Assert.AreEqual(header, settings.Header);
            Assert.AreEqual(record, settings.Record);
            Assert.AreEqual(keyColName, settings.KeyColumnName);
            Assert.AreEqual(pathPrefix, settings.PathPrefix);
            Assert.AreEqual(datRep, settings.RepresentativeColumnInfo);
        }
    }
}
