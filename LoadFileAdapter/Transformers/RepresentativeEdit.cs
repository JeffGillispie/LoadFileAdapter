using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LoadFileAdapter.Transformers
{
    public class RepresentativeEdit : Edit
    {
        private Representative.Type targetType;
        private Representative.Type? newType;
        
        public Representative.Type TargetType { get { return targetType; } }
        public Representative.Type? NewType { get { return newType; } }
        
        public RepresentativeEdit(
            Representative.Type targetType, Representative.Type? newType, 
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
                    if (rep.RepresentativeType == targetType)
                    {
                        Representative.Type updatedType = (newType != null) ? (Representative.Type)newType : rep.RepresentativeType;
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

                doc.SetRepresentatives(representatives);
            }
        }
    }
}
