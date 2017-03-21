using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Instructions
{
    public class DelimitersBuilder
    {
        public char FieldSeparator = ',';
        public char TextQualifier = Delimiters.Null;
        public char NewRecord = '\n';
        public char EscapeCharacter = Delimiters.Null;
        public char FlattenedNewLine = Delimiters.Null;

        public DelimitersBuilder()
        {

        }

        public DelimitersBuilder(Delimiters delims)
        {
            this.FieldSeparator = delims.FieldSeparator;
            this.TextQualifier = delims.TextQualifier;
            this.NewRecord = delims.NewRecord;
            this.EscapeCharacter = delims.EscapeCharacter;
            this.FlattenedNewLine = delims.FlattenedNewLine;
        }

        public Delimiters GetDelimiters()
        {
            return Delimiters.of(
                this.FieldSeparator, 
                this.TextQualifier, 
                this.NewRecord, 
                this.EscapeCharacter, 
                this.FlattenedNewLine);
        }
    }
}
