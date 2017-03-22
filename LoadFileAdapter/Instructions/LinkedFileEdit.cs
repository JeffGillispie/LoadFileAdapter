using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoadFileAdapter.Transformers;

namespace LoadFileAdapter.Instructions
{
    public class LinkedFileEdit : Edit
    {
        public LinkedFile.FileType TargetType;
        public LinkedFile.FileType? NewType;

        public LinkedFileEdit() : base()
        {

        }

        public LinkedFileEdit(LinkedFileTransformation edit)
        {
            this.TargetType = edit.TargetType;
            this.NewType = edit.NewType;
            base.FilterField = edit.FilterField;
            base.FilterText = edit.FilterText;
            base.FindText = edit.FindText;
            base.ReplaceText = edit.ReplaceText;
        }

        public new LinkedFileTransformation GetEdit()
        {
            return new LinkedFileTransformation(
                this.TargetType, 
                this.NewType, 
                this.FindText, 
                this.ReplaceText, 
                this.FilterField, 
                this.FilterText);
        }
    }
}
