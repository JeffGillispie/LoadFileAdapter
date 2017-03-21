using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LoadFileAdapter.Instructions
{
    class ImgExport : ExportInstructions
    {
        public string VolumeName = null;

        public ImgExport(FileInfo file, Encoding encoding, string volName) : base(file, encoding)
        {
            this.VolumeName = volName;
        }                
    }
}
