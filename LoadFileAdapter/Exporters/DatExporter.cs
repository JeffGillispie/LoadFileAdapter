using System;
using System.IO;
using System.Linq;
using System.Text;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Exporters
{
    public class DatExporter : IExporter<ExportFileDatSettings, ExportWriterDatSettings>
    {
        public void Export(ExportFileDatSettings args)
        {
            bool append = false;

            using (TextWriter writer = new StreamWriter(args.File.FullName, append, args.Encoding))
            {
                ExportWriterDatSettings writerArgs = new ExportWriterDatSettings(writer, args.Documents, args.Delimiters, args.ExportFields);
                Export(writerArgs);
            }
        }

        public void Export(ExportWriterDatSettings args)
        {
            string header = getHeader(args.ExportFields, args.Delimiters);
            args.Writer.WriteLine(header);

            foreach (Document document in args.Documents)
            {
                string record = getRecord(document, args.Delimiters, args.ExportFields);
                args.Writer.WriteLine(record);
            }
        }

        protected string getHeader(string[] fields, Delimiters delimiters)
        {            
            StringBuilder sb = new StringBuilder();            
            int counter = 1;
            
            foreach (string fieldName in fields)
            {
                string value = fieldName.Replace(
                    delimiters.TextQualifier.ToString(),
                    String.Format("{0}{1}", delimiters.EscapeCharacter, delimiters.TextQualifier)
                    );

                if (delimiters.TextQualifier != Delimiters.Null)
                {
                    sb.Append(delimiters.TextQualifier);
                }

                sb.Append(value);

                if (delimiters.TextQualifier != Delimiters.Null)
                {
                    sb.Append(delimiters.TextQualifier);
                }

                if (counter < fields.Length)
                {
                    sb.Append(delimiters.FieldSeparator);
                }

                counter++;
            }

            return sb.ToString();
        }

        protected string getRecord(Document doc, Delimiters delimiters, string[] fields)
        {
            StringBuilder sb = new StringBuilder();
            int fieldCount = doc.Metadata.Count;
            int counter = 1;

            foreach (string fieldName in fields)
            {
                string value = (doc.Metadata.ContainsKey(fieldName))
                    ? doc.Metadata[fieldName]
                    : String.Empty;
                
                if (delimiters.EscapeCharacter.Equals(Delimiters.Null))
                {
                    value.Replace(delimiters.TextQualifier.ToString(), String.Empty);
                }
                else
                {
                    value.Replace(
                        delimiters.TextQualifier.ToString(),
                        String.Format("{0}{1}", delimiters.EscapeCharacter, delimiters.TextQualifier)
                        );
                }
                    

                if (delimiters.TextQualifier != Delimiters.Null)
                {
                    sb.Append(delimiters.TextQualifier);
                }

                sb.Append(value);

                if (delimiters.TextQualifier != Delimiters.Null)
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
