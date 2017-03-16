using System.Collections.Generic;
using System.IO;
using System.Text;
using LoadFileAdapter.Builders;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Importers
{
    public class TabularImporter
    {
        Parser<TabularParseFileSetting, TabularParseReaderSetting, TabularParseLineSetting> parser;
        Builder<TabularBuildDocumentsSetting, TabularBuildDocumentSetting> builder;

        public TabularImporter()
        {
            this.parser = new TabularParser();
            this.builder = new TabularBuilder();
        }

        public TabularImporter(
            Parser<TabularParseFileSetting, TabularParseReaderSetting, TabularParseLineSetting> parser, 
            Builder<TabularBuildDocumentsSetting, TabularBuildDocumentSetting> builder)
        {
            this.parser = parser;
            this.builder = builder;
        }

        public DocumentCollection Import(FileInfo file, Encoding encoding, Delimiters delims, bool hasHeader, string keyColName, 
            string parentColName, string childColName, string childSeparator, List<SemiStructuredRepresentativeSetting> repColInfo)
        {
            TabularParseFileSetting parameters = new TabularParseFileSetting(file, encoding, delims);
            List<string[]> records = parser.Parse(parameters);            
            TabularBuildDocumentsSetting args = new TabularBuildDocumentsSetting(
                records, file.Directory.FullName, hasHeader, keyColName, parentColName, childColName, childSeparator, repColInfo);
            List<Document> documentList = builder.BuildDocuments(args);
            DocumentCollection documents = new DocumentCollection(documentList);            
            return documents;
        }
    }
}
