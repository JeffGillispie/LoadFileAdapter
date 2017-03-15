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
    public class LfpImporter
    {
        private LfpParser parser = new LfpParser();
        private Builder builder = new LfpBuilder();

        public DocumentSet ImportDocuments(FileInfo lfpFile, Encoding encoding, StructuredRepresentativeSetting textSetting)
        {
            Delimiters delimiters = null;
            List<string[]> records = parser.Parse(lfpFile, delimiters, encoding);
            DocumentSetBuilderArgs args = DocumentSetBuilderArgs.GetImageSetArgs(records, lfpFile.Directory.FullName, textSetting);
            List<Document> documents = builder.BuildDocuments(args);
            DocumentSet docSet = new DocumentSet();
            docSet.AddDocuments(documents);
            return docSet;
        }
    }
}
