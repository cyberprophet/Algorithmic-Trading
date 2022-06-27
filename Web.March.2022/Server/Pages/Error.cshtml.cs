using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Diagnostics;

namespace ShareInvest.Server.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true), IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        public string? RequestId
        {
            get; set;
        }
        public bool ShowRequestId
        {
            get
            {
                logger.LogInformation(nameof(ShowRequestId), RequestId);

                return string.IsNullOrEmpty(RequestId) is false;
            }
        }
        public ErrorModel(ILogger<ErrorModel> logger) => this.logger = logger;
        public void OnGet() => RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        readonly ILogger<ErrorModel> logger;
    }
}