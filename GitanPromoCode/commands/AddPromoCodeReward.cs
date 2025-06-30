using GitanPromoCode.Extensions;
using Rocket.API;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitanPromoCode.commands
{
    public class AddPromoCodeReward : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "addPromoReward";

        public string Help => "add a reward to your promo code";

        public string Syntax => "/addPromoReward <code> <item|xp|vehicle|uconomy> <itemId|ammount|vehicleId|ammount> <quantity|null|null>";

        public List<string> Aliases => [];

        public List<string> Permissions => ["promoManageReward"];

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (!caller.HasPermission("promoManageReward"))
            {
                caller.SayToCallerError("NotPerms"); return;
            }
            var arguments = Enum.TryParse<CommandArguments>(command.ElementAtOrDefault(1), true, out var argument);

            if (!arguments) {
                caller.SayToCallerError("argumentNotFound"); return;
            }
            ushort itemId;
            bool added = false;
            switch (argument)
            {
                case CommandArguments.Item:
                    if (!ushort.TryParse(command[2], out itemId)){ 
                        caller.SayToCallerError("invalidID");
                        return;
                    }
                    if (!Int32.TryParse(command[3], out int quantity)) { 
                        caller.SayToCallerError("invalidQuantity"); 
                        return;
                    }

                    added = GitanPromoCode.Instance.Database.AddReward(command[0], new(argument, itemId, quantity));
                    break;

                case CommandArguments.Vehicle:
                    if (!ushort.TryParse(command[2], out itemId))
                    {
                        caller.SayToCallerError("invalidID");
                        return;
                    }
                    added = GitanPromoCode.Instance.Database.AddReward(command[0], new(argument, itemId, 1));
                    break;

                case CommandArguments.Xp:                    
                    if (!int.TryParse(command[2], out int xp)) { 
                        caller.SayToCallerError("notGoodXPAmmount");
                        return; 
                    }
                    added = GitanPromoCode.Instance.Database.AddReward(command[0], new(argument, 0, xp));
                    break;
                case CommandArguments.Uconomy:
                    if (!GitanPromoCode.Instance.Configuration.Instance.useUconomy)
                    {
                        caller.SayToCallerError("enableUconomyException");
                        return;
                    }
                    
                    if (int.TryParse(command[2], out int balance)) { 
                        caller.SayToCallerError("notGoodXPAmmount"); return;
                    }
                    added = GitanPromoCode.Instance.Database.AddReward(command[0], new(argument, 0, balance));
                    break;
                default:
                    added = false;
                    break;
            }
            if(!added)
            {
                caller.SayToCallerError("noneRewardAdded");
                return;
            }
            caller.SayToCallerSuccess("RewardAdded");
        }
    }
}
