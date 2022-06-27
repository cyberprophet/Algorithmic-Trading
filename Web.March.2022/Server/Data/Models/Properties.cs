using Newtonsoft.Json;

using System.Runtime.Serialization;

namespace ShareInvest.Server.Data.Models
{
    public struct Properties
    {
        [DataMember, JsonProperty("nickname")]
        public string? NickName
        {
            get; set;
        }
        [DataMember, JsonProperty("profile_image")]
        public string? ProfileImage
        {
            get; set;
        }
        [DataMember, JsonProperty("thumbnail_image")]
        public string? ThumbnailImage
        {
            get; set;
        }
    }
}