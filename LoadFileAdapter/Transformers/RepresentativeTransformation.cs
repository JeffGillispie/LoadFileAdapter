using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LoadFileAdapter.Transformers
{
    /// <summary>
    /// Contains the settings to modify a <see cref="Document"/> 
    /// <see cref="Representative"/>.
    /// </summary>
    public class RepresentativeTransformation : Transformation
    {
        private Representative.FileType targetType;
        private Representative.FileType? newType;
        
        /// <summary>
        /// The target <see cref="Representative"/> to modify.
        /// </summary>
        public Representative.FileType TargetType { get { return targetType; } }

        /// <summary>
        /// The udpated type if any. A null value will not trigger a type change.
        /// </summary>
        public Representative.FileType? NewType { get { return newType; } }
        
        /// <summary>
        /// Initializes a new instance of <see cref="RepresentativeTransformation"/>.
        /// </summary>
        /// <param name="targetType">The rep type to modify.</param>
        /// <param name="newType">The updated type if any.</param>
        /// <param name="findText">The regex of the find / replace operation on file paths.</param>
        /// <param name="replaceText">The replace value used with the findText regex.</param>
        /// <param name="filterField">A metafield used to determine if an edit should be performed.</param>
        /// <param name="filterText">The regex used on the filter field.</param>
        public RepresentativeTransformation(
            Representative.FileType targetType, Representative.FileType? newType, 
            Regex findText, string replaceText,
            string filterField, Regex filterText) :
            base (findText, replaceText, filterField, filterText)
        {
            this.targetType = targetType;
            this.newType = newType;            
        }

        /// <summary>
        /// Modifies the type and path or a representative.
        /// </summary>
        /// <param name="doc">The document to be modified.</param>
        public override void Transform(Document doc)
        {
            if (base.hasEdit(doc))
            {
                HashSet<Representative> representatives = new HashSet<Representative>();

                foreach (Representative rep in doc.Representatives)
                {
                    if (rep.Type == targetType)
                    {
                        Representative.FileType updatedType = (newType != null) 
                            ? (Representative.FileType)newType 
                            : rep.Type;
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
