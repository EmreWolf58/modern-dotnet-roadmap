using System.Security.Claims;
using TaskManagement.Api.Interfaces;

namespace TaskManagement.Api.Services
{
    public class CurrentUserService: ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId
        {
            get
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(userId, out var id) || id <= 0)
                    throw new UnauthorizedAccessException("Geçersiz kullanıcı");
                return id;
            }
        }

    }
}
