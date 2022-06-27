using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace ShareInvest.Client.Shared
{
    public partial class LoginDisplayBase : ComponentBase
    {
        protected internal async Task BeginSignOut(MouseEventArgs _)
        {
            if (SignOutManager is not null)
                await SignOutManager.SetSignOutState();

            if (Navigation is not null)
                Navigation.NavigateTo("authentication/logout");
        }
        [Inject]
        NavigationManager? Navigation
        {
            get; set;
        }
        [Inject]
        SignOutSessionStateManager? SignOutManager
        {
            get; set;
        }
    }
}