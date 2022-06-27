using Microsoft.AspNetCore.Identity;

namespace ShareInvest.Server.Data.Models
{
    public class CoreUser : IdentityUser
    {
        public string? NickName
        {
            get; set;
        }
        public string? ProfileImage
        {
            get; set;
        }
        public string? ThumbnailImage
        {
            get; set;
        }
    }
}