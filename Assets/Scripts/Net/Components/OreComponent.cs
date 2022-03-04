using MLAPI;
using UnityEngine;

namespace Net.Components
{
    public class OreComponent: NetworkBehaviour
    {
        private void OnCollisionExit(Collision other)
        {
            if (!IsServer) return;
            
            transform.SetParent(null, true);
            gameObject.layer = LayerMask.NameToLayer("Units");
        }
    }
}