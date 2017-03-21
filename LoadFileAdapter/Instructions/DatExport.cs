using System.IO;
using System.Text;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Instructions
{
    public class DatExport : ExportInstructions
    {        
        public DelimitersBuilder Delimiters = new DelimitersBuilder(LoadFileAdapter.Parsers.Delimiters.CONCORDANCE);
        public string[] ExportFields = null;

        public DatExport() : base(null, null)
        {
            // do nothing here
        }

        public DatExport(FileInfo file, Encoding encoding, Delimiters delimiters) : base(file, encoding)
        {
            this.Delimiters = new DelimitersBuilder(delimiters);
        }                
    }
}
