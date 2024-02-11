using System;
using Mirror;

namespace Net.Core
{
    [Serializable]
    public struct MovementData
    {
        public float thrustValue;
        public float rotationValue;
        public float sideManeurValue;
        public float straightManeurValue;
    }

    //should be unnecessary
    public static class CustomMovementDataReadWrite
    {
        public static void WriteMovementData(this NetworkWriter writer, MovementData value)
        {
            writer.WriteFloat(value.thrustValue);
            writer.WriteFloat(value.rotationValue);
            writer.WriteFloat(value.sideManeurValue);
            writer.WriteFloat(value.straightManeurValue);
        }
        
        public static MovementData ReadMovementData(this NetworkReader reader)
        {
            return new MovementData()
            {
                thrustValue = reader.ReadFloat(),
                rotationValue = reader.ReadFloat(),
                sideManeurValue = reader.ReadFloat(),
                straightManeurValue = reader.ReadFloat(),
            };
        }
    }
}