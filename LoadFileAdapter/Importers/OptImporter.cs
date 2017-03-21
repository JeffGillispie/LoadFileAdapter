using System.Collections.Generic;
using System.IO;
using System.Text;
using LoadFileAdapter.Parsers;
using LoadFileAdapter.Builders;

namespace LoadFileAdapter.Importers
{
    public class OptImporter
    {
        private IParser<ParseFileDatSettings, ParseReaderDatSettings, ParseLineDatSettings> parser;
        private IBuilder<BuildDocCollectionImageSettings, BuildDocImageSettings> builder;

        public OptImporter()
        {
            this.parser = new DatParser();
            this.builder = new OptBuilder();
        }

        public OptImporter(
            IParser<ParseFileDatSettings, ParseReaderDatSettings, ParseLineDatSettings> parser, 
            IBuilder<BuildDocCollectionImageSettings, BuildDocImageSettings> builder)
        {
            this.parser = parser;
            this.builder = builder;
        }

        public DocumentCollection Import(FileInfo optFile, Encoding encoding, TextFileSettings textSetting)
        {
            Delimiters delimiters = Delimiters.COMMA_DELIMITED;
            ParseFileDatSettings parameters = new ParseFileDatSettings(optFile, encoding, delimiters);
            List<string[]> records = parser.Parse(parameters);            
            BuildDocCollectionImageSettings args = new BuildDocCollectionImageSettings(records, optFile.Directory.FullName, textSetting);
            List<Document> documentList = builder.BuildDocuments(args);
            DocumentCollection documents = new DocumentCollection(documentList);            
            return documents;
        }
    }
}
