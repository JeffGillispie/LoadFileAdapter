using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using LoadFileAdapter.Exporters;
using LoadFileAdapter.Importers;
using LoadFileAdapter.Transformers;

namespace LoadFileAdapter.Instructions
{    
    [XmlInclude(typeof(DatImport))]
    public class Job
    { 
        public ImportInstructions Import; 
        public ExportInstructions[] Exports; 
        public Edit[] Edits;

        public Job()
        {
            this.Import = null;
            this.Exports = null;
            this.Edits = null;
        }

        public Job(ImportInstructions import, ExportInstructions[] exports, Edit[] edits)
        {
            this.Import = import;
            this.Exports = exports;
            this.Edits = edits;
        }
        
        public string ToXml()
        {
            string xml = String.Empty;
            XmlSerializer serializer = new XmlSerializer(typeof(Job));

            using (var sw = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sw))
                {
                    serializer.Serialize(writer, this);
                    xml = sw.ToString();
                }
            }

            return xml;
        }

        public static Job Deserialize(string xml)
        {
            Job job = null;
            XmlSerializer serializer = new XmlSerializer(typeof(Job));

            using (StringReader reader = new StringReader(xml))
            {
                job = (Job)serializer.Deserialize(reader);
            }

            return job;
        }
    }
}
