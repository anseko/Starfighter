using System;
using MLAPI.Serialization;
using ScriptableObjects;
using UnityEngine;

namespace Core.Models
{
    [Serializable]
    public class SpaceShipDto : SpaceUnitDto
    {
        public float maxStress;
        public float currentStress;
        public string shipId;
        public UnitState shipState;
        public Color baseColor;

        public SpaceShipDto() : base() {}
        
        public SpaceShipDto(SpaceShipConfig config) : base(config)
        {
            maxStress = config.maxStress;
            currentStress = config.currentStress;
            shipId = config.shipId;
            shipState = config.shipState;
            baseColor = config.baseColor;
        }

        public SpaceShipDto(SpaceUnitConfig unitConfig) : base(unitConfig)
        {
            maxStress = 100;
            currentStress = 0;
            shipId = string.Empty;
            shipState = UnitState.InFlight;
        }
        
        public override void NetworkSerialize(NetworkSerializer serializer)
        {
            base.NetworkSerialize(serializer);
            serializer.Serialize(ref maxStress);
            serializer.Serialize(ref currentStress);
            serializer.Serialize(ref shipId);
            serializer.Serialize(ref shipState);
            serializer.Serialize(ref baseColor);
        }
    }
}