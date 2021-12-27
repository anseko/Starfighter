using System;
using System.Collections.Generic;
using System.Linq;
using Client.Core;
using Core;
using MLAPI;
using Net.Components;
using UnityEngine;
using UnityEngine.AI;

namespace Net
{
    public class AIComponent: NetworkBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private List<Transform> _wayPoints;
        private int _counter = 0;
        private UnitScript _unit;

        private void Awake()
        {
            _unit = GetComponent<UnitScript>();
        }

        public void Init()
        {
            _wayPoints = GameObject.FindGameObjectsWithTag(Constants.AIPointTag).Select(x=>x.transform).ToList();
            _agent.SetDestination(_wayPoints[_counter++ % _wayPoints.Count].position);
        }
        
        private void Update()
        {
            if (!_agent.hasPath && _wayPoints.Any())
            {
                _agent.SetDestination(_wayPoints[_counter++ % _wayPoints.Count].position);
            }
        }
    }
}