using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter
{
    public class Representative
    {
        public enum Type
        {
            Image, Native, Text
        }

        private Type type;
        private SortedDictionary<string, FileInfo> files;

        public Representative(Type type, SortedDictionary<string, FileInfo> files)
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

        public SortedDictionary<string, FileInfo> Files
        {
            get
            {
                return this.files;
            }            
        }
    }
}
