using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LoadFileAdapter.Exporters
{
    public class LfpExporter : IExporter<ExportFileImageSettings, ExportWriterImageSettings>
    {
        protected static Dictionary<string, int> ImageFileTypes = new Dictionary<string, int>() {
            { ".TIF", 2 },
            { ".JPG", 4 },
            { ".PDF", 7 }
        };

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
            List<string> pageRecords = new List<string>();
            LinkedFile imageRep = null;
            LinkedFile nativeRep = null;
            // find representatives
            foreach (LinkedFile rep in document.LinkedFiles)
            {
                if (rep.Type.Equals(LinkedFile.FileType.Image))
                {
                    imageRep = rep;
                }
                else if (rep.Type.Equals(LinkedFile.FileType.Native))
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
                    string ext = Path.GetExtension(kvp.Value).ToUpper();
                    int i = (iterations == 0) ? 0 : 1;

                    for (; i <= iterations; i++)
                    {
                        // IM,IMAGEKEY,DOCBREAK,OFFSET,@VOLUME;FILE\PATH;IMAGE.FILE;TYPE,ROTATION
                        int offset = i;
                        string docBreak = (counter == 0) ? getDocBreakValue(document) : String.Empty;
                        string fileName = Path.GetFileName(kvp.Value);
                        string directory = kvp.Value.Substring(0, kvp.Value.Length - fileName.Length - 1);
                        string pageRecord = String.Format("IM,{0},{1},{2},@{3};{4};{5};{6},0", 
                            batesNumber.Value,
                            docBreak,
                            offset,
                            volName,
                            directory,
                            fileName,
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

        protected string getNativeRecord(Document document, string volName, LinkedFile nativeRep)
        {
            // OF,IMAGEKEY,@VOLUME;FILE\PATH;IMAGE.FILE,1
            string fileName = Path.GetFileName(nativeRep.Files.First().Value);
            string directory = nativeRep.Files.First().Value;
            directory = directory.Substring(0, directory.Length - fileName.Length - 1);

            return String.Format("OF,{0},@{1};{2};{3},1",
                document.Key,
                volName,
                directory,
                fileName
                );
        }

        protected int getOffsetIterations(Document doc, LinkedFile rep)
        {
            int iterations = 1;
            // check if the page count is greater than represntative file count
            // if it is then we have a multipage image which requires a sequential
            // offset otherwise the offset is zero
            if (doc.Metadata.ContainsKey(Builders.LfpBuilder.PAGE_COUNT_FIELD))
            {
                int.TryParse(doc.Metadata[Builders.LfpBuilder.PAGE_COUNT_FIELD], out iterations);                
            }

            if (iterations == rep.Files.Count)
            {
                iterations = 0;
            }

            return iterations;
        }

        protected string getDocBreakValue(Document doc)
        {
            if (doc.Parent != null)
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
