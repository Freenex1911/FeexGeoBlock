using Rocket.API;
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
        public bool KickOnNoCountry;
        public bool Logging;

        [XmlArrayItem("WhitelistCountryCode")]
        [XmlArray(ElementName = "Whitelist")]
        public Whitelist[] Whitelist;

        public void LoadDefaults()
        {
            RejectionReason = "WHITELISTED";
            KickOnNoCountry = true;
            Logging = true;

            Whitelist = new Whitelist[]{
                new Whitelist("DE")
            };
        }
    }
}