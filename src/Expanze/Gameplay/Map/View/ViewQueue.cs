using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Expanze.Gameplay.Map
{
    public enum ItemKind{BuildTown, BuildRoad, NextTurn};

    class ItemQueue
    {
        ItemKind kind;
        int arg1;

        public ItemQueue(ItemKind kind, int arg1)
        {
            this.kind = kind;
            this.arg1 = arg1;
        }

        public ItemKind getItemKind() {return kind;}
        public int getArg1() { return arg1; }
    }

    class ViewQueue
    {
        Queue<ItemQueue> queue;
        Map map;

        private const int ENQUEUE_TIME = 100;
        private int lastEnque;

        public ViewQueue(Map map)
        {
            this.map = map;
            queue = new Queue<ItemQueue>();
            lastEnque = 0;
            queue.Clear();
        }

        public void Clear()
        {
            queue.Clear();
        }

        public void Add(ItemQueue item)
        {
            queue.Enqueue(item);
        }

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

        public bool getIsClear()
        {
            return queue.Count == 0;
        }
    }
}
