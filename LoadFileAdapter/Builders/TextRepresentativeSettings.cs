using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LoadFileAdapter.Builders
{
    /// <summary>
    /// Represents the settings used to create text representatives from an image load file.
    /// </summary>
    public class TextRepresentativeSettings
    {
        private const string FILE_DELIM = "\\";
        private const string TEXT_EXT = ".txt";                
        private TextLevel fileLevel = TextLevel.None;        
        private TextLocation fileLocation = TextLocation.None;        
        private Regex pathFind = null;        
        private string pathReplace = String.Empty;

        /// <summary>
        /// Initializes a new instance of <see cref="TextRepresentativeSettings"/>.
        /// </summary>
        /// <param name="textLevel">The level of the text file representatives.</param>
        /// <param name="textLocation">The location of the text file representatives.</param>
        /// <param name="textPathFind">The regex used to find a part of the image path to replace.</param>
        /// <param name="textPathReplace">The replace value that results in the correct text path.</param>
        public TextRepresentativeSettings(TextLevel textLevel, TextLocation textLocation, Regex textPathFind, string textPathReplace)
        {
            this.fileLevel = textLevel;
            this.fileLocation = textLocation;
            this.pathFind = textPathFind;
            this.pathReplace = textPathReplace;
        }

        /// <summary>
        /// The level or scope of the text files (i.e. page level,
        /// document level, or nonoe).
        /// </summary>
        public enum TextLevel
        {
            None, Page, Doc
        }

        /// <summary>
        /// The general location of the text files relative to the image files 
        /// (i.e. none, same folder as the images, or an alternate location).
        /// </summary>
        public enum TextLocation
        {
            None, SameAsImages, AlternateLocation
        }

        /// <summary>
        /// The level or scope of the text files (i.e. page level,
        /// doc level, or none).
        /// </summary>
        public TextLevel FileLevel { get { return this.fileLevel; } }

        /// <summary>
        /// The general location of the text files relative to the image
        /// files (i.e. SameAsImages, AlternateDestination, or None).
        /// </summary>
        public TextLocation FileLocation { get { return this.fileLocation; } }

        /// <summary>
        /// The search pattern and options used to identify the part
        /// of the image file path to replace with the replace value.
        /// The result should be the correct text path.
        /// </summary>
        public Regex PathFind { get { return this.pathFind; } }

        /// <summary>
        /// The replace value that results in the correct text path value.
        /// </summary>
        public string PathReplace { get { return this.pathReplace; } }

        /// <summary>
        /// Transforms the image path to the text path.
        /// </summary>
        /// <param name="imagePath">The path to the image file.</param>
        /// <returns>The path to the text file.</returns>
        public string GetTextPathFromImagePath(string imagePath)
        {
            string[] path = imagePath.Split(
                new string[] { FILE_DELIM }, 
                StringSplitOptions.RemoveEmptyEntries);
            path = path.Take(path.Length - 1).ToArray();
            string textFolder = String.Join(FILE_DELIM, path);
            string textFile = Path.GetFileNameWithoutExtension(imagePath) + TEXT_EXT;

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
