using System.Collections.Generic;

namespace LoadFileAdapter
{
    /// <summary>
    /// A representative is a model of a document which can consist of a set of 
    /// image files, text files, or a native file.
    /// </summary>
    public class Representative
    {        
        /// <summary>
        /// The file type of this representative.
        /// </summary>
        private FileType type;
        /// <summary>
        /// A map of the keys and file path values that comprise this representative.
        /// </summary>
        private SortedDictionary<string, string> files;

        /// <summary>
        /// Instantiates a new instance of <see cref="Representative"/>
        /// </summary>
        /// <param name="type">The <see cref="Representative.FileType"/> of the representative.</param>
        /// <param name="files">A map of the keys and file path values that comprise the representative.</param>
        public Representative(FileType type, SortedDictionary<string, string> files)
        {
            this.type = type;
            this.files = files;
        }

        /// <summary>
        /// A representative file type.
        /// </summary>
        public enum FileType
        {
            /// <summary>
            /// An image representative.
            /// </summary>
            Image,
            /// <summary>
            /// A native representative.
            /// </summary>
            Native,
            /// <summary>
            /// A text representative.
            /// </summary>
            Text
        }

        /// <summary>
        /// The file type of this representative.
        /// </summary>
        public FileType Type
        {
            get
            {
                return this.type;
            }            
        }

        /// <summary>
        /// A map of keys and file path values that comprise a type of document model. 
        /// </summary>
        public SortedDictionary<string, string> Files
        {
            get
            {
                return this.files;
            }            
        }
    }
}
