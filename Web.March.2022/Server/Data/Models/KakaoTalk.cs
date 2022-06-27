using Newtonsoft.Json;

using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace ShareInvest.Server.Data.Models
{
    public struct KakaoTalk
    {
        [DataMember, JsonProperty("connected_at")]
        public DateTime ConnectedAt
        {
            get; set;
        }
        [NotMapped, DataMember, JsonProperty("properties")]
        public Properties Property
        {
            get; set;
        }
        [DataMember, JsonProperty("id")]
        public string? Id
        {
            get; set;
        }
    }
}