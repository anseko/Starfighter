using System;
using Core;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace Net.Components
{
    public class TouchPaintingComponent : NetworkBehaviour
    {
        public Color colorToPaint;

        private void Start()
        {
            GetComponent<Renderer>().material.color = colorToPaint;
            
            if (!IsServer) return;
            GetComponent<NetworkObject>().Spawn();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsServer || collision.gameObject.TryGetComponent<Grappler>(out _)) return;
            
            var material = collision.gameObject.GetComponent<Renderer>().material;
            material.color = colorToPaint;

            PaintClientRpc(collision.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
            GetComponent<NetworkObject>().Despawn(true);
        }

        [ClientRpc]
        private void PaintClientRpc(ulong objectId)
        {
            var material = GetNetworkObject(objectId).GetComponent<Renderer>().material;
            material.color = colorToPaint;
        }
    }
}