using System;
using Mirror;
using ScriptableObjects;
using UnityEngine;

namespace Core.Models
{
    [Serializable]
    public class SpaceUnitDto
    {
        public float maxAngleSpeed;
        public float maxSpeed;
        public float maxHp;
        public float currentHp;
        public bool isDockable;
        public bool isMovable;
        public Vector3 position;
        public Quaternion rotation;
        public string prefabName;
        public Guid id;
        public float maxStress;
        public float currentStress;
        public string shipId;
        public UnitState shipState;
        public Color baseColor;
        public float acceleration;
        public float radarRange;
        public float accelerationCoefficient;
        public float physResistanceCoefficient;
        public float radResistanceCoefficient;
        public float radarRangeCoefficient;
        
        public SpaceUnitDto(){}
        
        public SpaceUnitDto(SpaceUnitConfig config)
        {
            maxAngleSpeed = config.maxAngleSpeed;
            maxSpeed = config.maxSpeed;
            maxHp = config.maxHp;
            currentHp = config.currentHp;
            isDockable = config.isDockable;
            isMovable = config.isMovable;
            position = config.position;
            rotation = config.rotation;
            prefabName = config.prefabName;
            id = config.id;
            
            if (config is SpaceShipConfig shipConfig)
            {
                maxStress = shipConfig.maxStress;
                currentStress = shipConfig.currentStress;
                shipId = shipConfig.shipId;
                shipState = shipConfig.shipState;
                baseColor = shipConfig.baseColor;
                acceleration = shipConfig.acceleration;
                radarRange = shipConfig.radarRange;
                accelerationCoefficient = shipConfig.accelerationCoefficient;
                physResistanceCoefficient = shipConfig.physResistanceCoefficient;
                radResistanceCoefficient = shipConfig.radResistanceCoefficient;
                radarRangeCoefficient = shipConfig.radarRangeCoefficient;
            }
            else
            {
                maxStress = 100;
                currentStress = 0;
                shipId = string.Empty;
                shipState = UnitState.InFlight;
                baseColor = Color.white;
                accelerationCoefficient = 1;
                physResistanceCoefficient = 1;
                radResistanceCoefficient = 1;
                radarRangeCoefficient = 1;
            }
        }
    }
    

    //should be unnecessary
    // public static class CustomSpaceUnitDtoReadWrite
    // {
    //     public static void WriteMovementData(this NetworkWriter writer, SpaceUnitDto value)
    //     {
    //     
    //         public void NetworkSerialize(NetworkSerializer serializer)
    //         {
    //             serializer.Serialize(ref maxAngleSpeed);
    //             serializer.Serialize(ref maxSpeed);
    //             serializer.Serialize(ref maxHp);
    //             serializer.Serialize(ref currentHp);
    //             serializer.Serialize(ref isDockable);
    //             serializer.Serialize(ref isMovable);
    //             serializer.Serialize(ref position);
    //             serializer.Serialize(ref rotation);
    //             serializer.Serialize(ref prefabName);
    //             serializer.Serialize(ref maxStress);
    //             serializer.Serialize(ref currentStress);
    //             serializer.Serialize(ref shipId);
    //             serializer.Serialize(ref shipState);
    //             serializer.Serialize(ref baseColor);
    //             serializer.Serialize(ref acceleration);
    //             serializer.Serialize(ref radarRange);
    //             serializer.Serialize(ref accelerationCoefficient);
    //             serializer.Serialize(ref physResistanceCoefficient);
    //             serializer.Serialize(ref radResistanceCoefficient);
    //             serializer.Serialize(ref radarRangeCoefficient);
    //         }
    //
    //     }
    // }
}