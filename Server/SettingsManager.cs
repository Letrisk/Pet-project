namespace Server
{
    using System.Xml.Serialization;
    using System.IO;
    using System.Net;
    using System.Configuration;

    public class SettingsManager
    {
        #region Fields

        private string _transport;

        private int _port;
        private long _timeout;

        private IPAddress _ip;

        private ConnectionStringSettings _connectionSettings;

        #endregion Fields

        #region Properties

        public string Transport
        {
            get => _transport;
            set => _transport = value;
        }

        public int Port
        {
            get => _port;
            set => _port = value;
        }

        public long Timeout
        {
            get => _timeout;
            set => _timeout = value;
        }

        public IPAddress Ip
        {
            get => _ip;
            set => _ip = value;
        }

        public ConnectionStringSettings ConnectionSettings
        {
            get => _connectionSettings;
            set => _connectionSettings = value;
        }

        #endregion Properties

        #region Constructors

        public SettingsManager(string fileName) 
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                Settings settings = (Settings)serializer.Deserialize(fileStream);

                Transport = settings.Transport;
                Ip = IPAddress.Parse(settings.Ip);
                Port = settings.Port;
                Timeout = settings.Timeout;
                ConnectionSettings = new ConnectionStringSettings(settings.DbName, settings.ConnectionString, settings.ProviderName);
            }
        }

        #endregion Constructors
    }
}
