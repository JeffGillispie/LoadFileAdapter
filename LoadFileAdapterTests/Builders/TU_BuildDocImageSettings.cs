using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter.Builders;

namespace LoadFileAdapterTests.Builders
{
    [TestClass]
    public class TU_BuildDocImageSettings
    {
        [TestMethod]
        public void Builders_BuildDocImageSettings_Properties()
        {
            string pathPrefix = "path";
            List<string[]> pageRecords = new List<string[]>() { new string[] { "test" } };
            TextRepresentativeSettings txtSettings = null;
            var settings = new BuildDocImageSettings(pageRecords, txtSettings, pathPrefix);
            Assert.AreEqual(pathPrefix, settings.PathPrefix);
            Assert.AreEqual(pageRecords, settings.PageRecords);
            Assert.AreEqual(txtSettings, settings.TextSettings);
        }
    }
}
