using System;
using System.IO;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// Extension methods for exporters.
    /// </summary>
    public static class ExporterExtensions
    {
        /// <summary>
        /// Creates the destination folder if it doesn't exist.
        /// </summary>
        /// <param name="exporter"></param>
        /// <param name="outfile">The destination file.</param>
        /// <returns>Returns true if the destination folder exists 
        /// or is created successfully.</returns>
        internal static bool CreateDestination(this IExporter exporter, FileInfo outfile)
        {            
            bool result = false;

            try
            {
                DirectoryInfo destination = outfile.Directory;

                result = destination.Exists;

                if (result == false)
                {
                    destination.Create();
                    result = destination.Exists;
                }
            }
            catch (Exception) { }

            return result;
        }
    }
}
