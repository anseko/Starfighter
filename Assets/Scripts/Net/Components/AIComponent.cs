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
        private UnitScript _unit;
        private DockComponent _dockComponent;
        private Coroutine _currentTask;
        private bool _isPaused = false;
        
        private void Awake()
        {
            _unit = GetComponent<UnitScript>();
            _unit.TryGetComponent(out _dockComponent);
            
            if(_wayPoints.Count == 0)
                _wayPoints = GameObject.FindGameObjectsWithTag(Constants.AIPointTag).Select(x=>x.transform).ToList();
        }

        private void Start()
        {
            _currentDestination = _wayPoints[++_counter % _wayPoints.Count].position;
            _currentTask = StartCoroutine(GoToPoint(_currentDestination));
        }
        
        public void Pause()
        {
            _isPaused = true;
            StopAllCoroutines();
        }

        public void Resume()
        {
            _isPaused = false;
            _currentTask = StartCoroutine(GoToPoint(_currentDestination));
        }
        
        private void Update()
        {
            if (_isPaused) return;
            if (Vector3.Distance(transform.position, _currentDestination) > _finishDistance) return;

            StopCoroutine(_currentTask);
            _currentDestination = _wayPoints[++_counter % _wayPoints.Count].position;
            _currentTask = StartCoroutine(GoToPoint(_currentDestination));
        }

        private IEnumerator RotateToPoint(Vector3 destination, float step = 0.001f)
        {
            var targetRotation = Quaternion.LookRotation(destination - transform.position, Vector3.up);
            var t = 0f;
            while (t <= 1)
            {
                t += step;
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t * Time.deltaTime);
                yield return null;
            }
            yield return null;
        }
        
        private IEnumerator GoToPoint(Vector3 destination, float step = 0.0001f)
        {
            yield return RotateToPoint(destination);
            var t = 0f;
            while (t <= 1)
            {
                t += step * Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, destination, t);
                yield return null;
            }
        }


    }
}