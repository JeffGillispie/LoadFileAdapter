﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using LoadFileAdapter.Parsers;
using LoadFileAdapter.Builders;

namespace LoadFileAdapter.Importers
{
    /// <summary>
    /// An importer used to import data from an OPT file.
    /// </summary>
    public class OptImporter : IImporter
    {
        private DatParser parser = new DatParser(Delimiters.COMMA_DELIMITED);
        private OptBuilder builder = new OptBuilder();
        
        /// <summary>
        /// Gets or sets the <see cref="DatParser"/>.
        /// </summary>
        public DatParser Parser
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
        /// Gets or sets the <see cref="OptBuilder"/>.
        /// </summary>
        public OptBuilder Builder
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
        /// Parses data from a file and builds the data into a collection of <see cref="Document"/> objects.
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
