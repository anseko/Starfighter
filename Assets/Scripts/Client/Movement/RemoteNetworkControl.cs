using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using Net.PackageData.EventsData;

namespace Client.Movement
{   
    public class RemoteNetworkControl: NetworkBehaviour, IMovementAdapter
    {
        private NetworkVariable<MovementData> _lastMovement; 

        public RemoteNetworkControl()
        {
            _lastMovement.Value = new MovementData()
            {
                rotationValue = 0f,
                thrustValue = 0f,
                sideManeurValue = 0f,
                straightManeurValue = 0f
            };
        }
        
        public EngineState getMovement()
        {
            var state = new EngineState();
            if(GetThrustSpeed() != 0)
            {
                state.Thrust = true;
            }
            if(GetShipAngle() < 0)
            {
                state.TopRight = true;
                state.BotLeft = true;
            }
            if(GetShipAngle() > 0)
            {
                state.TopLeft = true;
                state.BotRight = true;
            }
            if(GetStraightManeurSpeed() < 0)
            {
                state.TopLeft = true;
                state.TopRight = true;
            }
            if(GetStraightManeurSpeed() > 0)
            {
                state.BotLeft = true;
                state.BotRight = true;
            }
            if(GetSideManeurSpeed() > 0)
            {
                state.TopLeft = true;
                state.BotLeft = true;
            }
            if(GetSideManeurSpeed() < 0)
            {
                state.TopRight = true;
                state.BotRight = true;
            }
            return state;
        }

        public float GetThrustSpeed()
        { 
            return _lastMovement.Value.thrustValue;
        }

        public float GetSideManeurSpeed()
        { 
            return _lastMovement.Value.sideManeurValue;
        }

        public float GetStraightManeurSpeed()
        { 
            return _lastMovement.Value.straightManeurValue;
        }

        public float GetShipAngle()
        { 
            return _lastMovement.Value.rotationValue;
        }

        public bool GetAnyAction() => false;

        public bool GetFireAction() => false;

        public bool GetDockAction() => false;
        
        public bool GetGrappleAction() => false;
    }
}