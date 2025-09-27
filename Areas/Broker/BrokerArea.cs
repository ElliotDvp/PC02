using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PC02.Areas.Broker
{
    [Area("Broker")]
    [Authorize(Roles = "Broker")]
    public abstract class BrokerControllerBase : Controller { }
}
