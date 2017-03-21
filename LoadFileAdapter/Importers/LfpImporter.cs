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
        private IParser<ParseFileSetting, ParseReaderSetting, ParseLineSetting> parser;
        private IBuilder<BuildDocCollectionImageSettings, BuildDocLfpSettings> builder;
        
        public LfpImporter()
        {
            this.parser = new LfpParser();
            this.builder = new LfpBuilder();
        }

        public LfpImporter(
            IParser<ParseFileSetting, ParseReaderSetting, ParseLineSetting> parser, 
            IBuilder<BuildDocCollectionImageSettings, BuildDocLfpSettings> builder)
        {
            this.parser = parser;
            this.builder = builder;
        }

        public DocumentCollection Import(FileInfo lfpFile, Encoding encoding, StructuredRepresentativeSetting textSetting)
        {
            ParseFileSetting parameters = new ParseFileSetting(lfpFile, encoding);
            List<string[]> records = parser.Parse(parameters);            
            BuildDocCollectionImageSettings args = new BuildDocCollectionImageSettings(records, lfpFile.Directory.FullName, textSetting);
            List<Document> documentList = builder.BuildDocuments(args);
            DocumentCollection documents = new DocumentCollection(documentList);            
            return documents;
        }                    
    }
}
