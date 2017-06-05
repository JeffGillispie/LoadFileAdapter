using System;
using System.Collections.Generic;
using System.IO;

namespace LoadFileAdapter.Builders
{
    /// <summary>
    /// Builds a document <see cref="Representative"/> from a DAT file.
    /// </summary>
    public class RepresentativeBuilder
    {
        private const char FILE_PATH_DELIM = '\\';
        private string column;
        private Representative.FileType fileType;

        /// <summary>
        /// The name of the column containing the representative file path.
        /// </summary>
        public string ColumnName { get { return this.column; } }

        /// <summary>
        /// The file type of the document representative.
        /// </summary>
        public Representative.FileType Type { get { return this.fileType; } }

        /// <summary>
        /// Initializes a new instance of <see cref="RepresentativeBuilder"/>.
        /// </summary>
        /// <param name="column">The name of the field containing the representative file path.</param>
        /// <param name="type">The file type of the representative.</param>
        public RepresentativeBuilder(string column, Representative.FileType type)
        {
            this.column = column;
            this.fileType = type;
        }

        /// <summary>
        /// Builds a document <see cref="Representative"/>.
        /// </summary>
        /// <param name="key">The key value for the <see cref="Representative"/> being built.</param>
        /// <param name="metadata">The metadata map of the host <see cref="Document"/>.</param>
        /// <param name="pathPrefix">The path prefix that should be prepended to the path found in the metadata.</param>
        /// <returns>Returns a <see cref="Representative"/>.</returns>
        public Representative Build(string key, Dictionary<string, string> metadata, string pathPrefix)
        {
            Representative rep = null;
            SortedDictionary<string, string> files = new SortedDictionary<string, string>();
            // this format will only have one file per rep
            if (metadata.ContainsKey(column) && !String.IsNullOrWhiteSpace(metadata[column]))
            {
                string filePath = (String.IsNullOrEmpty(pathPrefix))
                    ? metadata[column]
                    : Path.Combine(pathPrefix, metadata[column].TrimStart(FILE_PATH_DELIM));
                files.Add(key, filePath);
                rep = new Representative(fileType, files);                
            }

            return rep;
        }
    }
}
