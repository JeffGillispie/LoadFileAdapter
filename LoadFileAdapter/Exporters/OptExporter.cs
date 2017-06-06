using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// An exporter that export data from a document collection to an OPT file.
    /// </summary>
    public class OptExporter : IExporter
    {
        private const string TRUE_VALUE = "Y";
        private const string FALSE_VALUE = "";
        private FileInfo file;
        private Encoding encoding;
        private TextWriter writer;
        private string volumeName;

        private OptExporter()
        {
            // do nothing here
        }

        ~OptExporter()
        {
            if (writer != null)
            {
                writer.Close();
            }
        }
                                
        /// <summary>
        /// Exports documents to an OPT file.
        /// </summary>
        /// <param name="docs">The docs to export.</param>
        public void Export(DocumentCollection docs)
        {
            foreach (Document document in docs)
            {
                List<string> pages = getPageRecords(document, volumeName);
                // write pages
                foreach (string page in pages)
                {
                    writer.WriteLine(page);
                }
            }
        }

        /// <summary>
        /// Gets a list of lines that contain OPT page records.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="volName"></param>
        /// <returns></returns>
        protected List<string> getPageRecords(Document document, string volName)
        {
            Representative imageRep = null;
            List<string> pageRecords = new List<string>();
            // find the image representative
            foreach (Representative rep in document.Representatives)
            {
                if (rep.Type.Equals(Representative.FileType.Image))
                {
                    imageRep = rep;
                    break;
                }
            }
            // check that a rep was found
            if (imageRep == null)
            {
                // do nothing here
            }
            else
            {
                int counter = 0;

                foreach (var kvp in imageRep.Files)
                {
                    // ImageKey,VolumeName,FullPath,DocBreak,BoxBreak,FolderBreak,PageCount                    
                    string pageRecord = String.Format("{0},{1},{2},{3},{4},{5},{6}", 
                        kvp.Key, 
                        volName, 
                        kvp.Value, 
                        (counter == 0) ? TRUE_VALUE : FALSE_VALUE,
                        FALSE_VALUE,
                        FALSE_VALUE,
                        (counter == 0) ? imageRep.Files.Count.ToString() : FALSE_VALUE
                        );
                    pageRecords.Add(pageRecord);
                    counter++;
                }
            }

            return pageRecords;
        }

        /// <summary>
        /// Builds a <see cref="OptExporter"/> instance.
        /// </summary>
        public class Builder
        {
            private OptExporter instance;

            private Builder()
            {
                this.instance = new OptExporter();
            }

            /// <summary>
            /// Starts the process of building a <see cref="OptExporter"/> instance.
            /// </summary>
            /// <param name="file">The destination file.</param>
            /// <param name="encoding">The encoding of the export.</param>
            /// <returns>Returns a <see cref="Builder"/></returns>
            public static Builder Start(FileInfo file, Encoding encoding)
            {
                Builder builder = new Builder();
                bool append = false;
                builder.instance.file = file;
                builder.instance.encoding = encoding;
                builder.instance.volumeName = Path.GetFileNameWithoutExtension(file.Name);                
                builder.instance.writer = new StreamWriter(file.FullName, append, encoding);
                builder.instance.CreateDestination(file);
                return builder;
            }

            /// <summary>
            /// Starts the process of building a <see cref="OptExporter"/> instance.
            /// </summary>
            /// <param name="writer">The <see cref="TextWriter"/> used to export data.</param>
            /// <returns>Returns a <see cref="Builder"/></returns>
            public static Builder Start(TextWriter writer)
            {
                Builder builder = new Builder();
                builder.instance.writer = writer;
                return builder;
            }

            /// <summary>
            /// Sets the volume name.
            /// </summary>
            /// <param name="value">The value to set.</param>
            /// <returns>Returns a <see cref="Builder"/></returns>
            public Builder SetVolumeName(string value)
            {
                instance.volumeName = value;
                return this;
            }

            /// <summary>
            /// Builds a <see cref="OptExporter"/> instance.
            /// </summary>
            /// <returns>Returns a <see cref="OptExporter"/>.</returns>
            public OptExporter Build()
            {
                OptExporter instance = this.instance;
                this.instance = null;
                return instance;
            }
        }
    }
}
