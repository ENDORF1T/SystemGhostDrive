using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Application.Utility.Timer
{
    public class FunctionTimer
    {

        private static List<FunctionTimer> _activeTimerList;
        private static GameObject _initGameObject;

        public static FunctionTimer Create(Action action, float timer, GameObject sender = null, string timerName = null)
        {
            InitIfNeeded();
            GameObject gameObject = new GameObject("FunctionTimer", typeof(MonoBehaviourHook));
            FunctionTimer functionTimer = new FunctionTimer(action, timer, sender, sender ? true: false,  timerName, gameObject);
            gameObject.GetComponent<MonoBehaviourHook>().ActionOnUpdate = functionTimer.Update;

            _activeTimerList.Add(functionTimer);

            return functionTimer;
        }

        public static void StopTimer(string timerName)
        {
            InitIfNeeded();
            for (int i = 0; i < _activeTimerList.Count; i++)
            {
                if (_activeTimerList[i]._timerName == timerName)
                {
                    _activeTimerList[i].DestroySelf();
                    i--;
                }
            }
        }

        private static void InitIfNeeded()
        {
            if (!_initGameObject)
            {
                _initGameObject = new GameObject("FunctionTimer_InitGameObject");
                _activeTimerList = new List<FunctionTimer>();
            }
        }

        private static void RemoveTimer(FunctionTimer functionTimer)
        {
            InitIfNeeded();
            _activeTimerList.Remove(functionTimer);
        }

        private Action _action = null;
        private float _timer = 0.0f;
        private string _timerName = null;
        private GameObject _sender = null;
        private bool _senderAssignedToCheck = false;
        private GameObject _gameObject = null;
        private bool _isDestroyed = false;

        private FunctionTimer(Action action, float timer, GameObject sender, bool senderAssignedToCheck, string timerName, GameObject gameObject)
        {
            _action = action;
            _timer = timer;
            _sender = sender;
            _senderAssignedToCheck = senderAssignedToCheck;
            _timerName = timerName;
            _gameObject = gameObject;
            _isDestroyed = false;
        }

        public void Update()
        {
            if (_isDestroyed) return;
            if (!_sender && _senderAssignedToCheck) DestroySelf();

            _timer -= Time.deltaTime;

            if (_timer <= 0.0f)
            {
                if (_sender && _senderAssignedToCheck) _action();
                else if (!_senderAssignedToCheck) _action();
                DestroySelf();
            }
        }

        private void DestroySelf()
        {
            _isDestroyed = true;
            UnityEngine.Object.Destroy(_gameObject);
            RemoveTimer(this);
        }

        private class MonoBehaviourHook : MonoBehaviourCashe.MonoBehaviourCashe
        {
            public Action ActionOnUpdate = null;

            protected override void OnUpdate()
            {
                if (ActionOnUpdate != null) ActionOnUpdate();
            }

            private void OnEnable()
            {
                SubscribeToUpdate();
            }

            private void OnDisable()
            {
                UnSubscribeToUpdate();
            }
        }
    }
}