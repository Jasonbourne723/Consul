using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNet.Microservices.Consul.Apps.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeathController : ControllerBase
    {
		[HttpGet]
		public IActionResult Index()
		{
			return Ok("ok");
		}
	}
}