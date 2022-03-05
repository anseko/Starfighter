using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Core;
using Core;
using MLAPI;
using UnityEngine;
using UnityEngine.AI;

namespace Net.Components
{
    public class AIComponent: MonoBehaviour
    {
        
        [SerializeField] private List<Transform> _wayPoints;
        private Vector3 _currentDestination;
        private float _finishDistance = 2f;
        private int _counter = 0;

        private Coroutine _currentTask;
        private bool _isPaused = false;
        
        private void Awake()
        {
            if(_wayPoints.Count == 0)
                _wayPoints = GameObject.FindGameObjectsWithTag(Constants.AIPointTag).Select(x=>x.transform).ToList();
        }

        private void Start()
        {
            if (NetworkManager.Singleton.IsClient) return;

            var minDist = _wayPoints.Min(x => Vector3.Distance(transform.position, x.position));
            _currentDestination = _wayPoints.First(x => Vector3.Distance(transform.position, x.position) <= minDist).position;
            // _currentDestination = _wayPoints[++_counter % _wayPoints.Count].position;
            _currentTask = StartCoroutine(GoToPoint(_currentDestination));
        }
        
        public void Pause()
        {
            if (NetworkManager.Singleton.IsClient) return;
            _isPaused = true;
            StopAllCoroutines();
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }

        public void Resume()
        {
            if (NetworkManager.Singleton.IsClient) return;
            _isPaused = false;
            _currentDestination = _wayPoints[++_counter % _wayPoints.Count].position;
            _currentTask = StartCoroutine(GoToPoint(_currentDestination));
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        }
        
        private void Update()
        {
            if (NetworkManager.Singleton.IsClient) return;
            
            if (_isPaused) return;
            if (Vector3.Distance(transform.position, _currentDestination) > _finishDistance) return;

            if(_currentTask != null)
                StopCoroutine(_currentTask);
            _currentDestination = _wayPoints[++_counter % _wayPoints.Count].position;
            _currentTask = StartCoroutine(GoToPoint(_currentDestination));
        }
        
        private IEnumerator GoToPoint(Vector3 destination, float step = 0.0001f)
        {
            var targetRotation = Quaternion.LookRotation(destination - transform.position);
            var t = 0f;
            while (Vector3.Distance(destination, transform.position) > _finishDistance)
            {
                t += step * 0.1f * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 0.5f);
                transform.position = Vector3.Lerp(transform.position, destination, t);
                yield return null;
            }
        }
    }
}