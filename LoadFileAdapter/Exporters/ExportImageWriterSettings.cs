﻿using System.IO;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// The settings used to write data for an image load file.
    /// </summary>
    public class ExportImageWriterSettings : IExportWriterSettings, IExportImageSettings
    {
        private DocumentCollection documents;
        private TextWriter writer;
        private string volumeName;
        
        /// <summary>
        /// Initializes a new instance of <see cref="ExportImageWriterSettings"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> used to write image load file data.</param>
        /// <param name="documents">The document collection to export.</param>
        /// <param name="volumeName">The volume name to use in the export.</param>
        public ExportImageWriterSettings(
            TextWriter writer, DocumentCollection documents, string volumeName)
        {
            this.documents = documents;
            this.writer = writer;
            this.volumeName = volumeName;
        }

        /// <summary>
        /// Gets the documents to be exported.
        /// </summary>
        /// <returns>Returns a <see cref="DocumentCollection"/>.</returns>
        public DocumentCollection GetDocuments()
        {
            return this.documents;
        }

        /// <summary>
        /// Gets the writer used write the export.
        /// </summary>
        /// <returns>Returns a <see cref="TextWriter"/>.</returns>
        public TextWriter GetWriter()
        {
            return this.writer;
        }

        /// <summary>
        /// Gets the export volume name.
        /// </summary>
        /// <returns>Returns the name of the export volume.</returns>
        public string GetVolumeName()
        {
            return this.volumeName;
        }
    }
}
