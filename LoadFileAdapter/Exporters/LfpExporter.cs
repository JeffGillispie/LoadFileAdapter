﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Exporters
{
    public class LfpExporter : Exporter
    {
        private static Dictionary<string, int> ImageFileTypes = new Dictionary<string, int>() {
            { ".TIF", 2 },
            { ".JPG", 4 },
            { ".PDF", 7 }
        };

        public void Export(DocumentSet documents, FileInfo file, Encoding encoding, string volumeName, Parsers.Delimiters delimiters)
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
                int iterations = getOffsetIterations(document, imageRep);

                
                foreach (var kvp in imageRep.Files)
                {
                    BatesNumber batesNumber = new BatesNumber(kvp.Key);
                    string ext = Path.GetExtension(kvp.Value.FullName).ToUpper();

                    for (int i = 0; i < iterations; i++)
                    {
                        // IM,IMAGEKEY,DOCBREAK,OFFSET,@VOLUME;FILE\PATH;IMAGE.FILE;TYPE,ROTATION
                        int offset = (iterations > 1) ? i + 1 : i;
                        string docBreak = (counter == 0) ? getDocBreakValue(document) : String.Empty;
                        
                        string pageRecord = String.Format("IM,{0},{1},{2},@{3};{4};{5};{6},0", 
                            batesNumber.Value,
                            docBreak,
                            offset,
                            volName,
                            kvp.Value.Directory.FullName,
                            kvp.Value.Name,
                            ImageFileTypes[ext]
                            );

                        pageRecords.Add(pageRecord);

                        if (nativeRep != null && counter == 0)
                        {
                            pageRecord = getNativeRecord(document, volName, nativeRep);
                            pageRecords.Add(pageRecord);
                        }

                        batesNumber.IterateNumber();
                        counter++;
                    }
                }
            }
            else if (nativeRep != null)
            {
                string pageRecord = getNativeRecord(document, volName, nativeRep);
                pageRecords.Add(pageRecord);
            }

            return pageRecords;
        }

        private string getNativeRecord(Document document, string volName, Representative nativeRep)
        {
            // OF,IMAGEKEY,@VOLUME;FILE\PATH;IMAGE.FILE,1
            return String.Format("OF,{0},@{1},{2},{3},1",
                document.Key,
                volName,
                nativeRep.Files.First().Value.Directory.FullName,
                nativeRep.Files.First().Value.Name
                );
        }

        private int getOffsetIterations(Document doc, Representative rep)
        {
            int iterations = 1;
            // check if the page count is greater than represntative file count
            // if it is then we have a multipage image which requires a sequential
            // offset otherwise the offset is zero
            if (doc.Metadata.ContainsKey(Builders.LfpBuilder.PAGE_COUNT_FIELD))
            {
                int.TryParse(doc.Metadata[Builders.LfpBuilder.PAGE_COUNT_FIELD], out iterations);                
            }

            return iterations;
        }

        private string getDocBreakValue(Document doc)
        {
            if (doc.Metadata.ContainsKey(Builders.LfpBuilder.RELATION_TYPE_FIELD) && 
                doc.Metadata[Builders.LfpBuilder.RELATION_TYPE_FIELD].Equals(Builders.LfpBuilder.CHILD_RELATIONSHIP_TYPE))
            {
                return Builders.LfpBuilder.BoundaryFlag.C.ToString();
            }
            else
            {
                return Builders.LfpBuilder.BoundaryFlag.D.ToString();
            }
        }
    }
}
