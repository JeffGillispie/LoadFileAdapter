using System;
using System.IO;
using System.Linq;
using System.Text;

namespace LoadFileAdapter.Exporters
{
    public class TabularExporter : IExporter<TabularExportFileSetting, TabularExportWriterSetting>
    {
        public void Export(TabularExportFileSetting args)
        {
            bool append = false;

            using (TextWriter writer = new StreamWriter(args.File.FullName, append, args.Encoding))
            {
                TabularExportWriterSetting writerArgs = new TabularExportWriterSetting(writer, args.Documents, args.Delimiters);
                Export(writerArgs);
            }
        }

        public void Export(TabularExportWriterSetting args)
        {
            string header = getHeader(args.Documents, args.Delimiters);
            args.Writer.WriteLine(header);

            foreach (Document document in args.Documents)
            {
                string record = getRecord(document, args.Delimiters);
                args.Writer.WriteLine(record);
            }
        }

        protected string getHeader(DocumentCollection docs, Parsers.Delimiters delimiters)
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

        protected string getRecord(Document doc, Parsers.Delimiters delimiters)
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
