using GitanPromoCode.commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GitanPromoCode
{
    public class promoSaveService : MonoBehaviour
    {
        private promoDatabase database => gitanPromoCode.Instance.promoDatabase;
        void Awake()
        {

        }
        void Start()
        {

        }
        void OnDestroy()
        {

        }
        public bool hasUserClaimed(string code, string playerId)
        {
            return database.AlreadyClaimed(playerId, code);
        }
        public void addClaim(string code, string playerId)
        {
            database.addClaim(playerId, code);
        }
        public string RegisterPromoCode(string code, bool isUnique, uint XP = 0, List<itemModel> items = null)
        {
            if (items == null) items = new List<itemModel>();
            var promoSaved = new promoModel()
            {
                code = code,
                items = items,
                claimed = new List<string>() { },
                isUnique = isUnique
            };
            if (database.Data.FirstOrDefault(x => x.code == code) == null)
            {
                database.AddPromoDatabase(promoSaved);
                return "good";
            }
            else return null;
        }
        public promoModel GetPromoCode(string code)
        {
            if (database.Data.FirstOrDefault(x => x.code == code) != null)
            {
                promoModel promo = database.getPromo(code);
                return promo;
            }
            else
            {
                return null;
            }
        }
        public string RemovePromoCode(string code)
        {
            return database.RemovePromoDatabase(code);
        }
        public string addRewardToPromoCode(string code, commandArguments type, ushort id, int quantity = 1)
        {
            if (database.Data.FirstOrDefault(x => x.code == code) != null)
            {
                itemModel reward;
                switch (type)
                {
                    case commandArguments.Item:
                        reward = new itemModel()
                        {
                            type = type.ToString(),
                            quantity = quantity,
                            itemId = id
                        };
                        break;
                    case commandArguments.Xp:
                        reward = new itemModel()
                        {
                            type = type.ToString(),
                            quantity = quantity,
                            itemId = 1
                        };
                        break;
                    case commandArguments.Vehicle:
                        reward = new itemModel()
                        {
                            type = type.ToString(),
                            itemId = id,
                            quantity = 1,
                        };
                        break;
                    case commandArguments.Uconomy:
                        reward = new itemModel()
                        {
                            type = type.ToString(),
                            itemId = 1,
                            quantity = quantity
                        };
                        break;
                    default:
                        reward = null;
                        break;
                }
                if (reward != null) { database.addReward(code, reward); return "good"; }
                else { return null; }
            }
            return null;
        }

    }
}
