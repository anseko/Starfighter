using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Control
{   
    public class PlayerControl: MovementAdapter
    {
         override public EngineState getMovement()
         {
            var state = new EngineState();
            if(Input.GetAxis("Jump") != 0)
            {
                state.Thrust = true;
            }
            if(Input.GetAxis("Rotation") < 0)
            {
                state.topRight = true;
            }
            if(Input.GetAxis("Rotation") > 0)
            {
                state.topLeft = true;
            }
            if(Input.GetAxis("Vertical") < 0)
            {
                state.topLeft = true;
                state.topRight = true;
            }
            if(Input.GetAxis("Vertical") > 0)
            {
                state.botLeft = true;
                state.botRight = true;
            }
            if(Input.GetAxis("Horizontal") > 0)
            {
                state.topLeft = true;
                state.botLeft = true;
            }
            if(Input.GetAxis("Horizontal") < 0)
            {
                state.topRight = true;
                state.botRight = true;
            }
            return state;
        }
    }

    //class AIControl: movementAdapter
    //{
        //написать что то надо
    //}
}