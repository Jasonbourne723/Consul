using System;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DotNet.Microservices.Consul.Repository
{
	public  static class AppBuilderExtensions
	{
		public static void RegisterConsul(this IApplicationBuilder app, IApplicationLifetime lifetime, IConfiguration configuration)
		{
			var consul = configuration.GetSection("Consul");
			Action<ConsulClientConfiguration> configClient = (consulConfig) =>
			{
				consulConfig.Address = new Uri($"http://{consul["ServerIp"]}:{consul["Port"]}");
				consulConfig.Datacenter = "dc1";
			};
			//建立连接
			var consulClient = new ConsulClient(configClient);
			var httpCheck = new AgentServiceCheck()
			{
				DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
				Interval = TimeSpan.FromSeconds(10),//健康监测
				HTTP = consul["Health"],//心跳检测地址
				Timeout = TimeSpan.FromSeconds(5)
			};
			//注册
			var registrtion = new AgentServiceRegistration()
			{
				Checks = new[] { httpCheck },
				ID = consul["ServerName"] + Guid.NewGuid().ToString(),//服务编号不可重复
				Name = consul["ServerName"],//服务名称
				Address = NetworkHelper.LocalIPAddress,//ip地址
				Port = int.Parse(consul["LocalPort"])//端口

			};
			//注册服务
			consulClient.Agent.ServiceRegister(registrtion);
			lifetime.ApplicationStopping.Register(() =>
			{
				consulClient.Agent.ServiceDeregister(registrtion.ID).Wait();//服务停止时取消注册
			});
		}
	}
}
