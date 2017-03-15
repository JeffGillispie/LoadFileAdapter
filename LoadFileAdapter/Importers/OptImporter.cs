using System.Collections.Generic;
using System.IO;
using System.Text;
using LoadFileAdapter.Parsers;
using LoadFileAdapter.Builders;

namespace LoadFileAdapter.Importers
{
    public class OptImporter
    {
        private Parser<TextDelimitedParseFileParameters, TextDelimitedParseReaderParameters, TextDelimitedParseLineParameters> parser = new TextDelimitedParser();
        private Builder builder = new OptBuilder();

        public DocumentSet Import(FileInfo optFile, Encoding encoding, StructuredRepresentativeSetting textSetting)
        {
            Delimiters delimiters = Delimiters.COMMA_DELIMITED;
            TextDelimitedParseFileParameters parameters = new TextDelimitedParseFileParameters(optFile, encoding, delimiters);
            List<string[]> records = parser.Parse(parameters);
            DocumentSetBuilderArgs args = DocumentSetBuilderArgs.GetImageSetArgs(records, optFile.Directory.FullName, textSetting);
            List<Document> documents = builder.BuildDocuments(args);
            DocumentSet docSet = new DocumentSet();
            docSet.AddDocuments(documents);
            return docSet;
        }
    }
}
