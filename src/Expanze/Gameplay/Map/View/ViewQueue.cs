using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Expanze.Gameplay.Map
{
    public enum ItemKind{BuildTown, BuildRoad, NextTurn};

    /// <summary>
    /// Item for queue in ViewQueue class.
    /// </summary>
    class ItemQueue
    {
        ItemKind kind;  /// Kind of item
        int arg1;       /// First int argument. Meaning is different according to ItemKind
                        /// BuildTown - townID
                        /// BuildRoad - roadID

        public ItemQueue(ItemKind kind, int arg1)
        {
            this.kind = kind;
            this.arg1 = arg1;
        }

        public ItemKind getItemKind() {return kind;}
        public int getArg1() { return arg1; }
    }

    /// <summary>
    /// It serves to showing AI actions not immidiatly but with delay
    /// </summary>
    class ViewQueue
    {
        Queue<ItemQueue> queue; /// queue of player actions
        Map map; /// reference to map

        private const int ENQUEUE_TIME = 100; /// Delay time between two actions
        private int lastEnque;  /// how much time last to new enque from queue

        public ViewQueue(Map map)
        {
            this.map = map;
            queue = new Queue<ItemQueue>();
            lastEnque = 0;
            queue.Clear();
        }

        /// <summary>
        /// Clears queue
        /// </summary>
        public void Clear()
        {
            queue.Clear();
        }

        /// <summary>
        /// Adds one item to queue
        /// </summary>
        /// <param name="item">Item added to queue</param>
        public void Add(ItemQueue item)
        {
            queue.Enqueue(item);
        }

        /// <summary>
        /// Is called every frame. It checks if should be shown next player action.
        /// </summary>
        /// <param name="gameTime">Time last from last frame</param>
        public void Update(GameTime gameTime)
        {
            lastEnque -= gameTime.ElapsedGameTime.Milliseconds;

            if (queue.Count > 0 && lastEnque < 0)
            {
                ItemQueue item = queue.Dequeue();
                switch (item.getItemKind())
                {
                    case ItemKind.BuildTown :
                        map.BuildTownView(item.getArg1());
                        break;

                    case ItemKind.BuildRoad :
                        map.BuildRoadView(item.getArg1());
                        break;
                    case ItemKind.NextTurn :
                        break;
                }
                lastEnque = ENQUEUE_TIME;
            }
        }

        /// <summary>
        /// Checks if there are some items waiting in queue
        /// </summary>
        /// <returns>True if there is no item in queue</returns>
        public bool getIsClear()
        {
            return queue.Count == 0;
        }
    }
}
