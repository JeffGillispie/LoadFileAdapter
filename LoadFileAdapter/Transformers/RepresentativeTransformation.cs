using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LoadFileAdapter.Transformers
{
    public class RepresentativeTransformation : Transformation
    {
        private Representative.FileType targetType;
        private Representative.FileType? newType;
        
        public Representative.FileType TargetType { get { return targetType; } }
        public Representative.FileType? NewType { get { return newType; } }
        
        public RepresentativeTransformation(
            Representative.FileType targetType, Representative.FileType? newType, 
            Regex findText, string replaceText,
            string filterField, Regex filterText) :
            base (findText, replaceText, filterField, filterText)
        {
            this.targetType = targetType;
            this.newType = newType;            
        }

        public override void Transform(Document doc)
        {
            if (base.hasEdit(doc))
            {
                HashSet<Representative> representatives = new HashSet<Representative>();

                foreach (Representative rep in doc.Representatives)
                {
                    if (rep.Type == targetType)
                    {
                        Representative.FileType updatedType = (newType != null) ? (Representative.FileType)newType : rep.Type;
                        SortedDictionary<string, string> updatedFiles = new SortedDictionary<string, string>();
                        
                        foreach (var file in rep.Files)
                        {
                            updatedFiles.Add(file.Key, base.Replace(file.Value));
                        }

                        Representative newRep = new Representative(updatedType, updatedFiles);
                        representatives.Add(newRep);
                    }
                    else
                    {
                        representatives.Add(rep);
                    }
                }

                doc.SetLinkedFiles(representatives);
            }
        }
    }
}
