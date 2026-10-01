using System;
using System.Linq;

namespace DataModelDevOpsExtractor.Model
{
    public sealed class DevOpsWikiPageReference
    {
        public string Project { get; private set; }
        public string WikiIdentifier { get; private set; }
        public int PageId { get; private set; }

        public static bool TryParse(string url, out DevOpsWikiPageReference pageReference)
        {
            pageReference = null;
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return false;
            }

            var segments = uri.AbsolutePath
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(Uri.UnescapeDataString)
                .ToArray();
            var wikiSegmentIndex = Array.FindIndex(segments, segment =>
                string.Equals(segment, "_wiki", StringComparison.OrdinalIgnoreCase));

            if (wikiSegmentIndex < 1 ||
                wikiSegmentIndex + 3 >= segments.Length ||
                !string.Equals(segments[wikiSegmentIndex + 1], "wikis", StringComparison.OrdinalIgnoreCase) ||
                !int.TryParse(segments[wikiSegmentIndex + 3], out var pageId))
            {
                return false;
            }

            var project = segments[wikiSegmentIndex - 1];
            var wikiIdentifier = segments[wikiSegmentIndex + 2];
            if (string.IsNullOrWhiteSpace(project) || string.IsNullOrWhiteSpace(wikiIdentifier))
            {
                return false;
            }

            pageReference = new DevOpsWikiPageReference
            {
                Project = project,
                WikiIdentifier = wikiIdentifier,
                PageId = pageId
            };
            return true;
        }
    }
}