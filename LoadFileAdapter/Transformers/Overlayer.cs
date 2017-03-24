using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Transformers
{
    public class Overlayer
    {
        private bool hasOverlayFamilies = false;
        private bool hasOverlayMetaData = false;
        private bool hasOverlayRepresentatives = false;

        public Overlayer(bool overlayFamilies, bool overlayMeta, bool overlayFiles)
        {
            this.hasOverlayFamilies = overlayFamilies;
            this.hasOverlayMetaData = overlayMeta;
            this.hasOverlayRepresentatives = overlayFiles;
        }

        public Document Overlay(Document original, Document updated)
        {
            Document document = original;

            if (original.Key.Equals(updated.Key))
            {
                if (this.hasOverlayFamilies)
                {
                    document = overlayFamilies(document, updated);
                }

                if (this.hasOverlayMetaData)
                {
                    document = overlayMetaData(document, updated);
                }

                if (this.hasOverlayRepresentatives)
                {
                    document = overlayRepresentatives(document, updated);
                }
            }
            else
            {
                string msg = String.Format(
                    "Overlay failure, the document key ({0}) does not match the overlay key ({1}).",
                    original.Key, updated.Key);
                throw new Exception(msg);
            }

            return document;
        }

        protected Document overlayFamilies(Document original, Document updated)
        {
            // disconnect the recipocal relationship of the parent doc
            // before updating the parent relationship
            if (original.Parent != null && original.Parent != updated.Parent)
            {
                original.Parent.Children.Remove(original);
            }
            
            // disconnect the reciprocal relationship of any children
            // that are not also in the updated document
            if (original.Children != null)
            {
                List<Document> childrenToRemove = new List<Document>();

                foreach (Document child in original.Children)
                {
                    if (!updated.Children.Contains(child))
                    {
                        childrenToRemove.Add(child);
                    }
                }
                                
                foreach (Document child in childrenToRemove)
                {                    
                    child.SetParent(null);
                }
            }

            Document doc = new Document(updated.Key, updated.Parent, updated.Children, 
                original.Metadata, original.Representatives);

            // update child docs
            if (doc.Children != null)
            {
                doc.Children.ToList().ForEach(child => child.SetParent(doc));                
            }

            return doc;
        }

        protected Document overlayMetaData(Document original, Document updated)
        {
            Dictionary<string, string> metadata = original.Metadata;
            
            foreach (var field in updated.Metadata)
            {
                if (metadata.ContainsKey(field.Key))
                {
                    metadata[field.Key] = field.Value;
                }
                else
                {
                    metadata.Add(field.Key, field.Value);
                }
            }

            return new Document(original.Key, original.Parent, original.Children, metadata, original.Representatives);
        }

        protected Document overlayRepresentatives(Document original, Document update)
        {
            HashSet<Representative> representatives = new HashSet<Representative>();

            if (original.Representatives == null)
            {
                representatives = update.Representatives;
            }
            else
            {
                Dictionary<Representative.FileType, Representative> files = new Dictionary<Representative.FileType, Representative>();

                foreach (Representative file in original.Representatives)
                {
                    files.Add(file.Type, file);
                }

                foreach (Representative file in update.Representatives)
                {
                    if (files.ContainsKey(file.Type))
                    {
                        files[file.Type] = file;
                    }
                    else
                    {
                        files.Add(file.Type, file);
                    }
                }

                foreach (var kvp in files)
                {
                    representatives.Add(kvp.Value);
                }
            }

            return new Document(original.Key, original.Parent, original.Children, original.Metadata, representatives);
        }
    }
}
