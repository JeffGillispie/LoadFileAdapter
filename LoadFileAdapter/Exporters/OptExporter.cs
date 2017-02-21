using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Exporters
{
    public class OptExporter
    {
        private const string TRUE_VALUE = "Y";
        private const string FALSE_VALUE = "";

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
            Representative imageRep = null;
            List<string> pageRecords = new List<string>();
            // find the image representative
            foreach (Representative rep in document.Representatives)
            {
                if (rep.RepresentativeType.Equals(Representative.Type.Image))
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
                        kvp.Value.FullName, 
                        (counter == 0) ? TRUE_VALUE : FALSE_VALUE,
                        FALSE_VALUE,
                        FALSE_VALUE,
                        (counter == 0) ? imageRep.Files.Count.ToString() : FALSE_VALUE
                        );
                    pageRecords.Add(pageRecord);
                }
            }

            return pageRecords;
        }
    }
}
