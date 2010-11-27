﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using CorePlugin;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Expanze
{
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
        public const int PASTURE_HOUSE = 0;
        public const int MILL_HOUSE = 1;
        public const int FORT = 2;
        Model[] buildingModel;
        Model[] mountainsCoverModel;
        Model[] mountainsMineModel;

        public const int HUD_HAMMERS_PASSIVE = 0;
        public const int HUD_HAMMERS_ACTIVE = 1;
        public const int HUD_INFO_PASSIVE = 2;
        public const int HUD_INFO_ACTIVE = 3;
        public const int HUD_ICON_FORT = 4;
        public const int HUD_ICON_MARKET = 5;
        public const int HUD_ICON_MILL = 6;
        public const int HUD_ICON_MINE = 7;
        public const int HUD_ICON_MONASTERY = 8;
        public const int HUD_ICON_QUARRY = 9;
        public const int HUD_ICON_SAW = 10;
        public const int HUD_ICON_STEPHERD = 11;
        public const int HUD_NUMBER = 12;
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

        public Texture2D getHudTexture(int id) { return hud[id]; }
        public Model getMountainsCover(int i) { return mountainsCoverModel[i]; }
        public Model getMountainsSourceBuildingModel(int i) { return mountainsMineModel[i]; }
        public Model getBuildingModel(int i) { return buildingModel[i]; }
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

            hud = new Texture2D[HUD_NUMBER];
            hud[HUD_HAMMERS_PASSIVE] = content.Load<Texture2D>("HUD/hammer");
            hud[HUD_HAMMERS_ACTIVE] = content.Load<Texture2D>("HUD/hammeractive");
            hud[HUD_INFO_PASSIVE] = content.Load<Texture2D>("HUD/info");
            hud[HUD_INFO_ACTIVE] = content.Load<Texture2D>("HUD/infoactive");
            hud[HUD_ICON_FORT] = content.Load<Texture2D>("HUD/ic_fort");
            hud[HUD_ICON_MARKET] = content.Load<Texture2D>("HUD/ic_market");
            hud[HUD_ICON_MILL] = content.Load<Texture2D>("HUD/ic_mill");
            hud[HUD_ICON_MINE] = content.Load<Texture2D>("HUD/ic_mine");
            hud[HUD_ICON_MONASTERY] = content.Load<Texture2D>("HUD/ic_monastery");
            hud[HUD_ICON_QUARRY] = content.Load<Texture2D>("HUD/ic_quarry");
            hud[HUD_ICON_SAW] = content.Load<Texture2D>("HUD/ic_saw");
            hud[HUD_ICON_STEPHERD] = content.Load<Texture2D>("HUD/ic_stepherd");

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

            buildingModel = new Model[3];
            buildingModel[PASTURE_HOUSE] = content.Load<Model>("Models/pastureHouse");
            buildingModel[MILL_HOUSE] = content.Load<Model>("Models/millnew");
            buildingModel[FORT] = content.Load<Model>("Models/fort");

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
