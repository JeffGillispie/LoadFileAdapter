using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Builders
{
    public class SemiStructuredRepresentativeSetting
    {
        private string repCol;
        private Representative.Type repType;

        public string RepresentativeColumn { get { return this.repCol; } }
        public Representative.Type RepresentativeType { get { return this.repType; } }

        public void SetColumn(string name)
        {
            this.repCol = name;
        }

        public void SetType(Representative.Type type)
        {
            this.repType = type;
        }
    }
}
