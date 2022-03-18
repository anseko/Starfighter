using System;
using Unity.Netcode;

namespace Net.Core
{
    [Serializable]
    public struct MovementData: INetworkSerializable
    {
        public float thrustValue;
        public float rotationValue;
        public float sideManeurValue;
        public float straightManeurValue;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref thrustValue);
            serializer.SerializeValue(ref rotationValue);
            serializer.SerializeValue(ref sideManeurValue);
            serializer.SerializeValue(ref straightManeurValue);
        }
    }
}