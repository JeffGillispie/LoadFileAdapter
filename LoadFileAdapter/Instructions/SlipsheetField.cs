using System;

namespace LoadFileAdapter.Instructions
{
    public class SlipsheetField :IEquatable<SlipsheetField>
    {
        public string FieldName;
        public string Alias;

        public SlipsheetField()
        {
            // do nothing here
        }

        public SlipsheetField(string name, string alias)
        {
            this.FieldName = name;
            this.Alias = alias;
        }

        public bool Equals(SlipsheetField field)
        {
            if (field == null) return false;

            return this.FieldName.Equals(field.FieldName) &&
                this.Alias.Equals(field.Alias);
        }
    }
}
