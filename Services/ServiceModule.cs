using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoRpa.Configs;
using AutoRpa.Services;
using Microsoft.Extensions.Configuration;
using SqlSugar;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceModule
    {
        public static void AddAutoRpaService(this IServiceCollection service)
        {
            IConfigurationRoot configuration;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                configuration = new ConfigurationBuilder()
               .AddJsonFile(@"Configs\appsettings.json", false, true)
               .Build();
            }
            else
            {
               configuration = new ConfigurationBuilder()
               .AddJsonFile("/albert/net6.0/Configs/appsettings.json", false, true)
               .Build();
            }

            service.AddOptions().Configure<AutoRpaRoot>(config => configuration.Bind(config));

            service.AddScoped<ISqlSugarClient>(s =>
            {
                if (configuration["ConnectionStrings:DBType"] == "MySql")
                {
                    SqlSugarClient sqlSugar = new SqlSugarClient(new ConnectionConfig()
                        {

                            DbType = SqlSugar.DbType.MySql,
                            ConnectionString = configuration["ConnectionStrings:DbServer"],
                            IsAutoCloseConnection = true,
                        },
                        db =>
                        {
                            // //单例参数配置，所有上下文生效
                            // db.Aop.OnLogExecuting = (sql, pars) =>
                            // {
                            //     Console.WriteLine($"SqlSugar执行：{sql}");
                            // };
                        });
                    return sqlSugar;
                }
                else
                {
                    SqlSugarClient sqlSugar = new SqlSugarClient(new ConnectionConfig()
                        {
                            DbType = SqlSugar.DbType.Sqlite,
                            ConnectionString = "DataSource=sqlsugar-dev.db",
                            IsAutoCloseConnection = true,
                        },
                        db =>
                        {
                            //单例参数配置，所有上下文生效
                            // db.Aop.OnLogExecuting = (sql, pars) =>
                            // {
                            //     Console.WriteLine($"SqlSugar执行：{sql}");
                            // };
                        });
                    return sqlSugar;
                }
            });
            service.AddScoped<CrawlService>();
            service.AddScoped<GitService>();
            service.AddScoped<ChatGptService>();
        }
    }
}
