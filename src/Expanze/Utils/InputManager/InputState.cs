using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Expanze.Utils
{
    class InputState
    {
        String name;
        private GameAction[] keyActions;
        InputState previousState;

        bool[] changeStateKeys;

        public InputState(String name)
        {
            this.name = name;
            keyActions = new GameAction[255];
            changeStateKeys = new bool[255];
            previousState = null;
        }

        public String GetName() { return name; }
        public InputState GetPreviousState() { return previousState; }
        public void SetPreviousState(InputState state) { previousState = state; }

        public void MapToKey(GameAction gameAction, Keys key)
        {
            keyActions[(int) key] = gameAction;
        }

        public GameAction GetGameAction(string actionName)
        {
            for (int loop1 = 0; loop1 < keyActions.Length; loop1++)
            {
                if (keyActions[loop1] != null && keyActions[loop1].GetName().CompareTo(actionName) == 0)
                    return keyActions[loop1];
            }

            return null;
        }

        public void ClearMap(GameAction gameAction)
        {
            for (int loop1 = 0; loop1 < keyActions.Length; loop1++)
            {
                if (keyActions[loop1] == gameAction)
                {
                    keyActions[loop1] = null;
                }
            }
            gameAction.Reset();
        }

        public void ResetAllGameActions()
        {
            for (int loop1 = 0; loop1 < keyActions.Length; loop1++)
            {
                changeStateKeys[loop1] = Keyboard.GetState().IsKeyDown((Keys)loop1);
                if (keyActions[loop1] != null)
                {
                    keyActions[loop1].Reset();
                }
            }
        }

        public void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            for (int loop1 = 0; loop1 < keyActions.Length; loop1++)
            {
                GameAction gameAction = keyActions[loop1];
                if (gameAction != null)
                {
                    if (keyboardState.IsKeyDown((Keys)loop1))
                    {
                        if(!changeStateKeys[loop1])
                            gameAction.Press();
                    }
                    else
                    {
                        changeStateKeys[loop1] = false;
                        gameAction.Release();
                    }
                }
            }
        }
    }
}
