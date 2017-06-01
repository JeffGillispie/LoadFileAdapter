using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// An exporter that export data from a document collection to an OPT file.
    /// </summary>
    public class OptExporter : IExporter<IExportImageSettings>
    {
        private const string TRUE_VALUE = "Y";
        private const string FALSE_VALUE = "";

        /// <summary>
        /// Exports an OPT load file.
        /// </summary>
        /// <param name="args">The settings used to export a document collection.</param>
        public void Export(IExportImageSettings args)
        {
            if (args.GetType().Equals(typeof(ExportImageFileSettings)))
            {
                Export((ExportImageFileSettings)args);
            }
            else if (args.GetType().Equals(typeof(ExportImageWriterSettings)))
            {
                Export((ExportImageWriterSettings)args);
            }
            else
            {
                throw new Exception("OptExporter Export Exception: The export settings were not a valid type.");
            }
        }

        /// <summary>
        /// Exports an OPT file to a supplied <see cref="FileInfo"/> destination.
        /// </summary>
        /// <param name="args">The export settings used to export data to a file.</param>
        public void Export(ExportImageFileSettings args)
        {
            args.CreateDestination();
            DocumentCollection docs = args.GetDocuments();
            string file = args.GetFile().FullName;
            Encoding encoding = args.GetEncoding();
            string vol = args.GetVolumeName();
            bool append = false;

            using (TextWriter writer = new StreamWriter(file, append, encoding))
            {
                ExportImageWriterSettings writerArgs = new ExportImageWriterSettings(writer, docs, vol);
                Export(writerArgs);
            }
        }

        /// <summary>
        /// Use a <see cref="TextWriter"/> to export data to an OPT file.
        /// </summary>
        /// <param name="args">The export settings used to write an OPT file.</param>
        public void Export(ExportImageWriterSettings args)
        {
            DocumentCollection docs = args.GetDocuments();
            TextWriter writer = args.GetWriter();
            string vol = args.GetVolumeName();

            foreach (Document document in docs)
            {
                List<string> pages = getPageRecords(document, vol);
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
    }
}
