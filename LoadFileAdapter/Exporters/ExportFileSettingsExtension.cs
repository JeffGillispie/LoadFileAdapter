using System;
using System.IO;

namespace LoadFileAdapter.Exporters
{
    public static class ExportFileSettingsExtension
    {
        public static bool CreateDestination(this IExportFileSettings settings)
        {            
            bool result = false;

            try
            {
                DirectoryInfo destination = settings.GetFile().Directory;

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
