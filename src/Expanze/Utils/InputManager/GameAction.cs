using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze.Utils
{
    class GameAction
    {
        public enum ActionKind { Normal, OnlyInitialPress };
        private enum ActionState { Released, Pressed, WaitingForReleased }

        private String name;
        private ActionKind behavior;
        private ActionState state;
        private int amount;

        public GameAction(String name) : this(name, ActionKind.Normal)
        {
        }

        public GameAction(String name, ActionKind behavior)
        {
            this.name = name;
            this.behavior = behavior;
            Reset();
        }

        public String GetName() { return name; }

        public void Reset()
        {
            state = ActionState.Released;
            amount = 0;
        }

        public void Press() {
            if (state != ActionState.WaitingForReleased) {
                state = ActionState.Pressed;
                amount++;
            }
        }

        public void Release() {
                state = ActionState.Released;
        }

        public bool IsPressed() {
            return (GetAmount() != 0);
        }

        public int GetAmount() {
            int temp = amount;
            if (temp != 0) {
                if (state == ActionState.Released) {
                    amount = 0;
                }
                else if (behavior == ActionKind.OnlyInitialPress) {
                    state = ActionState.WaitingForReleased;
                    amount = 0;
                }
            }
            return temp;
        }
    }
}
