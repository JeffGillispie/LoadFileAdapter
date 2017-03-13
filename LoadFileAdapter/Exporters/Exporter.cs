using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Exporters
{
    interface Exporter
    {
        void Export(DocumentSet documents, FileInfo file, Encoding encoding, string volumeName, Parsers.Delimiters delimiters);
    }
}
