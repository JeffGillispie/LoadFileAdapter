using System;
using System.Collections.Generic;
using System.IO;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// An exporter that export data from a document collection to an OPT file.
    /// </summary>
    public class OptExporter : IExporter<ExportFileImageSettings, ExportWriterImageSettings>
    {
        private const string TRUE_VALUE = "Y";
        private const string FALSE_VALUE = "";

        /// <summary>
        /// Exports an OPT file to a supplied <see cref="FileInfo"/> destination.
        /// </summary>
        /// <param name="args">The export settings used to export data to a file.</param>
        public void Export(ExportFileImageSettings args)
        {
            bool append = false;

            using (TextWriter writer = new StreamWriter(args.File.FullName, append, args.Encoding))
            {
                ExportWriterImageSettings writerArgs = new ExportWriterImageSettings(writer, args.Documents, args.VolumeName);
                Export(writerArgs);
            }
        }

        /// <summary>
        /// Use a <see cref="TextWriter"/> to export data to an OPT file.
        /// </summary>
        /// <param name="args">The export settings used to write an OPT file.</param>
        public void Export(ExportWriterImageSettings args)
        {
            foreach (Document document in args.Documents)
            {
                List<string> pages = getPageRecords(document, args.VolumeName);
                // write pages
                foreach (string page in pages)
                {
                    args.Writer.WriteLine(page);
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
