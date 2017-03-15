using System.Collections.Generic;
using System.IO;
using System.Text;
using LoadFileAdapter.Builders;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Importers
{
    public class TextDelimitedImporter
    {
        Parser<TextDelimitedParseFileParameters, TextDelimitedParseReaderParameters, TextDelimitedParseLineParameters> parser = new TextDelimitedParser();
        Builder builder = new TextDelimitedBuilder();

        public DocumentSet Import(FileInfo file, Encoding encoding, Delimiters delims, bool hasHeader, string keyColName, 
            string parentColName, string childColName, string childSeparator, List<SemiStructuredRepresentativeSetting> repColInfo)
        {
            TextDelimitedParseFileParameters parameters = new TextDelimitedParseFileParameters(file, encoding, delims);
            List<string[]> records = parser.Parse(parameters);
            DocumentSetBuilderArgs args = DocumentSetBuilderArgs.GetTextDelimitedArgs(records, hasHeader, keyColName, parentColName, childColName, childSeparator, repColInfo, file.Directory.FullName);
            List<Document> documents = builder.BuildDocuments(args);
            DocumentSet docSet = new DocumentSet();
            docSet.AddDocuments(documents);
            return docSet;
        }
    }
}
