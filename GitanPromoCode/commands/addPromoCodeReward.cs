using Rocket.API;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitanPromoCode.commands
{
    public class addPromoCodeReward : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "addPromoReward";

        public string Help => "add a reward to your promo code";

        public string Syntax => "/addPromoReward <code> <item|xp|vehicle|uconomy> <itemId|ammount|vehicleId|ammount> <quantity|null|null>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "promoManageReward" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (!caller.HasPermission("promoManageReward"))
            {
                UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("NotPerms"), UnityEngine.Color.red);
                return;
            }
            var arguments = Enum.TryParse<commandArguments>(command.ElementAtOrDefault(1), true, out var argument);

            if (!arguments) { UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("argumentNotFound"), UnityEngine.Color.red); return; }
            ushort itemId;
            string added;
            switch (argument)
            {
                case commandArguments.Item:
                    bool isGoodItemId = ushort.TryParse(command[2], out itemId);
                    bool goodItemQuantity = Int32.TryParse(command[3], out int quantity);
                    if (!isGoodItemId){ UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("invalidID"), UnityEngine.Color.red);return; }
                    if (!goodItemQuantity) { UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("invalidQuantity"), UnityEngine.Color.red);return; }
                    added = gitanPromoCode.Instance.promoSaveService.addRewardToPromoCode(command[0], commandArguments.Item, itemId, quantity);
                    if (added == null) UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("noneRewardAdded"), UnityEngine.Color.red);
                    else UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("RewardAdded"), UnityEngine.Color.green);                    
                    break;

                case commandArguments.Vehicle:
                    bool isGoodVehicleId = ushort.TryParse(command[2], out itemId);
                    if (!isGoodVehicleId) { UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("invalidID"), UnityEngine.Color.red); return; }
                    added = gitanPromoCode.Instance.promoSaveService.addRewardToPromoCode(command[0], commandArguments.Vehicle, itemId);
                    if (added == null) UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("noneRewardAdded"), UnityEngine.Color.red);
                    else UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("RewardAdded"), UnityEngine.Color.green);
                    break;

                case commandArguments.Xp:
                    bool isGoodXPAmmount = int.TryParse(command[2], out int xp);
                    if (!isGoodXPAmmount) { UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("notGoodXPAmmount"), UnityEngine.Color.red); return; }
                    added = gitanPromoCode.Instance.promoSaveService.addRewardToPromoCode(command[0], commandArguments.Xp, 0, xp);
                    if (added == null) UnturnedChat.Say(caller,gitanPromoCode.Instance.Translate("noneRewardAdded"), UnityEngine.Color.red);
                    else UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("RewardAdded"), UnityEngine.Color.green);
                    break;
                case commandArguments.Uconomy:
                    if (gitanPromoCode.Instance.Configuration.Instance.useUconomy)
                    {
                        bool isGoodUconomyAmmount = int.TryParse(command[2], out int balance);
                        if (!isGoodUconomyAmmount) { UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("notGoodXPAmmount"), UnityEngine.Color.red); return; }
                        added = gitanPromoCode.Instance.promoSaveService.addRewardToPromoCode(command[0], commandArguments.Uconomy, 0, balance);
                        if (added == null) UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("noneRewardAdded"), UnityEngine.Color.red);
                        else UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("RewardAdded"), UnityEngine.Color.green);
                    }
                    else UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("enableUconomyException"), UnityEngine.Color.red);
                    break;

            }
        }
    }
}
