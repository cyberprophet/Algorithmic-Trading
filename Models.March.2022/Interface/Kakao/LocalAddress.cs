using Newtonsoft.Json;

using System.Runtime.Serialization;

namespace ShareInvest.Interface.Kakao
{
    public struct LocalAddress
    {
        [DataMember, JsonProperty("meta")]
        public Meta Meta
        {
            get; set;
        }
        [DataMember, JsonProperty("documents")]
        public Documents[] Document
        {
            get; set;
        }
    }
    public struct Documents
    {
        [DataMember, JsonProperty("address_name")]
        public string Name
        {
            get; set;
        }
        [DataMember, JsonProperty("address_type")]
        public string Type
        {
            get; set;
        }
        [DataMember, JsonProperty("x")]
        public string Longitude
        {
            get; set;
        }
        [DataMember, JsonProperty("y")]
        public string Latitude
        {
            get; set;
        }
        [DataMember, JsonProperty("address")]
        public Address Address
        {
            get; set;
        }
        [DataMember, JsonProperty("road_address")]
        public RoadAddress Road
        {
            get; set;
        }
    }
    public struct RoadAddress
    {
        [DataMember, JsonProperty("address_name")]
        public string Name
        {
            get; set;
        }
        [DataMember, JsonProperty("region_1depth_name")]
        public string RegionName1
        {
            get; set;
        }
        [DataMember, JsonProperty("region_2depth_name")]
        public string RegionName2
        {
            get; set;
        }
        [DataMember, JsonProperty("region_3depth_name")]
        public string RegionName3
        {
            get; set;
        }
        [DataMember, JsonProperty("road_name")]
        public string RoadName
        {
            get; set;
        }
        [DataMember, JsonProperty("underground_yn")]
        public string Underground
        {
            get; set;
        }
        [DataMember, JsonProperty("main_building_no")]
        public string MainBuildingNumber
        {
            get; set;
        }
        [DataMember, JsonProperty("sub_building_no")]
        public string SubBuildingNumber
        {
            get; set;
        }
        [DataMember, JsonProperty("building_name")]
        public string BuildingName
        {
            get; set;
        }
        [DataMember, JsonProperty("zone_no")]
        public string ZoneNumber
        {
            get; set;
        }
        [DataMember, JsonProperty("x")]
        public string Longitude
        {
            get; set;
        }
        [DataMember, JsonProperty("y")]
        public string Latitude
        {
            get; set;
        }
    }
    public struct Address
    {
        [DataMember, JsonProperty("address_name")]
        public string Name
        {
            get; set;
        }
        [DataMember, JsonProperty("region_1depth_name")]
        public string RegionName1
        {
            get; set;
        }
        [DataMember, JsonProperty("region_2depth_name")]
        public string RegionName2
        {
            get; set;
        }
        [DataMember, JsonProperty("region_3depth_name")]
        public string RegionName3
        {
            get; set;
        }
        [DataMember, JsonProperty("region_3depth_h_name")]
        public string RegionNameH
        {
            get; set;
        }
        [DataMember, JsonProperty("h_code")]
        public string AdministrationCode
        {
            get; set;
        }
        [DataMember, JsonProperty("b_code")]
        public string CourtCode
        {
            get; set;
        }
        [DataMember, JsonProperty("mountain_yn")]
        public string Mountain
        {
            get; set;
        }
        [DataMember, JsonProperty("main_address_no")]
        public string Main
        {
            get; set;
        }
        [DataMember, JsonProperty("sub_address_no")]
        public string Sub
        {
            get; set;
        }
        [DataMember, JsonProperty("x")]
        public string Longitude
        {
            get; set;
        }
        [DataMember, JsonProperty("y")]
        public string Latitude
        {
            get; set;
        }
    }
    public struct Meta
    {
        [DataMember, JsonProperty("total_count")]
        public int Total
        {
            get; set;
        }
        [DataMember, JsonProperty("pageable_count")]
        public int PageableCount
        {
            get; set;
        }
        [DataMember, JsonProperty("is_end")]
        public bool IsEnd
        {
            get; set;
        }
    }
}