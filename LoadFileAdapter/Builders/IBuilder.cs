using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    public interface IBuilder<T, S>
        where T: BuildDocCollectionSettings
        where S: BuildDocSettings
    {
        List<Document> BuildDocuments(T args);
        Document BuildDocument(S args);
    }
}
