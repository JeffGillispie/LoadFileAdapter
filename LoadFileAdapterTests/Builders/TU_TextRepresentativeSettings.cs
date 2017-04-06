using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter.Builders;

namespace LoadFileAdapterTests.Builders
{
    [TestClass]
    public class TU_TextRepresentativeSettings
    {
        [TestMethod]
        public void Builders_TextRepresentativeSettings_GetTextPath()
        {
            TextRepresentativeSettings settingsA = new TextRepresentativeSettings(
                TextRepresentativeSettings.TextLevel.Page,
                TextRepresentativeSettings.TextLocation.SameAsImages,
                new Regex("\\\\IMAGES", RegexOptions.IgnoreCase), 
                "\\TEXT");
            TextRepresentativeSettings settingsB = new TextRepresentativeSettings(
                TextRepresentativeSettings.TextLevel.Page,
                TextRepresentativeSettings.TextLocation.AlternateLocation,
                new Regex("\\\\IMAGES", RegexOptions.IgnoreCase),
                "\\TEXT");
            TextRepresentativeSettings settingsC = new TextRepresentativeSettings(
                TextRepresentativeSettings.TextLevel.Page,
                TextRepresentativeSettings.TextLocation.None,
                new Regex("\\\\IMAGES", RegexOptions.IgnoreCase),
                "\\TEXT");
            TextRepresentativeSettings settingsD = new TextRepresentativeSettings(
                TextRepresentativeSettings.TextLevel.Doc,
                TextRepresentativeSettings.TextLocation.SameAsImages,
                new Regex("\\\\IMAGES", RegexOptions.IgnoreCase),
                "\\TEXT");
            TextRepresentativeSettings settingsE = new TextRepresentativeSettings(
                TextRepresentativeSettings.TextLevel.Doc,
                TextRepresentativeSettings.TextLocation.AlternateLocation,
                new Regex("\\\\IMAGES", RegexOptions.IgnoreCase),
                "\\TEXT");
            TextRepresentativeSettings settingsF = new TextRepresentativeSettings(
                TextRepresentativeSettings.TextLevel.Doc,
                TextRepresentativeSettings.TextLocation.None,
                new Regex("\\\\IMAGES", RegexOptions.IgnoreCase),
                "\\TEXT");
            string image = "X:\\VOL001\\images\\001\\DOC000001.TIF";
            Assert.AreEqual(@"X:\VOL001\images\001\DOC000001.txt", settingsA.GetTextPathFromImagePath(image));
            Assert.AreEqual(@"X:\VOL001\TEXT\001\DOC000001.txt", settingsB.GetTextPathFromImagePath(image));
            Assert.AreEqual(@"X:\VOL001\images\001\DOC000001.txt", settingsC.GetTextPathFromImagePath(image));
            Assert.AreEqual(@"X:\VOL001\images\001\DOC000001.txt", settingsD.GetTextPathFromImagePath(image));
            Assert.AreEqual(@"X:\VOL001\TEXT\001\DOC000001.txt", settingsE.GetTextPathFromImagePath(image));
            Assert.AreEqual(@"X:\VOL001\images\001\DOC000001.txt", settingsF.GetTextPathFromImagePath(image));

        }
    }
}
