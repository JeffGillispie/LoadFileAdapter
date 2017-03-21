using System;
using System.Collections.Generic;
using System.IO;

namespace LoadFileAdapter.Exporters
{
    public class OptExporter : IExporter<ExportFileImageSettings, ExportWriterImageSettings>
    {
        private const string TRUE_VALUE = "Y";
        private const string FALSE_VALUE = "";

        public void Export(ExportFileImageSettings args)
        {
            bool append = false;

            using (TextWriter writer = new StreamWriter(args.File.FullName, append, args.Encoding))
            {
                ExportWriterImageSettings writerArgs = new ExportWriterImageSettings(writer, args.Documents, args.VolumeName);
                Export(writerArgs);
            }
        }

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

        protected List<string> getPageRecords(Document document, string volName)
        {
            LinkedFile imageRep = null;
            List<string> pageRecords = new List<string>();
            // find the image representative
            foreach (LinkedFile rep in document.LinkedFiles)
            {
                if (rep.Type.Equals(LinkedFile.FileType.Image))
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
