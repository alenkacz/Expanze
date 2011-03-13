using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Expanze.Utils
{
    class InputManager
    {
        private List<InputState> states;
        private List<String> activeStateList;

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
            activeStateList = new List<String>();
        }

        public void Update()
        {
            if (activeStateList.Count > 0)
            {
                FindStateInAllStates(activeStateList[activeStateList.Count - 1]).Update();
            }
        }

        public string GetActiveState()
        {
            return activeStateList[activeStateList.Count - 1];
        }

        public void SetActiveState(String stateName)
        {
            InputState state = FindStateInAllStates(stateName);
            if (state != null)
            {
                while(activeStateList.Remove(stateName) == true)
                    ;

                activeStateList.Add(stateName);
                state.ResetAllGameActions();
            }
        }

        public void ClearActiveState(String stateName)
        {
            while (activeStateList.Remove(stateName) == true)
                ;

            if (activeStateList.Count > 0)
                FindStateInAllStates(activeStateList[activeStateList.Count - 1]).ResetAllGameActions();
        }

        public bool AddState(String stateName)
        {
            if (FindStateInAllStates(stateName) != null)
                return false;
            states.Add(new InputState(stateName));
            return true;
        }

        private InputState FindStateInAllStates(String stateName)
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
            InputState state = FindStateInAllStates(stateName);
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
            InputState state = FindStateInAllStates(stateName);
            if (state != null)
            {
                return state.GetGameAction(actionName);
            }
            return null;
        }
    }
}
