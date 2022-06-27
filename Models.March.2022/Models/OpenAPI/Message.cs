using System.ComponentModel.DataAnnotations;

namespace ShareInvest.Models.OpenAPI
{
    public class Message
    {
        public long Lookup
        {
            get; set;
        }
        [Required]
        public string? Title
        {
            get; set;
        }
        [Required]
        public string? Code
        {
            get; set;
        }
        [StringLength(4), Required]
        public string? Screen
        {
            get; set;
        }
        [StringLength(0x25)]
        public string? Key
        {
            get; set;
        }
        public int Company
        {
            get; set;
        }
    }
}