using System;
using System.Collections.Generic;
using System.Linq;
using Client.Core;
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
        private UnitScript _unit;
        private Queue<Vector3> _wayPointsQueue;

        private void Start()
        {
            _unit = GetComponent<UnitScript>();
            _wayPointsQueue = new Queue<Vector3>(_wayPoints.Select(x=>x.position));
            _agent.SetDestination(_wayPointsQueue.Dequeue());
            
        }

        private void Update()
        {
            if (!_agent.hasPath)
            {
                _agent.SetDestination(_wayPointsQueue.Dequeue());
            }

            if (!_wayPointsQueue.Any())
            {
                _wayPointsQueue = new Queue<Vector3>(_wayPoints.Select(x=>x.position));
            }
        }
    }
}