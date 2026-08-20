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
            NetworkServer.Spawn(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!isServer || collision.gameObject.TryGetComponent<Grappler>(out _)) return;
            
            var material = collision.gameObject.GetComponent<Renderer>().material;
            material.color = colorToPaint;

            PaintClientRpc(collision.gameObject.GetComponent<NetworkIdentity>().netId);
            NetworkServer.Destroy(collision.gameObject);
        }

        [ClientRpc]
        private void PaintClientRpc(uint objectId)
        {
            if (!NetworkClient.spawned.TryGetValue(objectId, out var identity)) return;
            var material = identity.GetComponent<Renderer>().material;
            material.color = colorToPaint;
        }
    }
}