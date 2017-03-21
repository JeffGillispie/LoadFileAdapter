using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LoadFileAdapter.Instructions
{
    public abstract class ExportInstructions
    {
        private FileInfo file;
        private Encoding encoding;

        public ExportInstructions(FileInfo file, Encoding encoding)
        {
            this.file = file;
            this.encoding = encoding;
        }

        public FileInfo File { get { return this.file; } }
        public Encoding Encoding { get { return this.encoding; } }
    }
}
