using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using CorePlugin;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Expanze
{
    public enum HUDTexture
    {
        HammersPassive, HammersActive, InfoPassive, InfoActive, IconActive,
        IconFort, IconMarket, IconMill, IconMine, IconMonastery, IconQuarry, IconSaw, IconStepherd,
        IconCorn1, IconCorn2, IconMeat1, IconMeat2, IconStone1, IconStone2, IconWood1, IconWood2,
        IconOre1, IconOre2, 
        IconFortParade, IconFortCapture, IconFortSources, IconFortHexa,
        HUDCount
    }

    public enum BuildingModel
    {
        PastureHouse, Saw, Mill, Fort, Market, Monastery, 
        CountModel
    }

    class GameResources
    {
        private static GameResources instance = null;
        private ContentManager content;
        private Game game;

        public const int N_MODEL = 7;
        Model[] hexaModel;
        public const int SHAPE_RECTANGLE = 0;
        public const int SHAPE_CIRCLE = 1;
        public const int SHAPE_SPHERE = 2;
        public const int N_SHAPE_MODEL = 3;
        Model[] shapeModel;
        Model townModel, roadModel;
        Model[] buildingModel;
        Model[] mountainsCoverModel;
        Model[] mountainsMineModel;
        Texture2D[] hud;


        public static GameResources Inst()
        {
            if (instance == null)
            {
                instance = new GameResources();
            }

            return instance;
        }

        private GameResources()
        {
            game = GameState.game;
        }

        public Model getHexaModel(HexaKind type)
        {
            return hexaModel[(int)type];
        }

        public Texture2D getHudTexture(HUDTexture id) { return hud[(int) id]; }
        public Model getMountainsCover(int i) { return mountainsCoverModel[i]; }
        public Model getMountainsSourceBuildingModel(int i) { return mountainsMineModel[i]; }
        public Model getBuildingModel(BuildingModel id) { return buildingModel[(int) id]; }
        public Model getTownModel() { return townModel; }
        public Model getRoadModel() { return roadModel; }

        public Model getShape(int shapeID)
        {
            return shapeModel[shapeID];
        }

        public void LoadContent()
        {
            if (content == null)
                content = new ContentManager(game.Services, "Content");

            hud = new Texture2D[(int) HUDTexture.HUDCount];
            hud[(int)HUDTexture.IconActive] = content.Load<Texture2D>("HUD/ic_active");
            hud[(int)HUDTexture.HammersPassive] = content.Load<Texture2D>("HUD/hammer");
            hud[(int)HUDTexture.HammersActive] = content.Load<Texture2D>("HUD/hammeractive");
            hud[(int)HUDTexture.InfoPassive] = content.Load<Texture2D>("HUD/info");
            hud[(int)HUDTexture.InfoActive] = content.Load<Texture2D>("HUD/infoactive");
            hud[(int)HUDTexture.IconFort] = content.Load<Texture2D>("HUD/ic_fort");
            hud[(int)HUDTexture.IconMarket] = content.Load<Texture2D>("HUD/ic_market");
            hud[(int)HUDTexture.IconMill] = content.Load<Texture2D>("HUD/ic_mill");
            hud[(int)HUDTexture.IconMine] = content.Load<Texture2D>("HUD/ic_mine");
            hud[(int)HUDTexture.IconMonastery] = content.Load<Texture2D>("HUD/ic_monastery");
            hud[(int)HUDTexture.IconQuarry] = content.Load<Texture2D>("HUD/ic_quarry");
            hud[(int)HUDTexture.IconSaw] = content.Load<Texture2D>("HUD/ic_saw");
            hud[(int)HUDTexture.IconStepherd] = content.Load<Texture2D>("HUD/ic_stepherd");
            hud[(int)HUDTexture.IconCorn1] = content.Load<Texture2D>("HUD/ic_corn1");
            hud[(int)HUDTexture.IconMeat1] = content.Load<Texture2D>("HUD/ic_meat1");
            hud[(int)HUDTexture.IconStone1] = content.Load<Texture2D>("HUD/ic_stone1");
            hud[(int)HUDTexture.IconWood1] = content.Load<Texture2D>("HUD/ic_wood1");
            hud[(int)HUDTexture.IconOre1] = content.Load<Texture2D>("HUD/ic_ore1");
            hud[(int)HUDTexture.IconCorn2] = content.Load<Texture2D>("HUD/ic_corn2");
            hud[(int)HUDTexture.IconMeat2] = content.Load<Texture2D>("HUD/ic_meat2");
            hud[(int)HUDTexture.IconStone2] = content.Load<Texture2D>("HUD/ic_stone2");
            hud[(int)HUDTexture.IconWood2] = content.Load<Texture2D>("HUD/ic_wood2");
            hud[(int)HUDTexture.IconOre2] = content.Load<Texture2D>("HUD/ic_ore2");
            hud[(int)HUDTexture.IconFortCapture] = content.Load<Texture2D>("HUD/ic_fort_capture");
            hud[(int)HUDTexture.IconFortHexa] = content.Load<Texture2D>("HUD/ic_fort_hexa");
            hud[(int)HUDTexture.IconFortParade] = content.Load<Texture2D>("HUD/ic_fort_parade");
            hud[(int)HUDTexture.IconFortSources] = content.Load<Texture2D>("HUD/ic_fort_sources");

            hexaModel = new Model[N_MODEL];
            hexaModel[(int)HexaKind.Cornfield] = content.Load<Model>(Settings.mapPaths[(int)HexaKind.Cornfield]);
            hexaModel[(int)HexaKind.Desert] = content.Load<Model>(Settings.mapPaths[(int)HexaKind.Desert]);
            hexaModel[(int)HexaKind.Forest] = content.Load<Model>(Settings.mapPaths[(int)HexaKind.Forest]);
            hexaModel[(int)HexaKind.Mountains] = content.Load<Model>(Settings.mapPaths[(int)HexaKind.Mountains]);
            hexaModel[(int)HexaKind.Pasture] = content.Load<Model>(Settings.mapPaths[(int)HexaKind.Pasture]);
            hexaModel[(int)HexaKind.Stone] = content.Load<Model>(Settings.mapPaths[(int)HexaKind.Stone]);
            hexaModel[(int)HexaKind.Water] = content.Load<Model>(Settings.mapPaths[(int)HexaKind.Water]);

            shapeModel = new Model[N_SHAPE_MODEL];

            shapeModel[SHAPE_RECTANGLE] = content.Load<Model>("Shapes/rectangle");
            shapeModel[SHAPE_CIRCLE] = content.Load<Model>("Shapes/circle");
            shapeModel[SHAPE_SPHERE] = content.Load<Model>("Shapes/sphere");

            buildingModel = new Model[(int) BuildingModel.CountModel];
            buildingModel[(int) BuildingModel.PastureHouse] = content.Load<Model>("Models/pastureHouse");
            buildingModel[(int) BuildingModel.Saw] = content.Load<Model>("Models/saw");
            buildingModel[(int) BuildingModel.Mill] = content.Load<Model>("Models/millnew");
            buildingModel[(int) BuildingModel.Fort] = content.Load<Model>("Models/fort");
            buildingModel[(int) BuildingModel.Market] = content.Load<Model>("Models/market");
            buildingModel[(int) BuildingModel.Monastery] = content.Load<Model>("Models/monastery");

            mountainsCoverModel = new Model[5];
            mountainsCoverModel[0] = content.Load<Model>("Models/cover4");
            mountainsCoverModel[1] = content.Load<Model>("Models/cover5");
            mountainsCoverModel[2] = content.Load<Model>("Models/cover1");
            mountainsCoverModel[3] = content.Load<Model>("Models/cover2");
            mountainsCoverModel[4] = content.Load<Model>("Models/cover3");

            mountainsMineModel = new Model[6];
            mountainsMineModel[0] = content.Load<Model>("Models/mine5");
            mountainsMineModel[1] = content.Load<Model>("Models/mine6");
            mountainsMineModel[2] = content.Load<Model>("Models/mine1");
            mountainsMineModel[3] = content.Load<Model>("Models/mine2");
            mountainsMineModel[4] = content.Load<Model>("Models/mine3");
            mountainsMineModel[5] = content.Load<Model>("Models/mine4");

            townModel = content.Load<Model>("Models/town");
            roadModel = content.Load<Model>("Models/road");
        }
    }
}
