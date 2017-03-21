using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoadFileAdapter;
using LoadFileAdapter.Builders;

namespace LoadFileAdapter.Instructions
{
    public class LinkedFileSettingsBuilder
    {
        public string ColumnName = null;
        public LinkedFile.FileType FileType = LinkedFile.FileType.Native;

        public LinkedFileSettingsBuilder()
        {

        }

        public LinkedFileSettingsBuilder(LinkedFileSettings setting)
        {
            this.ColumnName = setting.ColumnName;
            this.FileType = setting.Type;
        }

        public LinkedFileSettings GetSetting()
        {
            return new LinkedFileSettings(this.ColumnName, this.FileType);
        }
    }
}
