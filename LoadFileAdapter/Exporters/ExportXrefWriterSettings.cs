using System.IO;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// The export writer settings to use for an XREF export.
    /// </summary>
    public class ExportXrefWriterSettings : IExportXrefSettings, IExportWriterSettings
    {
        private DocumentCollection documents;
        private TextWriter writer;
        private XrefTrigger boxBreakTrigger;
        private XrefTrigger groupStartTrigger;
        private XrefTrigger codeStartTrigger;
        private string customerData;
        private string namedFolder;
        private string namedFile;
        private XrefSlipSheetSettings slipsheets;

        /// <summary>
        /// Initializes a new instance of <see cref="ExportXrefWriterSettings"/>.
        /// </summary>
        /// <param name="documents">The documents to export.</param>
        /// <param name="writer">The writer used to write the XREF.</param>
        /// <param name="boxBreakTrigger">The box break trigger.</param>
        /// <param name="groupStartTrigger">The group start trigger.</param>
        /// <param name="codeStartTrigger">The code start trigger.</param>
        /// <param name="customerData">The customer data field name.</param>
        /// <param name="namedFolder">The named folder field name.</param>
        /// <param name="namedFile">The named file field name.</param>
        /// <param name="slipsheets">The slipsheet settings to use in the export.</param>
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

        /// <summary>
        /// The trigger used to determine when a box should change.
        /// </summary>
        /// <returns>Returns a <see cref="XrefTrigger"/>.</returns>
        public XrefTrigger GetBoxBreakTrigger()
        {
            return boxBreakTrigger;
        }

        /// <summary>
        /// The trigger used to determine when the group start value should change.
        /// </summary>
        /// <returns>Returns a <see cref="XrefTrigger"/>.</returns>
        public XrefTrigger GetGroupStartTrigger()
        {
            return groupStartTrigger;
        }

        /// <summary>
        /// The trigger used to determine when the code start value should change.
        /// </summary>
        /// <returns>Returns a <see cref="XrefTrigger"/>.</returns>
        public XrefTrigger GetCodeStartTrigger()
        {
            return codeStartTrigger;
        }

        /// <summary>
        /// Gets the name of the metadata field to use for customer data.
        /// </summary>
        /// <returns>Returns the customer data metadata field name.</returns>
        public string GetCustomerData()
        {
            return customerData;
        }

        /// <summary>
        /// Gets the name of the metadata field to use for named folders.
        /// </summary>
        /// <returns>Returns the named folder metadata field name.</returns>
        public string GetNamedFolder()
        {
            return namedFolder;
        }

        /// <summary>
        /// Gets the name of the metadata field to use for named files.
        /// </summary>
        /// <returns>Returns the named file metadata field name.</returns>
        public string GetNamedFile()
        {
            return namedFile;
        }

        /// <summary>
        /// Gets the slipsheet settings used to export an XREF.
        /// </summary>
        /// <returns>Returns slipsheet settings.</returns>
        public XrefSlipSheetSettings GetSlipsheets()
        {
            return slipsheets;
        }
    }
}
