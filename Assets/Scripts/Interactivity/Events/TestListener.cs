using System;
using Interactivity.Components;
using UnityEngine;
using static Interactivity.Events.EventMessageExtension;

namespace Interactivity.Events
{
    public class TestListener : MonoBehaviour
    {
        public CustomEvent test;
        public CountdownEvent test2;
        public DefendEvent test3;
        public CountEntityEvent test4;
        

        private int _health = 100;

        private void Awake()
        {
            ListenToTheEvent();
        }

        public void TriggerEvent()
        {
            _health--;
            _health = Mathf.Clamp(_health, 0, 100);
            test.Raise(true, 10, "Test");
            test2.Raise(true, 5, "Test (delayed by 10 seconds)");
            test3.Raise(true, _health, 2, "Test (Dont have any health left)");
         
        }


        public void ListenToTheEvent()
        {
            test.EditEvent<Action<int, string>, CustomEvent>(SomeMethod, EditMode.Subscribe);
            test2.EditEvent<Action<int, string>, CountdownEvent>(SomeMethod, EditMode.Subscribe);
            test3.EditEvent<Action<int, string>, DefendEvent>(SomeMethod, EditMode.Subscribe);
            
            Debug.Log("Added test4");
            test4.EditEvent<Action<int, string>, CountEntityEvent>(SomeMethod, EditMode.Subscribe);
        }


        void SomeMethod(int value1, string value2)
        {
            Debug.Log($"{value1} & {value2} are detected!");
        }
    }
}