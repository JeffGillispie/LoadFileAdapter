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
    [XmlInclude(typeof(ImgImport))]
    [XmlInclude(typeof(DatExport))]
    [XmlInclude(typeof(ImgExport))]
    [XmlInclude(typeof(MetaDataEdit))]
    [XmlInclude(typeof(RepresentativeEdit))]
    public class Job
    { 
        public Import Import;        
        public Export[] Exports;        
        public Edit[] Edits;

        public Job()
        {
            this.Import = null;
            this.Exports = null;
            this.Edits = null;
        }

        public Job(Import import, Export[] exports, Transformation[] edits)
        {
            this.Import = import;
            this.Exports = exports;

            if (edits != null)
            {
                List<Edit> builderEdits = new List<Edit>();

                foreach (Transformation edit in edits)
                {
                    if (edit.GetType().Equals(typeof(MetaDataTransformation)))
                    {
                        builderEdits.Add(new MetaDataEdit((MetaDataTransformation)edit));
                    }
                    else
                    {
                        builderEdits.Add(new RepresentativeEdit((RepresentativeTransformation)edit));
                    }
                }

                this.Edits = builderEdits.ToArray();
            }
            else
                this.Edits = null;
        }

        public Transformation[] GetEdits()
        {
            return this.Edits.Select(e => e.GetEdit()).ToArray();
        }
        
        public string ToXml()
        {
            string xml = String.Empty;
            XmlSerializer serializer = new XmlSerializer(typeof(Job));

            using (var sw = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sw, new XmlWriterSettings { Indent = true }))
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
