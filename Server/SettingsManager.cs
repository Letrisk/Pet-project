﻿namespace Server
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;
    using System.Net;
    using System.Configuration;

    public class SettingsManager
    {
        #region Fields

        private string _transport;

        private int _port, _timeout;

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

        public int Timeout
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
            XmlSerializer ser = new XmlSerializer(typeof(Settings));
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                Settings settings = (Settings)ser.Deserialize(fs);

                Transport = settings.Transport;
                Ip = IPAddress.Parse(settings.Ip);
                Port = settings.Port;
                Timeout = settings.Timeout;
                ConnectionSettings = new ConnectionStringSettings(settings.DbName, settings.ConnectionString, settings.ProviderName);
            }

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.ConnectionStrings.ConnectionStrings.Clear();
            config.ConnectionStrings.ConnectionStrings.Add(ConnectionSettings);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.ConnectionStrings.SectionInformation.Name);
        }

        #endregion Constructors
    }
}
