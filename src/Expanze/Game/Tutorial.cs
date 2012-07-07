using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Expanze.Utils;

namespace Expanze
{
    enum PositionKind {
        POS_2D,
        POS_3D
    }
    class TutorialPair
    {
        PositionKind kind;
        String text;
        Vector3 position;

        public TutorialPair(String text, Vector2 position)
        {
            this.text = text;
            kind = PositionKind.POS_2D;
            this.position = new Vector3(position.X, position.Y, 0f);
        }
        public TutorialPair(String text, Vector3 position)
        {
            this.text = text;
            kind = PositionKind.POS_3D;
            this.position = position;
        }

        public PositionKind Kind
        {
            get { return kind; }
        }

        public String Text
        {
            get { return text; }
        }

        public Vector3 Position
        {
            get { return position; }
        }
    }

    class TutorialItem
    {
        List<TutorialPair> pairList;
        int triggerConstraint1;
        TriggerType triggerNextItem;

        public TutorialItem(List<TutorialPair> pairList, TriggerType triggerNextItem, int triggerConstraint1)
        {
            this.pairList = pairList;
            this.triggerNextItem = triggerNextItem;
            this.triggerConstraint1 = triggerConstraint1;
        }

        public List<TutorialPair> PairList { get { return pairList; } }
        public TriggerType NextItem { get { return triggerNextItem; } }
        public int TriggerContraint1
        {
            get
            {
                return triggerConstraint1;
            }
        }
    }

    class Tutorial : GameComponent, Trigger
    {
        Queue<TutorialItem> tutorialList;
        TutorialItem activeItem;
        SpriteFont font;

        private static Tutorial tutorial;
        public static Tutorial Inst()
        {
            if (tutorial == null)
                tutorial = new Tutorial();
            return tutorial;
        }
        private void LoadFromXML()
        {
            List<TutorialPair> pairList;
            TriggerType triggerNextItem;
            TutorialItem item;
            int triggerConstraint1;

            pairList = new List<TutorialPair>();
            pairList.Add(new TutorialPair("Ahoj, dej další kolo", new Vector2(0, 100)));
            pairList.Add(new TutorialPair("Tady je to skvely", new Vector2(200, 50)));
            triggerConstraint1 = 0;
            triggerNextItem = TriggerType.NextTurn;
            item = new TutorialItem(pairList, triggerNextItem, triggerConstraint1);
            tutorialList.Enqueue(item);

            pairList = new List<TutorialPair>();
            pairList.Add(new TutorialPair("Otevři market", new Vector2(300, 50)));
            triggerConstraint1 = 0;
            triggerNextItem = TriggerType.MarketOpen;
            item = new TutorialItem(pairList, triggerNextItem, triggerConstraint1);
            tutorialList.Enqueue(item);

            pairList = new List<TutorialPair>();
            pairList.Add(new TutorialPair("Vyměň kámen", new Vector2(300, 100)));
            triggerConstraint1 = 2;
            triggerNextItem = TriggerType.MarketFirstRow;
            item = new TutorialItem(pairList, triggerNextItem, triggerConstraint1);
            tutorialList.Enqueue(item);

            pairList = new List<TutorialPair>();
            pairList.Add(new TutorialPair("Za rudu", new Vector2(300, 100)));
            triggerConstraint1 = 4;
            triggerNextItem = TriggerType.MarketSecondRow;
            item = new TutorialItem(pairList, triggerNextItem, triggerConstraint1);
            tutorialList.Enqueue(item);

            pairList = new List<TutorialPair>();
            pairList.Add(new TutorialPair("Posuvníkem navol množství a potvrď", new Vector2(300, 100)));
            triggerConstraint1 = 0;
            triggerNextItem = TriggerType.MarketChangeSources;
            item = new TutorialItem(pairList, triggerNextItem, triggerConstraint1);
            tutorialList.Enqueue(item);

            pairList = new List<TutorialPair>();
            pairList.Add(new TutorialPair("A zavři", new Vector2(300, 100)));
            triggerConstraint1 = 0;
            triggerNextItem = TriggerType.MarketClose;
            item = new TutorialItem(pairList, triggerNextItem, triggerConstraint1);
            tutorialList.Enqueue(item);
        }
        private Tutorial()
        {
            tutorialList = new Queue<TutorialItem>();
            font = GameResources.Inst().GetFont(EFont.MedievalMedium);
        }

        public override void Initialize()
        {
            base.Initialize();
            tutorialList.Clear();
            LoadFromXML();
            TurnOn();
        }

        public override void Draw(GameTime gameTime)
        {
            if (activeItem != null)
            {
                SpriteBatch spriteBatch = GameState.spriteBatch;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Settings.spriteScale);
                foreach (TutorialPair pair in activeItem.PairList)
                {
                    if (pair.Kind == PositionKind.POS_2D)
                    {
                        TextWrapping.DrawStringOnScreen(pair.Text, font, Settings.colorMainText, pair.Position.X, pair.Position.Y, spriteBatch, 50f);
                    }
                    else
                    {

                    }
                }
                spriteBatch.End();
            }
        }

        #region Trigger Members

        public void TurnOn()
        {
            if (tutorialList.Count > 0)
            {
                if(activeItem != null)
                    TriggerManager.Inst().Dettach(this, activeItem.NextItem);

                activeItem = tutorialList.Dequeue();
                TriggerManager.Inst().Attach(this, activeItem.NextItem);
            }
            else
            {
                activeItem = null;
                Message.Inst().Show("Konec tutoriálu", "Teď už víš vše, abys dokázal vyhrát na této mapě. Hodně štěstí Ti přeji.", GameResources.Inst().GetHudTexture(HUDTexture.HammersPassive));
            }
        }

        public int Restriction1()
        {
            if (activeItem == null)
                return -1;

            return activeItem.TriggerContraint1;
        }

        #endregion
    }
}
