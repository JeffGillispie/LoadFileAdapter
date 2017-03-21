using System;
using System.IO;
using System.Text.RegularExpressions;

namespace LoadFileAdapter.Builders
{
    public class TextFileSettings
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

        private TextLevel fileLevel = TextLevel.None;
        private TextLocation fileLocation = TextLocation.None;        
        private Regex pathFind = null;
        private string pathReplace = String.Empty;

        public TextFileSettings(TextLevel textLevel, TextLocation textLocation, Regex textPathFind, string textPathReplace)
        {
            this.fileLevel = textLevel;
            this.fileLocation = textLocation;
            this.pathFind = textPathFind;
            this.pathReplace = textPathReplace;
        }

        public TextLevel FileLevel { get { return this.fileLevel; } }
        public TextLocation FileLocation { get { return this.fileLocation; } }
        public Regex PathFind { get { return this.pathFind; } }
        public string PathReplace { get { return this.pathReplace; } }

        public string GetTextPathFromImagePath(string imagePath)
        {
            FileInfo image = new FileInfo(imagePath);
            string textFolder = image.Directory.FullName;
            string textFile = Path.GetFileNameWithoutExtension(image.Name + TEXT_EXT);

            switch(this.fileLocation)
            {
                case TextLocation.SameAsImages:
                    // nothing to replace
                    // do nothing here
                    break;
                case TextLocation.AlternateLocation:
                    textFolder = this.pathFind.Replace(textFolder, this.pathReplace);
                    break;
                default:
                    // do nothing here
                    break;
            }

            return Path.Combine(textFolder, textFile);
        }
    }
}
