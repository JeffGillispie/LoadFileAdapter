using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LoadFileAdapter.Builders;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Importers
{
    public class LfpImporter
    {
        private IParser<ParseFileSettings, ParseReaderSettings, ParseLineSettings> parser;
        private IBuilder<BuildDocCollectionImageSettings, BuildDocLfpSettings> builder;
        
        public LfpImporter()
        {
            this.parser = new LfpParser();
            this.builder = new LfpBuilder();
        }

        public LfpImporter(
            IParser<ParseFileSettings, ParseReaderSettings, ParseLineSettings> parser, 
            IBuilder<BuildDocCollectionImageSettings, BuildDocLfpSettings> builder)
        {
            this.parser = parser;
            this.builder = builder;
        }

        public DocumentCollection Import(FileInfo lfpFile, Encoding encoding, TextRepresentativeSettings textSetting)
        {
            ParseFileSettings parameters = new ParseFileSettings(lfpFile, encoding);
            List<string[]> records = parser.Parse(parameters);            
            BuildDocCollectionImageSettings args = new BuildDocCollectionImageSettings(records, lfpFile.Directory.FullName, textSetting);
            List<Document> documentList = builder.BuildDocuments(args);
            DocumentCollection documents = new DocumentCollection(documentList);            
            return documents;
        }                    
    }
}
