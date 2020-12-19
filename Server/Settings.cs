namespace Server
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot(ElementName = "configuration")]
    public class Settings
    {
        #region Properties

        [XmlElement("transport")]
        public string Transport { get; set; }

        [XmlElement("ip")]
        public string Ip { get; set; }

        [XmlElement("port")]
        public int Port { get; set; }

        [XmlElement("timeout")]
        public long Timeout { get; set; }

        [XmlElement("dbName")]
        public string DbName { get; set; }

        [XmlElement("connectionString")]
        public string ConnectionString { get; set; }

        [XmlElement("providerName")]
        public string ProviderName { get; set; }

        #endregion Properties
    }
}
