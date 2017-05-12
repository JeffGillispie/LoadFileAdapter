using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter;
using LoadFileAdapter.Builders;

namespace LoadFileAdapterTests.Builders
{
    [TestClass]
    public class TU_DatRepresentativeSettings
    {
        [TestMethod]
        public void Builders_DatRepresentativeSettings_Properties()
        {
            string colName = "name";
            Representative.FileType type = Representative.FileType.Native;
            DatRepresentativeSettings settings = new DatRepresentativeSettings(colName, type);
            Assert.AreEqual(colName, settings.ColumnName);
            Assert.AreEqual(type, settings.Type);
        }
    }
}
