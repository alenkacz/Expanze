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

        public void SetActiveState(String stateName)
        {
            InputState state = FindState(stateName);
            if (state != null)
            {
                activeState = state;
                activeState.ResetAllGameActions();
            }
        }

        public void AddState(String stateName)
        {
            states.Add(new InputState(stateName));
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
    }
}
