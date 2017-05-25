using System.IO;
using System.Text;

namespace LoadFileAdapter.Exporters
{
    public class ExportXrefFileSettings : IExportXrefSettings, IExportFileSettings
    {
        //todo: comments
        private DocumentCollection documents;
        private FileInfo file;
        private Encoding encoding;
        private XrefTrigger boxBreakTrigger;
        private XrefTrigger groupStartTrigger;
        private XrefTrigger codeStartTrigger;
        private string customerData;
        private string namedFolder;
        private string namedFile;
        private XrefSlipSheetSettings slipsheets;

        public ExportXrefFileSettings(DocumentCollection documents, FileInfo file, Encoding encoding,
            XrefTrigger boxBreakTrigger, XrefTrigger groupStartTrigger, XrefTrigger codeStartTrigger,
            string customerData, string namedFolder, string namedFile,
            XrefSlipSheetSettings slipsheets)
        {
            this.documents = documents;
            this.file = file;
            this.encoding = encoding;
            this.boxBreakTrigger = boxBreakTrigger;
            this.groupStartTrigger = groupStartTrigger;
            this.codeStartTrigger = codeStartTrigger;
            this.customerData = customerData;
            this.namedFolder = namedFolder;
            this.namedFile = namedFile;
            this.slipsheets = slipsheets;
        }

        /// <summary>
        /// Gets the documents to export from the export settings.
        /// </summary>
        /// <returns>Returns a <see cref="DocumentCollection"/>.</returns>
        public DocumentCollection GetDocuments()
        {
            return this.documents;
        }

        /// <summary>
        /// Gets the destination of the export.
        /// </summary>
        /// <returns>Returns the destination <see cref="FileInfo"/>.</returns>
        public FileInfo GetFile()
        {
            return this.file;
        }

        /// <summary>
        /// Gets the encoding used to write the export.
        /// </summary>
        /// <returns>Returns the <see cref="Encoding"/> of the export.</returns>
        public Encoding GetEncoding()
        {
            return this.encoding;
        }

        public XrefTrigger GetBoxBreakTrigger()
        {
            return boxBreakTrigger;
        }

        public XrefTrigger GetGroupStartTrigger()
        {
            return groupStartTrigger;
        }

        public XrefTrigger GetCodeStartTrigger()
        {
            return codeStartTrigger;
        }

        public string GetCustomerData()
        {
            return customerData;
        }

        public string GetNamedFolder()
        {
            return namedFolder;
        }

        public string GetNamedFile()
        {
            return namedFile;
        }

        public XrefSlipSheetSettings GetSlipsheets()
        {
            return slipsheets;
        }
    }
}
