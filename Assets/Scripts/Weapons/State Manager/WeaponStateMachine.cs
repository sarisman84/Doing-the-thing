using System;
using System.Collections;
using Scripts.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility.FiniteStateMachine;

namespace Weapons.State_Manager
{
    public class WeaponStateMachine
    {
        public enum States
        {
            Idling,
            Cooldown,
            Firing
        }

        public enum Commands
        {
            Fire,
            Nothing
        }


        public States State { get; set; }
        public Coroutine CurrentCoroutine { get; set; }

        private Func<IEnumerator>[,] Fms;
        public event Func<IEnumerator> ONFiringState;
        public event Func<IEnumerator> ONCooldownState;
        public event Func<IEnumerator> ONIdlingState;


        public WeaponStateMachine(Func<IEnumerator> onFiringEvent,
            Func<IEnumerator> onCooldownEvent, Func<IEnumerator> onIdlingEvent)
        {
            State = States.Idling;
            ONFiringState += onFiringEvent;
            ONCooldownState += onCooldownEvent;
            ONIdlingState += onIdlingEvent;

            Fms = new[,]
            {
                //Fire,         Nothing
                {ONFiringState, ONIdlingState}, //Idling
                {ONCooldownState, ONCooldownState}, //Cooldown
                {ONFiringState, ONIdlingState} //Firing
            };
        }


        public void ProcessCommand(Commands commands, MonoBehaviour sceneObject)
        {
            if (CurrentCoroutine != null)
            {
                Debug.Log($"{CurrentCoroutine} is still running, skipping.");
                return;
            }

            Debug.Log(
                $"Processing Command: {commands} - selecting state {Fms[(int) State, (int) commands].Method.Name}");
            CurrentCoroutine = sceneObject.StartCoroutine(Fms[(int) State, (int) commands]?.Invoke());
        }
    }
}