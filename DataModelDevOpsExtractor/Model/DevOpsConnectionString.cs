using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelDevOpsExtractor.Model
{
    internal class DevOpsConnectionString
    {
        public DevOpsConnectionString()
        {

        }

        public Uri OrgUrl { get; set; }
        public string PersonalAccessToken { get; set; }

        public string Project { get; set; }

        public static DevOpsConnectionString Parse(string connectionString)
        {
            Dictionary<string, string> connectionProperties =
            connectionString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                .ToDictionary(r => r[0].Trim(), r => r[1].Trim());

            if (!connectionProperties.ContainsKey(nameof(OrgUrl)))
            {
                throw new ArgumentException($"Connection string does not contain {nameof(OrgUrl)} parameter");
            }

            if (!connectionProperties.ContainsKey(nameof(PersonalAccessToken)))
            {
                throw new ArgumentException($"Connection string does not contain {nameof(PersonalAccessToken)} parameter");
            }

            if (!connectionProperties.ContainsKey(nameof(Project)))
            {
                throw new ArgumentException($"Connection string does not contain {nameof(Project)} parameter");
            }

            return new DevOpsConnectionString()
            {
                OrgUrl = new Uri(connectionProperties[nameof(OrgUrl)]),
                PersonalAccessToken = connectionProperties[nameof(PersonalAccessToken)],
                Project = connectionProperties[nameof(Project)]
            };
        }

    }
}
