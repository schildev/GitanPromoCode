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
    public class gitanPromoCode : RocketPlugin<gitanPromoConfiguration>
    {
        public static gitanPromoCode Instance { get; private set; }
        public promoDatabase promoDatabase { get; private set; }
        public promoSaveService promoSaveService { get; private set; }

        public Dictionary<string, string> userCodes { get; set; }
        protected override void Load()
        {
            Instance = this;
            userCodes = new Dictionary<String, String>() { };
            promoDatabase = new promoDatabase();
            promoDatabase.Reload();
            promoSaveService = gameObject.AddComponent<promoSaveService>();
            EffectManager.onEffectButtonClicked += btnFun;
            EffectManager.onEffectTextCommitted += getText;
            EffectManager.onEffectButtonClicked += closeUi;
            Logger.Log("GitanPromo ON !");
        }
        public void getText(Player player, string key, string text)
        {
            UnturnedPlayer unturPlayer = UnturnedPlayer.FromPlayer(player);
            if (key == "inputCode")
            {
                string userID = unturPlayer.SteamPlayer().playerID.steamID.ToString();
                if (userCodes.ContainsKey(userID))
                {
                    userCodes[userID] = text;
                }
                else userCodes.Add(userID, text);
            }
        }
        public void btnFun(Player player, string key)
        {
            UnturnedPlayer calUntur = UnturnedPlayer.FromPlayer(player);
            if (key == "sendCodeButton")
            {
                
                bool codeGetable = userCodes.TryGetValue(calUntur.SteamPlayer().playerID.steamID.ToString(), out string code);
                if (!codeGetable)
                {
                    EffectManager.sendUIEffectText(Configuration.Instance.keyID, calUntur.SteamPlayer().transportConnection, true, "codeStatus", gitanPromoCode.Instance.Translate("CodeNotAvailable"));
                    return;
                }
                promoModel promo = gitanPromoCode.Instance.promoSaveService.GetPromoCode(code);
                if (promo == null | gitanPromoCode.Instance.promoSaveService.hasUserClaimed(code, calUntur.SteamPlayer().playerID.steamID.ToString()))
                {
                    EffectManager.sendUIEffectText(Configuration.Instance.keyID, calUntur.SteamPlayer().transportConnection, true, "codeStatus", gitanPromoCode.Instance.Translate("CodeNotAvailable"));
                    return;
                }
                if (promo.isUnique & promo.claimed.Count > 0)
                {
                    EffectManager.sendUIEffectText(Configuration.Instance.keyID, calUntur.SteamPlayer().transportConnection, true, "codeStatus", gitanPromoCode.Instance.Translate("UniqueException"));
                }
                foreach (itemModel item in promo.items)
                {
                    if (item.type == "Xp")
                    {
                        calUntur.Experience += (uint)item.quantity;
                    }
                    if (item.type == "Item")
                    {
                        calUntur.GiveItem(item.itemId, (byte)item.quantity);
                    }
                    if (item.type == "Vehicle")
                    {
                        calUntur.GiveVehicle(item.itemId);
                    }
                    if (Configuration.Instance.useUconomy)
                    {
                        if (item.type == "Uconomy")
                        {
                            RocketPlugin.ExecuteDependencyCode("Uconomy", (IRocketPlugin plugin) =>
                            {
                                Uconomy.Instance.Database.IncreaseBalance(calUntur.CSteamID.ToString(), decimal.Parse(item.quantity.ToString()));
                            });
                        }
                    }
                }
                gitanPromoCode.Instance.promoSaveService.addClaim(code, calUntur.SteamPlayer().playerID.steamID.ToString());
                EffectManager.sendUIEffectText(Configuration.Instance.keyID, calUntur.SteamPlayer().transportConnection, true, "codeStatus", "<color=#bab86c>" + gitanPromoCode.Instance.Translate("claimSuccess") + "</color>");
            }
        }
        public void closeUi(Player player, string key)
        {
            if (key == "closePromo")
            {
                UnturnedPlayer calUntur = UnturnedPlayer.FromPlayer(player);
                EffectManager.askEffectClearByID(gitanPromoCode.Instance.Configuration.Instance.effectID, calUntur.SteamPlayer().transportConnection);
                player.disablePluginWidgetFlag(EPluginWidgetFlags.Modal);
            }
        }
        protected override void Unload()
        {
            Logger.Log("GitapnPromo OFF !");
            EffectManager.onEffectButtonClicked -= closeUi;
            EffectManager.onEffectTextCommitted -= getText;
            EffectManager.onEffectButtonClicked -= btnFun;
            Destroy(promoSaveService);
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
