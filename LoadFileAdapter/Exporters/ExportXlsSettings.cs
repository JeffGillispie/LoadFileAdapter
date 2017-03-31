using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Exporters
{
    public class ExportXlsSettings : IExportSettings 
    {
        private DocumentCollection documents;
        private FileInfo file;

        public ExportXlsSettings(DocumentCollection documents, FileInfo file)
        {
            this.documents = documents;
            this.file = file;
        }

        public DocumentCollection GetDocuments()
        {
            return this.documents;
        }

        public FileInfo GetFile()
        {
            return this.file;
        }
    }
}
