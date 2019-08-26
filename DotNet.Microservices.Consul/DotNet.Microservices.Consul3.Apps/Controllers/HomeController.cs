using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DotNet.Microservices.Consul3.Apps.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class HomeController : ControllerBase
	{
		private readonly IConfiguration _iconfiguration;

		public HomeController(IConfiguration configuration)
		{
			this._iconfiguration = configuration;
		}
		// GET api/values
		[HttpGet]
		public void Get()
		{
			var url = $"http://{_iconfiguration["Consul:ServerIP"]}:{_iconfiguration["Consul:Port"]}";
			using (var consulClient = new ConsulClient(a => a.Address = new Uri(url)))
			{
				var services = consulClient.Catalog.Service("Test1").Result.Response;
				if (services != null && services.Any())
				{
					// 模拟随机一台进行请求，这里只是测试，可以选择合适的负载均衡工具或框架
					Random r = new Random();
					int index = r.Next(services.Count());
					var service = services.ElementAt(index);

					using (HttpClient client = new HttpClient())
					{
						var response =  client.GetAsync($"http://{service.ServiceAddress}:{service.ServicePort}/api/Heath");
						var result = response.Result;
					}
				}
			}
		}
	}
}
