using Newtonsoft.Json;
using SQE.Backend.DataAccess.Models;
using System;
using System.Collections.Generic;

namespace SQE.Backend.Access.RawModels
{
    internal interface IQueryResponse<T>
    {
        T CreateModel();
    }

    internal class ListScrollQueryResponse: IQueryResponse<Scroll>
    {
        public string name { get; set; }
        public int scroll_version_id { get; set; }
        public string scroll_version_ids { get; set; }
        public string thumbnails { get; set; }

        public Scroll CreateModel()
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

    internal class ListScrollVersionsQueryResponse: IQueryResponse<ScrollVersion>
    {
        // This model serves both the GetScrollVersions and GetMyScrollVersions repository calls.
        // The queries return slightly different fields, and should probably be changed in the future.
        public string name { get; set; }
        public int scroll_version_id { get; set; }
        public string owner { get; set; }
        public int? numOfArtefacts { get; set; }
        public int? numOfColsFrags { get; set; }
        public bool locked { get; set; }
        public bool? can_write { get; set; }
        public bool? can_lock { get; set; }
        public DateTime lastEdit { get; set; }
        public string shared { get; set; }
        public string thumbnails { get; set; }

        private class RawOwner
        {
            public string name { get; set; }
            public int user_id { get; set; }
        }

        private class RawShareInfo
        {
            public string name { get; set; }
            public int user_id { get; set; }
            public bool may_write { get; set; }
            public bool may_lock { get; set; }
        }
        public ScrollVersion CreateModel()
        {
            var ownerObj = JsonConvert.DeserializeObject<RawOwner>(owner);

            var sv = new ScrollVersion
            {
                Name = name,
                Id = scroll_version_id,
                ThumbnailUrls = thumbnails == null ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(thumbnails),
                Owner = new User
                {
                    Name = ownerObj.name,
                    Id = ownerObj.user_id,
                },

            };

            return sv;
        }
    }
}