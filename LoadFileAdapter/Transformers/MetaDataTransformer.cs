﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Transformers
{
    public class MetaDataTransformer
    {
        public void Transform(DocumentCollection docs, MetaDataEdit[] edits)
        {
            foreach (Document doc in docs)
            {
                foreach (MetaDataEdit edit in edits)
                {
                    doc.Transform(edit);
                    //doc = edit.Transform(doc);
                }
            }
        }        
    }
}
