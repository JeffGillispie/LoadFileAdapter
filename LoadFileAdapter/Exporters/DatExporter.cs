using System;
using System.IO;
using System.Text;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// An exporter that exports a document collection to a DAT file.
    /// </summary>
    public class DatExporter : IExporter<IExportDatSettings>
    {
        /// <summary>
        /// Exports a document collection to a DAT.
        /// </summary>
        /// <param name="args">DAT export settings.</param>
        public void Export(IExportDatSettings args)
        {
            if (args.GetType().Equals(typeof(ExportDatFileSettings)))
            {
                Export((ExportDatFileSettings)args);
            }
            else if (args.GetType().Equals(typeof(ExportDatWriterSettings)))
            {
                Export((ExportDatWriterSettings)args);
            }
            else
            {
                throw new Exception("DatExporter Export Exception: The export settings were not a valid type.");
            }
        }

        /// <summary>
        /// Exports a document collection to a DAT file using a supplied destination.
        /// </summary>
        /// <param name="args">Export file settings.</param>
        public void Export(ExportDatFileSettings args)
        {
            bool append = false;
            string file = args.GetFile().FullName;
            Encoding encoding = args.GetEncoding();
            DocumentCollection docs = args.GetDocuments();
            Delimiters delims = args.GetDelimiters();
            string[] fields = args.GetExportFields();

            using (TextWriter writer = new StreamWriter(file, append, encoding))
            {
                ExportDatWriterSettings writerArgs = new ExportDatWriterSettings(
                    writer, docs, delims, fields);
                Export(writerArgs);
            }
        }

        /// <summary>
        /// Exports a document collection to a DAT file using a supplied writer.
        /// </summary>
        /// <param name="args">Export writer settings.</param>
        public void Export(ExportDatWriterSettings args)
        {
            DocumentCollection docs = args.GetDocuments();
            TextWriter writer = args.GetWriter();
            Delimiters delims = args.GetDelimiters();
            string[] fields = args.GetExportFields();
            string header = getHeader(fields, delims);
            writer.WriteLine(header);

            foreach (Document document in docs)
            {
                string record = getRecord(document, delims, fields);
                writer.WriteLine(record);
            }
        }

        /// <summary>
        /// Gets the header as a string.
        /// </summary>
        /// <param name="fields">The fields to use in the header.</param>
        /// <param name="delimiters">The delimiters to use in the header.</param>
        /// <returns>Returns a delimited string of field names.</returns>
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

        /// <summary>
        /// Gets a metadata record from a document.
        /// </summary>
        /// <param name="doc">The document containing the metadata record.</param>
        /// <param name="delimiters">The delimiters used to build the string.</param>
        /// <param name="fields">The fields to include in the record.</param>
        /// <returns>Returns a delimited string of the specified metadata fields.</returns>
        protected string getRecord(Document doc, Delimiters delimiters, string[] fields)
        {
            StringBuilder sb = new StringBuilder();
            int fieldCount = fields.Length;
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
