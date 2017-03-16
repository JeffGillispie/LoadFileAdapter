using System;
using System.IO;
using System.Text.RegularExpressions;

namespace LoadFileAdapter
{
    public class StructuredRepresentativeSetting
    {
        private const string TEXT_EXT = ".txt";

        public enum TextLevel
        {
            None, Page, Doc
        }

        public enum TextLocation
        {
            None, SameAsImages, AlternateLocation
        }

        private TextLevel textLevel = TextLevel.None;
        private TextLocation textLocation = TextLocation.None;        
        private Regex textPathFind = null;
        private string textPathReplace = String.Empty;

        public StructuredRepresentativeSetting(TextLevel textLevel, TextLocation textLocation, Regex textPathFind, string textPathReplace)
        {
            this.textLevel = textLevel;
            this.textLocation = textLocation;
            this.textPathFind = textPathFind;
            this.textPathReplace = textPathReplace;
        }

        public TextLevel RepresentativeTextLevel { get { return this.textLevel; } }
        public TextLocation RepresentativeTextLocation { get { return this.textLocation; } }
        public Regex TextPathFind { get { return this.textPathFind; } }
        public string TextPathReplace { get { return this.textPathReplace; } }

        public string GetTextPathFromImagePath(string imagePath)
        {
            FileInfo image = new FileInfo(imagePath);
            string textFolder = image.Directory.FullName;
            string textFile = Path.GetFileNameWithoutExtension(image.Name + TEXT_EXT);

            switch(this.textLocation)
            {
                case TextLocation.SameAsImages:
                    // nothing to replace
                    // do nothing here
                    break;
                case TextLocation.AlternateLocation:
                    textFolder = this.textPathFind.Replace(textFolder, this.textPathReplace);
                    break;
                default:
                    // do nothing here
                    break;
            }

            return Path.Combine(textFolder, textFile);
        }
    }
}
