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
using System.Xml;

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
            set { matrix = value; kind = PositionKind.POS_3D; }
        }
    }

    class TutorialItem
    {
        List<TutorialPair> pairList;
        int triggerConstraint1;
        TriggerType triggerNextItem;

        public TutorialItem(List<TutorialPair> pairList, TriggerType triggerNextItem) : this(pairList, triggerNextItem, -1)
        {   
        }
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

    public enum Layer
    {
        Layer1,
        Layer2,
        Layer3
    }

    class Tutorial : Trigger
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

        public void ClearAll()
        {
            tutorialList.Clear();
            ClearActiveItem();
            activeItem = null;
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

        public void Initialize(String src)
        {
            mapView = GameMaster.Inst().GetMap().GetMapView();
            tutorialList.Clear();
            LoadFromXML(src);
        }

        private void LoadFromXML(string src)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("Content/Maps/" + src);
            tutorialList.Clear();
            List<TutorialPair> pairList;
            int constraint1;

            XmlNodeList tutorialItems = xDoc.GetElementsByTagName("tutorialItem");
            foreach (XmlNode item in tutorialItems)
            {
                pairList = GetPairList(item.ChildNodes[1]);
                constraint1 = GetConstraint1(item.ChildNodes[2]);
                switch (item.FirstChild.InnerText)
                {
                    case "MessageClose" : tutorialList.Enqueue(new TutorialItem(pairList, TriggerType.MessageClose)); break;
                    case "NextTurn" : tutorialList.Enqueue(new TutorialItem(pairList, TriggerType.NextTurn)); break;
                    case "MarketOpen": tutorialList.Enqueue(new TutorialItem(pairList, TriggerType.MarketOpen)); break;
                    case "MarketClose": tutorialList.Enqueue(new TutorialItem(pairList, TriggerType.MessageClose)); break;
                    case "MarketFirstRow": tutorialList.Enqueue(new TutorialItem(pairList, TriggerType.MarketFirstRow, constraint1)); break;
                    case "MarketSecondRow": tutorialList.Enqueue(new TutorialItem(pairList, TriggerType.MarketSecondRow, constraint1)); break;
                    case "MarketChangeSources": tutorialList.Enqueue(new TutorialItem(pairList, TriggerType.MarketChangeSources)); break;
                    
                    case "RoadBuild" : 
                        for(int loop1 = 0; loop1 < pairList.Count; loop1++)
                            pairList[loop1].World = mapView.GetRoadViewByID(constraint1).World;
                        tutorialList.Enqueue(new TutorialItem(pairList, TriggerType.RoadBuild, constraint1));
                        break;
                    case "TownBuild":
                        for (int loop1 = 0; loop1 < pairList.Count; loop1++)
                            pairList[loop1].World = mapView.GetTownViewByID(constraint1).getWorld();
                        tutorialList.Enqueue(new TutorialItem(pairList, TriggerType.TownBuild, constraint1));
                        break;
                    case "TownChoose":
                        for (int loop1 = 0; loop1 < pairList.Count; loop1++)
                            pairList[loop1].World = mapView.GetTownViewByID(constraint1).getWorld();
                        tutorialList.Enqueue(new TutorialItem(pairList, TriggerType.TownChoose, constraint1));
                        break;
                    case "TownUnchoose":
                        for(int loop1 = 0; loop1 < pairList.Count; loop1++)
                            pairList[loop1].World = mapView.GetTownViewByID(constraint1).getWorld();
                        tutorialList.Enqueue(new TutorialItem(pairList, TriggerType.TownUnchoose, constraint1));
                        break;
                    case "HexaBuild" :
                        for (int loop1 = 0; loop1 < pairList.Count; loop1++)
                            pairList[loop1].World = mapView.GetHexaViewByID(constraint1).World;
                        tutorialList.Enqueue(new TutorialItem(pairList, TriggerType.HexaBuild, constraint1));
                        break;
                    case "BuildingBuild" :
                        tutorialList.Enqueue(new TutorialItem(pairList, TriggerType.BuildingBuild, constraint1));
                        break;
                }
            }
        }

        private int GetConstraint1(XmlNode item)
        {
            if (item == null)
                return -1;

            return Convert.ToInt32(item.InnerText);
        }

        private List<TutorialPair> GetPairList(XmlNode item)
        {
            List<TutorialPair> pairList = new List<TutorialPair>();

            foreach (XmlNode pairNode in item)
            {
                String text = pairNode.FirstChild.InnerText;
                Vector2 position;
                if (pairNode.ChildNodes.Count == 2)
                {
                    int x = Convert.ToInt32(pairNode.ChildNodes[1].FirstChild.InnerText);
                    int y = Convert.ToInt32(pairNode.ChildNodes[1].LastChild.InnerText);
                    position = new Vector2(x, y);
                }
                else
                    position = new Vector2();

                TutorialPair pair = new TutorialPair(text, position);
                pairList.Add(pair);
            }

            return pairList;
        }

        public void Draw(Layer layer)
        {
            if (activeItem != null)
            {
                switch (activeItem.NextItem)
                {
                    case TriggerType.MessageClose :
                        break;

                    default :
                        SpriteBatch spriteBatch = GameState.spriteBatch;
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Settings.spriteScale);
                        foreach (TutorialPair pair in activeItem.PairList)
                        {
                            if (pair.Kind == PositionKind.POS_2D && layer == Layer.Layer3)
                            {
                                TextWrapping.DrawStringOnScreen(pair.Text, font, Settings.colorMainText, pair.Position.X, pair.Position.Y, spriteBatch, 50f);
                            }
                            else if (pair.Kind == PositionKind.POS_3D && layer == Layer.Layer1)
                            {
                                BoundingFrustum frustum = new BoundingFrustum(GameState.view * GameState.projection);
                                ContainmentType containmentType = frustum.Contains(Vector3.Transform(new Vector3(0.0f, 0.0f, 0.0f), pair.World));
                                if (containmentType != ContainmentType.Disjoint)
                                {
                                    Vector3 point3D = GameState.game.GraphicsDevice.Viewport.Project(new Vector3(0.0f, 0.0f, 0.0f), GameState.projection, GameState.view, pair.World);
                                    TextWrapping.DrawStringOnScreen(pair.Text, font, Settings.colorMainText, Settings.UnScaleW(point3D.X) + pair.Position.X, Settings.UnScaleH(point3D.Y) + pair.Position.Y, spriteBatch, 50f);
                                }
                            }
                        }
                        spriteBatch.End();
                        break;
                }
            }
        }

        #region Trigger Members

        public void TurnOn()
        {
            ClearActiveItem();

            if (tutorialList.Count > 0)
            {
                activeItem = tutorialList.Dequeue();
                TriggerManager.Inst().Attach(this, activeItem.NextItem);
                switch (activeItem.NextItem)
                {
                    case TriggerType.MessageClose :
                        for(int loop1 = 1; loop1 < activeItem.PairList.Count; loop1++)
                            Message.Inst().Show(activeItem.PairList[0].Text, activeItem.PairList[loop1].Text, GameResources.Inst().GetHudTexture(HUDTexture.IconMedalTown));
                        break;

                    case TriggerType.TownBuild :
                        if (activeItem.TriggerContraint1 >= 0)
                        {
                            if (mapView.GetTownViewByID(activeItem.TriggerContraint1).getTownModel().GetIsBuild())
                                TurnOn();
                            else
                                TownView.TutorialID = activeItem.TriggerContraint1;
                        }
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
            }
        }

        private void ClearActiveItem()
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
