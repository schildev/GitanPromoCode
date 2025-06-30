using GitanPromoCode.Extensions;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitanPromoCode.commands
{
    public class ShowPromo : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "showPromo";

        public string Help => "Affiche une UI de promotion d'ID 29863.";

        public string Syntax => "/showPromo";

        public List<string> Aliases => [];

        public List<string> Permissions => ["showPromo"];
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (!caller.HasPermission("showPromo"))
            {
                caller.SayToCallerError("NotPerms");
                return;
            }

            UnturnedPlayer uPlayer = (UnturnedPlayer)caller;
            Player player = uPlayer.Player;
            player.enablePluginWidgetFlag(EPluginWidgetFlags.Modal);
            EffectManager.sendUIEffect(GitanPromoCode.Instance.Configuration.Instance.effectID, GitanPromoCode.Instance.Configuration.Instance.keyID, uPlayer.SteamPlayer().transportConnection, true);
            EffectManager.sendUIEffectVisibility(GitanPromoCode.Instance.Configuration.Instance.keyID, player.channel.owner.transportConnection, true, "Canvas/Panel", true);
            EffectManager.sendUIEffectText(GitanPromoCode.Instance.Configuration.Instance.keyID, uPlayer.SteamPlayer().transportConnection, true, "titleCode", GitanPromoCode.Instance.Translate("UiTitle"));
            EffectManager.sendUIEffectText(GitanPromoCode.Instance.Configuration.Instance.keyID, uPlayer.SteamPlayer().transportConnection, true, "buttonText", GitanPromoCode.Instance.Translate("buttonText"));

        }
    }
}
