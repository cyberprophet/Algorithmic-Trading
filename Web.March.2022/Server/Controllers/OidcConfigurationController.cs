using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Mvc;

namespace ShareInvest.Server.Controllers
{
    public class OidcConfigurationController : Controller
    {
        public OidcConfigurationController(IClientRequestParametersProvider clientRequestParametersProvider, ILogger<OidcConfigurationController> logger)
        {
            ClientRequestParametersProvider = clientRequestParametersProvider;
            this.logger = logger;
        }
        public IClientRequestParametersProvider ClientRequestParametersProvider
        {
            get;
        }
        [HttpGet("_configuration/{clientId}")]
        public IActionResult GetClientRequestParameters([FromRoute] string clientId)
        {
            logger.LogInformation(nameof(GetClientRequestParameters), clientId);

            return Ok(ClientRequestParametersProvider.GetClientParameters(HttpContext, clientId));
        }
        readonly ILogger<OidcConfigurationController> logger;
    }
}