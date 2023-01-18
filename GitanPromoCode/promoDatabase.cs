using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitanPromoCode
{
    public class promoDatabase
    {
        private promoStorage<List<promoModel>> DataStorage { get; set; }

        public List<promoModel> Data { get; private set; }
        public promoDatabase()
        {

            DataStorage = new promoStorage<List<promoModel>>(gitanPromoCode.Instance.Directory, "gitanPromo.json");

        }
        public void Reload()
        {
            Data = DataStorage.Read();
            if (Data == null)
            {
                Data = new List<promoModel>();
                DataStorage.Save(Data);
            }
        }
        public void AddPromoDatabase(promoModel promo)
        {
            Data.Add(promo);
            DataStorage.Save(Data);
        }

        public string RemovePromoDatabase(string code)
        {
            if (Data.FirstOrDefault(x => x.code == code) != null)
            {
                promoModel promoDestroy = Data.FirstOrDefault(x => x.code == code);
                Data.Remove(promoDestroy);
                DataStorage.Save(Data);
                return "good";
            }
            else
            {
                return null;
            }
        }
        public promoModel getPromo(string code)
        {
            if (Data.FirstOrDefault(x => x.code == code) != null)
            {
                promoModel promoCode = Data.FirstOrDefault(x => x.code == code);
                return promoCode;
            }
            else
            {
                return null;
            }

        }
        public bool AlreadyClaimed(string playerId, string code)
        {
            bool result = Data.FirstOrDefault(x => x.code == code).claimed.Contains(playerId);
            return result;
        }
        public void addClaim(string playerId, string code)
        {
            promoModel promocode = Data.FirstOrDefault(x => x.code == code);
            promocode.claimed.Add(playerId);
            RemovePromoDatabase(code);
            AddPromoDatabase(promocode);
        }
        public void addReward(string code, itemModel reward)
        {
            promoModel promoCode = Data.FirstOrDefault(x => x.code == code);
            promoCode.items.Add(reward);
            RemovePromoDatabase(code);
            AddPromoDatabase(promoCode);
        }
    }
}
