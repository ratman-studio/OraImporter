using System;
using UnityEngine;
using UnityEngine.Events;

namespace com.szczuro.events
{
    public class GameEventListener:MonoBehaviour
    {
        public GameEvent Event;
        public UnityEvent Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }
        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }
        internal void OnEventRaised()
        {
            Response.Invoke();
        }
    }
}