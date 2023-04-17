using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using HtmlAgilityPack;
using Models;
using ShellProgressBar;
using SqlSugar;

namespace AutoRpa.Services
{
    [Command("crawl", Description = "crawl")]
    public class CrawlService : ICommand
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public CrawlService(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }

        [CommandParameter(0, Description = "选择功能进行网站爬取")]
        public CrawlSupportFunc CrawlSupportFunc { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            console.WithColors(ConsoleColor.White, ConsoleColor.Black);
            switch (CrawlSupportFunc)
            {
                case CrawlSupportFunc.zhihuhot:
                    try
                    {
                        console.WithColors(ConsoleColor.White, ConsoleColor.Black);
                        await console.Output.WriteLineAsync("开启爬虫 ....");
                        var client = new HttpClient();
                        client.DefaultRequestHeaders.Add("Cookie", "_zap=7bc38a57-c82e-438a-8b3b-9b9bc136bd49; d_c0=AKCY4t6XuRWPTo35vyQWTRu6K_AQHW9RmYg=|1666012204; YD00517437729195%3AWM_TID=1rAzAxrWGzFEBRFERBOBMJ%2FC2Ix%2B4NEm; __snaker__id=Eb57Jsu7X5TKSkPV; YD00517437729195%3AWM_NI=4tOwX1jPxzb7dUVruiMFENWeKucRtxI7X1WjxjttvE6kn6WAFRNNDC5mdQgWF2DonSoFTrxEnhSF54xnD2jPtdMrwYdMiLLd10of3RLEKAgoLzzHUzfDiPfm1zHlm9TteEM%3D; YD00517437729195%3AWM_NIKE=9ca17ae2e6ffcda170e2e6ee93d35ff6b3fd92f97087eb8fb7c55b929f9b86c552a2a886abe5419bbda2ccef2af0fea7c3b92af8ac8198db3baa90b89be13da88b9a93f8638f9a8bafd66f89abadb3f7748be998d2d662bcb6bc95f54ff6b5a2d7d64b868e82afc749f7b3acb2e65d86ee8c90ed59ad9a9c93fb4faeb89da7b840bcb28cd3e95cb8bc8393f653a893a7adcf70f3f0a187b25a909b85a2e66590a6b6a9d872a69ca7b1e567f3a6888df03398acaba6d037e2a3; q_c1=23509e8721404c20a15ee48e6531ea4c|1670244083000|1670244083000; z_c0=2|1:0|10:1678450927|4:z_c0|80:MS4xRDkzVENRQUFBQUFtQUFBQVlBSlZUZjdsOVdURFpscDdRVHhQblA1azBuT3Znb0tqWllCZ1dRPT0=|bce30e248664d39ccc39b0e77729825637d1fe8aeeac2e8bc78c5c1f072905fd; _xsrf=64c15890-a9bd-4dbf-84c6-3e2a7810f4d5; Hm_lvt_98beee57fd2ef70ccdd5ca52b9740c49=1679403142,1679556773,1679576356,1679577049; tst=h; amp_e5f096=NojDdU7Pjan-SxODL_ES7x...1gs8ni7km.1gs8ni7km.0.0.0; amp_e5f096_zhihu.com=NojDdU7Pjan-SxODL_ES7x...1gs8ni7km.1gs8ni7l4.0.0.0; KLBRSID=0a401b23e8a71b70de2f4b37f5b4e379|1679625364|1679625360; SESSIONID=PwMlHDmiLClWxc1FgG6JrPu3B7gjH1LTomeC8pSG5Iu; JOID=UV0dAUolQ3Xh21anPiRnqeUwwBogczkyjpEUx11rBhS8oDeQdbpIYYTfXaA3hg0zFsgeQezw0yBY6SQ4BS72LbQ=; osd=VVAdB00hTnXn3FKqPiJgregwxh0kfjk0iZUZx1tsAhm8pjCUeLpOZoDSXaYwggAzEM8aTOz21CRV6SI_ASP2K7M=; Hm_lpvt_98beee57fd2ef70ccdd5ca52b9740c49=1679625367");
                        var response = await client.GetAsync("https://www.zhihu.com/hot");
                        var html = await response.Content.ReadAsStringAsync();
                        var doc = new HtmlDocument();
                        doc.LoadHtml(html);
                        var titleNodes = doc.DocumentNode.SelectNodes("//*[@id=\"TopstoryContent\"]/div/div/div[1]/section/a");

                        var optionsProgressBar = new ProgressBarOptions
                        {
                            ForegroundColor = ConsoleColor.Yellow,
                            ForegroundColorDone = ConsoleColor.DarkGreen,
                            BackgroundColor = ConsoleColor.DarkGray,
                            BackgroundCharacter = '\u2593'
                        };

                        using (var pbar = new ProgressBar(titleNodes.Count, $"爬虫", optionsProgressBar))
                        {
                            foreach (var titleNode in titleNodes)
                            {
                                pbar.Tick();
                                // 判断是否存在，存在则更新，不存在则插入
                                await _sqlSugarClient.Storageable(new ZhihuHot
                                {
                                    TitleLink = titleNode.Attributes[1].Value,
                                    Title = titleNode.Attributes[2].Value,
                                }).WhereColumns(hot => hot.Title).ExecuteCommandAsync();
                            }
                        }
                        console.WithColors(ConsoleColor.White, ConsoleColor.Black);
                        await console.Output.WriteLineAsync("爬虫结束 ....");
                    }
                    catch (Exception e)
                    {
                        await console.Output.WriteLineAsync(e.Message);
                    }
                    break;
                default:
                    goto case CrawlSupportFunc.zhihuhot;
            }
        }
    }

    public enum CrawlSupportFunc
    {
        zhihuhot
    }
}
