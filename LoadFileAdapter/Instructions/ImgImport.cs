using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LoadFileAdapter;
using LoadFileAdapter.Builders;

namespace LoadFileAdapter.Instructions
{
    public class ImgImport : ImportInstructions
    {
        public StructuredRepresentativeSetting TextImportSetting;

        public ImgImport(FileInfo file, Encoding encoding, StructuredRepresentativeSetting txtRep) : base(file, encoding)
        {
            this.TextImportSetting = txtRep;
        }                
    }
}
