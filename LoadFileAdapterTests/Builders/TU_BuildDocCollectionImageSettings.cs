using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter.Builders;

namespace LoadFileAdapterTests.Builders
{
    [TestClass]
    public class TU_BuildDocCollectionImageSettings
    {
        [TestMethod]
        public void Builders_BuildDocCollectionImageSettings_Properties()
        {
            List<string[]> records = new List<string[]>() { new string[] { "one", "two", "three" } };
            string pathPrefix = "path";
            TextRepresentativeSettings txtSettings = new TextRepresentativeSettings(
                TextRepresentativeSettings.TextLevel.Doc, 
                TextRepresentativeSettings.TextLocation.None, 
                new System.Text.RegularExpressions.Regex("find"), 
                "replace");
            BuildDocCollectionImageSettings settings = new BuildDocCollectionImageSettings(
                records, pathPrefix, txtSettings);

            Assert.AreEqual(records, settings.Records);
            Assert.AreEqual(pathPrefix, settings.PathPrefix);
            Assert.AreEqual(txtSettings, settings.TextSettings);
        }
    }
}
