using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SQE.Backend.DataAccess.POCOs
{
    public class Scroll
    {
        public string Name { get; set; }
        public int DefaultScrollVersionId { get; set; }
        public List<string> ThumbnailURLs { get; set; }
        public List<int> ScrollVersionIds { get; set; }

        public Scroll() { }

        internal Scroll(RawScroll raw)
        {
            Name = raw.name;
            DefaultScrollVersionId = raw.scroll_version_id;
            ScrollVersionIds = JsonConvert.DeserializeObject<List<int>>(raw.scroll_version_ids);
            ThumbnailURLs = raw.thumbnails == null ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(raw.thumbnails);
        }
    }

    internal class RawScroll
    {
        public string name { get; set; }
        public int scroll_version_id { get; set; }
        public string scroll_version_ids { get; set; }
        public string thumbnails { get; set; }
    }
}