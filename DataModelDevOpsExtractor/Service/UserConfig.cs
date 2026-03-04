using System.IO;
using System;

namespace DataModelDevOpsExtractor
{
    public static class UserConfig
    {

        private static string DataModelEnvConnPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Avanade.XrmToolbox.DataModelDevOpsExtractor.datamodelenv");
        private static string ConfigPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Avanade.XrmToolbox.DataModelDevOpsExtractor.config");
        public static void SaveConnectionString(string connStr)
        {
            File.WriteAllText(ConfigPath, connStr ?? "");
        }

        public static void SaveDataModelEnvConnectionString(string connStr)
        {
            File.WriteAllText(DataModelEnvConnPath, connStr ?? "");
        }

        public static string LoadConnectionString()
        {
            if (File.Exists(ConfigPath))
                return File.ReadAllText(ConfigPath);
            return string.Empty;
        }

        public static string LoadDataModelEnvConnectionString()
        {
            if (File.Exists(DataModelEnvConnPath))
                return File.ReadAllText(DataModelEnvConnPath);
            return string.Empty;
        }
    }
}
