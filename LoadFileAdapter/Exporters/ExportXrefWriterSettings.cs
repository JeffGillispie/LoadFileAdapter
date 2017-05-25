using System.IO;

namespace LoadFileAdapter.Exporters
{
    public class ExportXrefWriterSettings : IExportXrefSettings, IExportWriterSettings
    {
        //todo: comments
        private DocumentCollection documents;
        private TextWriter writer;
        private XrefTrigger boxBreakTrigger;
        private XrefTrigger groupStartTrigger;
        private XrefTrigger codeStartTrigger;
        private string customerData;
        private string namedFolder;
        private string namedFile;
        private XrefSlipSheetSettings slipsheets;

        public ExportXrefWriterSettings(DocumentCollection documents, TextWriter writer,
            XrefTrigger boxBreakTrigger, XrefTrigger groupStartTrigger, XrefTrigger codeStartTrigger,
            string customerData, string namedFolder, string namedFile,
            XrefSlipSheetSettings slipsheets)
        {
            this.documents = documents;
            this.writer = writer;
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
        /// Gets the writer used to write the export.
        /// </summary>
        /// <returns>Returns a <see cref="TextWriter"/>.</returns>
        public TextWriter GetWriter()
        {
            return this.writer;
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
