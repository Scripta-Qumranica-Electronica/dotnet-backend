using SQE.Backend.DataAccess.POCOs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SQE.Backend.DataAccess
{
    public interface IScrollRepository
    {
        Task<IEnumerable<Scroll>> ListScrolls();
        // Task<IEnumerable<ScrollVersion>> ListMyScrollVersions();
        // Task<ScrollVersion> GetScrollVersion(int id);
    }

    public class ScrollRepository: BaseRepository, IScrollRepository
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
                var results = await connection.QueryAsync<RawScroll>(sql);
                return results.Select(raw => new Scroll(raw));
            }
        }
    }
}
