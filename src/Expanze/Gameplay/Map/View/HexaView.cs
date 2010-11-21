using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Expanze.Gameplay.Map.View;
using CorePlugin;
using Microsoft.Xna.Framework.Graphics;
using Expanze.Gameplay.Map;

namespace Expanze
{
    class HexaView
    {
        protected int                           hexaID;           // from counter, useable for picking
        Color                         pickHexaColor;    // color o hexa in render texture
        protected PickVariables         pickVars;
        protected Matrix                world;   // wordl position of Hex
        protected HexaModel model;    // reference to model
        protected RoadView[] roadView;
        protected TownView[] townView;
        protected HexaKind kind;
        public HexaView(HexaModel model)
        {
            this.model = model;
            this.hexaID = model.getID();
            this.kind = model.getType();
            this.pickHexaColor = new Color(this.hexaID / 256.0f, 0.0f, 0.0f);

            pickVars = new PickVariables(pickHexaColor);

            roadView = new RoadView[(int) RoadPos.Count];
            townView = new TownView[(int) TownPos.Count];
        }

        public void setWorld(Matrix m)
        {
            world = m;
        }

        public void createRoadView(RoadPos pos, Matrix relative)
        {
            roadView[(int)pos] = new RoadView(model.getRoad(pos), relative * world);
        }

        public RoadView getRoadView(RoadPos pos) { return roadView[(int)pos]; }

        public void setRoadView(RoadPos pos, RoadView road)
        {
            roadView[(int)pos] = road;
        }

        public void createTownView(TownPos pos, Matrix relative)
        {
            townView[(int)pos] = new TownView(model.getTown(pos), relative * world);
        }

        public TownView getTownView(TownPos pos) { return townView[(int)pos]; }

        public void setTownView(TownPos pos, TownView town)
        {
            townView[(int)pos] = town;
        }

        public void Draw2D()
        {
            if (kind == HexaKind.Desert)
                return;

            BoundingFrustum frustum = new BoundingFrustum(GameState.view * GameState.projection);
            ContainmentType containmentType = frustum.Contains(Vector3.Transform(new Vector3(0.0f, 0.0f, 0.0f), world));

            if (containmentType != ContainmentType.Disjoint)
            {
                Vector3 point3D = GameState.game.GraphicsDevice.Viewport.Project(new Vector3(0.0f, 0.0f, 0.0f), GameState.projection, GameState.view, world);
                Vector2 point2D = new Vector2();
                point2D.X = point3D.X;
                point2D.Y = point3D.Y;
                SpriteBatch spriteBatch = GameState.spriteBatch;

                Vector2 stringCenter = GameState.hudMaterialsFont.MeasureString(model.getValue() + "") * 0.5f;

                // now subtract the string center from the text position to find correct position 
                point2D.X = (int)(point2D.X - stringCenter.X);
                point2D.Y = (int)(point2D.Y - stringCenter.Y);

                Color numberColor;

                spriteBatch.Begin();

                spriteBatch.DrawString(GameState.hudMaterialsFont, model.getValue() + "", new Vector2(point2D.X + 1, point2D.Y + 1), Color.Black);
                if (pickVars.pickActive)
                    numberColor = Color.BlueViolet;
                else
                    numberColor = Color.DarkRed;
                spriteBatch.DrawString(GameState.hudMaterialsFont, model.getValue() + "", point2D, numberColor);
                spriteBatch.End();
            }
        }

        public void Draw(GameTime gameTime)
        {
            Model m = GameState.map.getHexaModel(kind);
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GameState.game.GraphicsDevice.RasterizerState = rasterizerState;

            Matrix rotation;
            rotation = (hexaID % 6 == 0) ? Matrix.Identity : Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (hexaID % 6));
            Matrix tempMatrix = ((kind == HexaKind.Desert || kind == HexaKind.Forest || kind == HexaKind.Mountains || kind == HexaKind.Pasture) ? Matrix.CreateScale(0.00028f) * rotation : Matrix.CreateRotationZ((float)Math.PI));



            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = GameState.MaterialAmbientColor;
                    effect.DirectionalLight0.Direction = GameState.LightDirection;
                    effect.DirectionalLight0.DiffuseColor = GameState.LightDiffusionColor;
                    effect.DirectionalLight0.SpecularColor = GameState.LightSpecularColor;
                    effect.DirectionalLight0.Enabled = true;

                    /*
                    if (pickVars.pickActive)
                        effect.EmissiveColor = new Vector3(0.3f, 0.0f, 0.0f);
                    else
                        effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);
                    */
                    effect.World = transforms[mesh.ParentBone.Index] * tempMatrix * world;
                    effect.View = GameState.view;
                    effect.Projection = GameState.projection;
                }
                mesh.Draw();
            }

            DrawBuildings(gameTime);

            for (int loop1 = 0; loop1 < roadView.Length; loop1++)
                if (model.getRoadOwner(loop1))
                    roadView[loop1].Draw(gameTime);


            for (int loop1 = 0; loop1 < roadView.Length; loop1++)
                if (model.getTownOwner(loop1))
                    townView[loop1].draw(gameTime);
        }

        public virtual void DrawBuildings(GameTime gameTime)
        {

        }

        public void DrawPickableAreas()
        {
            Model m = GameState.map.getShape(Map.SHAPE_CIRCLE);
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
                if (model.getRoadOwner(loop1))
                    roadView[loop1].DrawPickableAreas();


            for (int loop1 = 0; loop1 < townView.Length; loop1++)
                if (model.getTownOwner(loop1))
                    townView[loop1].drawPickableAreas();
        }

        public void HandlePickableAreas(Color c)
        {
            for (int loop1 = 0; loop1 < roadView.Length; loop1++)
                if (model.getRoadOwner(loop1))
                    roadView[loop1].HandlePickableAreas(c);

            for (int loop1 = 0; loop1 < townView.Length; loop1++)
            {
                if (model.getTownOwner(loop1))
                {
                    townView[loop1].handlePickableAreas(c);
                }
            }

            Map.SetPickVariables(c == pickHexaColor, pickVars);

            if (pickVars.pickNewPress)
            {
                for (int loop1 = 0; loop1 < townView.Length; loop1++)
                {
                    if (townView[loop1].getIsMarked() && 
                        kind != HexaKind.Desert &&
                        kind != HexaKind.Nothing &&
                        kind != HexaKind.Water &&
                        kind != HexaKind.Null)
                    {
                        WindowPromt wP = GameState.windowPromt;

                        String title = "";
                        switch (kind)
                        {
                            case HexaKind.Mountains: title = Strings.PROMT_TITLE_WANT_TO_BUILD_MINE; break;
                            case HexaKind.Forest: title = Strings.PROMT_TITLE_WANT_TO_BUILD_SAW; break;
                            case HexaKind.Cornfield: title = Strings.PROMT_TITLE_WANT_TO_BUILD_MILL; break;
                            case HexaKind.Pasture: title = Strings.PROMT_TITLE_WANT_TO_BUILD_STEPHERD; break;
                            case HexaKind.Stone: title = Strings.PROMT_TITLE_WANT_TO_BUILD_QUARRY; break;
                        }

                        wP.showPromt(title, wP.BuildBuildingInTown, model.getSourceBuildingCost());
                        wP.setArgInt1(townView[loop1].getTownModel().getTownID());
                        wP.setArgInt2(hexaID);
                    }
                }
            }
        }

        public RoadView GetRoadViewByID(int roadID)
        {
            for (int loop1 = 0; loop1 < roadView.Length; loop1++)
                if (model.getRoadOwner(loop1))
                {
                    if (roadID == roadView[loop1].getRoadID())
                        return roadView[loop1];
                }

            return null;
        }

        public TownView GetTownByID(int townID)
        {
            for (int loop1 = 0; loop1 < townView.Length; loop1++)
                if (model.getTownOwner(loop1))
                {
                    if (townID == townView[loop1].getTownID())
                        return townView[loop1];
                }

            return null;
        }
    }
}
