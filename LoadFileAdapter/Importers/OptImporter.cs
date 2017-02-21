using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadFileAdapter.Parsers;
using LoadFileAdapter.Builders;

namespace LoadFileAdapter.Importers
{
    public class OptImporter
    {
        private Parser parser = new Parser();
        private Builder builder = new OptBuilder();

        public DocumentSet ImportDocuments(FileInfo optFile, Encoding encoding, StructuredRepresentativeSetting textSetting)
        {
            Delimiters delimiters = Delimiters.COMMA_DELIMITED;            
            List<string[]> records = parser.Parse(optFile, delimiters, encoding);
            DocumentSetBuilderArgs args = DocumentSetBuilderArgs.GetImageSetArgs(records, optFile.Directory.FullName, textSetting);
            List<Document> documents = builder.BuildDocuments(args);
            DocumentSet docSet = new DocumentSet();
            docSet.SetDocuments(documents);
            return docSet;
        }
    }
}
