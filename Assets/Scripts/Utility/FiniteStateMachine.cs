using System;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine.InputSystem.LowLevel;

namespace Utility.FiniteStateMachine
{
    //want something like this
    // State1.When(IsCommandedTo(Command0));

    public class BizhansStateMachine<TState, TCommand> where TState : struct, IConvertible, IComparable
        where TCommand : struct, IConvertible, IComparable
    {
        protected class StateTransition<TS, TC> where TS : struct, IConvertible, IComparable
            where TC : struct, IConvertible, IComparable
        {
            private readonly TS m_CurrentState;
            private readonly TC m_Command;

            public StateTransition(TS currentState, TC command)
            {
                if (!typeof(TS).IsEnum || !typeof(TC).IsEnum)
                {
                    throw new ArgumentException("Input State type or Input Command Type must be an enumerated type");
                }

                m_CurrentState = currentState;
                m_Command = command;
            }

            //Some hashcode calculations, not sure what this does
            public override int GetHashCode()
            {
                return 17 + 31 * m_CurrentState.GetHashCode() + 31 * m_Command.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                StateTransition<TS, TC> other = obj as StateTransition<TS, TC>;
                return other != null && m_CurrentState.CompareTo(other.m_CurrentState) == 0 &&
                       m_Command.CompareTo(other.m_Command) == 0;
            }
        }

        private Dictionary<StateTransition<TState, TCommand>, TState> m_Transitions;
        public TState CurrentState { get; private set; }

        protected BizhansStateMachine(TState initialState)
        {
            if (!typeof(TState).IsEnum || !typeof(TCommand).IsEnum)
            {
                throw new ArgumentException("Input State type or Input Command Type must be an enumerated type");
            }

            CurrentState = initialState;
            m_Transitions = new();
        }

        protected void AddTransition(TState start, TCommand command, TState end)
        {
            m_Transitions.Add(new StateTransition<TState, TCommand>(start, command), end);
        }

        public TransitionResult<TState> TryExecutingCommand(TCommand command)
        {
            StateTransition<TState, TCommand> transition = new StateTransition<TState, TCommand>(CurrentState, command);
            TState nextState;
            if (m_Transitions.TryGetValue(transition, out nextState))
            {
                return new TransitionResult<TState>(nextState, true);
            }

            return new TransitionResult<TState>(CurrentState, false);
        }

        public TransitionResult<TState> ExecuteCommand(TCommand command)
        {
            var result = TryExecutingCommand(command);
            if (result.isValid)
            {
                CurrentState = result.newState;
            }

            return result;
        }

        public struct TransitionResult<TState>
        {
            public TransitionResult(TState newState, bool isValid)
            {
                this.newState = newState;
                this.isValid = isValid;
            }

            public TState newState;
            public bool isValid;
        }
    }


    public class StateMachine<TState, TCommand> where TState : Enum where TCommand : Enum
    {
        private Dictionary<Transition, State<TState>> m_Transitions;

        public StateMachine<TState, TCommand> AddTransition(State<TState> startingState, TCommand command,
            State<TState> endState)
        {
            m_Transitions.Add(new Transition(startingState, command), endState);
            return this;
        }

        protected class Transition
        {
            private readonly State<TState> CurrentState;
            private readonly TCommand Command;

            public Transition(State<TState> startingState, TCommand command)
            {
                CurrentState = startingState;
                Command = command;
            }
        }

        public struct State<TS> where TS : Enum
        {
            public State(TS @enum) : this()
            {
                this.@enum = @enum;
            }

            public TS @enum;
            public event Action ONStateEnter;
            public event Action ONStateUpdate;
            public event Action ONStateExit;

            private void OnStateEnter()
            {
                ONStateEnter?.Invoke();
            }

            private void Update()
            {
                ONStateUpdate?.Invoke();
            }

            private void OnStateExit()
            {
                ONStateExit?.Invoke();
            }
        }
    }
}