using System.Security.Claims;

namespace TaskManagment.Extencions
{
    //helper class to get user data
    public static class ClaimsPrincipalExtensions
    {

        //get the user guid from the claims
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var guid = Guid.TryParse(claim, out var id) ? id : Guid.Empty;
            return guid;
        }
    }
}
