﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using LoadFileAdapter.Parsers;
using LoadFileAdapter.Builders;

namespace LoadFileAdapter.Importers
{
    public class OptImporter
    {
        private IParser<TabularParseFileSetting, TabularParseReaderSetting, TabularParseLineSetting> parser;
        private IBuilder<ImageBuildDocumentsSetting, ImageBuildDocumentSetting> builder;

        public OptImporter()
        {
            this.parser = new TabularParser();
            this.builder = new OptBuilder();
        }

        public OptImporter(
            IParser<TabularParseFileSetting, TabularParseReaderSetting, TabularParseLineSetting> parser, 
            IBuilder<ImageBuildDocumentsSetting, ImageBuildDocumentSetting> builder)
        {
            this.parser = parser;
            this.builder = builder;
        }

        public DocumentCollection Import(FileInfo optFile, Encoding encoding, StructuredRepresentativeSetting textSetting)
        {
            Delimiters delimiters = Delimiters.COMMA_DELIMITED;
            TabularParseFileSetting parameters = new TabularParseFileSetting(optFile, encoding, delimiters);
            List<string[]> records = parser.Parse(parameters);            
            ImageBuildDocumentsSetting args = new ImageBuildDocumentsSetting(records, optFile.Directory.FullName, textSetting);
            List<Document> documentList = builder.BuildDocuments(args);
            DocumentCollection documents = new DocumentCollection(documentList);            
            return documents;
        }
    }
}
