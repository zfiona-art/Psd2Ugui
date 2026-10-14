
namespace GameConfig.Network
{
    public class NetworkConfigLoader : Singleton<NetworkConfigLoader>
    {

        private string devSerDomain = "https://test-api-br.brickchallenge.site";

        public string DevSerDomain
        {
            get { return devSerDomain; }
            set { devSerDomain = value; }
        }

        private string proServerDomain = "https://api-br.brickchallenge.site";

        public string ProServerDomain
        {
            get { return proServerDomain; }
            set { proServerDomain = value; }
        }

        private string logDevDomain = "http://47.74.180.115:8009";

        public string LogDevSerDomain
        {
            get { return logDevDomain; }
            set { logDevDomain = value; }
        }

        private string logProdDomain = "https://l.brickchallenge.site";
        public string LogProdDomain
        {
            get { return logProdDomain; }
            set { logProdDomain = value; }
        }


        public byte MaxConnectionPerServer
        {
            get
            {
                return 10;
            }
        }

        public int RequestTimeout
        {
            get
            {
                return 10;
            }
        }

        public int ConnectionTimeout
        {
            get
            {
                return 10;
            }
        }


        public string GetServerDomain()
        {
            if (PlatformHelper.Platform.IsProd())
                return ProServerDomain;
            return DevSerDomain;
        }

        public string GetLogServerDomain()
        {
            if (PlatformHelper.Platform.IsProd())
                return LogProdDomain;
            return LogDevSerDomain;
        }

        byte[] enBytes;
        public byte[] GetEnBytes()
        {
            if (enBytes == null)
                enBytes = PlatformHelper.Platform.IsProd() ? System.Text.Encoding.UTF8.GetBytes("IgnalX71IEf456PT") : System.Text.Encoding.UTF8.GetBytes("IgkibX71IEf456PT");
            return enBytes;
        }

        byte[] deBytes;
        public byte[] GetDeBytes()
        {
            if (deBytes == null)
                deBytes = PlatformHelper.Platform.IsProd() ? System.Text.Encoding.UTF8.GetBytes("IgmatX71IEf456PT") : System.Text.Encoding.UTF8.GetBytes("IgkibX71IEf456PT");
            return deBytes;
        }


        public string Ping
        {
            get { return GetServerDomain() + "/api/com-api/ping"; }
        }

        public string feedback
        {
            get { return GetServerDomain() + "/api/v1/user/feedback"; }
        }
    }
}