using System;
using MLAPI.Serialization;

namespace Net.PackageData.EventsData
{
    [Serializable]
    public struct MovementData: INetworkSerializable
    {
        public float thrustValue;
        public float rotationValue;
        public float sideManeurValue;
        public float straightManeurValue;
        
        public void NetworkSerialize(NetworkSerializer serializer)
        {
            serializer.Serialize(ref thrustValue);
            serializer.Serialize(ref rotationValue);
            serializer.Serialize(ref sideManeurValue);
            serializer.Serialize(ref straightManeurValue);
        }
    }
}