using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Expanze.Utils
{
    class InputManager
    {
        private InputState activeState;
        private List<InputState> states;

        private static InputManager instance = null;

        public static InputManager Inst()
        {
            if (instance == null)
            {
                instance = new InputManager();
            }
            return instance;
        }

        private InputManager()
        {
            states = new List<InputState>();
            activeState = null;
        }

        public void Update()
        {
            if (activeState != null)
            {
                activeState.Update();
            }
        }

        public void ReturnToPreviousState()
        {
            if(activeState != null)
                SetActiveState(activeState.GetPreviousState().GetName());
        }

        public void SetActiveState(String stateName)
        {
            InputState state = FindState(stateName);
            if (state != null)
            {
                if (activeState != null &&
                    state.GetName().CompareTo(activeState.GetName()) != 0)
                    state.SetPreviousState(activeState);
                else
                    state.SetPreviousState(FindState("game"));
                activeState = state;
                activeState.ResetAllGameActions();
            }
        }

        public bool AddState(String stateName)
        {
            if (FindState(stateName) != null)
                return false;
            states.Add(new InputState(stateName));
            return true;
        }

        private InputState FindState(String stateName)
        {
            for (int loop1 = 0; loop1 < states.Count; loop1++)
            {
                if (states[loop1].GetName().CompareTo(stateName) == 0)
                    return states[loop1];
            }

            return null;
        }

        public void MapToKey(String stateName, GameAction gameAction, Keys key)
        {
            InputState state = FindState(stateName);
            if (state != null)
            {
                state.MapToKey(gameAction, key);
            }
        }

        public void MapToGamepad(string stateName, GameAction gameAction, Buttons button)
        {
            throw new NotImplementedException();
        }

        public GameAction GetGameAction(String stateName, String actionName)
        {
            InputState state = FindState(stateName);
            if (state != null)
            {
                return state.GetGameAction(actionName);
            }
            return null;
        }
    }
}
