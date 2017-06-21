﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using LoadFileAdapter.Importers;
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
    [XmlInclude(typeof(LfpImport))]
    [XmlInclude(typeof(OptImport))]
    [XmlInclude(typeof(DatExport))]
    [XmlInclude(typeof(LfpExport))]
    [XmlInclude(typeof(OptExport))]
    [XmlInclude(typeof(XlsExport))]
    [XmlInclude(typeof(XrefExport))]
    [XmlInclude(typeof(MetaDataEdit))]
    [XmlInclude(typeof(RepresentativeEdit))]
    [XmlInclude(typeof(DateFormatEdit))]
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
        /// <param name="transformations">An array of <see cref="Transformation"/> used in the job.</param>
        public Job(Import[] imports, Export[] exports, Transformation[] transformations) : this()
        {
            this.Imports = imports;
            this.Exports = exports;
            
            if (transformations != null)
            {                
                List<Edit> edits = new List<Edit>();
                Dictionary<Type, Func<Transformation, Edit>> editMap;
                editMap = new Dictionary<Type, Func<Transformation, Edit>>();

                editMap.Add(
                    typeof(MetaDataTransformation), 
                    (t) => new MetaDataEdit((MetaDataTransformation)t));
                editMap.Add(
                    typeof(RepresentativeTransformation), 
                    (t) => new RepresentativeEdit((RepresentativeTransformation)t));
                editMap.Add(
                    typeof(DateFormatTransformation), 
                    (t) => new DateFormatEdit((DateFormatTransformation)t));
                
                foreach (Transformation transformation in transformations)
                {
                    Edit edit = editMap[transformation.GetType()].Invoke(transformation);
                    edits.Add(edit);                    
                }

                this.Edits = edits.ToArray();
            }
            else
                this.Edits = null;
        }

        /// <summary>
        /// Converts an array of <see cref="Edit"/> which support serialization to
        /// an array of <see cref="Transformation"/>, which are immutable.
        /// </summary>
        /// <returns>An array of <see cref="Transformation"/>.</returns>
        public IList<Transformation> GetTransformations()
        {
            return (this.Edits != null) 
                ? this.Edits.Select(e => e.ToTransformation()).ToList()
                : null;
        }
        
        /// <summary>
        /// Serializes this job into XML.
        /// </summary>
        /// <returns>A serialized <see cref="Job"/> instance.</returns>
        public virtual string ToXml()
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

        /// <summary>
        /// Executes the imports, edits, and exports in the job.        
        /// </summary>
        public virtual DocumentCollection Execute()
        {
            DocumentCollection docs = new DocumentCollection();
            Transformer transformer = new Transformer();
            IList<Transformation> transformations = GetTransformations();

            foreach (Import import in Imports)
            {
                IImporter importer = import.BuildImporter();
                DocumentCollection importedDocs = importer.Import(import.File, import.Encoding);
                docs.AddRange(importedDocs);

                if (import.GetType().Equals(typeof(DatImport)))
                {
                    DatImport datImport = (DatImport)import;

                    if (datImport.FolderPrependFields != null)
                    {
                        foreach (string field in datImport.FolderPrependFields)
                        {
                            MetaDataTransformation edit = MetaDataTransformation.Builder
                                .Start(field, null, String.Empty)
                                .SetPrependDir(import.File.Directory)
                                .Build();
                            transformations.Add(edit);
                        }
                    }

                    if (datImport.FolderPrependLinks != null)
                    {
                        Regex find = new Regex("\\?(.+)");
                        string replace = Path.Combine(import.File.Directory.FullName, "$1");

                        foreach (var type in datImport.FolderPrependLinks)
                        {                            
                            RepresentativeTransformation edit = RepresentativeTransformation.Builder
                                .Start(type, find, replace)
                                .Build();                                                        
                            transformations.Add(edit);
                        }
                    }
                }                
            }
                        
            transformer.Transform(docs, transformations);

            foreach (Export export in Exports)
            {
                export.BuildExporter().Export(docs);
            }

            return docs;
        }
    }
}
