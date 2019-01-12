using System;
using System.Collections.Generic;

namespace SQE.Backend.DTOs
{
    public class Scroll
    {
        public string name { get; set; }
        public List<string> URLs { get; set; }
        public List<int> scrollVersionIds { get; set; }
        public int defaultScrollVersionId { get; set; }
        public int numImageFragments;
    }

    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Permission
    {
        public bool canWrite { get; set; }
        public bool canLock { get; set; }
    }

    public class Share
    {
        public User user { get; set; }
        public Permission permission { get; set; }
    }

    public class ScrollVersion
    {
        public int id { get; set; }
        public string name { get; set; }
        public int versionId { get; set; }
        public User owner { get; set; }
        public Permission ownerPermission { get; set; }
        public List<string> thumbnailUrls { get; set; }
        public List<Share> shares { get; set; }

        public int numOfArtefacts { get; set; }
        public int numOfColumns { get; set; }
        public int numOfFragments { get; set; }

        public bool locked { get; set; }
        public DateTime? lastEdit { get; set; }
    }
}