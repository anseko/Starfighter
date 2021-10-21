using Client.Core;
using UnityEngine;

namespace Client.UI
{
    public class BasePlayerUIElement: MonoBehaviour
    {
        protected PlayerScript PlayerScript;

        public virtual void Init(PlayerScript playerScript)
        {
            PlayerScript = playerScript;
        }
    }
}