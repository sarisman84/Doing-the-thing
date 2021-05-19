using System;
using System.Collections.Generic;
using UnityEditor;

namespace Utility
{
    //want something like this
    // State1.When(IsCommandedTo(Command0));


    public class State
    {
        public Guid ID { get; }

        public State()
        {
            ID = Guid.NewGuid();
        }
    }

    public class Command
    {
        public Guid ID { get; }
        Transition m_Transitions;


        public Command()
        {
            ID = Guid.NewGuid();
        }

        public Command AddTransition(State requiredState, State resultingState)
        {
            m_Transitions = new Transition(requiredState, resultingState);
            return this;
        }

        public bool IsRequiredStateCorrect(State requiredState)
        {
            return m_Transitions.RequiredState == requiredState;
        }
    }

    public struct Transition
    {
        public Transition(State requiredState, State resultingState)
        {
            RequiredState = requiredState;
            ResultingState = resultingState;
        }

        public State RequiredState { get; }
        public State ResultingState { get; }
    }

    public struct Result
    {
        public State NewState { get; }
        public bool IsValid { get; }

        public Result(State state, bool isValid)
        {
            NewState = state;
            IsValid = isValid;
        }
    }


    public class Test
    {
        public Test()
        {
            // State idling = new State();
            // State fired = new State();
            // State recoiled = new State();
            // Command fire = new Command().AddTransition(idling, fired);
            // Command idle = new Command().AddTransition(recoiled, idling).AddTransition(fired, recoiled);
            //
            // State currentState = idling;
            // fire.ExcecuteCommand(ref currentState);
            // idle.ExcecuteCommand(ref currentState);
            //
            // WhileIn(idling).

        }
    }
}