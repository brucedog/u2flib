namespace DemoU2FSite.Repository
{
    public class AuthenticationRequest
    {
        public string Version { get; private set; }

        public string KeyHandle { get; private set; }

        public string Challenge { get; private set; }

        public string AppId { get; private set; }
    }
}