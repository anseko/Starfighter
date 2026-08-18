using System;
using Mirror;

namespace Net.Core
{
    [Serializable]
    public struct EngineState
    {
        public bool Thrust;
        public bool TopRight;
        public bool TopLeft;
        public bool BotLeft;
        public bool BotRight;
    }

    [Serializable]
    public struct MovementData
    {
        public float thrustValue;
        public float rotationValue;
        public float sideManeurValue;
        public float straightManeurValue;
    }

    public static class MovementDataReadWrite
    {
        public static void WriteEngineState(this NetworkWriter writer, EngineState value)
        {
            writer.WriteBool(value.Thrust);
            writer.WriteBool(value.TopRight);
            writer.WriteBool(value.TopLeft);
            writer.WriteBool(value.BotLeft);
            writer.WriteBool(value.BotRight);
        }
        
        public static EngineState ReadEngineState(this NetworkReader reader)
        {
            return new EngineState()
            {
                Thrust = reader.ReadBool(),
                TopRight = reader.ReadBool(),
                TopLeft = reader.ReadBool(),
                BotLeft = reader.ReadBool(),
                BotRight = reader.ReadBool()
            };
        }

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
