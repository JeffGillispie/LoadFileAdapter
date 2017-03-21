using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LoadFileAdapter.Transformers
{
    public class LinkedFileEdit : Edit
    {
        private LinkedFile.FileType targetType;
        private LinkedFile.FileType? newType;
        
        public LinkedFile.FileType TargetType { get { return targetType; } }
        public LinkedFile.FileType? NewType { get { return newType; } }
        
        public LinkedFileEdit(
            LinkedFile.FileType targetType, LinkedFile.FileType? newType, 
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
                HashSet<LinkedFile> representatives = new HashSet<LinkedFile>();

                foreach (LinkedFile rep in doc.LinkedFiles)
                {
                    if (rep.Type == targetType)
                    {
                        LinkedFile.FileType updatedType = (newType != null) ? (LinkedFile.FileType)newType : rep.Type;
                        SortedDictionary<string, string> updatedFiles = new SortedDictionary<string, string>();
                        
                        foreach (var file in rep.Files)
                        {
                            updatedFiles.Add(file.Key, base.Replace(file.Value));
                        }

                        LinkedFile newRep = new LinkedFile(updatedType, updatedFiles);
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
