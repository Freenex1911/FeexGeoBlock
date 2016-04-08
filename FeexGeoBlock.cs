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

            if (!System.IO.File.Exists(Configuration.Instance.DatabasePath))
            {
                Logger.LogError("FeexGeoBlock >> Could not find file \"" + System.IO.Path.GetFullPath(Configuration.Instance.DatabasePath) + "\".");
                UnloadPlugin();
            }
        }

        protected override void Unload()
        {
            UnturnedPermissions.OnJoinRequested -= UnturnedPermissions_OnJoinRequested;
            Logger.Log("Freenex's FeexGeoBlock has been unloaded!");
        }

        private void UnturnedPermissions_OnJoinRequested(CSteamID player, ref ESteamRejection? rejectionReason)
        {
            if (Configuration.Instance.Bypass.Contains(player.ToString()))
            {
                if (Configuration.Instance.Logging) { Logger.LogWarning("Access granted: " + player + " // Reason: Bypass."); }
                return;
            }

            P2PSessionState_t sessionStateP2P;
            SteamGameServerNetworking.GetP2PSessionState(player, out sessionStateP2P);

            LookupService playerLookup = new LookupService(Configuration.Instance.DatabasePath, LookupService.GEOIP_MEMORY_CACHE);
            Country playerCountry = playerLookup.getCountry(Parser.getIPFromUInt32(sessionStateP2P.m_nRemoteIP));

            bool grantAccess = false;

            if (Configuration.Instance.EnableWhitelist)
            {
                if (Configuration.Instance.Whitelist.Contains(playerCountry.getCode())) { grantAccess = true; }
            }
            if (Configuration.Instance.EnableBlacklist)
            {
                if (!Configuration.Instance.Blacklist.Contains(playerCountry.getCode())) { grantAccess = true; }
            }

            if (!grantAccess)
            {
                if (Configuration.Instance.Logging) { Logger.LogWarning("Access denied: " + player + " // Country: " + playerCountry.getCode()); }
                rejectionReason = GetSteamRejection();
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
