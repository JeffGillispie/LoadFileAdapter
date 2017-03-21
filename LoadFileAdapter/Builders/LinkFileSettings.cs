using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Builders
{
    public class LinkFileSettings
    {
        private string repCol;
        private Representative.Type repType;

        public string RepresentativeColumn { get { return this.repCol; } }
        public Representative.Type RepresentativeType { get { return this.repType; } }

        public LinkFileSettings(string column, Representative.Type type)
        {
            this.repCol = column;
            this.repType = type;
        }
    }
}
