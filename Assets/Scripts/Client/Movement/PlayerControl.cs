using Core;
using Core.InputManager;
using MLAPI;
using MLAPI.NetworkVariable;
using Net.PackageData.EventsData;
using ScriptableObjects;
using UnityEngine;

namespace Client.Movement
{   
    public class PlayerControl: NetworkBehaviour, IMovementAdapter
    {
        private NetworkVariable<MovementData> _lastMovement;
        private KeyConfig _keyConfig;
        
        public PlayerControl(UnitState state)
        {
            _keyConfig = InputManager.instance.keyConfig;
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

        public float GetThrustSpeed() => Input.GetAxis("Jump") * 5f;

        public float GetSideManeurSpeed() => Input.GetAxis("Horizontal");

        public float GetStraightManeurSpeed() => Input.GetAxis("Vertical");

        public float GetShipAngle() => Input.GetAxis("Rotation") * 4.5f;

        public bool GetDockAction() => Input.GetKeyDown(_keyConfig.dock);

        public bool GetFireAction() => Input.GetKeyDown(_keyConfig.fire);

        public bool GetGrappleAction() => Input.GetKeyDown(_keyConfig.grapple);
        
    }
}