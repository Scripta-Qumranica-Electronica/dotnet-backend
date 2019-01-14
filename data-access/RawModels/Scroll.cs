using Newtonsoft.Json;
using SQE.Backend.DataAccess.Models;
using System.Collections.Generic;

namespace SQE.Backend.Access.RawModels
{
    internal class ListScrollQueryResponse
    {
        public string name { get; set; }
        public int scroll_version_id { get; set; }
        public string scroll_version_ids { get; set; }
        public string thumbnails { get; set; }

        public Scroll CreateScroll()
        {
            return new Scroll
            {
                Name = name,
                DefaultScrollVersionId = scroll_version_id,
                ScrollVersionIds = JsonConvert.DeserializeObject<List<int>>(scroll_version_ids),
                ThumbnailURLs = thumbnails == null ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(thumbnails),
            };
        }
    }
}