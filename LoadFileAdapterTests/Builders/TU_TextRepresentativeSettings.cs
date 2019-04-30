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
            TextBuilder settingsA = new TextBuilder(
                TextBuilder.TextLevel.Page,
                TextBuilder.TextLocation.SameAsImages,
                new Regex("\\\\IMAGES", RegexOptions.IgnoreCase), 
                "\\TEXT");
            TextBuilder settingsB = new TextBuilder(
                TextBuilder.TextLevel.Page,
                TextBuilder.TextLocation.AlternateLocation,
                new Regex("\\\\IMAGES", RegexOptions.IgnoreCase),
                "\\TEXT");
            TextBuilder settingsC = new TextBuilder(
                TextBuilder.TextLevel.Page,
                TextBuilder.TextLocation.None,
                new Regex("\\\\IMAGES", RegexOptions.IgnoreCase),
                "\\TEXT");
            TextBuilder settingsD = new TextBuilder(
                TextBuilder.TextLevel.Doc,
                TextBuilder.TextLocation.SameAsImages,
                new Regex("\\\\IMAGES", RegexOptions.IgnoreCase),
                "\\TEXT");
            TextBuilder settingsE = new TextBuilder(
                TextBuilder.TextLevel.Doc,
                TextBuilder.TextLocation.AlternateLocation,
                new Regex("\\\\IMAGES", RegexOptions.IgnoreCase),
                "\\TEXT");
            TextBuilder settingsF = new TextBuilder(
                TextBuilder.TextLevel.Doc,
                TextBuilder.TextLocation.None,
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
