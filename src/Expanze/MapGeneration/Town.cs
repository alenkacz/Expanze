using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Expanze
{
    class Town
    {
        private Player playerOwner;
        private bool isBuild;
        private Road[] roadNeighbour; // two or 3 neighbours
        private Town[] townNeighbour; // two or 3 neighbours
        private Hexa[] hexaNeighbour; // 1 - 3 neighbours

        private int number;
        public static int counter = 0;
        private Color pickTownColor;
        private Boolean pickActive = false;         // if mouse is over pickable area
        private Boolean pickNewActive = false;      // if mouse is newly over pickable area
        private Boolean pickNewPress = false;       // if mouse press on pickable area newly
        private Boolean pickPress = false;          // if mouse press on pickable area
        private Boolean pickNewRelease = false;     // if mouse is newly release above pickable area

        private Matrix world;

        public Boolean getPickNewPress() {return pickNewPress;}
        public bool getIsBuild() { return isBuild; }
        public Player getPlayerOwner() { return playerOwner; }

        public Town(Matrix world)
        {
            this.world = world;

            number = ++counter;
            isBuild = false;

            this.pickTownColor = new Color(0.0f, 0.0f, this.number / 256.0f);

            roadNeighbour = new Road[3];
            townNeighbour = new Town[3];
            hexaNeighbour = new Hexa[3];

            playerOwner = null;
        }

        public static void resetCounter() { counter = 0; }

        public void setRoadNeighbours(Road road1, Road road2, Road road3)
        {
            roadNeighbour[0] = road1;
            roadNeighbour[1] = road2;
            roadNeighbour[2] = road3;
        }

        public void setTownNeighbours(Town town1, Town town2, Town town3)
        {
            townNeighbour[0] = town1;
            townNeighbour[1] = town2;
            townNeighbour[2] = town3;
        }

        public void setHexaNeighbours(Hexa hexa1, Hexa hexa2, Hexa hexa3)
        {
            hexaNeighbour[0] = hexa1;
            hexaNeighbour[1] = hexa2;
            hexaNeighbour[2] = hexa3;
        }

        public void CollectSources(Player player)
        {
            if (playerOwner != player)
                return;

            SourceCost cost = new SourceCost();
            int amount;

            foreach (Hexa hexa in hexaNeighbour)
            {
                if (hexa != null)
                {
                    amount = hexa.getValue();

                    switch (hexa.getType())
                    {
                        case Settings.Types.Forest:
                            cost = new SourceCost(amount, 0, 0, 0, 0);
                            break;

                        case Settings.Types.Stone:
                            cost = new SourceCost(0, amount, 0, 0, 0);
                            break;

                        case Settings.Types.Cornfield :
                            cost = new SourceCost(0, 0, amount, 0, 0);
                            break;

                        case Settings.Types.Pasture:
                            cost = new SourceCost(0, 0, 0, amount, 0);
                            break;

                        case Settings.Types.Mountains:
                            cost = new SourceCost(0, 0, 0, 0, amount);
                            break;
                    }
                    player.addSources(cost);
                }
            }
        }

        public void BuildTown(Player player)
        {
            playerOwner = player;
            isBuild = true;
        }

        public Boolean HasPlayerRoadNeighbour(Player player)
        {
            foreach (Road road in roadNeighbour)
            {
                if (road != null)
                {
                    if (road.getOwner() == player)
                        return true;
                }
            }
            return false;
        }

        // has someone already built town next to this spot?
        public Boolean HasTownBuildNeighbour()
        {
            for (int loop1 = 0; loop1 < townNeighbour.Length; loop1++)
            {
                if (townNeighbour[loop1] != null)
                {
                    if (townNeighbour[loop1].getIsBuild())
                        return true;
                }
            }

            return false;
        }

        public void Draw(GameTime gameTime)
        {
            if (pickActive || isBuild)
            {
                Model m = GameState.map.getTownModel();
                Matrix[] transforms = new Matrix[m.Bones.Count];
                m.CopyAbsoluteBoneTransformsTo(transforms);

                Matrix mWorld = Matrix.CreateTranslation(new Vector3(0.0f, 0.01f, 0.0f)) * Matrix.CreateScale(0.00032f) * world;

                foreach (ModelMesh mesh in m.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        if (pickActive)
                        {
                            if (!CanActivePlayerBuildTown())
                                effect.DiffuseColor = new Vector3(1, 0.0f, 0);
                            else
                                effect.DiffuseColor = new Vector3(0, 1.0f, 0);
                        }
                        else
                            effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
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
            Model m = GameState.map.getCircleShape();
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix mWorld = Matrix.CreateTranslation(new Vector3(0.0f, 0.05f, 0.0f)) * Matrix.CreateScale(0.22f) * world;

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = pickTownColor.ToVector3();
                    effect.World = transforms[mesh.ParentBone.Index] * mWorld;
                    effect.View = GameState.view;
                    effect.Projection = GameState.projection;
                }
                mesh.Draw();
            }
        }

        public void HandlePickableAreas(Color c)
        {
            if (c == pickTownColor)
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

            // create new town?
            GameMaster gm = GameMaster.getInstance();
            if (pickNewPress && CanActivePlayerBuildTown())
            {
                BuildTown(gm.getActivePlayer());
                if (gm.getState() != GameMaster.State.StateGame)
                {
                    gm.nextTurn();
                }
                else
                    gm.getActivePlayer().payForSomething(Settings.costTown);
            }
        }

        private Boolean CanActivePlayerBuildTown()
        {
            GameMaster gm = GameMaster.getInstance();
            if (gm.getState() == GameMaster.State.StateGame)
            {
                Player activePlayer = gm.getActivePlayer();
                Boolean hasActivePlayerRoadNeighbour = false;

                foreach(Road road in roadNeighbour)
                {
                    if (road != null && road.getOwner() == activePlayer)
                        hasActivePlayerRoadNeighbour = true;
                }

                return !isBuild && !HasTownBuildNeighbour() && Settings.costTown.HasPlayerSources(activePlayer) && hasActivePlayerRoadNeighbour;
            } else
                return !isBuild && !HasTownBuildNeighbour();
        }
    }
}
