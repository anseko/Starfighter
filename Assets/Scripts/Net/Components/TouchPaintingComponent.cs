using Core;
using Mirror;
using UnityEngine;

namespace Net.Components
{
    public class TouchPaintingComponent : NetworkBehaviour
    {
        public Color colorToPaint;

        private void Start()
        {
            GetComponent<Renderer>().material.color = colorToPaint;
            
            if (!isServer) return;
            GetComponent<NetworkIdentity>().Spawn();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!isServer || collision.gameObject.TryGetComponent<Grappler>(out _)) return;
            
            var material = collision.gameObject.GetComponent<Renderer>().material;
            material.color = colorToPaint;

            PaintClientRpc(collision.gameObject.GetComponent<NetworkIdentity>().NetworkObjectId);
            GetComponent<NetworkIdentity>().Despawn(true);
        }

        [ClientRpc]
        private void PaintClientRpc(ulong objectId)
        {
            var material = GetNetworkObject(objectId).GetComponent<Renderer>().material;
            material.color = colorToPaint;
        }
    }
}