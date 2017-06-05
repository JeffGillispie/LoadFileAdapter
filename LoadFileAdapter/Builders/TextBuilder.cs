﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LoadFileAdapter.Builders
{
    /// <summary>
    /// Builds text representatives from an image load file.
    /// </summary>
    public class TextBuilder
    {
        private const string FILE_DELIM = "\\";
        private const string TEXT_EXT = ".txt";                
        private TextLevel fileLevel = TextLevel.None;        
        private TextLocation fileLocation = TextLocation.None;        
        private Regex pathFind = null;        
        private string pathReplace = String.Empty;

        /// <summary>
        /// Initializes a new instance of <see cref="TextBuilder"/>.
        /// </summary>
        /// <param name="textLevel">The level of the text file representatives.</param>
        /// <param name="textLocation">The location of the text file representatives.</param>
        /// <param name="textPathFind">The regex used to find a part of the image path to replace.</param>
        /// <param name="textPathReplace">The replace value that results in the correct text path.</param>
        public TextBuilder(TextLevel textLevel, TextLocation textLocation, Regex textPathFind, string textPathReplace)
        {
            this.fileLevel = textLevel;
            this.fileLocation = textLocation;
            this.pathFind = textPathFind;
            this.pathReplace = textPathReplace;
        }

        /// <summary>
        /// The level or scope of the text files (i.e. page level,
        /// document level, or none).
        /// </summary>
        public enum TextLevel
        {
            /// <summary>
            /// No text exists.
            /// </summary>
            None,
            /// <summary>
            /// There is a text file for each page.
            /// </summary>
            Page,
            /// <summary>
            ///  There is a single text file for the document.
            /// </summary>
            Doc
        }

        /// <summary>
        /// The general location of the text files relative to the image files 
        /// (i.e. none, same folder as the images, or an alternate location).
        /// </summary>
        public enum TextLocation
        {
            /// <summary>
            /// No text exits.
            /// </summary>
            None,
            /// <summary>
            /// The text files are in the same folder as the image files.
            /// With the same base name.
            /// </summary>
            SameAsImages,
            /// <summary>
            /// The text files are in a different folder, but the text
            /// sub-folder structure matches the image sub-folder structure
            /// and the base name of the text files match the base name of 
            /// the image files.
            /// </summary>
            AlternateLocation
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

            if (this.fileLocation == TextLocation.AlternateLocation)
            {
                textFolder = this.pathFind.Replace(textFolder, this.pathReplace);
            }
                        
            return Path.Combine(textFolder, textFile);
        }

        /// <summary>
        /// Builds a text <see cref="Representative"/> from an image load file.
        /// </summary>
        /// <param name="pages">The parsed lines from a load file which span all pages for a <see cref="Document"/>.</param>
        /// <param name="assembler">A function that assembles the parsed line from a page into a image key and image path.</param>
        /// <returns>Returns a text <see cref="Representative"/>.</returns>
        public Representative Build(IEnumerable<string[]> pages, Func<string[], KeyValuePair<string, string>> assembler)
        {
            SortedDictionary<string, string> files = new SortedDictionary<string, string>();
            List<string[]> records = new List<string[]>();

            if (FileLevel == TextLevel.Page)
            {
                foreach (string[] page in pages)
                {
                    records.Add(page);                    
                }
            }
            else if (FileLevel == TextLevel.Doc)
            {
                records.Add(pages.First());
            }

            foreach (string[] record in records)
            {
                var result = assembler.Invoke(record);
                string key = result.Key;
                string path = GetTextPathFromImagePath(result.Value);
                files.Add(key, path);
            }

            Representative text = (files.Count > 0 ) 
                ? new Representative(Representative.FileType.Text, files) 
                : null;
            return text;
        }
    }
}
