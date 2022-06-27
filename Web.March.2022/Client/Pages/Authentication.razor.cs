using Microsoft.AspNetCore.Components;

namespace ShareInvest.Client.Pages
{
    public partial class AuthenticationBase : ComponentBase
    {
        [Parameter]
        public string? Action
        {
            get; set;
        }
    }
}