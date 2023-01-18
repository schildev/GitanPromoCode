using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitanPromoCode
{
    public class itemModel
    {
        public string type {get; set; }
        public ushort itemId { get; set; }
        public int quantity { get; set; }
    }
    public class promoModel
    {
        public string code { get; set; }
        public List<itemModel> items { get; set; }

        public List<string> claimed { get; set; }
        public bool isUnique { get; set; }
    }
}
