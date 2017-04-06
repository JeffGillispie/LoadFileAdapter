using System;
using System.Linq;
using LoadFileAdapter.Importers;
using LoadFileAdapter.Exporters;
using LoadFileAdapter.Transformers;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// The executor executes the instructions contained in a job.
    /// It imports, edits, and exports data.
    /// </summary>
    public class Executor
    {
        private const string LFP_EXT = ".LFP";
        private const string OPT_EXT = ".OPT";

        /// <summary>
        /// Imports the data specified in the import instructions.
        /// </summary>
        /// <param name="instructions">The import instructions.</param>
        /// <returns>Returns the collection of documents imported.</returns>
        public DocumentCollection ImportDocs(Import instructions)
        {
            if (instructions.GetType().Equals(typeof(DatImport)))
            {
                DatImport import = (DatImport)instructions;
                DatImporter importer = new DatImporter();
                return importer.Import(
                    import.File, 
                    import.Encoding, 
                    import.Delimiters.GetDelimiters(), 
                    import.HasHeader,
                    import.KeyColumnName, 
                    import.ParentColumnName, 
                    import.ChildColumnName, 
                    import.ChildColumnDelimiter,
                    (import.LinkedFiles != null) 
                        ? import.LinkedFiles.Select(f => f.GetSetting()).ToList()
                        : null);
            }
            else if (instructions.File.Extension.ToUpper().Equals(LFP_EXT))
            {
                ImgImport import = (ImgImport)instructions;
                LfpImporter importer = new LfpImporter();
                return importer.Import(
                    import.File, 
                    import.Encoding, 
                    (import.TextSetting != null)
                        ? import.TextSetting.GetSettings()
                        : null,
                    import.BuildAbsolutePath);
            }
            else if (instructions.File.Extension.ToUpper().Equals(OPT_EXT))
            {
                ImgImport import = (ImgImport)instructions;
                OptImporter importer = new OptImporter();
                return importer.Import(
                    import.File, 
                    import.Encoding,
                    (import.TextSetting != null)
                        ? import.TextSetting.GetSettings()
                        : null,
                    import.BuildAbsolutePath);
            }
            else
            {
                throw new Exception(
                    "Executor Exception: The import is not a DAT import and it does not have the correct file extension.");
            }
        }

        /// <summary>
        /// Exports the supplied document collection using the supplied export instructions.
        /// </summary>
        /// <param name="export">The export instructions to use in the export.</param>
        /// <param name="docs">The collection of documents to be exported.</param>
        public void ExportDocs(Export export, DocumentCollection docs)
        {
            if (export.GetType().Equals(typeof(DatExport)))
            {
                DatExport ex = (DatExport)export;
                ExportDatFileSettings settings = new ExportDatFileSettings(
                    docs, ex.File, ex.Encoding, ex.Delimiters.GetDelimiters(), ex.ExportFields);
                DatExporter exporter = new DatExporter();
                exporter.Export(settings);
            }
            else if (export.GetType().Equals(typeof(XlsExport)))
            {
                XlsExport ex = (XlsExport)export;
                ExportXlsSettings settings = new ExportXlsSettings(
                    docs, ex.File, ex.ExportFields, 
                    ex.Hyperlinks.Select(l => l.GetLinkSettings()).ToArray());
                XlsExporter exporter = new XlsExporter();
                exporter.Export(settings);
            }
            else if (export.File.Extension.ToUpper().Equals(LFP_EXT))
            {
                ImgExport ex = (ImgExport)export;
                ExportImageFileSettings settings = new ExportImageFileSettings(
                    docs, ex.File, ex.Encoding, ex.VolumeName);
                LfpExporter exporter = new LfpExporter();
                exporter.Export(settings);
            }
            else if (export.File.Extension.ToUpper().Equals(OPT_EXT))
            {
                ImgExport ex = (ImgExport)export;
                ExportImageFileSettings settings = new ExportImageFileSettings(
                    docs, ex.File, ex.Encoding, ex.VolumeName);
                OptExporter exporter = new OptExporter();
                exporter.Export(settings);
            }
            else
            {
                throw new Exception(
                    "Executor Exception: The export is not a DAT export and it does not have the correct file extension.");
            }
        }

        /// <summary>
        /// Imports the data specified in the job's imports,
        /// edits the data specified in the job's edits,
        /// and exports the data specified in the job's exports.
        /// </summary>
        /// <param name="job">The job to execute.</param>
        public void Execute(Job job)
        {
            DocumentCollection docs = new DocumentCollection();
            Transformer transformer = new Transformer();

            foreach (Import import in job.Imports)
            {
                DocumentCollection importedDocs = ImportDocs(import);
                docs.AddRange(importedDocs);
            }
            
            transformer.Transform(docs, job.GetTransformations());

            foreach (Export export in job.Exports)
            {
                ExportDocs(export, docs);
            }
        }    
    }
}
