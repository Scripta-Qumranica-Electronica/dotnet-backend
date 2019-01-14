using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SQE.Backend.DataAccess.Models
{
    public class Scroll
    {
        public string Name { get; set; }
        public int DefaultScrollVersionId { get; set; }
        public List<string> ThumbnailURLs { get; set; }
        public List<int> ScrollVersionIds { get; set; }
    }

    public class RawUser
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class ScrollVersion
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public List<int> scroll_version_ids;
        public List<string> ThumbnailUrls;
        // public User Owner;
    }

    internal class RawScrollVersion
    {
        // Taken from the query in line 1258 of scollery-cgi.pl
        public string name { get; set; }
        public int scroll_version_group_id { get; set; }
        public string owner { get; set; }
        public string shared { get; set; }
        // Ignore number_of_versions - it's just the number of elements in scroll_version_ids
        public string scroll_version_ids { get; set; }
        public string thumbnails { get; set; }
        // Ignore image_fragments - it's just number of elements in thumbnails
    }
}