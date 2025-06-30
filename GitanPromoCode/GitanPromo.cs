using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using fr34kyn01535.Uconomy;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.Unturned.Chat;

namespace GitanPromoCode
{
    public class GitanPromoCode : RocketPlugin<GitanPromoConfiguration>
    {
        public static GitanPromoCode Instance { get; private set; }
        public PromoDatabase Database { get; private set; }

        public Dictionary<string, string> UserCodes { get; set; }
        protected override void Load()
        {
            Instance = this;
            UserCodes = [];
            Database = new();
            EffectManager.onEffectButtonClicked += ReclaimCode;
            EffectManager.onEffectTextCommitted += GetTextOnInput;
            EffectManager.onEffectButtonClicked += CloseUi;
            WorkshopDownloadConfig.getOrLoad().File_IDs.Add(3440793037UL);
            Logger.Log("GitanPromo ON !");
        }
        protected override void Unload()
        {
            Logger.Log("GitapnPromo OFF !");
            EffectManager.onEffectButtonClicked -= CloseUi;
            EffectManager.onEffectTextCommitted -= GetTextOnInput;
            EffectManager.onEffectButtonClicked -= ReclaimCode;
        }
        public void GetTextOnInput(Player player, string key, string text)
        {
            if (key != "inputCode")
                return;
            string userID = player.channel.owner.playerID.steamID.ToString();
            if (UserCodes.ContainsKey(userID))
            {
                UserCodes[userID] = text;
            }
            else UserCodes.Add(userID, text);
        }
        public void ReclaimCode(Player player, string key)
        {
            if (key == "sendCodeButton")
            {
                
                if (!UserCodes.TryGetValue(player.channel.owner.playerID.steamID.ToString(), out string code))
                {
                    EffectManager.sendUIEffectText(Configuration.Instance.keyID, player.channel.owner.transportConnection, true, "codeStatus", Translate("CodeNotAvailable"));
                    return;
                }
                if (Database.GetPromo(code) is not PromoModel promo || Database.AlreadyClaimed(player.channel.owner.playerID.steamID.ToString(), promo))
                {
                    EffectManager.sendUIEffectText(Configuration.Instance.keyID, player.channel.owner.transportConnection, true, "codeStatus", Translate("CodeNotAvailable"));
                    return;
                }

                if (promo.IsUnique && promo.Claimed.Count > 0)
                {
                    EffectManager.sendUIEffectText(Configuration.Instance.keyID, player.channel.owner.transportConnection, true, "codeStatus", Translate("UniqueException"));
                }
                
                foreach (ItemModel item in promo.Items)
                    item.GiveReward(player);

                Database.AddClaim(player.channel.owner.playerID.steamID.ToString(), code);
                EffectManager.sendUIEffectText(Configuration.Instance.keyID, player.channel.owner.transportConnection, true, "codeStatus", "<color=#bab86c>" + Translate("claimSuccess") + "</color>");
            }
        }
        public void CloseUi(Player player, string key)
        {
            if (key != "closePromo")
                return;
            EffectManager.askEffectClearByID(Configuration.Instance.effectID, player.channel.owner.transportConnection);
            player.disablePluginWidgetFlag(EPluginWidgetFlags.Modal);
        }
        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "NotPerms", "you don't have perms to do it !" },
            { "createPromoCodeSucceed", "The promo code has been successfully created !" },
            { "createPromoCodeNotSucceed", "There was an error in the creation of the promo code !" },
            { "delPromoCodeSucceed", "The deletion of the promo code has been successfull !" },
            { "delPromoCodeNotSucceed", "There was an error during the deletion of the promo code !" },
            { "argumentNotFound", "The argument you gived is not valid !" },
            { "CodeNotAvailable", "The promo code doesn't exist or you already claimed it !" },
            { "UniqueException", "This code is unique and has been already claimed !" },
            {"claimSuccess", "You have successfully claimed the code !" },
            {"invalidID", "The ID you gived is not correct !" },
            {"invalidQuantity", "The quantity you gived is incorrect !" },
            {"noneRewardAdded", "The reward could not been added to the promo code !" },
            {"RewardAdded", "The reward has been successfully added !" },
            {"incorrectXpAmmount", "The quantity of XP you gived is incorrect !" },
            {"UiTitle", "Reedeem promo code" },
            {"buttonText", "Reedeem Code" },
            {"enableUconomyException", "You have to enable Uconomy in the configuration !" },
            {"NoCodes", "You haven't create any code" }
        };
    }
}
