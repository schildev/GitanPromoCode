using GitanPromoCode.Extensions;
using Rocket.API;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GitanPromoCode.commands
{
    public enum CommandArguments {
        Create,
        Delete,
        Show,
        Item,
        Xp,
        Vehicle,
        Uconomy
    }
    public class CreatePromo : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "promoManage";

        public string Help => "Create a promo code for your serv, if you don't give a promocode value a random one will be generated, you have to set the rewards after creation";

        public string Syntax => "/promoManage <create|delete|show> <code|code|null> <unique|null|null>";

        public List<string> Aliases => [];

        public List<string> Permissions => ["promoManage"];

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (!caller.HasPermission("promoManage"))
            {
                caller.SayToCallerError("NotPerms");
                return;
            }
            var arguments = Enum.TryParse<CommandArguments>(command.ElementAtOrDefault(0), true, out var argument);
            string code;
            if (arguments)
            {
                switch (argument)
                {
                    case CommandArguments.Create:
                        bool isUnique = command.ElementAtOrDefault(2) == "unique";
                        if (command.Length < 2)
                        {
                            var bytes = new byte[4];
                            var rng = RandomNumberGenerator.Create();
                            rng.GetBytes(bytes);
                            uint random = BitConverter.ToUInt32(bytes, 0) % 100000000;
                            code = String.Format("{0:D8}", random);
                        }
                        else
                            code = command[1];
                        if (GitanPromoCode.Instance.Database.AddPromoDatabase(new(code, isUnique)))
                            caller.SayToCallerSuccess("createPromoCodeSucceed");
                        else
                            caller.SayToCallerError("createPromoCodeNotSucceed");
                        break;
                    case CommandArguments.Delete:
                        code = command[1];
                        bool del = GitanPromoCode.Instance.Database.RemovePromoDatabase(code);
                        if (del) 
                            caller.SayToCallerSuccess("delPromoCodeSucceed");
                        else 
                            caller.SayToCallerError("delPromoCodeNotSucceed");
                        break;
                    case CommandArguments.Show:
                        List<PromoModel> codes = GitanPromoCode.Instance.Database.GetCodes();
                        if (codes.Count == 0) {
                            caller.SayToCallerError("NoCodes");
                            break;
                        }
                        foreach (PromoModel promo in codes)
                            UnturnedChat.Say(caller, $"{promo.Code} -- {promo.Items.Count} rewards");
                        break;
                }
            } else caller.SayToCallerError("argumentNotFound");
            
        }
    }
}
