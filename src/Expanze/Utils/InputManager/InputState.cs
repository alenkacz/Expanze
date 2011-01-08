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

        public InputState(String name)
        {
            this.name = name;
            keyActions = new GameAction[255];
        }

        public String GetName() { return name; }

        public void MapToKey(GameAction gameAction, Keys key)
        {
            keyActions[(int) key] = gameAction;
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
                        gameAction.Press();
                    else
                        gameAction.Release();
                }
            }
        }
    }
}
