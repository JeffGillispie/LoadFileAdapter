using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// An exporter that export data from a document collection to a LFP file.
    /// </summary>
    public class LfpExporter : IExporter
    {
        private FileInfo file;
        private Encoding encoding;
        private TextWriter writer;
        private string volumeName;

        /// <summary>
        /// Map of image file extensions to the LFP numeric type indicator.
        /// </summary>
        protected static Dictionary<string, int> ImageFileTypes = new Dictionary<string, int>() {
            { ".TIF", 2 },
            { ".JPG", 4 },
            { ".PDF", 7 }
        };

        private LfpExporter()
        {
            // do nothing here
        }

        ~LfpExporter()
        {
            if (writer != null)
            {
                writer.Close();
            }
        }
               
        
        /// <summary>
        /// Uses a <see cref="TextWriter"/> to export data to a LFP file.
        /// </summary>
        /// <param name="args">The export settings used to write a LFP file.</param>
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
        /// Gets a list of lines that contain page records.
        /// </summary>
        /// <param name="document">The document being exported.</param>
        /// <param name="volName">The export volume name.</param>
        /// <returns>A series of formatted LFP image lines.</returns>
        protected List<string> getPageRecords(Document document, string volName)
        {
            List<string> pageRecords = new List<string>();
            Representative imageRep = null;
            Representative nativeRep = null;
            // find representatives
            foreach (Representative rep in document.Representatives)
            {
                if (rep.Type.Equals(Representative.FileType.Image))
                {
                    imageRep = rep;
                }
                else if (rep.Type.Equals(Representative.FileType.Native))
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
                            batesNumber.ToString(),
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

        /// <summary>
        /// Gets a line that contains the native representative record.
        /// </summary>
        /// <param name="document">The document being exported.</param>
        /// <param name="volName">The export volume name.</param>
        /// <param name="nativeRep">The native rep being exported.</param>
        /// <returns>A formatted LFP native record line.</returns>
        protected string getNativeRecord(Document document, string volName, Representative nativeRep)
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

        /// <summary>
        /// A multi-page file like a PDF will have only a single file, but will
        /// need to write multiple lines (pages) to a load file. This will get the count
        /// of iterations needed per file.
        /// </summary>
        /// <param name="doc">The document being exported.</param>
        /// <param name="rep">The image rep being exported.</param>
        /// <returns>Returns a count of iterations (lines) per file.</returns>
        protected int getOffsetIterations(Document doc, Representative rep)
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

        /// <summary>
        /// Gets a document break token based on the documents family relationships.
        /// If the document has a parent then it is a child, otherwise it is a document.
        /// </summary>
        /// <param name="doc">The document to export.</param>
        /// <returns>Returns a 'C' for a child break or a 'D' for a document break.</returns>
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

        public class Builder
        {
            private LfpExporter instance;

            private Builder()
            {
                this.instance = new LfpExporter();
            }

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

            public static Builder Start(TextWriter writer)
            {
                Builder builder = new Builder();
                builder.instance.writer = writer;
                return builder;
            }

            public Builder SetVolumeName(string value)
            {
                instance.volumeName = value;
                return this;
            }

            public LfpExporter Build()
            {
                LfpExporter instance = this.instance;
                this.instance = null;
                return instance;
            }
        }
    }
}
