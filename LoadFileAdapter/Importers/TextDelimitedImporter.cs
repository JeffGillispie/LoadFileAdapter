using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadFileAdapter.Builders;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Importers
{
    public class TextDelimitedImporter
    {
        Parser parser = new Parser();
        Builder builder = new TextDelimitedBuilder();

        public DocumentSet ImportDocuments(FileInfo file, Encoding encoding, Delimiters delims, bool hasHeader, string keyColName, 
            string parentColName, string childColName, string childSeparator, List<SemiStructuredRepresentativeSetting> repColInfo)
        {
            List<string[]> records = parser.Parse(file, delims, encoding);
            DocumentSetBuilderArgs args = DocumentSetBuilderArgs.GetTextDelimitedArgs(records, hasHeader, keyColName, parentColName, childColName, childSeparator, repColInfo, file.Directory.FullName);
            List<Document> documents = builder.BuildDocuments(args);
            DocumentSet docSet = new DocumentSet();
            docSet.SetDocuments(documents);
            return docSet;
        }
    }
}
