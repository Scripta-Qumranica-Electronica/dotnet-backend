using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQE.Backend.DTOs;

namespace sqe_dotnet_backend.Controllers
{
    [Route("api/scroll")]
    [ApiController]
    public class ScrollController : ControllerBase
    {
        [HttpGet("all")] // api/scroll/all
        public async Task<ActionResult<IEnumerable<Scroll>>> ListAllScrolls()
        {
            return new List<Scroll>();
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