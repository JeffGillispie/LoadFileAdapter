using System.IO;
using System.Text;

namespace LoadFileAdapter.Importers
{
    /// <summary>
    /// Imports load file data into a <see cref="DocumentCollection"/>.
    /// </summary>
    interface IImporter
    {
        /// <summary>
        /// Imports data into a <see cref="DocumentCollection"/> from a file.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        DocumentCollection Import(FileInfo file, Encoding encoding);
    }
}
