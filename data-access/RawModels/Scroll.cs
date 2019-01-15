using Newtonsoft.Json;
using SQE.Backend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

    internal class ListVersionsOfScrollQueryResponse: IQueryResponse<ScrollVersion>
    {
        public string name { get; set; }
        public int scroll_version_id { get; set; }
        public string owner { get; set; }
        public int? numOfArtefacts { get; set; }
        public int? numOfColsFrags { get; set; }
        public int locked { get; set; }
        public int can_write { get; set; }
        public int can_lock { get; set; }
        public DateTime lastEdit { get; set; }

        private class RawOwner
        {
            public string name { get; set; }
            public int user_id { get; set; }
        }

        public ScrollVersion CreateModel()
        {
            var ownerObj = JsonConvert.DeserializeObject<RawOwner>(owner);

            var sv = new ScrollVersion
            {
                Name = name,
                Id = scroll_version_id,
                Owner = new User
                {
                    Name = ownerObj.name,
                    Id = ownerObj.user_id,
                },
                Permission = new Permission
                {
                    CanWrite = can_write == 1,
                    CanLock = can_lock == 1,
                },
                Locked =  locked == 1,
                NumOfArtefacts = numOfArtefacts,
                NumOfColsFrags = numOfColsFrags,
            };

            return sv;
        }
    }

    internal class ListMyScrollVersionsQueryResponse: IQueryResponse<ScrollVersion>
    {
        public string name { get; set; }
        public int scroll_version_group_id { get; set; }
        public string owner { get; set; }
        public string shared { get; set; }
        public string scroll_version_ids { get; set; }
        public string thumbnails { get; set; }
        public int image_fragments { get; set; }
        public int scroll_version_id { get; set; }

        private class RawOwner
        {
            public string name { get; set; }
            public int user_id { get; set; }
        }

        private class RawShareInfo
        {
            public string name { get; set; }
            public int user_id { get; set; }
            public int scroll_version_id { get; set; }
            public int may_write { get; set; }
            public int may_lock { get; set; }
        }

        public ScrollVersion CreateModel()
        {
            var ownerObj = JsonConvert.DeserializeObject<RawOwner>(owner);
            var sharedList = JsonConvert.DeserializeObject<List<RawShareInfo>>(shared);
            var thumbnailList = JsonConvert.DeserializeObject<List<string>>(thumbnails);

            var sv = new ScrollVersion
            {
                Name = name,
                Id = scroll_version_id,
                ThumbnailUrls = thumbnailList,
                Owner = new User
                {
                    Name = ownerObj.name,
                    Id = ownerObj.user_id,
                },
                Sharing = sharedList.Select(shared => new ShareInfo
                {
                    User = new User
                    {
                        Name = shared.name,
                        Id = shared.user_id,
                    },
                    Permission = new Permission
                    {
                        CanWrite = shared.may_write == 1,
                        CanLock = shared.may_lock == 1,
                    },
                }).ToList(),
            };

            return sv;
        }
    }
}