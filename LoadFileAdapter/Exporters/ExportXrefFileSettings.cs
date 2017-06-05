using System.IO;
using System.Text;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// The export file settings to use for XREF export.
    /// </summary>
    public class ExportXrefFileSettings : IExportXrefSettings, IExportFileSettings
    {
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
        
        private ExportXrefFileSettings()
        {
            // do nothing here
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

        public class Builder
        {
            private ExportXrefFileSettings instance;
            
            private Builder(DocumentCollection documents, FileInfo file, Encoding encoding)
            {
                instance = new ExportXrefFileSettings();
                instance.documents = documents;
                instance.file = file;
                instance.encoding = encoding;
            }

            public static Builder Start(DocumentCollection documents, FileInfo file, Encoding encoding)
            {
                return new Builder(documents, file, encoding);
            }

            public Builder SetBoxTrigger(XrefTrigger trigger)
            {
                instance.boxBreakTrigger = trigger;
                return this;
            }

            public Builder SetGroupStartTrigger(XrefTrigger trigger)
            {
                instance.groupStartTrigger = trigger;
                return this;
            }

            public Builder SetCodeStartTrigger(XrefTrigger trigger)
            {
                instance.codeStartTrigger = trigger;
                return this;
            }

            public Builder SetCustomerData(string value)
            {
                instance.customerData = value;
                return this;
            }

            public Builder SetNamedFolder(string value)
            {
                instance.namedFolder = value;
                return this;
            }

            public Builder SetNamedFile(string value)
            {
                instance.namedFile = value;
                return this;
            }

            public Builder SetSlipsheets(XrefSlipSheetSettings settings)
            {
                instance.slipsheets = settings;
                return this;
            }

            public ExportXrefFileSettings Build()
            {
                ExportXrefFileSettings instance = this.instance;
                this.instance = null;
                return instance;
            }
        }
    }
}
