using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Expanze.Utils;
using Expanze.Gameplay.Map;
using Expanze.Gameplay.Map.View;
using CorePlugin;

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
        Vector2 position;
        Matrix matrix;

        public TutorialPair(String text, Vector2 position)
        {
            this.text = text;
            kind = PositionKind.POS_2D;
            this.position = position;
        }
        public TutorialPair(String text, Matrix matrix)
        {
            this.text = text;
            kind = PositionKind.POS_3D;
            this.matrix = matrix;
        }

        public PositionKind Kind
        {
            get { return kind; }
        }

        public String Text
        {
            get { return text; }
        }

        public Vector2 Position
        {
            get { return position; }
        }

        public Matrix World
        {
            get { return matrix; }
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
        MapView mapView;

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

            /*
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
             */

            pairList = new List<TutorialPair>();
            pairList.Add(new TutorialPair("Vyber žluté město", mapView.GetTownViewByID(10).getWorld()));
            triggerConstraint1 = 10;
            triggerNextItem = TriggerType.TownChoose;
            item = new TutorialItem(pairList, triggerNextItem, triggerConstraint1);
            tutorialList.Enqueue(item);

            pairList = new List<TutorialPair>();
            pairList.Add(new TutorialPair("Postav klášter", mapView.GetHexaViewByID(2).World));
            triggerConstraint1 = 2;
            triggerNextItem = TriggerType.HexaBuild;
            item = new TutorialItem(pairList, triggerNextItem, triggerConstraint1);
            tutorialList.Enqueue(item);

            pairList = new List<TutorialPair>();
            pairList.Add(new TutorialPair("Postav klášter", new Vector2(300, 300)));
            triggerConstraint1 = (int)BuildingKind.MonasteryBuilding;
            triggerNextItem = TriggerType.BuildingBuild;
            item = new TutorialItem(pairList, triggerNextItem, triggerConstraint1);
            tutorialList.Enqueue(item);

            pairList = new List<TutorialPair>();
            pairList.Add(new TutorialPair("Klikni znovu", mapView.GetTownViewByID(10).getWorld()));
            triggerConstraint1 = 10;
            triggerNextItem = TriggerType.TownUnchoose;
            item = new TutorialItem(pairList, triggerNextItem, triggerConstraint1);
            tutorialList.Enqueue(item); 

            pairList = new List<TutorialPair>();
            pairList.Add(new TutorialPair("Postav žluté město", mapView.GetTownViewByID(14).getWorld()));
            triggerConstraint1 = 14;
            triggerNextItem = TriggerType.TownBuild;
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
            mapView = GameMaster.Inst().GetMap().GetMapView();
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
                        BoundingFrustum frustum = new BoundingFrustum(GameState.view * GameState.projection);
                        ContainmentType containmentType = frustum.Contains(Vector3.Transform(new Vector3(0.0f, 0.0f, 0.0f), pair.World));
                        if (containmentType != ContainmentType.Disjoint)
                        {
                            Vector3 point3D = GameState.game.GraphicsDevice.Viewport.Project(new Vector3(0.0f, 0.0f, 0.0f), GameState.projection, GameState.view, pair.World);
                            TextWrapping.DrawStringOnScreen(pair.Text, font, Settings.colorMainText, Settings.UnScaleW(point3D.X), Settings.UnScaleH(point3D.Y), spriteBatch, 50f);
                        }
                    }
                }
                spriteBatch.End();

            }
        }

        #region Trigger Members

        public void TurnOn()
        {
            if (activeItem != null)
            {
                TriggerManager.Inst().Dettach(this, activeItem.NextItem);

                switch (activeItem.NextItem)
                {
                    case TriggerType.TownBuild:
                    case TriggerType.TownChoose:
                    case TriggerType.TownUnchoose:
                        TownView.TutorialID = -1;
                        break;
                    case TriggerType.RoadBuild:
                        RoadView.TutorialID = -1;
                        break;
                    default:
                        break;
                }
            }

            if (tutorialList.Count > 0)
            {
                activeItem = tutorialList.Dequeue();
                TriggerManager.Inst().Attach(this, activeItem.NextItem);
                switch (activeItem.NextItem)
                {
                    case TriggerType.TownBuild :
                        if (mapView.GetTownViewByID(activeItem.TriggerContraint1).getTownModel().GetIsBuild())
                            TurnOn();
                        else
                            TownView.TutorialID = activeItem.TriggerContraint1;
                        break;
                    case TriggerType.TownChoose:
                    case TriggerType.TownUnchoose:
                        TownView.TutorialID = activeItem.TriggerContraint1;
                        break;

                    case TriggerType.RoadBuild :
                        if (mapView.GetRoadViewByID(activeItem.TriggerContraint1).Model.GetIsBuild())
                            TurnOn();
                        else
                            RoadView.TutorialID = activeItem.TriggerContraint1;
                        break;
                    default :
                        break;
                }
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
