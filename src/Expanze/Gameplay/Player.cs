using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CorePlugin;
using Expanze.Gameplay;

namespace Expanze
{
    public enum TransactionState {TransactionStart, TransactionMiddle, TransactionEnd };

    class Player : IPlayer
    {
        private String name;
        private Color color;
        private int orderID;
        private bool materialChanged;
        private int [] points;
        private bool active;            /// is he still playing? (ex. AI make exception and it cant continue in game

        SourceAll prevSource;       // Source before last source change (for example before paying for something, collecting resources
        SourceAll source;
        SourceAll transactionSource;
        SourceAll collectSourcesLastTurn; /// sources getted on the start of player's last turn
        SourceAll collectSourcesNormal;   /// collect sources without disasters and miracles
        SourceAll sumSpentSources;        /// which sources were used

        int[] buildingCount;

        LicenceKind[] licenceMarket;
        UpgradeKind[] upgradeMonastery;

        List<IMonastery> monastery;
        List<IMarket> market;
        List<IFort> fort;
        List<ITown> town;
        List<IRoad> road;

        IComponentAI componentAI;   // is null if player is NOT controled by computer but is controled by human
        int[][] personality;    // only for Gen AI
        bool gen;

        private Statistic statistic;

        public Player(String name, Color color, IComponentAI componentAI, int orderID) : this(name, color, componentAI, orderID, null, false)
        {
        }
        public Player(String name, Color color, IComponentAI componentAI, int orderID, int[][] personality, bool gen)
        {
            this.orderID = orderID;
            this.gen = gen;
            points = new int[(int) PlayerPoints.Count];
            prevSource = new SourceAll(0);
            source = new SourceAll(0);
            transactionSource = new SourceAll(0);
            sumSpentSources = new SourceAll(0);

            ClearCollectSources();
            collectSourcesNormal = new SourceAll(0);

            buildingCount = new int[(int) Building.Count];
            for (int loop1 = 0; loop1 < (int)Building.Count; loop1++)
            {
                buildingCount[loop1] = 0;
            }

            licenceMarket = new LicenceKind[(int)SourceKind.Count];
            for (int loop1 = 0; loop1 < (int)SourceBuildingKind.Count; loop1++)
            {
                licenceMarket[loop1] = LicenceKind.NoLicence;
            }

            upgradeMonastery = new UpgradeKind[(int) SourceBuildingKind.Count];
            for (int loop1 = 0; loop1 < (int)SourceBuildingKind.Count; loop1++)
            {
                upgradeMonastery[loop1] = UpgradeKind.NoUpgrade;
            }

            this.color = color;
            this.name = name;
            this.componentAI = componentAI;
            this.personality = personality;

            materialChanged = false;
            active = true;

            monastery = new List<IMonastery>();
            market = new List<IMarket>();
            fort = new List<IFort>();
            town = new List<ITown>();
            road = new List<IRoad>();

            statistic = new Statistic();
        }

        public int GetBuildingCount(Building building) { return buildingCount[(int)building]; }
        public IComponentAI GetComponentAI() { return componentAI; }
        public bool GetIsAI() { return componentAI != null; }
        public Color GetColor() { return color; }
        public bool GetActive() { return active; }
        public UpgradeKind GetMonasteryUpgrade(SourceBuildingKind kind) { return upgradeMonastery[(int)kind]; }
        public LicenceKind GetMarketLicence(SourceKind kind) { return licenceMarket[(int)kind]; }
        public Statistic GetStatistic() { return statistic; }
        public int GetOrderID() { return orderID; }

        public void AddBuilding(Building building)
        {
            buildingCount[(int)building]++;
            GameMaster.Inst().PlayerWantMedail(this, building);

            int turn = GameMaster.Inst().GetTurnNumber();
            switch (building)
            {
                case Building.Town: statistic.AddStat(Statistic.Kind.Towns, 1, turn); break;
                case Building.Road: statistic.AddStat(Statistic.Kind.Roads, 1, turn); break;
                case Building.Fort: statistic.AddStat(Statistic.Kind.Fort, 1, turn); break;
                case Building.Monastery: statistic.AddStat(Statistic.Kind.Monastery, 1, turn); break;
                case Building.Market: statistic.AddStat(Statistic.Kind.Market, 1, turn); break;
            }

            switch (building)
            {
                case Building.Saw: AddPoints(PlayerPoints.Saw); break;
                case Building.Mine: AddPoints(PlayerPoints.Mine); break;
                case Building.Stepherd: AddPoints(PlayerPoints.Stepherd); break;
                case Building.Quarry: AddPoints(PlayerPoints.Quarry); break;
                case Building.Mill: AddPoints(PlayerPoints.Mill); break;
            }
        }

        public void AddRoad(IRoad t) { 
            road.Add(t);
            AddPoints(PlayerPoints.Road);
            AddBuilding(Building.Road);

            foreach (int id in Settings.goalRoad)
            {
                if (id == t.GetRoadID())
                {
                    AddPoints(PlayerPoints.RoadID);
                    break;
                }
            }
        }

        public List<IRoad> GetRoad() { return road; }
        public void AddTown(ITown t) { 
            town.Add(t);
            foreach (int id in Settings.goalTown)
            {
                if (id == t.GetTownID())
                {
                    AddPoints(PlayerPoints.TownID);
                    break;
                }
            }
        }
        public List<ITown> GetTown() { return town; }

        public void AddMarket(IMarket m)
        {
            market.Add(m);
        }

        public List<IMarket> GetMarket()
        {
            return market;
        }

        public void AddMonastery(IMonastery m)
        {
            monastery.Add(m);
        }

        public List<IMonastery> GetMonastery() { return monastery; }

        public void AddFort(IFort f)
        {
            fort.Add(f);
        }

        public List<IFort> GetFort()
        {
            return fort;
        }

        public void AddPoints(PlayerPoints which) {
            bool areTherePoints = false;
            switch (which)
            {
                case PlayerPoints.Fort: areTherePoints = Settings.pointsFort > 0 && Settings.pointsFort > points[(int) which]; break;
                case PlayerPoints.FortCaptureHexa: areTherePoints = Settings.pointsFortCapture > 0 && Settings.pointsFortCapture > points[(int)which]; break;
                case PlayerPoints.FortParade: areTherePoints = Settings.pointsFortParade > 0 && Settings.pointsFortParade > points[(int)which]; break;
                case PlayerPoints.FortStealSources: areTherePoints = Settings.pointsFortSteal > 0 && Settings.pointsFortSteal > points[(int)which]; break;
                case PlayerPoints.LicenceLvl1: areTherePoints = Settings.pointsMarketLvl1 > 0 && Settings.pointsMarketLvl1 > points[(int)which]; break;
                case PlayerPoints.LicenceLvl2: areTherePoints = Settings.pointsMarketLvl2 > 0 && Settings.pointsMarketLvl2 > points[(int)which]; break;
                case PlayerPoints.Market: areTherePoints = Settings.pointsMarket > 0 && Settings.pointsMarket > points[(int)which]; break;
                case PlayerPoints.RoadID: areTherePoints = Settings.goalRoadID > 0 && Settings.goalRoadID > points[(int)which]; break;
                case PlayerPoints.TownID: areTherePoints = Settings.goalTownID > 0 && Settings.goalTownID > points[(int)which]; break;
                case PlayerPoints.Mill: areTherePoints = Settings.pointsMill > 0 && Settings.pointsMill > points[(int)which]; break;
                case PlayerPoints.Mine: areTherePoints = Settings.pointsMine > 0 && Settings.pointsMine > points[(int)which]; break;
                case PlayerPoints.Monastery: areTherePoints = Settings.pointsMonastery > 0 && Settings.pointsMonastery > points[(int)which]; break;
                case PlayerPoints.Quarry: areTherePoints = Settings.pointsQuarry > 0 && Settings.pointsQuarry > points[(int)which]; break;
                case PlayerPoints.Road: areTherePoints = Settings.pointsRoad > 0 && Settings.pointsRoad > points[(int)which]; break;
                case PlayerPoints.Saw: areTherePoints = Settings.pointsSaw > 0 && Settings.pointsSaw > points[(int)which]; break;
                case PlayerPoints.Stepherd: areTherePoints = Settings.pointsStepherd > 0 && Settings.pointsStepherd > points[(int)which]; break;
                case PlayerPoints.Town: areTherePoints = Settings.pointsTown > 0 && Settings.pointsTown > points[(int)which]; break;
                case PlayerPoints.UpgradeLvl1: areTherePoints = Settings.pointsUpgradeLvl1 > 0 && Settings.pointsUpgradeLvl1 > points[(int)which]; break;
                case PlayerPoints.UpgradeLvl2: areTherePoints = Settings.pointsUpgradeLvl2 > 0 && Settings.pointsUpgradeLvl2 > points[(int)which]; break;
            }
            if (!areTherePoints)
                return;

            points[(int) which] ++;
            statistic.AddStat(Statistic.Kind.Points, 1, GameMaster.Inst().GetTurnNumber());
        }

        public void RemovePoints(PlayerPoints which)
        {
            if (points[(int)which] > 0)
            {
                points[(int)which]--;
                statistic.AddStat(Statistic.Kind.Points, -1, GameMaster.Inst().GetTurnNumber());
            }
        }

        public int GetPoints(PlayerPoints kind) { return points[(int)kind]; }
        public int[] GetPoints() { return points; }
        public int GetPointSum()
        {
            int pointSum = 0;
            for (int loop1 = 0; loop1 < (int)PlayerPoints.Count; loop1++)
                pointSum += points[loop1];
            return pointSum;
        }

        public int GetConversionRate(SourceKind kind)
        {
            if (kind == SourceKind.Null)
                return -1;

            switch (licenceMarket[(int)kind])
            {
                case LicenceKind.NoLicence: return 4;
                case LicenceKind.FirstLicence: return 3;
                case LicenceKind.SecondLicence: return 2;
            }

            return -1;
        }

        public SourceAll GetSumSpentSources() { return sumSpentSources; }

        public ISourceAll GetSource()
        {
            return source;
        }

        public String GetName()
        {
            return this.name;
        }

        public int GetCorn()
        {
            return source.GetCorn();
        }

        public int GetWood()
        {
            return source.GetWood();
        }

        public int GetOre()
        {
            return source.GetOre();
        }

        public int GetMeat()
        {
            return source.GetMeat();
        }

        public int GetStone()
        {
            return source.GetStone();
        }

        public void PayForSomething(ISourceAll cost)
        {
            SourceAll sourceCost = (SourceAll)cost;

            ChangeSources(-sourceCost.GetWood(), -sourceCost.GetStone(), -sourceCost.GetCorn(), -sourceCost.GetMeat(), -sourceCost.GetOre());
            
            sumSpentSources = sumSpentSources + sourceCost;

            source = source - sourceCost;
        }

        public void ClearCollectSources()
        {
            collectSourcesLastTurn = new SourceAll(0);
            collectSourcesNormal = new SourceAll(0);
        }

        public void AddCollectSources(SourceAll normal, SourceAll now)
        {
            collectSourcesNormal += normal;
            collectSourcesLastTurn += now;
        }

        public ISourceAll GetCollectSourcesNormal() { return collectSourcesNormal; }
        public SourceAll GetCollectSourcesLastTurn() { return collectSourcesLastTurn; }

        public void AddSources(SourceAll amount, TransactionState state)
        {
            switch (state)
            {
                case TransactionState.TransactionStart :
                    if (transactionSource.Equals(new SourceAll(0)))
                        transactionSource = amount;
                    else
                        transactionSource += amount;
                    break;
                case TransactionState.TransactionMiddle :
                    transactionSource = transactionSource + amount;
                    break;
                case TransactionState.TransactionEnd :
                    transactionSource = transactionSource + amount;
                    source = source + transactionSource;               
                    ChangeSources(transactionSource.GetWood(), transactionSource.GetStone(), transactionSource.GetCorn(), transactionSource.GetMeat(), transactionSource.GetOre());
                    transactionSource = new SourceAll(0);
                    break;
            }
        }
        
        /// <summary>
        /// Remembers state of material from previous round, active when active player is changed
        /// </summary>
        public void ChangeSources(int wood, int stone, int corn, int meat, int ore)
        {
            materialChanged = true;
            prevSource = new SourceAll(corn, meat, stone, wood, ore);
        }

        public bool HasMaterialChanged()
        {
            bool temp = materialChanged;
            materialChanged = false;
            return temp;
        }

        public void SetMaterialChange(SourceAll change)
        {
            materialChanged = true;
            prevSource = change;
        }
        public SourceAll GetMaterialChange()
        {
            return prevSource;
        }

        public bool HaveEnoughMaterialForConversion(SourceKind kind)
        {
            return source.Get(kind) >= GetConversionRate(kind);
        }

        public int GetMaterialNumber(SourceKind k)
        {
            return source.Get(k);
        }

        public void SetActive(bool active) 
        {
            if (this.active && !active)
            {
                GameMaster.Inst().AddToPlayerCount(-1);
                points = new int[(int) PlayerPoints.Count];
                Message.Inst().Show(Strings.GAME_ALERT_TITLE_AI_EXCEPTION, GetName() + " " + Strings.GAME_ALERT_DESCRIPTION_AI_EXCEPTION, GameResources.Inst().GetHudTexture(HUDTexture.IconTown));
            }

            this.active = active; 
            
        }

        public void SetSourceBuildingUpdate(UpgradeKind upgradeKind, int upgradeNumber)
        {
            statistic.AddStat(Statistic.Kind.Upgrades, 1, GameMaster.Inst().GetTurnNumber());
            if (upgradeKind == UpgradeKind.FirstUpgrade)
            {
                AddPoints(PlayerPoints.UpgradeLvl1);
                switch (upgradeNumber)
                {
                    case 0:
                        AddCollectSources(new SourceAll(collectSourcesNormal.GetCorn() / 2, 0, 0, 0, 0), new SourceAll(0)); break;
                    case 1:
                        AddCollectSources(new SourceAll(0, collectSourcesNormal.GetMeat() / 2, 0, 0, 0), new SourceAll(0)); break;
                    case 2:
                        AddCollectSources(new SourceAll(0, 0, collectSourcesNormal.GetStone() / 2, 0, 0), new SourceAll(0)); break;
                    case 3:
                        AddCollectSources(new SourceAll(0, 0, 0, collectSourcesNormal.GetWood() / 2, 0), new SourceAll(0)); break;
                    case 4:
                        AddCollectSources(new SourceAll(0, 0, 0, 0, collectSourcesNormal.GetOre() / 2), new SourceAll(0)); break;
                }
            }
            else
            {
                AddPoints(PlayerPoints.UpgradeLvl2);
                switch (upgradeNumber)
                {
                    case 0:
                        AddCollectSources(new SourceAll(collectSourcesNormal.GetCorn() / 3, 0, 0, 0, 0), new SourceAll(0)); break;
                    case 1:
                        AddCollectSources(new SourceAll(0, collectSourcesNormal.GetMeat() / 3, 0, 0, 0), new SourceAll(0)); break;
                    case 2:
                        AddCollectSources(new SourceAll(0, 0, collectSourcesNormal.GetStone() / 3, 0, 0), new SourceAll(0)); break;
                    case 3:
                        AddCollectSources(new SourceAll(0, 0, 0, collectSourcesNormal.GetWood() / 3, 0), new SourceAll(0)); break;
                    case 4:
                        AddCollectSources(new SourceAll(0, 0, 0, 0, collectSourcesNormal.GetOre() / 3), new SourceAll(0)); break;
                }
            }
            upgradeMonastery[upgradeNumber] = upgradeKind;
        }

        public void BuyMarketLicence(LicenceKind licenceKind, int upgradeNumber)
        {
            statistic.AddStat(Statistic.Kind.Licences, 1, GameMaster.Inst().GetTurnNumber());
            if (licenceKind == LicenceKind.FirstLicence)
                AddPoints(PlayerPoints.LicenceLvl1);
            else
                AddPoints(PlayerPoints.LicenceLvl2);

            licenceMarket[upgradeNumber] = licenceKind;
        }

        public void AddFortAction()
        {
            statistic.AddStat(Statistic.Kind.Actions, 1, GameMaster.Inst().GetTurnNumber());
        }

        internal void AddSumSourcesStat()
        {
            statistic.AddStat(Statistic.Kind.SumSources, source.GetAsArray().Sum(), GameMaster.Inst().GetTurnNumber());
        }

        internal int[][] GetPersonality()
        {
            return personality;
        }

        internal bool GetGen()
        {
            return gen;
        }
    }
}
