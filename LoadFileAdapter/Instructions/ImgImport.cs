using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LoadFileAdapter;
using LoadFileAdapter.Builders;

namespace LoadFileAdapter.Instructions
{
    public class ImgImport : Import
    {
        public TextFileSettingsBuilder TextSetting;

        public ImgImport() : base(null, null)
        {

        }

        public ImgImport(FileInfo file, Encoding encoding, TextFileSettings txtSetting) : base(file, encoding)
        {
            this.TextSetting = new TextFileSettingsBuilder(txtSetting);
        }                
    }
}
