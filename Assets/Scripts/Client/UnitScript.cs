using Core;
using MLAPI;
using ScriptableObjects;

namespace Client
{
    public class UnitScript : NetworkBehaviour
    {
        public SpaceUnitConfig unitConfig;

        public UnitState GetState() => UnitState.InFlight;
    }
}