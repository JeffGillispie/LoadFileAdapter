﻿using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace LoadFileAdapter.Instructions
{   
    public abstract class ImportInstructions
    {
    
        [XmlIgnore]
        public FileInfo File;
        [XmlIgnore] 
        public Encoding Encoding;

        public ImportInstructions(FileInfo file, Encoding encoding)
        {
            this.File = file;
            this.Encoding = encoding;
        }
        
        public string FilePath
        {
            get
            {
                return this.File.FullName;
            }

            set
            {
                this.File = new FileInfo(value);
            }
        }
        
        public int CodePage
        {
            get
            {
                return this.Encoding.CodePage;
            }

            set
            {
                this.Encoding = Encoding.GetEncoding(value);
            }
        }     
    }
}
