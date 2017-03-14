using System.Collections.Generic;

namespace LoadFileAdapter
{
    public class Representative
    {
        public enum Type
        {
            Image, Native, Text
        }

        private Type type;
        private SortedDictionary<string, string> files;

        public Representative(Type type, SortedDictionary<string, string> files)
        {
            this.type = type;
            this.files = files;
        }

        public Type RepresentativeType
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
