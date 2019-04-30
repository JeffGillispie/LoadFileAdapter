using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LoadFileAdapter.Builders;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Importers
{
    /// <summary>
    /// An importer used to import a LFP file.
    /// </summary>
    public class LfpImporter : IImporter
    {
        private LfpParser parser = new LfpParser();
        private LfpBuilder builder = new LfpBuilder();
             
        /// <summary>
        /// Gets or sets the <see cref="LfpParser"/>.
        /// </summary>
        public LfpParser Parser
        {
            get
            {
                return parser;
            }

            set
            {
                parser = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="LfpBuilder"/>.
        /// </summary>
        public LfpBuilder Builder
        {
            get
            {
                return builder;
            }

            set
            {
                builder = value;
            }
        }
         
        /// <summary>
        /// Parses data from a file and builds it into a collection of <see cref="Document"/> objects.
        /// </summary>
        /// <param name="file">The file to import.</param>
        /// <param name="encoding">The encoding of the file.</param>
        /// <returns>Returns a <see cref="DocumentCollection"/>.</returns>
        public DocumentCollection Import(FileInfo file, Encoding encoding)
        {
            List<string[]> records = parser.Parse(file, encoding);            
            List<Document> documentList = builder.Build(records);
            DocumentCollection documents = new DocumentCollection(documentList);
            return documents;
        }                    
    }
}
