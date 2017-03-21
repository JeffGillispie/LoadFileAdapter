using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoadFileAdapter.Importers;
using LoadFileAdapter.Exporters;
using LoadFileAdapter.Transformers;

namespace LoadFileAdapter.Instructions
{
    public class Executor
    {
        public DocumentCollection ImportDocs(ImportInstructions instructions)
        {

            if (instructions.GetType().Equals(typeof(DatImport)))
            {
                DatImport import = (DatImport)instructions;
                DatImporter importer = new DatImporter();
                return importer.Import(import.File, import.Encoding, import.Delimiters.GetDelimiters(), import.HasHeader,
                    import.KeyColumnName, import.ParentColumnName, import.ChildColumnName, import.ChildColumnDelimiter,
                    import.LinkedFiles.Select(f => f.GetSetting()).ToList());
            }
            else if (instructions.File.Extension.ToUpper().Equals(".LFP"))
            {
                ImgImport import = (ImgImport)instructions;
                LfpImporter importer = new LfpImporter();
                return importer.Import(import.File, import.Encoding, import.TextSetting.GetSettings());
            }
            else if (instructions.File.Extension.ToUpper().Equals(".OPT"))
            {
                ImgImport import = (ImgImport)instructions;
                OptImporter importer = new OptImporter();
                return importer.Import(import.File, import.Encoding, import.TextSetting.GetSettings());
            }
            else
            {
                throw new Exception("Invalid Import");
            }
        }

        public void ExportDocs(ExportInstructions export, DocumentCollection docs)
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

        public void Execute(Job job)
        {            
            DocumentCollection docs = ImportDocs(job.Import);
            Transformer transformer = new Transformer();
            transformer.Transform(docs, job.GetEdits());

            foreach (ExportInstructions export in job.Exports)
            {
                ExportDocs(export, docs);
            }
        }    
    }
}
