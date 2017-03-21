using System.Collections.Generic;

namespace LoadFileAdapter
{
    public class LinkedFile
    {
        public enum FileType
        {
            Image, Native, Text
        }

        private FileType type;
        private SortedDictionary<string, string> files;

        public LinkedFile(FileType type, SortedDictionary<string, string> files)
        {
            this.type = type;
            this.files = files;
        }

        public FileType Type
        {
            get
            {
                return this.type;
            }            
        }

        public SortedDictionary<string, string> Files
        {
            get
            {
                return this.files;
            }            
        }
    }
}
