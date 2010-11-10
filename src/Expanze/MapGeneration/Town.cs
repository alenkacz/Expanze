using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CorePlugin;

namespace Expanze
{
    class Town : ITownGet
    {
        private Player playerOwner;
        private bool isBuild;
        private Road[] roadNeighbour; // two or 3 neighbours
        private Town[] townNeighbour; // two or 3 neighbours
        private Hexa[] hexaNeighbour; // 1 - 3 neighbours

        private int townID;
        public static int counter = 0;
        private Color pickTownColor;
        private PickVariables pickVars;

        private Matrix world;

        public int getTownID() { return townID; }
        public Boolean getPickNewPress() { return pickVars.pickNewPress; }
        public bool getIsBuild() { return isBuild; }
        public Player getPlayerOwner() { return playerOwner; }
        public ISourceAll getCost() { return Settings.costTown; }

        public Town(Matrix world)
        {
            this.world = world;

            townID = ++counter;
            isBuild = false;

            this.pickTownColor = new Color(0.0f, 0.0f, this.townID / 256.0f);

            roadNeighbour = new Road[3];
            townNeighbour = new Town[3];
            hexaNeighbour = new Hexa[3];

            playerOwner = null;

            pickVars = new PickVariables();
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

            SourceAll cost = new SourceAll(0);
            int amount;

            foreach (Hexa hexa in hexaNeighbour)
            {
                if (hexa != null)
                {
                    amount = hexa.getValue();

                    switch (hexa.getType())
                    {
                        case HexaType.Forest:
                            cost = cost + new SourceAll(amount, 0, 0, 0, 0);
                            break;

                        case HexaType.Stone:
                            cost = cost + new SourceAll(0, amount, 0, 0, 0);
                            break;

                        case HexaType.Cornfield :
                            cost = cost + new SourceAll(0, 0, amount, 0, 0);
                            break;

                        case HexaType.Pasture:
                            cost = cost + new SourceAll(0, 0, 0, amount, 0);
                            break;

                        case HexaType.Mountains:
                            cost = cost + new SourceAll(0, 0, 0, 0, amount);
                            break;
                    }
                }
            }
            player.addSources(cost, TransactionState.TransactionMiddle);
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
            if (pickVars.pickActive || isBuild)
            {
                Model m = GameState.map.getTownModel();
                Matrix[] transforms = new Matrix[m.Bones.Count];
                m.CopyAbsoluteBoneTransformsTo(transforms);

                Matrix rotation;
                rotation = (townID % 6 == 0) ? Matrix.Identity : Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (townID % 6));
                Matrix mWorld = rotation * Matrix.CreateTranslation(new Vector3(0.0f, 0.01f, 0.0f)) * Matrix.CreateScale(0.00032f) * world;

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

                        if (a == 1 || a == 2)
                        {
                            effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);
                            effect.DiffuseColor = color * 0.9f;
                            effect.AmbientLightColor = new Vector3(0.0533f, 0.0988f, 0.1819f);
                        }
                        else
                        {
                            effect.DiffuseColor = new Vector3(0.64f, 0.64f, 0.64f);
                        }

                        if (pickVars.pickActive && !isBuild)
                        {
                            if (!CanActivePlayerBuildTown())
                                effect.DiffuseColor = new Vector3(1, 0.0f, 0);
                            else
                                effect.DiffuseColor = new Vector3(0, 1.0f, 0);
                        }

                        effect.World = transforms[mesh.ParentBone.Index] * mWorld;
                        effect.View = GameState.view;
                        effect.Projection = GameState.projection;
                    }
                    mesh.Draw();

                    a++;
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
            Map.SetPickVariables(c == pickTownColor, pickVars);
            

            // create new town?
            GameMaster gm = GameMaster.getInstance();
            if (pickVars.pickNewPress)
            {
                GameState.map.BuildTown(townID);
            }
        }

        public Boolean CanActivePlayerBuildTown()
        {
            GameMaster gm = GameMaster.getInstance();
            if (gm.getState() == EGameState.StateGame)
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
