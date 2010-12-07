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
        IconTown, IconRoad, 
        IconFort, IconFortActive,
        IconMarket, IconMarketActive,
        IconMill, IconMill1, IconMill2, IconMillActive,
        IconMine, IconMine1, IconMine2, IconMineActive,
        IconMonastery, IconMonasteryActive,
        IconQuarry, IconQuarry1, IconQuarry2, IconQuarryActive,
        IconSaw, IconSaw1, IconSaw2, IconSawActive,
        IconStepherd, IconStepherd1, IconStepherd2, IconStepherdActive,
        IconCorn1, IconCorn2, IconMeat1, IconMeat2, IconStone1, IconStone2, IconWood1, IconWood2,
        IconOre1, IconOre2, 
        IconFortParade, IconFortCapture, IconFortSources, IconFortHexa,

        PlayerColor,

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
        Model[] stoneCoverModel;
        Model[] stoneQuarryModel;
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
        public Model getStoneCover(int i) { return stoneCoverModel[i]; }
        public Model getStoneSourceBuildingModel(int i) { return stoneQuarryModel[i]; }
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
            hud[(int)HUDTexture.PlayerColor] = content.Load<Texture2D>("pcolor");

            hud[(int)HUDTexture.IconActive] = content.Load<Texture2D>("HUD/ic_active");
            hud[(int)HUDTexture.HammersPassive] = content.Load<Texture2D>("HUD/hammer");
            hud[(int)HUDTexture.HammersActive] = content.Load<Texture2D>("HUD/hammeractive");
            hud[(int)HUDTexture.InfoPassive] = content.Load<Texture2D>("HUD/info");
            hud[(int)HUDTexture.InfoActive] = content.Load<Texture2D>("HUD/infoactive");
            hud[(int)HUDTexture.IconTown] = content.Load<Texture2D>("HUD/ic_town");
            hud[(int)HUDTexture.IconRoad] = content.Load<Texture2D>("HUD/ic_road");
            hud[(int)HUDTexture.IconFort] = content.Load<Texture2D>("HUD/ic_fort");
            hud[(int)HUDTexture.IconFortActive] = content.Load<Texture2D>("HUD/ic_fort_active");
            hud[(int)HUDTexture.IconMarket] = content.Load<Texture2D>("HUD/ic_market");
            hud[(int)HUDTexture.IconMarketActive] = content.Load<Texture2D>("HUD/ic_market_active");
            hud[(int)HUDTexture.IconMill] = content.Load<Texture2D>("HUD/ic_mill");
            hud[(int)HUDTexture.IconMill1] = content.Load<Texture2D>("HUD/ic_mill1");
            hud[(int)HUDTexture.IconMill2] = content.Load<Texture2D>("HUD/ic_mill2");
            hud[(int)HUDTexture.IconMillActive] = content.Load<Texture2D>("HUD/ic_mill_active");
            hud[(int)HUDTexture.IconMine] = content.Load<Texture2D>("HUD/ic_mine");
            hud[(int)HUDTexture.IconMine1] = content.Load<Texture2D>("HUD/ic_mine1");
            hud[(int)HUDTexture.IconMine2] = content.Load<Texture2D>("HUD/ic_mine2");
            hud[(int)HUDTexture.IconMineActive] = content.Load<Texture2D>("HUD/ic_mine_active");
            hud[(int)HUDTexture.IconMonastery] = content.Load<Texture2D>("HUD/ic_monastery");
            hud[(int)HUDTexture.IconMonasteryActive] = content.Load<Texture2D>("HUD/ic_monastery_active");
            hud[(int)HUDTexture.IconQuarry] = content.Load<Texture2D>("HUD/ic_quarry");
            hud[(int)HUDTexture.IconQuarry1] = content.Load<Texture2D>("HUD/ic_quarry1");
            hud[(int)HUDTexture.IconQuarry2] = content.Load<Texture2D>("HUD/ic_quarry2");
            hud[(int)HUDTexture.IconQuarryActive] = content.Load<Texture2D>("HUD/ic_quarry_active");
            hud[(int)HUDTexture.IconSaw] = content.Load<Texture2D>("HUD/ic_saw");
            hud[(int)HUDTexture.IconSaw1] = content.Load<Texture2D>("HUD/ic_saw1");
            hud[(int)HUDTexture.IconSaw2] = content.Load<Texture2D>("HUD/ic_saw2");
            hud[(int)HUDTexture.IconSawActive] = content.Load<Texture2D>("HUD/ic_saw_active");
            hud[(int)HUDTexture.IconStepherd] = content.Load<Texture2D>("HUD/ic_stepherd");
            hud[(int)HUDTexture.IconStepherd1] = content.Load<Texture2D>("HUD/ic_stepherd1");
            hud[(int)HUDTexture.IconStepherd2] = content.Load<Texture2D>("HUD/ic_stepherd2");
            hud[(int)HUDTexture.IconStepherdActive] = content.Load<Texture2D>("HUD/ic_stepherd_active");
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

            stoneCoverModel = new Model[6];
            stoneCoverModel[0] = content.Load<Model>("Models/krytka5");
            stoneCoverModel[1] = content.Load<Model>("Models/krytka6");
            stoneCoverModel[2] = content.Load<Model>("Models/krytka1");
            stoneCoverModel[3] = content.Load<Model>("Models/krytka2");
            stoneCoverModel[4] = content.Load<Model>("Models/krytka3");
            stoneCoverModel[5] = content.Load<Model>("Models/krytka4");

            stoneQuarryModel = new Model[6];
            stoneQuarryModel[0] = content.Load<Model>("Models/lom_model5");
            stoneQuarryModel[1] = content.Load<Model>("Models/lom_model6");
            stoneQuarryModel[2] = content.Load<Model>("Models/lom_model1");
            stoneQuarryModel[3] = content.Load<Model>("Models/lom_model2");
            stoneQuarryModel[4] = content.Load<Model>("Models/lom_model3");
            stoneQuarryModel[5] = content.Load<Model>("Models/lom_model4");

            townModel = content.Load<Model>("Models/town");
            roadModel = content.Load<Model>("Models/road");
        }
    }
}
