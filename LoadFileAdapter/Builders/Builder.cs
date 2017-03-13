using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Builders
{
    public interface Builder
    {
        List<Document> BuildDocuments(DocumentSetBuilderArgs e);
        Document BuildDocument(DocumentBuilderArgs e);
    }
}
