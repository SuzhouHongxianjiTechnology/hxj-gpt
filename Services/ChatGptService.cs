using SqlSugar;
using System.IO;
using Models;
using ChatGPTSharp;
using AutoRpa.Configs;
using Microsoft.Extensions.Options;
using CliFx.Infrastructure;
using System;
using CliFx;
using CliFx.Attributes;

namespace AutoRpa.Services
{
    [Command("chatgpt", Description = "chatgpt")]
    public class ChatGptService : ICommand
    {
        private readonly ISqlSugarClient _db;
        private readonly IOptionsSnapshot<AutoRpaRoot> _options; //依赖注入可选项
        private ChatGPTClient _client;

        [CommandParameter(0, Description = "")]
        public ChatGptFuncEnum ChatGptFunc { get; set; }

        [CommandOption("Seo", 's',
            Description = "Seo 策略查看请输入 chatgpt ls,此命令直接按照策略进行文章生成")]
        public int Seo { get; set; }

        public ChatGptService(ISqlSugarClient db, IOptionsSnapshot<AutoRpaRoot> options)
        {
            this._db = db;
            _options = options;
            _client = new ChatGPTClient(
                _options.Value.ChatGpt.SecretKey,
                _options.Value.ChatGpt.Model,
                "",
                _options.Value.ChatGpt.OutTime);
        }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            switch (ChatGptFunc)
            {
                case ChatGptFuncEnum.ls:
                    await _db.Queryable<SEO>()
                        .ForEachAsync(x => console.Output.WriteLineAsync($"{x.InkPk}--{x.SEOTitle}--{x.SEOContent}"));
                    break;
                case ChatGptFuncEnum.tg:
                    if (Seo == 0)
                    {
                        console.WithColors(ConsoleColor.Red, ConsoleColor.Black);
                        await console.Output.WriteLineAsync("请选择策略使用 chatgpt tg -s 1 进行文章生成");
                        console.WithColors(ConsoleColor.White, ConsoleColor.Black);
                        return;
                    }

                    var seoEntity = await _db.Queryable<SEO>().InSingleAsync(Seo);
                    // 判断 seoEntity 是否为空 
                    if (seoEntity == null)
                    {
                        console.WithColors(ConsoleColor.Red, ConsoleColor.Black);
                        await console.Output.WriteLineAsync("选择的策略在数据库中查询不到，请选择正确的策略");
                        console.WithColors(ConsoleColor.White, ConsoleColor.Black);
                        return;
                    }

                    var titleHotList = await _db.Queryable<TitleHot>().Where(x => x.IsOk == 0).ToListAsync();
                    foreach (var x in titleHotList)
                    {
                        try
                        {
                            await console.Output.WriteLineAsync($"ChatGpt 开始生成文章-{x.Title} ....");
                            var msg = await _client.SendMessage(x.Title,
                                sendSystemType: ChatGPTSharp.Model.SendSystemType.Custom,
                                sendSystemMessage: seoEntity.SEOContent);

                            if (msg?.Response?.Length < 10)
                            {
                                await console.Output.WriteLineAsync("ChatGpt 生成文章失败 ....");
                                await console.Output.WriteLineAsync($"ChatGpt 生成文章失败标题为-{x.Title} ....");
                            }
                            else
                            {
                                var titleContent = _options.Value.ChatGpt.TitleHeader + msg.Response;
                                
                                if (_options.Value.ChatGpt.IsUseFileStorage)
                                {
                                    await console.Output.WriteLineAsync($"ChatGpt 在本地生成文章-{x.Title} ....");
                                    // 判断文件是否存在，如果不存在则创建 
                                    var filePath = _options.Value.ChatGpt.TitlePath + x.Title + _options.Value.ChatGpt.TitleType;
                                    // var titleContent = _options.Value.ChatGpt.TitleHeader + msg.Response;
                                    await File.WriteAllTextAsync(filePath, titleContent);
                                    await console.Output.WriteLineAsync($"ChatGpt 生成文章完毕-{x.Title} ....");
                                }
                                
                                if (_options.Value.ChatGpt.IsUseSqlStorage)
                                {
                                    // 开始将文章上传数据库
                                    await console.Output.WriteLineAsync($"ChatGpt 开始将文章上传数据库-{x.Title} ....");
                                    x.Anser = titleContent;
                                }

                                // 更新数据库
                                x.IsOk = 1;
                                await _db.Updateable<TitleHot>(x).ExecuteCommandAsync();
                                await console.Output.WriteLineAsync($"ChatGpt 更新数据库完毕数据库-{x.Title} ....\n");
                                await console.Output.WriteLineAsync($"===========================================\n");
                            }
                        }
                        catch (Exception e)
                        {
                            await console.Output.WriteLineAsync(e.Message);
                        }
                    }
                    break;
                default:
                    goto case ChatGptFuncEnum.ls;
            }
        }
    }

    public enum ChatGptFuncEnum
    {
        tg,
        ls
    }
}
