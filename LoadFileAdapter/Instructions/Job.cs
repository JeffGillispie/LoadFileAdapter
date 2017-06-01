using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using LoadFileAdapter.Transformers;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// The Job class is a container of instructions that details
    /// what will be imported, edited, and exported. It is also 
    /// used to serialize instructions and deserialize instructions
    /// from XML.
    /// </summary>
    [Serializable, XmlRoot("Job")]
    [XmlInclude(typeof(DatImport))]
    [XmlInclude(typeof(ImgImport))]
    [XmlInclude(typeof(DatExport))]
    [XmlInclude(typeof(ImgExport))]
    [XmlInclude(typeof(XlsExport))]
    [XmlInclude(typeof(MetaDataEdit))]
    [XmlInclude(typeof(RepresentativeEdit))]
    public class Job
    {
        /// <summary>
        /// The collection of import instructions that specifies 
        /// what will be imported when the job is executed.
        /// </summary>
        public Import[] Imports = null;

        /// <summary>
        /// The collection of export instructions that specifies
        /// what will be exported when the job is executed.
        /// </summary>
        public Export[] Exports = null;

        /// <summary>
        /// the collection of edit instructions that specifies
        /// what will be edited when the job executes.
        /// </summary>
        public Edit[] Edits = null;

        /// <summary>
        /// Initializes a new instance of <see cref="Job"/>.
        /// This parameterless constructor is use serializing this object to XML.
        /// </summary>
        public Job()
        {
            // do nothing here
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Job"/>.
        /// </summary>
        /// <param name="imports">An array of <see cref="Import"/> used in the job.</param>
        /// <param name="exports">An array of <see cref="Export"/> used in the job.</param>
        /// <param name="edits">An array of <see cref="Transformation"/> used in the job.</param>
        public Job(Import[] imports, Export[] exports, Transformation[] edits) : this()
        {
            this.Imports = imports;
            this.Exports = exports;

            if (edits != null)
            {
                List<Edit> editsList = new List<Edit>();

                foreach (Transformation edit in edits)
                {
                    if (edit.GetType().Equals(typeof(MetaDataTransformation)))
                    {
                        editsList.Add(new MetaDataEdit((MetaDataTransformation)edit));
                    }
                    else
                    {
                        editsList.Add(new RepresentativeEdit((RepresentativeTransformation)edit));
                    }
                }

                this.Edits = editsList.ToArray();
            }
            else
                this.Edits = null;
        }

        /// <summary>
        /// Converts an array of <see cref="Edit"/> which support serialization to
        /// an array of <see cref="Transformation"/>, which are immutable.
        /// </summary>
        /// <returns>An array of <see cref="Transformation"/>.</returns>
        public Transformation[] GetTransformations()
        {
            return (this.Edits != null ) 
                ? this.Edits.Select(e => e.GetEdit()).ToArray()
                : null;
        }
        
        /// <summary>
        /// Serializes this job into XML.
        /// </summary>
        /// <returns>A serialized <see cref="Job"/> instance.</returns>
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

        /// <summary>
        /// Desearializes an xml value into a <see cref="Job"/> instance.
        /// </summary>
        /// <param name="xml">An XML serialized value of a <see cref="Job"/>.</param>
        /// <returns>A new instance of a <see cref="Job"/>.</returns>
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
