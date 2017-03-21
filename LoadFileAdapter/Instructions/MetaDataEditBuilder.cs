using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LoadFileAdapter.Transformers;

namespace LoadFileAdapter.Instructions
{
    public class MetaDataEditBuilder : EditBuilder
    {
        public string FieldName = String.Empty;
        public string AlternateDestinationField = String.Empty;
        public string PrependField = String.Empty;
        public string AppendField = String.Empty;
        public string JoinDelimiter = String.Empty;
        [XmlIgnore]
        public DirectoryInfo PrependDirectory = null;

        public MetaDataEditBuilder() : base()
        {

        }

        public MetaDataEditBuilder(MetaDataEdit edit)
        {            
            base.FilterField = edit.FilterField;
            base.FilterText = edit.FilterText;
            base.FindText = edit.FindText;
            base.ReplaceText = edit.ReplaceText;
            this.FieldName = edit.FieldName;
            this.AlternateDestinationField = edit.AlternateDestinationField;
            this.PrependField = edit.PrependField;
            this.AppendField = edit.AppendField;
            this.JoinDelimiter = edit.JoinDelimiter;
            this.PrependDirectory = edit.PrependDirectory;
        }

        public string PrependDirectoryPath
        {
            get
            {
                return this.PrependDirectory.FullName;
            }

            set
            {
                this.PrependDirectory = new DirectoryInfo(value);
            }
        }

        public new MetaDataEdit GetEdit()
        {
            return new MetaDataEdit(
                this.FieldName, base.FindText, base.ReplaceText,
                this.AlternateDestinationField, this.PrependField, this.AppendField, this.JoinDelimiter,
                base.FilterField, base.FilterText, this.PrependDirectory);
        }
    }
}
