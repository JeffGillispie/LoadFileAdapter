using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LoadFileAdapter.Transformers;

namespace LoadFileAdapter.Instructions
{
    public class MetaDataEdit : Edit
    {
        public string FieldName = null;
        public string AlternateDestinationField = null;
        public string PrependField = null;
        public string AppendField = null;
        public string JoinDelimiter = null;
        [XmlIgnore]
        public DirectoryInfo PrependDirectory = null;

        public MetaDataEdit() : base()
        {

        }

        public MetaDataEdit(MetaDataTransformation edit)
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
                if (this.PrependDirectory == null)
                    return null;
                else
                    return this.PrependDirectory.FullName;
            }

            set
            {
                if (value == null)
                    this.PrependDirectory = null;
                else
                    this.PrependDirectory = new DirectoryInfo(value);
            }
        }

        public new MetaDataTransformation GetEdit()
        {
            return new MetaDataTransformation(
                this.FieldName, base.FindText, base.ReplaceText,
                this.AlternateDestinationField, this.PrependField, this.AppendField, this.JoinDelimiter,
                base.FilterField, base.FilterText, this.PrependDirectory);
        }
    }
}
