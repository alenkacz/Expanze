using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Expanze.MapGeneration;

namespace Expanze
{
    class Road
    {
        private Player playerOwner;
        private bool isBuild;

        private int roadID;
        public static int counter = 0;
        private Color pickRoadColor;
        private PickVariables pickVars;
        private Matrix world;

        private Town[] neighbour; // every road must have two neighbour

        public Road(Matrix world)
        {
            this.world = world;

            roadID = ++counter;
            this.pickRoadColor = new Color(0.0f, counter / 256.0f, 0.0f);

            neighbour = new Town[2];
            isBuild = false;
            playerOwner = null;

            pickVars = new PickVariables();
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
            if ((pickVars.pickActive && gm.getState() == EGameState.StateGame) || isBuild)
            {
                Model m = GameState.map.getRoadModel();
                Matrix[] transforms = new Matrix[m.Bones.Count];
                m.CopyAbsoluteBoneTransformsTo(transforms);

                Matrix mWorld = Matrix.CreateTranslation(new Vector3(0.0f, 0.01f, 0.0f)) * Matrix.CreateScale(0.023f) * world;

                int a = 0;

                Player player = playerOwner;
                if (playerOwner == null)
                    player = GameMaster.getInstance().getActivePlayer();
                Vector3 color = player.getColor().ToVector3();
                foreach (ModelMesh mesh in m.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.DirectionalLight0.Enabled = false;
                        effect.DirectionalLight1.Enabled = true;
                        effect.DirectionalLight1.DiffuseColor = new Vector3(0.3f, 0.3f, 0.3f);

                        // is it model part which is for flags? They have to be in player colors
                        if (a % 5 == 1 || a % 5 == 2 || a % 5 == 3 || a == 4 || a == 5 || a == 15 || a == 14)
                        {                         
                            effect.EmissiveColor = color * 0.5f;
                            effect.DiffuseColor = color * 0.9f;
                            effect.AmbientLightColor = color / 3.0f;
                        }
                        else
                        {
                            effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);
                            if(a == 20)
                                effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
                            else
                                effect.DiffuseColor = new Vector3(0.0f, 0.0f, 0.0f);
                            effect.AmbientLightColor = new Vector3(0.7f, 0.7f, 0.7f);
                        }

                        // if player wants to build new Road, can he? Show it in red/green color
                        if (pickVars.pickActive && !isBuild)
                        {
                            if (!CanActivePlayerBuildRoad())
                                effect.DiffuseColor = new Vector3(1, 0.0f, 0);
                            else
                                effect.DiffuseColor = new Vector3(0, 1.0f, 0);
                        }

                        effect.World = transforms[mesh.ParentBone.Index] * mWorld;
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
            Model m = GameState.map.getRectangleShape();
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix mWorld = Matrix.CreateTranslation(new Vector3(0.0f, 0.03f, 0.0f)) * Matrix.CreateScale(0.25f) * world;

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
            Map.SetPickVariables(c == pickRoadColor, pickVars);

            if (pickVars.pickNewPress)
            {
                if (CanActivePlayerBuildRoad())
                {
                    GameMaster gm = GameMaster.getInstance();
                    Player activePlayer = gm.getActivePlayer();
                    activePlayer.payForSomething(Settings.costRoad);
                    isBuild = true;
                    playerOwner = activePlayer;
                }
            }
        }

        public Boolean CanActivePlayerBuildRoad()
        {
            GameMaster gm = GameMaster.getInstance();
            if (gm.getState() == EGameState.StateGame)
            {
                Player activePlayer = gm.getActivePlayer();
                if (!isBuild &&
                    (IsActivePlayersRoadOnEndOfRoad(activePlayer) || IsActivePlayersTownOnEndOfRoad(activePlayer)) &&
                    Settings.costRoad.HasPlayerSources(activePlayer))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
