using System.Collections.Generic;
using System.IO;
using System.Text;
using LoadFileAdapter.Builders;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Importers
{
    public class TabularImporter
    {
        IParser<TabularParseFileSetting, TabularParseReaderSetting, TabularParseLineSetting> parser;
        IBuilder<BuildDocCollectionDatSettings, BuildDocDatSettings> builder;

        public TabularImporter()
        {
            this.parser = new TabularParser();
            this.builder = new DatBuilder();
        }

        public TabularImporter(
            IParser<TabularParseFileSetting, TabularParseReaderSetting, TabularParseLineSetting> parser, 
            IBuilder<BuildDocCollectionDatSettings, BuildDocDatSettings> builder)
        {
            this.parser = parser;
            this.builder = builder;
        }

        public DocumentCollection Import(FileInfo file, Encoding encoding, Delimiters delims, bool hasHeader, string keyColName, 
            string parentColName, string childColName, string childSeparator, List<LinkFileSettings> repColInfo)
        {
            TabularParseFileSetting parameters = new TabularParseFileSetting(file, encoding, delims);
            List<string[]> records = parser.Parse(parameters);            
            BuildDocCollectionDatSettings args = new BuildDocCollectionDatSettings(
                records, file.Directory.FullName, hasHeader, keyColName, parentColName, childColName, childSeparator, repColInfo);
            List<Document> documentList = builder.BuildDocuments(args);
            DocumentCollection documents = new DocumentCollection(documentList);            
            return documents;
        }
    }
}
