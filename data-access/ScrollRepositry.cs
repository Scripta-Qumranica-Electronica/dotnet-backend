using SQE.Backend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using Microsoft.Extensions.Configuration;
using SQE.Backend.Access.RawModels;

namespace SQE.Backend.DataAccess
{
    public interface IScrollRepository
    {
        Task<IEnumerable<Scroll>> ListScrolls();
        Task<IEnumerable<ScrollVersion>> ListVersionsOfScroll(int masterVersionId, int userId);
        Task<IEnumerable<ScrollVersion>> ListMyScrollVersions(int userId);
    }

    public class ScrollRepository : BaseRepository, IScrollRepository
    {
        public ScrollRepository(IConfiguration config) : base(config) { }

        public async Task<IEnumerable<Scroll>> ListScrolls()
        {
            var sql = @"
SELECT scroll_data.name, 
	scroll_data_owner.scroll_version_id, 
	CONCAT('[', GROUP_CONCAT(DISTINCT scroll_version.scroll_version_id SEPARATOR ','),']') AS scroll_version_ids,
	CONCAT('[', GROUP_CONCAT('""', image_urls.proxy, image_urls.url, SQE_image.filename, '""' SEPARATOR ','),']') AS thumbnails 
FROM scroll_data
JOIN scroll_data_owner USING(scroll_data_id)
JOIN scroll_version USING(scroll_version_id)
LEFT JOIN edition_catalog ON edition_catalog.scroll_id = scroll_data.scroll_id

    AND edition_catalog.edition_side = 0
LEFT JOIN image_to_edition_catalog USING(edition_catalog_id)
LEFT JOIN SQE_image ON SQE_image.image_catalog_id = image_to_edition_catalog.image_catalog_id

    AND SQE_image.type = 0
LEFT JOIN image_urls USING(image_urls_id)
WHERE scroll_version.user_id = 1
GROUP BY scroll_data.scroll_id
";
            using (var connection = OpenConnection())
            {
                var results = await connection.QueryAsync<ListScrollQueryResponse>(sql);
                return results.Select(raw => raw.CreateModel());
            }
        }

        public async Task<IEnumerable<ScrollVersion>> ListVersionsOfScroll(int masterVersionId, int userId)
        {
            var sql = @"
SELECT  sv2.scroll_version_id, 
	JSON_OBJECT(""name"", user.user_name, ""user_id"", user.user_id) AS owner,
    scroll_data.name AS name,
    COUNT(DISTINCT artefact_position_owner.artefact_position_id) AS numOfArtefacts,
    COUNT(DISTINCT col_data_owner.col_data_id) AS numOfColsFrags,
    svg2.locked,
    sv2.may_write AS can_write,
    sv2.may_lock AS can_lock,
    MAX(main_action.time) AS lastEdit
FROM scroll_version_group AS svg1
JOIN scroll_version AS sv1 ON svg1.scroll_version_group_id = sv1.scroll_version_group_id
JOIN scroll_version_group AS svg2 ON svg2.scroll_id = svg1.scroll_id
JOIN scroll_version AS sv2 ON svg2.scroll_version_group_id = sv2.scroll_version_group_id
JOIN user ON user.user_id = sv2.user_id
JOIN scroll_data_owner ON sv2.scroll_version_id = scroll_data_owner.scroll_version_id
JOIN scroll_data USING(scroll_data_id)
LEFT JOIN artefact_position_owner ON artefact_position_owner.scroll_version_id = sv2.scroll_version_id
LEFT JOIN col_data_owner ON col_data_owner.scroll_version_id = sv2.scroll_version_id
LEFT JOIN main_action ON main_action.scroll_version_id = sv2.scroll_version_id
WHERE sv1.scroll_version_id = @ScrollVersionId
    AND(sv2.user_id = 1 OR sv2.user_id = @UserId)
GROUP BY sv2.scroll_version_id";

            using (var connection = OpenConnection())
            {
                var results = await connection.QueryAsync<ListVersionsOfScrollQueryResponse>(sql, new
                {
                    ScrollVersionId = masterVersionId,
                    UserId = userId,
                });
                return results.Select(raw => raw.CreateModel());
            }
        }

        public async Task<IEnumerable<ScrollVersion>> ListMyScrollVersions(int userId)
        {
            var sql = @"
SELECT scroll_data.name, 
	scroll_version_group.scroll_version_group_id, 
	JSON_OBJECT(""name"", admin.user_name, ""user_id"", admin.user_id) AS owner,

    CONCAT('[', GROUP_CONCAT(DISTINCT '{""name"":""', user.user_name, '"",""user_id"":', user.user_id, ',""scroll_version_id"":', sv2.scroll_version_id, ',""may_write"":', sv2.may_write, ',""may_lock"":', sv2.may_lock, '}'  SEPARATOR ','), ']') AS shared,
     COUNT(DISTINCT sv2.scroll_version_id) AS number_of_versions,
     CONCAT('[', GROUP_CONCAT(DISTINCT sv2.scroll_version_id SEPARATOR ','), ']') AS scroll_version_ids,
      CONCAT('[', GROUP_CONCAT('""', image_urls.proxy, image_urls.url, SQE_image.filename, '""' SEPARATOR ','), ']') AS thumbnails,
       COUNT(DISTINCT SQE_image.sqe_image_id) as imaged_fragments,
	sv1.scroll_version_id
FROM scroll_data
JOIN scroll_data_owner USING(scroll_data_id)
JOIN scroll_version AS sv1 USING(scroll_version_id)
JOIN scroll_version_group ON sv1.scroll_version_group_id = scroll_version_group.scroll_version_group_id
JOIN scroll_version AS sv2 ON sv2.scroll_version_group_id = scroll_version_group.scroll_version_group_id
LEFT JOIN edition_catalog ON edition_catalog.scroll_id = scroll_data.scroll_id

    AND edition_catalog.edition_side = 0
LEFT JOIN image_to_edition_catalog USING(edition_catalog_id)
LEFT JOIN SQE_image ON SQE_image.image_catalog_id = image_to_edition_catalog.image_catalog_id

    AND SQE_image.type = 0
LEFT JOIN image_urls USING(image_urls_id)
JOIN user ON user.user_id = sv2.user_id
JOIN scroll_version_group_admin ON scroll_version_group.scroll_version_group_id = scroll_version_group_admin.scroll_version_group_id
JOIN user AS admin ON scroll_version_group_admin.user_id = admin.user_id
WHERE sv1.user_id = @UserId
GROUP BY sv1.scroll_version_group_id
";
            using (var connection = OpenConnection())
            {
                var results = await connection.QueryAsync<ListMyScrollVersionsQueryResponse>(sql, new
                {
                    UserId = userId,
                });
                return results.Select(raw => SetPermissions(raw.CreateModel(), userId));
            }
        }

        private ScrollVersion SetPermissions(ScrollVersion sv, int userId)
        {
            // Set permissions based on the sharing info and userid

            var shareInfo = sv.Sharing.FirstOrDefault(si => si.User.Id == userId);
            if (shareInfo != null)
                sv.Permission = shareInfo.Permission;
            else
                sv.Permission = new Permission { CanLock = false, CanWrite = false };
            return sv;
        }
    }
}
