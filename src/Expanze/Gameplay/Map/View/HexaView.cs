using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Expanze.Gameplay.Map.View;
using CorePlugin;
using Microsoft.Xna.Framework.Graphics;
using Expanze.Gameplay.Map;
using Expanze.Gameplay;

namespace Expanze
{

    class BuildingPromptItem : PromptItem
    {
        int townID;
        int hexaID;
        BuildingKind kind;

        public BuildingPromptItem(int townID, int hexaID, BuildingKind kind, String title, String description, SourceAll source, bool isSourceCost, Texture2D icon)
            : base(title, description, source, isSourceCost, false, icon)
        {
            this.townID = townID;
            this.hexaID = hexaID;
            this.kind = kind;
        }

        public override void Execute()
        {
            GameState.map.GetMapController().BuildBuildingInTown(townID, hexaID, kind);
        }

        public override string TryExecute()
        {
            GameMaster gm = GameMaster.Inst();
            TownModel town = GameState.map.GetTownByID(townID);
            int buildingPos = town.FindBuildingByHexaID(hexaID);
            HexaModel hexa = town.GetHexa(buildingPos);

            BuildingBuildError error = town.CanActivePlayerBuildBuildingInTown(buildingPos, kind);
            switch (error)
            {
                case BuildingBuildError.NoSources: return "";
            }

            return base.TryExecute();
        }
    }

    class BuildingItemQueue : ItemQueue
    {
        int townID;
        int pos;

        public BuildingItemQueue(MapView mapView, int townID, int pos)
            : base(mapView)
        {
            this.townID = townID;
            this.pos = pos;
        }

        public override void Execute()
        {
            mapView.BuildBuildingView(townID, pos);
            base.Execute();
        }
    } 

    class HexaView
    {
        private static int              activeHexaID;     // focused hexa with keys
        private static bool             activeHexaEnter;  // press someone enter when was activeHexaID != -1?
        protected int                   hexaID;           // from counter, useable for picking
        Color                           pickHexaColor;    // color o hexa in render texture
        protected PickVariables         pickVars;
        protected Matrix                world;            // wordl position of Hex
        protected HexaModel model;                        // reference to model
        protected RoadView[] roadView;
        protected TownView[] townView;
        protected HexaKind kind;
        bool centerOnScreen;
        private HexaView[] hexaNeighbours;
        private int x, y;

        public HexaView(HexaModel model, int x, int y)
        {
            this.x = x;
            this.y = y;

            this.model = model;
            this.hexaID = model.GetID();
            this.kind = model.GetKind();

            if (kind != HexaKind.Water)
            {
                this.pickHexaColor = new Color(1.0f - this.hexaID / 256.0f, 0.0f, 0.0f);

                pickVars = new PickVariables(pickHexaColor);

                roadView = new RoadView[(int)RoadPos.Count];
                townView = new TownView[(int)TownPos.Count];
            }
        }

        public void SetWorld(Matrix m)
        {
            world = m;
        }

        public void CreateRoadView(RoadPos pos, Matrix relative)
        {
            roadView[(int)pos] = new RoadView(model.GetRoad(pos), relative * world);
        }

        public bool GetIsCenterOnScreen() { return centerOnScreen; }
        public RoadView GetRoadView(RoadPos pos) { return roadView[(int)pos]; }
        public static void SetActiveHexaID(int id) { activeHexaID = id; }
        public static int GetActiveHexaID() { return activeHexaID; }
        public static void ActiveHexaEnter() { activeHexaEnter = true; }

        public void SetRoadView(RoadPos pos, RoadView road)
        {
            roadView[(int)pos] = road;
        }

        public void CreateTownView(TownPos pos, Matrix relative)
        {
            townView[(int)pos] = new TownView(model.getTown(pos), relative * world);
        }

        public TownView GetTownView(TownPos pos) { return townView[(int)pos]; }

        public void SetTownView(TownPos pos, TownView town)
        {
            townView[(int)pos] = town;
        }

        private void DrawHexaIcon(SpriteBatch spriteBatch, Vector2 pos, Texture2D passive, Texture2D active)
        {
            Texture2D text = passive;
            spriteBatch.Draw(text, new Vector2(pos.X - (text.Width >> 1), pos.Y - (text.Height >> 1)), Color.White);
            if (!pickVars.pickActive && activeHexaID != hexaID)
            {
                text = active;
                spriteBatch.Draw(text, new Vector2(pos.X - (text.Width >> 1), pos.Y - (text.Height >> 1)), Color.White);
            }
        }
        private void DrawHexaIcon(SpriteBatch spriteBatch, Vector2 pos, HUDTexture passive, HUDTexture active)
        {
            DrawHexaIcon(spriteBatch, pos, GameResources.Inst().GetHudTexture(passive), GameResources.Inst().GetHudTexture(active));
        }

        private void DrawHexaNumber(SpriteBatch spriteBatch, Vector2 pos)
        {
            /* Draw x, y coord
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalMedium), "[" + x + ";" + y + "]", new Vector2(pos.X + 2, pos.Y + 2), Color.Black);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalMedium), "[" + x + ";" + y + "]", new Vector2(pos.X, pos.Y), Color.White);
            return;
             */

            if (model.GetCurrentSource() == 0) // desert
                return;

            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalMedium), model.GetCurrentSource() + "", new Vector2(pos.X + 1, pos.Y + 1), Color.Black);

            Color numberColor;
            if (pickVars.pickActive)
                numberColor = Color.Red;
            else
                numberColor = Color.DarkRed;

            if (hexaID == activeHexaID)
                numberColor = Color.IndianRed;

            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalMedium), model.GetCurrentSource() + "", pos, numberColor);
        }

        public void Draw2D()
        {
            BoundingFrustum frustum = new BoundingFrustum(GameState.view * GameState.projection);
            ContainmentType containmentType = frustum.Contains(Vector3.Transform(new Vector3(0.0f, 0.0f, 0.0f), world));

            centerOnScreen = containmentType != ContainmentType.Disjoint;

            if (kind == HexaKind.Water)
            {
                return;
            }

            if (containmentType != ContainmentType.Disjoint)
            {
                Vector3 point3D = GameState.game.GraphicsDevice.Viewport.Project(new Vector3(0.0f, 0.0f, 0.0f), GameState.projection, GameState.view, world);
                Vector2 point2D = new Vector2();
                Vector2 posHexaIcon = new Vector2();
                point2D.X = point3D.X;
                point2D.Y = point3D.Y;
                posHexaIcon = point2D;

                Texture2D playerIcon = GameResources.Inst().GetHudTexture(HUDTexture.PlayerColor);
                Rectangle playerIconRect = new Rectangle((int)point2D.X - playerIcon.Width / 2, (int)point2D.Y - playerIcon.Height / 2, playerIcon.Width, playerIcon.Height);

                SpriteBatch spriteBatch = GameState.spriteBatch;

                Vector2 stringCenter = GameResources.Inst().GetFont(EFont.MedievalMedium).MeasureString(model.GetCurrentSource() + "") * 0.5f;

                /* x, y coord
                Vector2 stringCenter = GameResources.Inst().GetFont(EFont.MedievalMedium).MeasureString("[" + x + ";" + y + "]") * 0.5f;
                */

                // now subtract the string center from the text position to find correct position 
                point2D.X = (int)(point2D.X - stringCenter.X);
                point2D.Y = (int)(point2D.Y - stringCenter.Y);

                spriteBatch.Begin();

                /* x, y coord
                if (kind == HexaKind.Water)
                {
                    DrawHexaNumber(spriteBatch, point2D);
                    spriteBatch.End();
                    return;
                }*/

                bool drawNumber = true;
                TownModel tempTown = null;
                if (GameMaster.Inst().GetFortState() == EFortState.Normal)
                {
                    foreach (TownView town in townView)
                    {
                        if (town.getIsMarked())
                        {
                            tempTown = town.getTownModel();
                            if (tempTown.GetBuildingKind(hexaID) != BuildingKind.NoBuilding || 
                                tempTown.GetOwner() == GameMaster.Inst().GetActivePlayer())
                                drawNumber = false;
                            break;
                        }
                    }
                }
                else
                {
                    if (!IsInFortRadius())
                        drawNumber = false;
                }

                if (GameMaster.Inst().GetFortState() != EFortState.Normal)
                {
                    HUDTexture textPassive = HUDTexture.DestroyPassive, textActive = HUDTexture.DestroyActive;
                    if (IsInFortRadius())
                    {
                        switch(GameMaster.Inst().GetFortState())
                        {
                            case EFortState.CapturingHexa:
                                textPassive = HUDTexture.SwordsPassive;
                                textActive = HUDTexture.SwordsActive; 
                                break;
                            case EFortState.DestroyingHexa:
                                textPassive = HUDTexture.DestroyPassive;
                                textActive = HUDTexture.DestroyActive;
                                break;
                        }
                        DrawHexaIcon(spriteBatch, posHexaIcon, textPassive, textActive);
                    }
                }

                if (model.GetCaptured())
                {
                    spriteBatch.Draw(playerIcon, playerIconRect, model.GetCapturedPlayer().GetColor());
                }

                if (drawNumber)
                {
                    DrawHexaNumber(spriteBatch, point2D);
                }
                else if(GameMaster.Inst().GetFortState() == EFortState.Normal)
                {
                    if (tempTown.GetBuildingKind(hexaID) == BuildingKind.NoBuilding && tempTown.GetOwner() == GameMaster.Inst().GetActivePlayer())
                    {
                        DrawHexaIcon(spriteBatch, posHexaIcon, HUDTexture.HammersPassive, HUDTexture.HammersActive);
                        DrawHexaNumber(spriteBatch, point2D);
                    }
                    else
                    {
                            DrawHexaIcon(spriteBatch, posHexaIcon, tempTown.GetSpecialBuilding(hexaID).GetIconPassive(), tempTown.GetSpecialBuilding(hexaID).GetIconActive());
                            if (model.GetCaptured())
                            {
                                spriteBatch.Draw(playerIcon, playerIconRect, model.GetCapturedPlayer().GetColor());
                            }
                            if(tempTown.GetBuildingKind(hexaID) == BuildingKind.SourceBuilding)
                                DrawHexaNumber(spriteBatch, point2D);
                    }
                }
                spriteBatch.End();
            }
        }

        public virtual void Draw(GameTime gameTime)
        {

            Model m = GameResources.Inst().GetHexaModel(kind);
            Texture2D texture = GameResources.Inst().GetHexaTexture(kind);
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            GameState.game.GraphicsDevice.RasterizerState = GameState.rasterizerState;

            Matrix rotation;
            rotation = (hexaID % 6 == 0) ? Matrix.Identity : Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (hexaID % 6));
            Matrix tempMatrix = Matrix.CreateScale(0.00028f) * rotation;

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    if(kind == HexaKind.Forest)
                        effect.Texture = texture;
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = GameState.MaterialAmbientColor;
                    effect.DirectionalLight0.Direction = GameState.LightDirection;
                    effect.DirectionalLight0.DiffuseColor = GameState.LightDiffusionColor;
                    effect.DirectionalLight0.SpecularColor = GameState.LightSpecularColor;
                    effect.DirectionalLight0.Enabled = true;
                    effect.World = transforms[mesh.ParentBone.Index] * tempMatrix * world;
                    effect.View = GameState.view;
                    effect.Projection = GameState.projection;
                }
                mesh.Draw();
            }

            DrawBuildings(gameTime);

            for (int loop1 = 0; loop1 < roadView.Length; loop1++)
                if (model.GetRoadOwner(loop1))
                    roadView[loop1].Draw(gameTime);


            for (int loop1 = 0; loop1 < roadView.Length; loop1++)
                if (model.GetTownOwner(loop1))
                    townView[loop1].Draw(gameTime);
        }

        public virtual void DrawBuildings(GameTime gameTime)
        {
            for (int loop1 = 0; loop1 < 6; loop1++)
            {
                if (!townView[loop1].getBuildingIsBuild(model.GetID()))
                {
                    continue;
                }

                Model m;
                Matrix rotation;

                rotation = (loop1 == 4) ? Matrix.Identity : Matrix.CreateRotationY(((float)Math.PI / 3.0f) * -(loop1 - 4));
                //rotation = Matrix.Identity;
                Matrix tempMatrix = Matrix.CreateScale(0.00028f) * rotation;

                int roofID = -1;
                switch(model.getTown((CorePlugin.TownPos)loop1).GetBuildingKind(model.GetID()))
                {
                    case BuildingKind.NoBuilding :
                        m = null;
                        break;
                    case BuildingKind.SourceBuilding :
                        switch (kind)
                        {
                            case HexaKind.Cornfield :
                                m = GameResources.Inst().GetBuildingModel(BuildingModel.Mill);
                                break;
                            case HexaKind.Forest :
                                m = GameResources.Inst().GetBuildingModel(BuildingModel.Saw);
                                break;
                            default :
                                m = GameResources.Inst().GetBuildingModel(BuildingModel.PastureHouse);
                                //roofID = 0;
                                break;
                        }
                        break;
                    case BuildingKind.FortBuilding :
                        m = GameResources.Inst().GetBuildingModel(BuildingModel.Fort);
                        break;
                    case BuildingKind.MarketBuilding:
                        m = GameResources.Inst().GetBuildingModel(BuildingModel.Market);
                        break;
                    case BuildingKind.MonasteryBuilding:
                        m = GameResources.Inst().GetBuildingModel(BuildingModel.Monastery);
                        break;
                    default :
                        m = null;
                        break;
                }
                
                if(m == null)
                    continue;

                Matrix[] transforms = new Matrix[m.Bones.Count];
                m.CopyAbsoluteBoneTransformsTo(transforms);

                int a = 0;
                foreach (ModelMesh mesh in m.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        if (a == roofID)
                        {
                            Vector3 color = model.getTown((CorePlugin.TownPos)loop1).GetOwner().GetColor().ToVector3();
                            effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);
                            effect.DiffuseColor = color * 0.6f;
                            effect.AmbientLightColor = color * 0.3f;
                        }

                        effect.LightingEnabled = true;
                        effect.AmbientLightColor = GameState.MaterialAmbientColor;
                        effect.DirectionalLight0.Direction = GameState.LightDirection;
                        effect.DirectionalLight0.DiffuseColor = GameState.LightDiffusionColor;
                        effect.DirectionalLight0.SpecularColor = GameState.LightSpecularColor;
                        effect.DirectionalLight0.Enabled = true;
                        effect.World = transforms[mesh.ParentBone.Index] * tempMatrix * world;
                        effect.View = GameState.view;
                        effect.Projection = GameState.projection;
                    }
                    a++;
                    mesh.Draw();
                }
            }
        }

        public void DrawPickableAreas()
        {
            // Water is not pickable
            if (kind == HexaKind.Water)
                return;

            Model m = GameResources.Inst().GetShape(GameResources.SHAPE_CIRCLE);
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix mWorld = Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, 0.0f)) * Matrix.CreateScale(0.8f) * world;

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = pickHexaColor.ToVector3(); //new Vector3((float) number / counter, (float) number / counter, (float) number / counter);
                    effect.World = transforms[mesh.ParentBone.Index] * mWorld;
                    effect.View = GameState.view;
                    effect.Projection = GameState.projection;
                }
                mesh.Draw();
            }

            for (int loop1 = 0; loop1 < roadView.Length; loop1++)
                if (model.GetRoadOwner(loop1))
                    roadView[loop1].DrawPickableAreas();


            for (int loop1 = 0; loop1 < townView.Length; loop1++)
                if (model.GetTownOwner(loop1))
                    townView[loop1].DrawPickableAreas();
        }

        /// <summary>
        /// What happens when you click/press enter on hexa
        /// </summary>
        public void ActionOnHexa()
        {
            switch (GameMaster.Inst().GetFortState())
            {
                case EFortState.Normal:
                    for (int loop1 = 0; loop1 < townView.Length; loop1++)
                    {
                        if (townView[loop1].getIsMarked() &&
                            kind != HexaKind.Nothing &&
                            kind != HexaKind.Water &&
                            kind != HexaKind.Null)
                        {
                            PromptWindow.Mod mod;
                            switch (townView[loop1].getTownModel().GetBuildingKind(hexaID))
                            {
                                case BuildingKind.NoBuilding:
                                    if (townView[loop1].getTownModel().GetOwner() == GameMaster.Inst().GetActivePlayer())
                                    {
                                        String titleWindow = "";
                                        String titleBuilding = "";
                                        String descriptionBuilding = "";
                                        Texture2D icon = null;
                                        switch (kind)
                                        {
                                            case HexaKind.Mountains:
                                                titleWindow = Strings.HEXA_NAME_MOUNTAINS;
                                                titleBuilding = Strings.PROMT_TITLE_WANT_TO_BUILD_MINE;
                                                descriptionBuilding = Strings.PROMPT_DESCRIPTION_WANT_TO_BUILD_MINE;
                                                icon = GameResources.Inst().GetHudTexture(HUDTexture.IconMine); break;
                                            case HexaKind.Forest:
                                                titleWindow = Strings.HEXA_NAME_FOREST;
                                                titleBuilding = Strings.PROMT_TITLE_WANT_TO_BUILD_SAW;
                                                descriptionBuilding = Strings.PROMPT_DESCRIPTION_WANT_TO_BUILD_SAW;
                                                icon = GameResources.Inst().GetHudTexture(HUDTexture.IconSaw); break;
                                            case HexaKind.Cornfield:
                                                titleWindow = Strings.HEXA_NAME_CORNFIELD;
                                                titleBuilding = Strings.PROMT_TITLE_WANT_TO_BUILD_MILL;
                                                descriptionBuilding = Strings.PROMPT_DESCRIPTION_WANT_TO_BUILD_MILL;
                                                icon = GameResources.Inst().GetHudTexture(HUDTexture.IconMill); break;
                                            case HexaKind.Pasture:
                                                titleWindow = Strings.HEXA_NAME_PASTURE;
                                                titleBuilding = Strings.PROMT_TITLE_WANT_TO_BUILD_STEPHERD;
                                                descriptionBuilding = Strings.PROMPT_DESCRIPTION_WANT_TO_BUILD_STEPHERD;
                                                icon = GameResources.Inst().GetHudTexture(HUDTexture.IconStepherd); break;
                                            case HexaKind.Stone:
                                                titleWindow = Strings.HEXA_NAME_STONE;
                                                titleBuilding = Strings.PROMT_TITLE_WANT_TO_BUILD_QUARRY;
                                                descriptionBuilding = Strings.PROMPT_DESCRIPTION_WANT_TO_BUILD_QUARRY;
                                                icon = GameResources.Inst().GetHudTexture(HUDTexture.IconQuarry); break;
                                            case HexaKind.Desert:
                                                titleWindow = Strings.HEXA_NAME_DESERT;
                                                break;
                                        }

                                        int townID = townView[loop1].getTownModel().GetTownID();
                                        mod = (townView[loop1].getTownModel().GetOwner() == GameMaster.Inst().GetActivePlayer()) ? PromptWindow.Mod.Buyer : PromptWindow.Mod.Viewer;
                                        PromptWindow.Inst().Show(mod, titleWindow, true);
                                        if (kind != HexaKind.Desert)
                                            PromptWindow.Inst().AddPromptItem(
                                                new BuildingPromptItem(townID,
                                                                   hexaID,
                                                                   BuildingKind.SourceBuilding,
                                                                   titleBuilding,
                                                                   descriptionBuilding,
                                                                   (SourceAll) model.GetSourceBuildingCost(), true,
                                                                   icon));

                                        if (kind != HexaKind.Mountains)
                                        {
                                            if(!Settings.banMonastery)
                                                PromptWindow.Inst().AddPromptItem(MonasteryModel.GetPromptItemBuildMonastery(townID, hexaID));
                                            if (!Settings.banMarket)
                                                PromptWindow.Inst().AddPromptItem(MarketModel.GetPromptItemBuildMarket(townID, hexaID));
                                            if (!Settings.banFort)
                                                PromptWindow.Inst().AddPromptItem(FortModel.GetPromptItemBuildFort(townID, hexaID));
                                        }
                                    }
                                    break;

                                default:
                                    mod = (townView[loop1].getTownModel().GetOwner() == GameMaster.Inst().GetActivePlayer()) ? PromptWindow.Mod.Buyer : PromptWindow.Mod.Viewer;
                                    townView[loop1].getTownModel().GetSpecialBuilding(hexaID).SetPromptWindow(mod);
                                    break;
                            }
                        }
                    }
                    break;

                case EFortState.DestroyingHexa:
                    if (IsInFortRadius())
                    {
                        GameState.map.GetMapController().DestroyHexa(hexaID, null);
                        GameMaster.Inst().SetFortState(EFortState.Normal);
                    }
                    break;

                case EFortState.CapturingHexa:
                    if (IsInFortRadius())
                    {
                        GameState.map.GetMapController().CaptureHexa(hexaID, HexaModel.GetHexaFort());
                        GameMaster.Inst().SetFortState(EFortState.Normal);
                    }
                    break;
            }
        }

        public void HandlePickableAreas(Color c)
        {
            // Water is not pickable
            if (kind == HexaKind.Water)
                return;

            for (int loop1 = 0; loop1 < roadView.Length; loop1++)
                if (model.GetRoadOwner(loop1))
                    roadView[loop1].HandlePickableAreas(c);

            for (int loop1 = 0; loop1 < townView.Length; loop1++)
            {
                if (model.GetTownOwner(loop1))
                {
                    townView[loop1].HandlePickableAreas(c);
                }
            }

            Map.SetPickVariables(c == pickHexaColor, pickVars);

            if (pickVars.pickNewPress ||
                (activeHexaEnter && activeHexaID == hexaID))
            {
                pickVars.pickNewPress = false;
                activeHexaEnter = false;

                ActionOnHexa();
            }
        }

        private bool IsInFortRadius()
        {
            return model.IsInFortRadius();
        }

        public virtual RoadView GetRoadViewByID(int roadID)
        {
            for (int loop1 = 0; loop1 < roadView.Length; loop1++)
                if (model.GetRoadOwner(loop1))
                {
                    if (roadID == roadView[loop1].getRoadID())
                        return roadView[loop1];
                }

            return null;
        }

        public virtual TownView GetTownByID(int townID)
        {
            for (int loop1 = 0; loop1 < townView.Length; loop1++)
                if (model.GetTownOwner(loop1))
                {
                    if (townID == townView[loop1].getTownID())
                        return townView[loop1];
                }

            return null;
        }

        public Matrix GetWorldMatrix()
        {
            return world;
        }

        public bool IsOnScreen()
        {
            foreach (HexaView view in hexaNeighbours)
            {
                if (view == null || view.GetIsCenterOnScreen())
                    return true;
            }
            return centerOnScreen;
        }

        public void SetHexaNeighbours(HexaView[] neighboursView)
        {
            hexaNeighbours = new HexaView[neighboursView.Length];
            for (int loop1 = 0; loop1 < neighboursView.Length; loop1++)
                hexaNeighbours[loop1] = neighboursView[loop1];

        }
    }
}
