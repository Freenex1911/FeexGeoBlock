using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Freenex.FeexGeoBlock
{
    public class FeexGeoBlockConfiguration : IRocketPluginConfiguration
    {
        public string RejectionReason;
        public string DatabasePath;
        public bool Logging;
        [XmlArrayItem(ElementName = "SteamID")]
        public List<string> Bypass;
        public bool EnableWhitelist;
        public bool EnableBlacklist;
        [XmlArrayItem(ElementName = "CountryCode")]
        public List<string> Whitelist;
        [XmlArrayItem(ElementName = "CountryCode")]
        public List<string> Blacklist;

        public void LoadDefaults()
        {
            RejectionReason = "WHITELISTED";
            DatabasePath = ".\\Libraries\\GeoIP.dat";
            Logging = true;
            EnableWhitelist = true;
            EnableBlacklist = false;
            Bypass = new List<string>(new string[] { "76561198187138313" });
            Whitelist = new List<string>(new string[] { "DE" });
            Blacklist = new List<string>(new string[] { "--" });
        }
    }
}