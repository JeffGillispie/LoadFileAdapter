using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LoadFileAdapter.Builders;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Importers
{
    /// <summary>
    /// An importer used to import a LFP file.
    /// </summary>
    public class LfpImporter
    {
        private IParser<ParseFileSettings, ParseReaderSettings, ParseLineSettings> parser;
        private IBuilder<BuildDocCollectionImageSettings, BuildDocLfpSettings> builder;
        
        /// <summary>
        /// Initializes a new instance of <see cref="LfpImporter"/>.
        /// </summary>
        public LfpImporter()
        {
            this.parser = new LfpParser();
            this.builder = new LfpBuilder();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="LfpImporter"/>.
        /// </summary>
        /// <param name="parser">The parser used to import the LFP file.</param>
        /// <param name="builder">The builder used to build the document collection.</param>
        public LfpImporter(
            IParser<ParseFileSettings, ParseReaderSettings, ParseLineSettings> parser, 
            IBuilder<BuildDocCollectionImageSettings, BuildDocLfpSettings> builder)
        {
            this.parser = parser;
            this.builder = builder;
        }

        /// <summary>
        /// Imports a LFP file into a document collection.
        /// </summary>
        /// <param name="lfpFile">The file to import.</param>
        /// <param name="encoding">The encoding used to read the import file.</param>
        /// <param name="textSetting">The text representative setting.</param>
        /// <param name="buildAbsolutePath">Use load file path in representative paths.</param>
        /// <returns>Returns a document collection of imported documents.</returns>
        public DocumentCollection Import(FileInfo lfpFile, Encoding encoding, 
            TextRepresentativeSettings textSetting, bool buildAbsolutePath)
        {
            ParseFileSettings parameters = new ParseFileSettings(lfpFile, encoding);
            List<string[]> records = parser.Parse(parameters);
            string pathPrefix = (buildAbsolutePath) ? lfpFile.Directory.FullName : null;
            BuildDocCollectionImageSettings args = new BuildDocCollectionImageSettings(
                records, pathPrefix, textSetting);
            List<Document> documentList = builder.BuildDocuments(args);
            DocumentCollection documents = new DocumentCollection(documentList);            
            return documents;
        }                    
    }
}
