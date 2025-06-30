using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fr34kyn01535.Uconomy;
using GitanPromoCode.commands;
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace GitanPromoCode
{
    public class ItemModel
    {
        public ItemModel()
        {

        }
        public ItemModel(CommandArguments type, ushort itemId, int quantity)
        {
            Type = type.ToString();
            Quantity = quantity;
            ItemId = type == CommandArguments.Item ? itemId : (ushort)1;
        }

        public string Type { get; set; }
        public ushort ItemId { get; set; }
        public int Quantity { get; set; }

        public void GiveReward(Player player)
        {
            var uPlayer = UnturnedPlayer.FromPlayer(player);
            if (Type == "Xp")
                uPlayer.Experience += (uint)Quantity;
            if (Type == "Item")
                uPlayer.GiveItem(ItemId, (byte)Quantity);
            if (Type == "Vehicle")
                uPlayer.GiveVehicle(ItemId);

            if (GitanPromoCode.Instance.Configuration.Instance.useUconomy)
            {
                if (Type == "Uconomy")
                {
                    RocketPlugin.ExecuteDependencyCode("Uconomy", (IRocketPlugin plugin) =>
                    {
                        Uconomy.Instance.Database.IncreaseBalance(uPlayer.CSteamID.ToString(), decimal.Parse(Quantity.ToString()));
                    });
                }
            }

        }
    }
    public class PromoModel
    {
        public string Code { get; set; }
        public List<ItemModel> Items { get; set; } = [];

        public List<string> Claimed { get; set; } = [];
        public bool IsUnique { get; set; }
        public PromoModel()
        {

        }
        public PromoModel(string code, bool isUnique)
        {
            Code = code;
            IsUnique = isUnique;
        }
    }
}
