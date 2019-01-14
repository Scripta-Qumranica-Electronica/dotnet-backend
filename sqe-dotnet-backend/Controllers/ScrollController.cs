using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQE.Backend.DataAccess;
using SQE.Backend.DTOs;

namespace sqe_dotnet_backend.Controllers
{
    [Route("api/scroll")]
    [ApiController]
    public class ScrollController : ControllerBase
    {
        private IScrollRepository _repo;
        public ScrollController(IScrollRepository scrollRepo)
        {
            this._repo = scrollRepo;
        }

        [HttpGet("all")] // api/scroll/all
        public async Task<ListResult<Scroll>> ListAllScrolls()
        {
            var scrolls = await _repo.ListScrolls();

            var result = scrolls.Select(poco => new Scroll
            {
                name = poco.Name,
                thumbnailUrls = poco.ThumbnailURLs,
                scrollVersionIds = poco.ScrollVersionIds,
                defaultScrollVersionId = poco.DefaultScrollVersionId,
                numImageFragments = poco.ThumbnailURLs.Count
            });

            return new ListResult<Scroll>(result);
        }

        [HttpGet("versions/my")] // api/scroll/versions/all - return all of my scroll versions
        public async Task<ActionResult<IEnumerable<ScrollVersion>>> ListMyScrllVersions()
        {
            return new List<ScrollVersion>();
        }

        [HttpGet("versions/{id}")] // api/scroll/versions/<version-id>
        public async Task<ActionResult<ScrollVersion>> GetScrollVersion(int id)
        {
            return new ScrollVersion();
        }
    }
}