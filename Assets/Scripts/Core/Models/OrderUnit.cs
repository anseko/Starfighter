using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Core.Models
{
    public struct OrderUnit: INetworkSerializable
    {
        public FixedString32Bytes shipName;
        public Vector3 position;
        public Vector3 size;
        public FixedString32Bytes text;
        public OrderOperation operation;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref shipName);
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref size);
            serializer.SerializeValue(ref text);
            serializer.SerializeValue(ref operation);
        }
    }

    public enum OrderOperation
    {
        Add,
        Edit,
        Remove
    }
}
