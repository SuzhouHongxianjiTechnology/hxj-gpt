using ChatGPTSharp;
using System.Reflection;
using System.Text;
using AutoRpa.Configs;
using CliFx;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Serilog;
using Serilog.Formatting.Json;
using SqlSugar;

public class Program
{
    private static ServiceCollection service = new ServiceCollection();

    static async Task Main(string[] args)
    {
        Console.WriteLine("程序启动中...");
        service.AddAutoRpaService();
        using (var sp = service.BuildServiceProvider())
        {
            //使用的前置条件是service要将实现ICommand接口的对象注入进去
            //从当前程序集注入所有实现ICommand接口的CMD指令
            var application = new CliApplicationBuilder()
                .AddCommandsFromThisAssembly()
                .UseTypeActivator(t => sp.GetRequiredService(t))
                .Build();
            //执行CMD
            await application.RunAsync();
        }
    }
}






