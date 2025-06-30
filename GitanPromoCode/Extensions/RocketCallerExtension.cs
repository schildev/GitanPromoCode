using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Chat;

namespace GitanPromoCode.Extensions
{
    public static class RocketCallerExtension
    {
        public static void SayToCaller(this IRocketPlayer caller, string translateKey, UnityEngine.Color color)
        => UnturnedChat.Say(caller, GitanPromoCode.Instance.Translate(translateKey), color);
        public static void SayToCallerError(this IRocketPlayer caller, string message)
        => SayToCaller(caller, message, UnityEngine.Color.red);
        public static void SayToCallerSuccess(this IRocketPlayer caller, string message)
        => SayToCaller(caller, message, UnityEngine.Color.green);
    }
}
