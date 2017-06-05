using System;
using System.IO;

namespace LoadFileAdapter.Exporters
{
    public static class ExporterExtensions
    {
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
