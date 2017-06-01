using LoadFileAdapter.Transformers;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// Contains the instructions for editing a <see cref="Representative"/>.
    /// It is a wrapper for <see cref="RepresentativeTransformation"/>.
    /// It is also used to serialize instructions and deserialize instructions
    /// from XML.
    /// </summary>
    public class RepresentativeEdit : Edit
    {
        /// <summary>
        /// The <see cref="Representative.FileType"/> this edit should modify.
        /// </summary>
        public Representative.FileType TargetType;

        /// <summary>
        /// The final type the <see cref="Representative.FileType"/> should be changed to 
        /// or null if it shouldn't be changed.
        /// </summary>
        public Representative.FileType? NewType;

        /// <summary>
        /// Initializes a new instance of <see cref="RepresentativeEdit"/>.
        /// </summary>
        public RepresentativeEdit() : base()
        {
            // do nothing here
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RepresentativeEdit"/>.
        /// </summary>
        /// <param name="edit">The <see cref="RepresentativeTransformation"/> used to build the edit.</param>
        public RepresentativeEdit(RepresentativeTransformation edit)
        {
            this.TargetType = edit.TargetType;
            this.NewType = edit.NewType;
            base.FilterField = edit.FilterField;
            base.FilterText = edit.FilterText;
            base.FindText = edit.FindText;
            base.ReplaceText = edit.ReplaceText;
        }

        /// <summary>
        /// Gets <see cref="RepresentativeTransformation"/> value of the edit.
        /// </summary>
        /// <returns>A <see cref="RepresentativeTransformation"/>.</returns>
        public override Transformation GetTransformation()
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
