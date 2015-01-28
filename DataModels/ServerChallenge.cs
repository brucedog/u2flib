namespace DataModels
{
    public class ServerChallenge
    {
        public string AppId { get; set; }
        public string KeyHandle { get; set; }
        public string Version { get; set; }
        public string Challenge { get; set; }
    }
}