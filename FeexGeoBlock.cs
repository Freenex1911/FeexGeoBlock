using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Permissions;
using SDG.Unturned;
using Steamworks;
using System;

namespace Freenex.FeexGeoBlock
{
    public class FeexGeoBlock : RocketPlugin<FeexGeoBlockConfiguration>
    {
        public static FeexGeoBlock Instance;

        protected override void Load()
        {
            Instance = this;
            UnturnedPermissions.OnJoinRequested += UnturnedPermissions_OnJoinRequested;
            Logger.Log("Freenex's FeexGeoBlock has been loaded!");
        }

        protected override void Unload()
        {
            UnturnedPermissions.OnJoinRequested -= UnturnedPermissions_OnJoinRequested;
            Logger.Log("Freenex's FeexGeoBlock has been unloaded!");
        }

        private void UnturnedPermissions_OnJoinRequested(CSteamID player, ref ESteamRejection? rejectionReason)
        {
            P2PSessionState_t sessionStateP2P;
            SteamGameServerNetworking.GetP2PSessionState(player, out sessionStateP2P);

            LookupService playerLookup = new LookupService(".\\Libraries\\GeoIP.dat", LookupService.GEOIP_MEMORY_CACHE);
            Country playerCountry = playerLookup.getCountry(Parser.getIPFromUInt32(sessionStateP2P.m_nRemoteIP));
            
            if (playerCountry.getCode() == "--")
            {
                if (Configuration.Instance.KickNoCountry)
                {
                    if (Configuration.Instance.Logging) { Logger.LogWarning("Access denied: " + player + " // Reason: No country."); }
                    rejectionReason = GetSteamRejection();
                }
                else
                {
                    if (Configuration.Instance.Logging) { Logger.LogWarning("Access granted: " + player + " // Reason: No country."); }
                }
            }
            else
            {
                bool grantAccess = false;
                for (int i = 0; i < Configuration.Instance.Whitelist.Length; i++)
                {
                    if (Configuration.Instance.Whitelist[i].WhitelistCountry == playerCountry.getCode())
                    {
                        grantAccess = true;
                    }
                }

                if (!grantAccess)
                {
                    if (Configuration.Instance.Logging) { Logger.LogWarning("Access denied: " + player + " // Reason: Wrong country (" + playerCountry.getCode() + ")."); }
                    rejectionReason = GetSteamRejection();
                }
            }

        }

        public ESteamRejection GetSteamRejection()
        {
            var RejectionReason = ESteamRejection.WHITELISTED;

            try
            {
                RejectionReason = (ESteamRejection)Enum.Parse(typeof(ESteamRejection), Configuration.Instance.RejectionReason, true);
            }
            catch { }

            return RejectionReason;
        }
    }
}
