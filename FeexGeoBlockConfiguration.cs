using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Freenex.FeexGeoBlock
{
    public sealed class Whitelist
    {
        [XmlAttribute("CountryCode")]
        public string WhitelistCountry;

        public Whitelist(string countryCode)
        {
            WhitelistCountry = countryCode;
        }
        public Whitelist()
        {
            WhitelistCountry = string.Empty;
        }
    }

    public class FeexGeoBlockConfiguration : IRocketPluginConfiguration
    {
        public string RejectionReason;
        public bool Logging;
        public bool EnableWhitelist;
        public bool EnableBlacklist;
        [XmlArrayItem(ElementName = "CountryCode")]
        public List<string> Whitelist;
        [XmlArrayItem(ElementName = "CountryCode")]
        public List<string> Blacklist;

        public void LoadDefaults()
        {
            RejectionReason = "WHITELISTED";
            Logging = true;
            EnableWhitelist = true;
            EnableBlacklist = false;
            Whitelist = new List<string>(new string[] { "DE" });
            Blacklist = new List<string>(new string[] { "--" });
        }
    }
}