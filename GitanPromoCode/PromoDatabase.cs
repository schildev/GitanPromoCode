using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitanPromoCode
{
    public class PromoDatabase
    {
        private PromoStorage<List<PromoModel>> DataStorage { get; set; }

        public List<PromoModel> Data { get; private set; }
        public PromoDatabase()
        {

            DataStorage = new PromoStorage<List<PromoModel>>(GitanPromoCode.Instance.Directory, "gitanPromo.json");
            Reload();

        }
        public void Reload()
        {
            Data = DataStorage.Read();
            if (Data == null)
            {
                Data = [];
                DataStorage.Save(Data);
            }
        }
        public bool AddPromoDatabase(PromoModel promo)
        {
            if (Data.FirstOrDefault(x => x.Code == promo.Code) is not null)
                return false;
            
            Data.Add(promo);
            DataStorage.Save(Data);
            return true;
        }

        public bool RemovePromoDatabase(string code)
        {
            if (Data.FirstOrDefault(x => x.Code == code) != null)
            {
                PromoModel promoDestroy = Data.FirstOrDefault(x => x.Code == code);
                Data.Remove(promoDestroy);
                DataStorage.Save(Data);
                return true;
            }
            return false;
        }
        public PromoModel GetPromo(string code)
        => Data.FirstOrDefault(x => x.Code == code);
        
        public bool AlreadyClaimed(string playerId, PromoModel promo) =>
            promo.Claimed.Contains(playerId) || promo.IsUnique && promo.Claimed.Count > 0;


        public void AddClaim(string playerId, string code)
        {
            PromoModel promocode = Data.FirstOrDefault(x => x.Code == code);
            promocode.Claimed.Add(playerId);
            RemovePromoDatabase(code);
            AddPromoDatabase(promocode);
        }
        public bool AddReward(string code, ItemModel reward)
        {
            if(Data.FirstOrDefault(x => x.Code == code) is not PromoModel Promocode)
                return false;

            Promocode.Items.Add(reward);
            RemovePromoDatabase(code);
            return AddPromoDatabase(Promocode);
        }
        public List<PromoModel> GetCodes() => [.. Data];
        
    }
}
