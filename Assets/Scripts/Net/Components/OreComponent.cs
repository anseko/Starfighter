using Mirror;
using UnityEngine;

namespace Net.Components
{
    public class OreComponent: NetworkBehaviour
    {
        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Asteroids")) return;
            
            for (var i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Units");
            }
            gameObject.layer = LayerMask.NameToLayer("Units");
        }
    }
}