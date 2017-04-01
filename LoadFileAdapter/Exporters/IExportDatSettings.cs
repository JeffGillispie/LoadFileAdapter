using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// DAT export settings.
    /// </summary>
    public interface IExportDatSettings : IExportSettings
    {
        /// <summary>
        /// The delimiters used in a DAT file export.
        /// </summary>
        Delimiters GetDelimiters();

        /// <summary>
        /// The fields exported to a DAT file.
        /// </summary>
        string[] GetExportFields();
    }
}
