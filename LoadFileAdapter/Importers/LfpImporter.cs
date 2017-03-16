﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LoadFileAdapter.Builders;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Importers
{
    public class LfpImporter
    {
        private Parser<ParseFileSetting, ParseReaderSetting, ParseLineSetting> parser;
        private Builder<ImageBuildDocumentsSetting, LfpBuildDocumentSetting> builder;
        
        public LfpImporter()
        {
            this.parser = new LfpParser();
            this.builder = new LfpBuilder();
        }

        public LfpImporter(
            Parser<ParseFileSetting, ParseReaderSetting, ParseLineSetting> parser, 
            Builder<ImageBuildDocumentsSetting, LfpBuildDocumentSetting> builder)
        {
            this.parser = parser;
            this.builder = builder;
        }

        public DocumentCollection Import(FileInfo lfpFile, Encoding encoding, StructuredRepresentativeSetting textSetting)
        {
            ParseFileSetting parameters = new ParseFileSetting(lfpFile, encoding);
            List<string[]> records = parser.Parse(parameters);            
            ImageBuildDocumentsSetting args = new ImageBuildDocumentsSetting(records, lfpFile.Directory.FullName, textSetting);
            List<Document> documentList = builder.BuildDocuments(args);
            DocumentCollection documents = new DocumentCollection(documentList);            
            return documents;
        }                    
    }
}
