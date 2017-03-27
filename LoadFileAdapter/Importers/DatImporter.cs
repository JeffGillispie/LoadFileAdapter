using System.Collections.Generic;
using System.IO;
using System.Text;
using LoadFileAdapter.Builders;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Importers
{
    /// <summary>
    /// An importer used to import a DAT file.
    /// </summary>
    public class DatImporter
    {
        private IParser<ParseFileDatSettings, ParseReaderDatSettings, ParseLineDatSettings> parser;
        private IBuilder<BuildDocCollectionDatSettings, BuildDocDatSettings> builder;

        /// <summary>
        /// Initializes a new instance of <see cref="DatImporter"/>.
        /// </summary>
        public DatImporter()
        {
            this.parser = new DatParser();
            this.builder = new DatBuilder();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DatImporter"/>.
        /// </summary>
        /// <param name="parser">The parser used to import the DAT file.</param>
        /// <param name="builder">The builder used to build the document collection.</param>
        public DatImporter(
            IParser<ParseFileDatSettings, ParseReaderDatSettings, ParseLineDatSettings> parser, 
            IBuilder<BuildDocCollectionDatSettings, BuildDocDatSettings> builder)
        {
            this.parser = parser;
            this.builder = builder;
        }

        /// <summary>
        /// Imports data from a DAT file into a document collection.
        /// </summary>
        /// <param name="file">The file to import.</param>
        /// <param name="encoding">The encoding of the import file.</param>
        /// <param name="delims">The delimiters used in the import file.</param>
        /// <param name="hasHeader">Indicates if the import file contains a header.</param>
        /// <param name="keyColName">The name of the DocID column.</param>
        /// <param name="parentColName">The name of the ParentID column.</param>
        /// <param name="childColName">The name of the AttachIDs column.</param>
        /// <param name="childSeparator">The delimiter used in the AttachIDs column.</param>
        /// <param name="repColInfo">A list of representative settings.</param>
        /// <returns>Returns a document collection of the imported DAT file.</returns>
        public DocumentCollection Import(FileInfo file, Encoding encoding, Delimiters delims, bool hasHeader, string keyColName, 
            string parentColName, string childColName, string childSeparator, List<DatRepresentativeSettings> repColInfo)
        {
            ParseFileDatSettings parameters = new ParseFileDatSettings(file, encoding, delims);
            List<string[]> records = parser.Parse(parameters);            
            BuildDocCollectionDatSettings args = new BuildDocCollectionDatSettings(
                records, file.Directory.FullName, hasHeader, keyColName, parentColName, childColName, childSeparator, repColInfo);
            List<Document> documentList = builder.BuildDocuments(args);
            DocumentCollection documents = new DocumentCollection(documentList);            
            return documents;
        }
    }
}
