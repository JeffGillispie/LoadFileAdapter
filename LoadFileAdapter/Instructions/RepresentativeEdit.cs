using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoadFileAdapter.Transformers;

namespace LoadFileAdapter.Instructions
{
    public class RepresentativeEdit : Edit
    {
        public Representative.FileType TargetType;
        public Representative.FileType? NewType;

        public RepresentativeEdit() : base()
        {

        }

        public RepresentativeEdit(RepresentativeTransformation edit)
        {
            this.TargetType = edit.TargetType;
            this.NewType = edit.NewType;
            base.FilterField = edit.FilterField;
            base.FilterText = edit.FilterText;
            base.FindText = edit.FindText;
            base.ReplaceText = edit.ReplaceText;
        }

        public new RepresentativeTransformation GetEdit()
        {
            return new RepresentativeTransformation(
                this.TargetType, 
                this.NewType, 
                this.FindText, 
                this.ReplaceText, 
                this.FilterField, 
                this.FilterText);
        }
    }
}
