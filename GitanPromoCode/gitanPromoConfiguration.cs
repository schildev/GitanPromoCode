using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitanPromoCode
{
    public class gitanPromoConfiguration : IRocketPluginConfiguration
    {
        public ushort effectID;
        public short keyID;
        public bool useUconomy;

        public void LoadDefaults()
        {
            effectID = 29863;
            keyID = 777;
            useUconomy = false;
        }
    }
}
