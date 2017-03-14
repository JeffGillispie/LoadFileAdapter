using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Exporters
{
    public class TextDelimitedExporter : Exporter
    {
        public void Export(DocumentSet documents, FileInfo file, Encoding encoding, string volumeName, Parsers.Delimiters delimiters)
        {
            bool append = false;

            using (StreamWriter writer = new StreamWriter(file.FullName, append, encoding))
            {
                string header = getHeader(documents, delimiters);
                writer.WriteLine(header);

                foreach (Document document in documents)
                {
                    string record = getRecord(document, delimiters);
                    writer.WriteLine(record);
                }
            }
        }

        private string getHeader(DocumentSet docs, Parsers.Delimiters delimiters)
        {
            Document doc = docs.First();
            StringBuilder sb = new StringBuilder();
            int fieldCount = doc.Metadata.Count;
            int counter = 1;
            
            foreach (string fieldName in doc.Metadata.Keys)
            {
                string value = fieldName.Replace(
                    delimiters.TextQualifier.ToString(),
                    String.Format("{0}{1}", delimiters.EscapeCharacter, delimiters.TextQualifier)
                    );

                if (delimiters.TextQualifier != Parsers.Delimiters.Null)
                {
                    sb.Append(delimiters.TextQualifier);
                }

                sb.Append(value);

                if (delimiters.TextQualifier != Parsers.Delimiters.Null)
                {
                    sb.Append(delimiters.TextQualifier);
                }

                if (counter < fieldCount)
                {
                    sb.Append(delimiters.FieldSeparator);
                }

                counter++;
            }

            return sb.ToString();
        }

        private string getRecord(Document doc, Parsers.Delimiters delimiters)
        {
            StringBuilder sb = new StringBuilder();
            int fieldCount = doc.Metadata.Count;
            int counter = 1;

            foreach (string fieldValue in doc.Metadata.Values)
            {
                string value = fieldValue.Replace(
                    delimiters.TextQualifier.ToString(), 
                    String.Format("{0}{1}", delimiters.EscapeCharacter, delimiters.TextQualifier)
                    );

                if (delimiters.TextQualifier != Parsers.Delimiters.Null)
                {
                    sb.Append(delimiters.TextQualifier);
                }

                sb.Append(value);

                if (delimiters.TextQualifier != Parsers.Delimiters.Null)
                {
                    sb.Append(delimiters.TextQualifier);
                }

                if (counter < fieldCount)
                {
                    sb.Append(delimiters.FieldSeparator);
                }

                counter++;
            }
            
            return sb.ToString();
        }
    }
}
