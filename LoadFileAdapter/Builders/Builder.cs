using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    public interface Builder<T, S>
        where T: BuildDocumentsSetting
        where S: BuildDocumentSetting
    {
        List<Document> BuildDocuments(T args);
        Document BuildDocument(S args);
    }
}
