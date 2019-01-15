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

    public class User
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class Permission
    {
        public bool CanWrite { get; set; }
        public bool CanLock { get; set; }
    }

    public class ShareInfo
    {
        public User User { get; set; }
        public Permission Permission { get; set; }
    }

    public class ScrollVersion
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<string> ThumbnailUrls { get; set; }
        public User Owner { get; set; }
        public Permission Permission { get; set; }
        public List<ShareInfo> Sharing { get; set; }
        public bool? Locked { get; set; }
        public int? NumOfArtefacts { get; set; }
        public int? NumOfColsFrags { get; set; }
    }
}