namespace AutoRpa.Configs
{
    public class AutoRpaRoot
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public ChatGpt ChatGpt { get; set; }
    }

    public class ConnectionStrings
    {
        public string DbType { get; set; }
        public string DbServer { get; set; }
        public string UseRedis { get; set; }
        public string RedisServer { get; set; }
    }

    public class ChatGpt
    {
        public string Model { get; set; }
        public int OutTime { get; set; }
        public string SecretKey { get; set; }
        public bool IsUseSqlStorage { get; set; }
        public bool IsUseFileStorage { get; set; }
        public string TitleHeader { get; set; }
        public string TitlePath { get; set; }
        public string TitleType { get; set; }
    }
}
