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
        
        private RepresentativeTransformation(Regex findText, string replaceText, string filterField, Regex filterText)
            : base(findText, replaceText, filterField, filterText)
        {
            // do nothing here
        }

        /// <summary>
        /// The target <see cref="Representative"/> to modify.
        /// </summary>
        public Representative.FileType TargetType { get { return targetType; } }

        /// <summary>
        /// The udpated type if any. A null value will not trigger a type change.
        /// </summary>
        public Representative.FileType? NewType { get { return newType; } }
                
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

        /// <summary>
        /// Builds a new instance of <see cref="RepresentativeTransformation"/>.
        /// </summary>
        public class Builder
        {
            private RepresentativeTransformation instance;

            private Builder(Regex findText, string replaceText, string filterField, Regex filterText)
            {
                instance = new RepresentativeTransformation(findText, replaceText, filterField, filterText);
            }

            /// <summary>
            /// Starts the process of building a <see cref="RepresentativeTransformation"/>.
            /// </summary>
            /// <param name="targetType">The <see cref="Representative"/> <see cref="Representative.FileType"/> to edit.</param>
            /// <param name="findText">The Regex pattern to find in the file path.</param>
            /// <param name="replaceText">The file path replacement text.</param>
            /// <returns>Returns a <see cref="RepresentativeTransformation"/> builder.</returns>
            public static Builder Start(Representative.FileType targetType, Regex findText, string replaceText)
            {                
                Builder builder = new Builder(findText, replaceText, null, null);
                builder.instance.targetType = targetType;
                return builder;
            }

            /// <summary>
            /// Starts the process of building a <see cref="RepresentativeTransformation"/>.
            /// </summary>
            /// <param name="targetType">The <see cref="Representative"/> <see cref="Representative.FileType"/> to edit.</param>
            /// <returns>Returns a <see cref="RepresentativeTransformation"/> builder.</returns>
            public static Builder Start(Representative.FileType targetType)
            {
                Builder builder = new Builder(null, null, null, null);
                builder.instance.targetType = targetType;
                return builder;
            }

            /// <summary>
            /// Sets the metadata field used to filter documents to edit.
            /// </summary>
            /// <param name="value">The filter field value.</param>
            /// <returns>Returns an updated builder.</returns>
            public Builder SetFilterField(string value)
            {
                var instance = new RepresentativeTransformation(
                    this.instance.FindText,
                    this.instance.ReplaceText,
                    value,
                    this.instance.FilterText);
                instance.newType = this.instance.newType;
                instance.targetType = this.instance.targetType;                
                this.instance = instance;
                return this;
            }

            /// <summary>
            /// Sets the pattern used to filter on the filter field values.
            /// </summary>
            /// <param name="value">The filter text value.</param>
            /// <returns>Returns an updated builder.</returns>
            public Builder SetFilterText(Regex value)
            {
                var instance = new RepresentativeTransformation(
                    this.instance.FindText,
                    this.instance.ReplaceText,
                    this.instance.FilterField,
                    value);
                instance.newType = this.instance.newType;
                instance.targetType = this.instance.targetType;
                this.instance = instance;
                return this;
            }

            /// <summary>
            /// Sets the replacement <see cref="Representative"/> <see cref="Representative.FileType"/>.
            /// </summary>
            /// <param name="value">The new representative file type.</param>
            /// <returns>Returns an updated builder.</returns>
            public Builder SetNewType(Representative.FileType? value)
            {
                instance.newType = value;
                return this;
            }

            /// <summary>
            /// Builds an instance of <see cref="RepresentativeTransformation"/>.
            /// </summary>
            /// <returns>Returns the final <see cref="RepresentativeTransformation"/>.</returns>
            public RepresentativeTransformation Build()
            {
                RepresentativeTransformation instance = this.instance;
                this.instance = null;
                return instance;
            }
        }
    }
}
