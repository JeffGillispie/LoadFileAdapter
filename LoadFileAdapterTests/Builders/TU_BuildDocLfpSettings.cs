using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter.Builders;

namespace LoadFileAdapterTests.Builders
{
    [TestClass]
    public class TU_BuildDocLfpSettings
    {
        [TestMethod]
        public void Builders_BuildDocLfpSettings_Properties()
        {
            List<string[]> pageRecords = new List<string[]>() { new string[] { "test" } };
            string[] nativeRecord = new string[] { "test" };
            TextRepresentativeSettings txtSetting = null;
            string pathPrefix = "test";
            var settings = new BuildDocLfpSettings(pageRecords, nativeRecord, txtSetting, pathPrefix);
            Assert.AreEqual(pageRecords, settings.PageRecords);
            Assert.AreEqual(nativeRecord, settings.NativeRecord);
            Assert.AreEqual(txtSetting, settings.TextSettings);
            Assert.AreEqual(pathPrefix, settings.PathPrefix);
        }
    }
}
