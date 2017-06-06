using System.Collections.Generic;
using System.IO;
using System.Text;
using LoadFileAdapter.Builders;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Importers
{
    /// <summary>
    /// An importer used to import a DAT file.
    /// </summary>
    public class DatImporter : IImporter
    {
        private DatParser parser;
        private DatBuilder builder;

        /// <summary>
        /// Initializes a new instance of <see cref="DatImporter"/>.
        /// </summary>
        public DatImporter(Delimiters delimiters)
        {
            this.parser = new DatParser(delimiters);
            this.builder = new DatBuilder();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DatImporter"/>.
        /// </summary>
        /// <param name="parser">The parser used to import the DAT file.</param>
        public DatImporter(DatParser parser)
        {
            this.parser = parser;
            this.builder = new DatBuilder();
        }

        /// <summary>
        /// Gets or sets the <see cref="DatBuilder"/>.
        /// </summary>
        public DatBuilder Builder
        {
            get
            {
                return this.builder;
            }

            set
            {
                this.builder = value;
            }
        }

        /// <summary>
        /// Parses data from a file and builds it into a collection of <see cref="Document"/> objects.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public DocumentCollection Import(FileInfo file, Encoding encoding)
        {        
            List<string[]> records = parser.Parse(file, encoding);                                            
            List<Document> documentList = builder.Build(records);
            DocumentCollection documents = new DocumentCollection(documentList);            
            return documents;
        }
    }
}
