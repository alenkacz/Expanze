using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Expanze
{
    class Road
    {
        private Player playerOwner;
        private bool isBuild;

        private int number;
        public static int counter = 0;
        private Color pickRoadColor;
        private Boolean pickActive = false;         // if mouse is over pickable area
        private Boolean pickNewActive = false;      // if mouse is newly over pickable area
        private Boolean pickNewPress = false;       // if mouse press on pickable area newly
        private Boolean pickPress = false;          // if mouse press on pickable area
        private Boolean pickNewRelease = false;     // if mouse is newly release above pickable area
        private Matrix world;

        private Town[] neighbour; // every road must have two neighbour

        public Road(Matrix world)
        {
            this.world = world;

            number = ++counter;
            this.pickRoadColor = new Color(0.0f, counter / 256.0f, 0.0f);

            neighbour = new Town[2];
            isBuild = false;
            playerOwner = null;
        }

        public static void resetCounter() { counter = 0; }

        public Player getOwner() { return playerOwner; }

        public void SetTownNeighbours(Town one, Town two)
        {
            neighbour[0] = one;
            neighbour[1] = two;
        }

        public void Draw(GameTime gameTime)
        {
            GameMaster gm = GameMaster.getInstance();
            if ((pickActive && gm.getState() == GameMaster.State.StateGame) || isBuild)
            {
                Model m = GameState.map.getRoadModel();
                Matrix[] transforms = new Matrix[m.Bones.Count];
                m.CopyAbsoluteBoneTransformsTo(transforms);

                Matrix mWorld = Matrix.CreateTranslation(new Vector3(0.0f, 0.1f, 0.0f)) * Matrix.CreateScale(0.023f) * world;

                foreach (ModelMesh mesh in m.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);
                        effect.World = transforms[mesh.ParentBone.Index] * mWorld;
                        effect.View = GameState.view;
                        effect.Projection = GameState.projection;
                    }
                    mesh.Draw();
                }
            }
        }

        public void DrawPickableAreas()
        {
            Model m = GameState.map.getRectangleShape();
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix mWorld = Matrix.CreateTranslation(new Vector3(0.0f, 0.05f, 0.0f)) * Matrix.CreateScale(0.2f) * world;

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.DiffuseColor = new Vector3(0.0f, 0.0f, 0.0f);
                    effect.EmissiveColor = pickRoadColor.ToVector3();
                    effect.World = transforms[mesh.ParentBone.Index] * mWorld;
                    effect.View = GameState.view;
                    effect.Projection = GameState.projection;
                }
                mesh.Draw();
            }
        }

        public Boolean IsActivePlayersRoadOnEndOfRoad(Player player)
        {
            foreach (Town town in neighbour)
            {
                if (town.HasPlayerRoadNeighbour(player))
                    return true;
            }

            return false;
        }

        public Boolean IsActivePlayersTownOnEndOfRoad(Player player)
        {
            foreach(Town town in neighbour)
            {
                if (town.getPlayerOwner() == player)
                    return true;
            }

            return false;
        }

        public void HandlePickableAreas(Color c)
        {
            if (c == pickRoadColor)
            {
                if (!pickActive)
                    pickNewActive = true;
                pickActive = true;

                if (GameState.CurrentMouseState.LeftButton == ButtonState.Pressed)
                {
                    if (!pickPress)
                        pickNewPress = true;
                    pickPress = true;
                }
                else
                {
                    if (pickPress)
                        pickNewRelease = true;
                    pickPress = false;
                }
            }
            else
            {
                pickActive = false;
                pickPress = false;
                pickNewActive = false;
                pickNewPress = false;
                pickNewRelease = false;
            }

            GameMaster gm = GameMaster.getInstance();
            if (pickNewPress)
            {
                if (gm.getState() == GameMaster.State.StateGame)
                {
                    Player activePlayer = gm.getActivePlayer();
                    if (!isBuild && 
                        (IsActivePlayersRoadOnEndOfRoad(activePlayer) || IsActivePlayersTownOnEndOfRoad(activePlayer)) &&
                        Settings.costRoad.HasPlayerSources(activePlayer))
                    {
                        activePlayer.payForSomething(Settings.costRoad);
                        isBuild = true;
                        playerOwner = activePlayer;
                    }
                }
            }
        }
    }
}
