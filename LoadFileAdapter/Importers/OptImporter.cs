using System.Collections.Generic;
using System.IO;
using System.Text;
using LoadFileAdapter.Parsers;
using LoadFileAdapter.Builders;

namespace LoadFileAdapter.Importers
{
    /// <summary>
    /// An importer used to import data from an OPT file.
    /// </summary>
    public class OptImporter
    {
        private IParser<ParseFileDatSettings, ParseReaderDatSettings, ParseLineDatSettings> parser;
        private IBuilder<BuildDocCollectionImageSettings, BuildDocImageSettings> builder;

        /// <summary>
        /// Initializes a new instance of <see cref="OptImporter"/>.
        /// </summary>
        public OptImporter()
        {
            this.parser = new DatParser();
            this.builder = new OptBuilder();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="OptImporter"/>.
        /// </summary>
        /// <param name="parser">The parser used to import the OPT file.</param>
        /// <param name="builder">The builder used to build the document collection.</param>
        public OptImporter(
            IParser<ParseFileDatSettings, ParseReaderDatSettings, ParseLineDatSettings> parser, 
            IBuilder<BuildDocCollectionImageSettings, BuildDocImageSettings> builder)
        {
            this.parser = parser;
            this.builder = builder;
        }

        /// <summary>
        /// Imports an OPT file into a document collection.
        /// </summary>
        /// <param name="optFile">The file to import.</param>
        /// <param name="encoding">The encoding used to read the import file.</param>
        /// <param name="textSetting">The text representative settings.</param>
        /// <param name="buildAbsolutePath">Use load file path in representative paths.</param>
        /// <returns>Returns a document collection of imported documents.</returns>
        public DocumentCollection Import(FileInfo optFile, Encoding encoding, 
            TextRepresentativeSettings textSetting, bool buildAbsolutePath)
        {
            Delimiters delimiters = Delimiters.COMMA_DELIMITED;
            ParseFileDatSettings parameters = new ParseFileDatSettings(optFile, encoding, delimiters);
            List<string[]> records = parser.Parse(parameters);
            string pathPrefix = (buildAbsolutePath) ? optFile.Directory.FullName : null;            
            BuildDocCollectionImageSettings args = new BuildDocCollectionImageSettings(
                records, pathPrefix, textSetting);
            List<Document> documentList = builder.BuildDocuments(args);
            DocumentCollection documents = new DocumentCollection(documentList);            
            return documents;
        }
    }
}
