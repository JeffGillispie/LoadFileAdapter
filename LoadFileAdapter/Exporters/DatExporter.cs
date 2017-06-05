using System;
using System.IO;
using System.Text;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// An exporter that exports a document collection to a DAT file.
    /// </summary>
    public class DatExporter : IExporter
    {
        private FileInfo file;
        private Encoding encoding;
        private TextWriter writer;
        private string[] exportFields;
        private Delimiters delimiters = Delimiters.CONCORDANCE;
        
        private DatExporter()
        {
            // do nothing here
        }

        ~DatExporter()
        {
            if (writer != null)
            {
                writer.Close();
            }
        }
        
        
        public void Export(DocumentCollection docs)
        {                                       
            string header = getHeader(exportFields, delimiters);
            writer.WriteLine(header);

            foreach (Document document in docs)
            {
                string record = getRecord(document, delimiters, exportFields);
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

        public class Builder
        {
            private DatExporter instance;

            private Builder()
            {
                this.instance = new DatExporter();
            }

            public static Builder Start(FileInfo file, Encoding encoding, string[] exportFields)
            {
                Builder builder = new Builder();
                bool append = false;
                builder.instance.file = file;
                builder.instance.encoding = encoding;
                builder.instance.exportFields = exportFields;                
                builder.instance.writer = new StreamWriter(file.FullName, append, encoding);
                builder.instance.CreateDestination(file);
                return builder;
            }

            public static Builder Start(TextWriter writer, string[] exportFields)
            {
                Builder builder = new Builder();
                builder.instance.writer = writer;
                builder.instance.exportFields = exportFields;
                return builder;
            }

            public Builder SetDelimiters(Delimiters value)
            {
                instance.delimiters = value;
                return this;
            }

            public DatExporter Build()
            {
                DatExporter instance = this.instance;
                this.instance = null;
                return instance;
            }
        }
    }
}
