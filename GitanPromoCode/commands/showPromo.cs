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
    public class showPromo : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "showPromo";

        public string Help => "Affiche une UI de promotion d'ID 29863.";

        public string Syntax => "/showPromo";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "showPromo"};
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (!caller.HasPermission("showPromo"))
            {
                UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("NotPerms"), UnityEngine.Color.red);
                return;
            }

            UnturnedPlayer uPlayer = (UnturnedPlayer)caller;
            Player player = uPlayer.Player;
            player.enablePluginWidgetFlag(EPluginWidgetFlags.Modal);
            EffectManager.sendUIEffect(gitanPromoCode.Instance.Configuration.Instance.effectID, gitanPromoCode.Instance.Configuration.Instance.keyID, uPlayer.SteamPlayer().transportConnection, true);
            EffectManager.sendUIEffectText(gitanPromoCode.Instance.Configuration.Instance.keyID, uPlayer.SteamPlayer().transportConnection, true, "titleCode", gitanPromoCode.Instance.Translate("UiTitle"));
            EffectManager.sendUIEffectText(gitanPromoCode.Instance.Configuration.Instance.keyID, uPlayer.SteamPlayer().transportConnection, true, "buttonText", gitanPromoCode.Instance.Translate("buttonText"));

        }
    }
}
