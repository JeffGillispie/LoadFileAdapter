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
                DatImporter importer = new DatImporter(import.Delimiters.GetDelimiters());
                importer.Builder.HasHeader = import.HasHeader;
                importer.Builder.KeyColumnName = import.KeyColumnName;
                importer.Builder.ParentColumnName = import.ParentColumnName;
                importer.Builder.ChildColumnName = import.ChildColumnName;
                importer.Builder.ChildSeparator = import.ChildColumnDelimiter;
                importer.Builder.RepresentativeBuilders = (import.LinkedFiles != null)
                    ? import.LinkedFiles.Select(f => f.GetSetting()).ToList()
                    : null;                
                return importer.Import(import.File, import.Encoding);
            }
            else if (instructions.File.Extension.ToUpper().Equals(LFP_EXT))
            {
                ImgImport import = (ImgImport)instructions;
                LfpImporter importer = new LfpImporter();
                importer.Builder.PathPrefix = (import.BuildAbsolutePath) 
                    ? import.File.Directory.FullName : null;
                importer.Builder.TextBuilder = (import.TextSetting != null)
                    ? import.TextSetting.GetSettings() : null;
                return importer.Import(import.File, import.Encoding);
            }
            else if (instructions.File.Extension.ToUpper().Equals(OPT_EXT))
            {
                ImgImport import = (ImgImport)instructions;
                OptImporter importer = new OptImporter();
                importer.Builder.PathPrefix = (import.BuildAbsolutePath)
                    ? import.File.Directory.FullName : null;
                importer.Builder.TextBuilder = (import.TextSetting != null)
                    ? import.TextSetting.GetSettings() : null;
                return importer.Import(import.File, import.Encoding);
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
                DatExporter exporter = DatExporter.Builder
                    .Start(ex.File, ex.Encoding, ex.ExportFields)
                    .SetDelimiters(ex.Delimiters.GetDelimiters())
                    .Build();
                exporter.Export(docs);
            }
            else if (export.GetType().Equals(typeof(XlsExport)))
            {
                XlsExport ex = (XlsExport)export;                
                XlsExporter exporter = XlsExporter.Builder
                    .Start(ex.File, ex.ExportFields)
                    .SetLinks(ex.Hyperlinks.Select(l => l.GetLinkSettings()).ToArray())
                    .Build();                    
                exporter.Export(docs);
            }
            else if (export.GetType().Equals(typeof(XrefExport)))
            {
                XrefExport ex = (XrefExport)export;                
                XrefExporter exporter = XrefExporter.Builder
                    .Start(ex.File, ex.Encoding)
                    .SetSlipsheets(ex.SlipsheetSettings.GetSlipsheets())                    
                    .SetBoxTrigger(ex.BoxBreakTrigger.GetSwitch())
                    .SetGroupStartTrigger(ex.GroupStartTrigger.GetSwitch())
                    .SetCodeStartTrigger(ex.CodeStartTrigger.GetSwitch())
                    .SetCustomerDataField(ex.CustomerDataField)
                    .SetNamedFolderField(ex.NamedFolderField)
                    .SetNamedFileField(ex.NamedFileField)
                    .Build();                   
                exporter.Export(docs);
            }
            else if (export.File.Extension.ToUpper().Equals(LFP_EXT))
            {
                ImgExport ex = (ImgExport)export;                
                LfpExporter exporter = LfpExporter.Builder
                    .Start(ex.File, ex.Encoding)
                    .SetVolumeName(ex.VolumeName)
                    .Build();
                exporter.Export(docs);
            }
            else if (export.File.Extension.ToUpper().Equals(OPT_EXT))
            {
                ImgExport ex = (ImgExport)export;
                OptExporter exporter = OptExporter.Builder
                    .Start(ex.File, ex.Encoding)
                    .SetVolumeName(ex.VolumeName)
                    .Build();                
                exporter.Export(docs);
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
