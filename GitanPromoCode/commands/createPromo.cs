using Rocket.API;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GitanPromoCode.commands
{
    public enum commandArguments {
        Create,
        Delete,
        Show,
        Item,
        Xp,
        Vehicle,
        Uconomy
    }
    public class createPromo : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "promoManage";

        public string Help => "Create a promo code for your serv, if you don't give a promocode value a random one will be generated, you have to set the rewards after creation";

        public string Syntax => "/promoManage <create|delete|show> <code|null|null> <unique|null|null>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "promoManage" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (!caller.HasPermission("promoManage"))
            {
                UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("NotPerms"), UnityEngine.Color.red);
                return;
            }
            var arguments = Enum.TryParse<commandArguments>(command.ElementAtOrDefault(0), true, out var argument);
            string code;
            if (arguments)
            {
                switch (argument)
                {
                    case commandArguments.Create:
                        bool isUnique = false;
                        if (command.Length < 1)
                        {
                            var bytes = new byte[4];
                            var rng = RandomNumberGenerator.Create();
                            rng.GetBytes(bytes);
                            uint random = BitConverter.ToUInt32(bytes, 0) % 100000000;
                            code = String.Format("{0:D8}", random);
                            if (command.Length > 1)
                            {
                                if (command[2] == "unique") {  isUnique = true; }
                            }
                        }
                        else
                        {
                            code = command[1];
                        }
                        string add = gitanPromoCode.Instance.promoSaveService.RegisterPromoCode(code, isUnique);
                        if (add != null) UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("createPromoCodeSucceed"), UnityEngine.Color.green);
                        else UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("createPromoCodeNotSucceed"), UnityEngine.Color.red);
                        break;
                    case commandArguments.Delete:
                        code = command[1];
                        string del = gitanPromoCode.Instance.promoSaveService.RemovePromoCode(code);
                        if (del != null) UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("delPromoCodeSucceed"), UnityEngine.Color.green);
                        else UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("delPromoCodeNotSucceed"), UnityEngine.Color.red);
                        break;
                    case commandArguments.Show:
                        List < promoModel > codes = gitanPromoCode.Instance.promoSaveService.getCodes();
                        if (codes == null) {UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("NoCodes")); break; }
                        foreach (promoModel promo in codes)
                        {
                            UnturnedChat.Say(caller, $"{promo.code} -- {promo.items.Count} rewards");
                        }
                        break;
                }
            } else UnturnedChat.Say(caller, gitanPromoCode.Instance.Translate("argumentNotFound"), UnityEngine.Color.red);
            
        }
    }
}
