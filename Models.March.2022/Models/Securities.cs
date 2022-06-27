using ShareInvest.Models.OpenAPI;

using System.ComponentModel.DataAnnotations;

namespace ShareInvest.Models
{
    public class Securities
    {
        public bool IsAdministrator
        {
            get; set;
        }
        public int Company
        {
            get; set;
        }
        public int Count
        {
            get; set;
        }
        [Required]
        public string? Id
        {
            get; set;
        }
        [StringLength(0x20), Required]
        public string? Name
        {
            get; set;
        }
        public string? User
        {
            get; set;
        }
        [StringLength(0x25)]
        public string? Key
        {
            get; set;
        }
        public virtual ICollection<Account> Accounts
        {
            get; set;
        }
        public virtual ICollection<Message> Messages
        {
            get; set;
        }
        public Securities()
        {
            Accounts = new HashSet<Account>();
            Messages = new HashSet<Message>();
        }
    }
}