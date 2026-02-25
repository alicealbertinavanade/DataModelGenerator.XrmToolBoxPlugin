using System.IO;
using System;

namespace Avanade.XrmToolbox.DataModelDevOpsExtractor
{
    public static class UserConfig
    {
        private static string ConfigPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Avanade.XrmToolbox.DataModelDevOpsExtractor.config");

        public static void SaveConnectionString(string connStr)
        {
            File.WriteAllText(ConfigPath, connStr ?? "");
        }

        public static string LoadConnectionString()
        {
            if (File.Exists(ConfigPath))
                return File.ReadAllText(ConfigPath);
            return string.Empty;
        }
    }
}
