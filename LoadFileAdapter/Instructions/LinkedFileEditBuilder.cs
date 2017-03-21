using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoadFileAdapter.Transformers;

namespace LoadFileAdapter.Instructions
{
    public class LinkedFileEditBuilder : EditBuilder
    {
        public LinkedFile.FileType TargetType;
        public LinkedFile.FileType? NewType;

        public LinkedFileEditBuilder() : base()
        {

        }

        public LinkedFileEditBuilder(LinkedFileEdit edit)
        {
            this.TargetType = edit.TargetType;
            this.NewType = edit.NewType;
            base.FilterField = edit.FilterField;
            base.FilterText = edit.FilterText;
            base.FindText = edit.FindText;
            base.ReplaceText = edit.ReplaceText;
        }

        public new LinkedFileEdit GetEdit()
        {
            return new LinkedFileEdit(
                this.TargetType, 
                this.NewType, 
                this.FindText, 
                this.ReplaceText, 
                this.FilterField, 
                this.FilterText);
        }
    }
}
