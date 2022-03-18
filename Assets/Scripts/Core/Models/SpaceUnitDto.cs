using System;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Core.Models
{
    [Serializable]
    public class SpaceUnitDto: INetworkSerializable
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

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref maxAngleSpeed);
            serializer.SerializeValue(ref maxSpeed);
            serializer.SerializeValue(ref maxHp);
            serializer.SerializeValue(ref currentHp);
            serializer.SerializeValue(ref isDockable);
            serializer.SerializeValue(ref isMovable);
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref rotation);
            serializer.SerializeValue(ref prefabName);
            serializer.SerializeValue(ref maxStress);
            serializer.SerializeValue(ref currentStress);
            serializer.SerializeValue(ref shipId);
            serializer.SerializeValue(ref shipState);
            serializer.SerializeValue(ref baseColor);
            serializer.SerializeValue(ref acceleration);
            serializer.SerializeValue(ref radarRange);
            serializer.SerializeValue(ref accelerationCoefficient);
            serializer.SerializeValue(ref physResistanceCoefficient);
            serializer.SerializeValue(ref radResistanceCoefficient);
            serializer.SerializeValue(ref radarRangeCoefficient);        }
    }
    

}