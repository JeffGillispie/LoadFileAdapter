﻿using System.IO;
using System.Text;
using LoadFileAdapter.Builders;
using LoadFileAdapter.Importers;

namespace LoadFileAdapter.Instructions
{
    public class OptImport : ImgImport
    {
        public OptImport() : base()
        {
            // do nothing here
        }

        /// <summary>
        /// Initializes a new instance of <see cref="OptImport"/>.
        /// </summary>
        /// <param name="file">The file to import.</param>
        /// <param name="encoding">The encoding used to read the import.</param>
        /// <param name="txtSetting">The text representative settings for the import.</param>
        /// <param name="buildAbsolutePath">Setting for building representative absolute paths.</param>
        public OptImport(FileInfo file, Encoding encoding, TextBuilder txtSetting, bool buildAbsolutePath) : 
            base(file, encoding, txtSetting, buildAbsolutePath)
        {
            // do nothing here
        }

        public override IImporter BuildImporter()
        {
            OptImporter importer = new OptImporter();
            importer.Builder.PathPrefix = (BuildAbsolutePath)
                ? File.Directory.FullName : null;
            importer.Builder.TextBuilder = (TextBuilder != null)
                ? TextBuilder.GetBuilder() : null;
            return importer;
        }
    }
}
