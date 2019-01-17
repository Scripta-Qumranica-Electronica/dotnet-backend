using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQE.Backend.DataAccess;
using SQE.Backend.DTOs;
using SQE.Backend.Services;

namespace sqe_dotnet_backend.Controllers
{
    [Route("api/scroll")]
    [ApiController]
    public class ScrollController : ControllerBase
    {
        private IScrollRepository _repo;
        private IUserService _userService;

        public ScrollController(IScrollRepository scrollRepo, IUserService userService)
        {
            _userService = userService;
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

        [Authorize]
        [HttpGet("versions/my")] // api/scroll/versions/all - return all of my scroll versions
        public async Task<ActionResult<ListResult<ScrollVersion>>> ListMyScrollVersions()
        {
            int userId = Int32.Parse(_userService.getCurrentUserId());

            var scrollVersions = await _repo.ListMyScrollVersions(userId);
            var result = scrollVersions.Select(svModel => CreateScrollVersionDTO(svModel));

            return new ListResult<ScrollVersion>(result);
        }

        [Authorize]
        [HttpGet("versions/{id}")] // api/scroll/versions/<version-id>
        public async Task<ActionResult<ListResult<ScrollVersion>>> ListVersionsOfScroll(int id)
        {
            int userId = Int32.Parse(_userService.getCurrentUserId());

            var scrollVersions = await _repo.ListVersionsOfScroll(id, userId);
            var result = scrollVersions.Select(svModel => CreateScrollVersionDTO(svModel));

            return new ListResult<ScrollVersion>(result);
        }

        private ScrollVersion CreateScrollVersionDTO(SQE.Backend.DataAccess.Models.ScrollVersion model)
        {
            List<Share> shares = null;

            if (model.Sharing != null)
            {
                shares = model.Sharing.Select(sharingModel => new Share
                {
                    user = CreateUserDTO(sharingModel.User),
                    permission = CreatePermissionDTO(sharingModel.Permission)
                }).ToList();
            }

            return new ScrollVersion
            {
                id = model.Id,
                name = model.Name,
                owner = CreateUserDTO(model.Owner),
                permission = CreatePermissionDTO(model.Permission),
                thumbnailUrls = model.ThumbnailUrls,
                shares = shares,
                numOfArtefacts = model.NumOfArtefacts,
                numOfColsFrags = model.NumOfColsFrags,
                locked = model.Locked,
            };
        }

        private User CreateUserDTO(SQE.Backend.DataAccess.Models.User model)
        {
            return new User
            {
                name = model.Name,
                id = model.Id
            };
        }

        private Permission CreatePermissionDTO(SQE.Backend.DataAccess.Models.Permission model)
        {
            return new Permission
            {
                canWrite = model.CanWrite,
                canLock = model.CanLock,
            };
        }
    }
}