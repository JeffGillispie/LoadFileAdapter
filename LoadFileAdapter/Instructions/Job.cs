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
                
        public void Run()
        {
            DocumentCollection docs = importDocs();
            Transformer transformer = new Transformer();
            transformer.Transform(docs, this.Edits);

            foreach (ExportInstructions export in this.Exports)
            {
                exportDocs(export, docs);
            }
        }

        protected DocumentCollection importDocs()
        {
            
            if (this.Import.GetType().Equals(typeof(DatImport)))
            {
                DatImport import = (DatImport)this.Import;
                DatImporter importer = new DatImporter();
                return importer.Import(import.File, import.Encoding, import.Delimiters.GetDelimiters(), import.HasHeader,
                    import.KeyColumnName, import.ParentColumnName, import.ChildColumnName, import.ChildColumnDelimiter,
                    import.LinkedFiles.Select(f => f.GetSetting()).ToList());
            }
            else if (this.Import.File.Extension.ToUpper().Equals(".LFP"))
            {
                ImgImport import = (ImgImport)this.Import;
                LfpImporter importer = new LfpImporter();
                return importer.Import(import.File, import.Encoding, import.TextImportSetting);
            }
            else if (this.Import.File.Extension.ToUpper().Equals(".OPT"))
            {
                ImgImport import = (ImgImport)this.Import;
                OptImporter importer = new OptImporter();
                return importer.Import(import.File, import.Encoding, import.TextImportSetting);
            }
            else
            {
                throw new Exception("Invalid Import");
            }
        }

        protected void exportDocs(ExportInstructions export, DocumentCollection docs)
        {            
            if (export.GetType().Equals(typeof(DatExport)))
            {
                DatExport ex = (DatExport)export;
                ExportFileDatSettings settings = new ExportFileDatSettings(docs, ex.File, ex.Encoding, ex.Delimiters.GetDelimiters());
                DatExporter exporter = new DatExporter();
                exporter.Export(settings);
            }
            else if (export.File.Extension.ToUpper().Equals(".LFP"))
            {
                ImgExport ex = (ImgExport)export;
                ExportFileImageSettings settings = new ExportFileImageSettings(docs, ex.File, ex.Encoding, ex.VolumeName);
                LfpExporter exporter = new LfpExporter();
                exporter.Export(settings);
            }
            else if (export.File.Extension.ToUpper().Equals(".OPT"))
            {
                ImgExport ex = (ImgExport)export;
                ExportFileImageSettings settings = new ExportFileImageSettings(docs, ex.File, ex.Encoding, ex.VolumeName);
                OptExporter exporter = new OptExporter();
                exporter.Export(settings);
            }
            else
            {
                throw new Exception("Invalid Export");
            }
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
