using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Exporters
{
    public class LfpExporter
    {
        public void Export(DocumentSet documents, FileInfo file, Encoding encoding, string volumeName)
        {
            bool append = false;

            using (StreamWriter writer = new StreamWriter(file.FullName, append, encoding))
            {
                foreach (Document document in documents)
                {
                    List<string> pages = getPageRecords(document, volumeName);
                    // write pages
                    foreach (string page in pages)
                    {
                        writer.WriteLine(page);
                    }
                }
            }
        }

        private List<string> getPageRecords(Document document, string volName)
        {
            List<string> pageRecords = new List<string>();
            Representative imageRep = null;
            Representative nativeRep = null;
            // find representatives
            foreach (Representative rep in document.Representatives)
            {
                if (rep.RepresentativeType.Equals(Representative.Type.Image))
                {
                    imageRep = rep;
                }
                else if (rep.RepresentativeType.Equals(Representative.Type.Native))
                {
                    nativeRep = rep;
                }
            }
            // check if an image rep was found
            if (imageRep != null)
            {
                int counter = 0;
                int iterations = 
            }
            return pageRecords;
        }

        private int getOffsetIterations(Document doc, Representative rep)
        {
            int iterations = 1;
            // check if the page count is greater than represntative file count
            // if it is then we have a multipage image which requires a sequential
            // offset otherwise the offset is zero
            if (doc.Metadata.ContainsKey())
        }
    }
}
