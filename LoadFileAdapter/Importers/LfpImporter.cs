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
        private Parser<ParseFileParameters, ParseReaderParameters, ParseLineParameters> parser = new LfpParser();
        private Builder builder = new LfpBuilder();

        public DocumentSet Import(FileInfo lfpFile, Encoding encoding, StructuredRepresentativeSetting textSetting)
        {
            ParseFileParameters parameters = new ParseFileParameters(lfpFile, encoding);
            List<string[]> records = parser.Parse(parameters);
            DocumentSetBuilderArgs args = DocumentSetBuilderArgs.GetImageSetArgs(records, lfpFile.Directory.FullName, textSetting);
            List<Document> documents = builder.BuildDocuments(args);
            DocumentSet docSet = new DocumentSet();
            docSet.AddDocuments(documents);
            return docSet;
        }

        public DocumentSet Import(TextReader reader, StructuredRepresentativeSetting textSetting)
        {
            ParseReaderParameters parameters = new ParseReaderParameters(reader);
            List<string[]> records = parser.Parse(parameters);
            DocumentSetBuilderArgs args = DocumentSetBuilderArgs.GetImageSetArgs(records, String.Empty, textSetting);
            List<Document> documents = builder.BuildDocuments(args);
            DocumentSet docSet = new DocumentSet();
            docSet.AddDocuments(documents);
            return docSet;
        }                
    }
}
