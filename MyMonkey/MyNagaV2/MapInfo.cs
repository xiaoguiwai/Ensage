using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using System.Diagnostics.CodeAnalysis;
using Ensage;
using Ensage.Common;

namespace MyMonkey
{
    public class MapInfo
    {
        public static List<LanePoint> LanePoints = new List<LanePoint>()
        {

            new LanePoint() { Name="RMidHT",Position=new Vector3(-4640,-4144,384),Lane=Lane.Mid},
            new LanePoint() { Name="RMid2T",Position=new Vector3(-3552,-2792,256),Lane=Lane.Mid},
            new LanePoint() { Name="RMid1T",Position=new Vector3(-1656,-1512,256),Lane=Lane.Mid},
            new LanePoint() { Name="DMid1T",Position=new Vector3(1024,320,256),Lane=Lane.Mid},
            new LanePoint() { Name="DMid2T",Position=new Vector3(2496,2112,256),Lane=Lane.Mid},
            new LanePoint() { Name="DMidHT",Position=new Vector3(4272,3759,384),Lane=Lane.Mid},

            new LanePoint() { Name="RTopHT",Position=new Vector3(-6592,-3408,384),Lane=Lane.Top},
            new LanePoint() { Name="RTop2T",Position=new Vector3(-6160,-872,384),Lane=Lane.Top},
            new LanePoint() { Name="RTop1T",Position=new Vector3(-6208,1816,384),Lane=Lane.Top},
            new LanePoint() { Name="TopCenter",Position=new Vector3(-6321,4255,384),Lane=Lane.Top},
            new LanePoint() { Name="DTop1T",Position=new Vector3(-4736,6016,384),Lane=Lane.Top},
            new LanePoint() { Name="DTop2T",Position=new Vector3(0,6016,384),Lane=Lane.Top},
            new LanePoint() { Name="DTopHT",Position=new Vector3(3552,5776,384),Lane=Lane.Top},

            new LanePoint() { Name="RBotHT",Position=new Vector3(-3953,-6112,384),Lane=Lane.Bot},
            new LanePoint() { Name="RBot2T",Position=new Vector3(-104,-6240,384),Lane=Lane.Bot},
            new LanePoint() { Name="RBot1T",Position=new Vector3(4924,-6128,384),Lane=Lane.Bot},
            new LanePoint() { Name="BotCenter",Position=new Vector3(6271,-4291,384),Lane=Lane.Bot},
            new LanePoint() { Name="DBot1T",Position=new Vector3(6253,-1648,405),Lane=Lane.Bot},
            new LanePoint() { Name="DBot2T",Position=new Vector3(6272,384,384),Lane=Lane.Bot},
            new LanePoint() { Name="DBotHT",Position=new Vector3(6336,3032,384),Lane=Lane.Bot},

        };
        public static Vector3 GetClosestPoint(Lane Lane)
        {
            switch (Lane)
            {
                case Lane.Top:
                    return new Vector3(-6321, 4255, 384);
                    
                case Lane.Bot:
                    return new Vector3(6271, -4291, 384);
                    
                default:
                    return new Vector3(-600, -300, 384);

            }
        }
    }
    


    public enum Lane
    {
        Top,
        Mid,
        Bot
    }

    public  class LanePoint
    {
        public Vector3 Position { get; set; } = new Vector3();
        public Lane Lane { get; set; }
        public string Name { get; set; }
        
    }



    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class Objects
    {
        public class Towers
        {
            private static List<Unit> _towerList;

            public static List<Unit> GetTowers()
            {
                if (!Utils.SleepCheck("Towers.refresh")) return _towerList;
                _towerList = ObjectManager.GetEntitiesParallel<Unit>()
                    .Where(x => (
                    x.ClassID == ClassID.CDOTA_BaseNPC_Tower||x.ClassID==ClassID.CDOTA_BaseNPC_Barracks


                    ) && x.IsValid && x.IsAlive)
                    .ToList();
                if (_towerList.Any())
                    Utils.Sleep(1000, "Towers.refresh");
                return _towerList;
            }
        }

        public class Tempest
        {
            private static IEnumerable<Hero> _clones;
            private static IEnumerable<Hero> _clonesF;

            public static IEnumerable<Hero> GetCloneList(Hero me)
            {
                if (!Utils.SleepCheck("Tempest.refresh")) return _clones;
                _clones = ObjectManager.GetEntities<Hero>()
                    .Where(
                        x =>
                            x.IsAlive && x.IsControllable && x.Team == me.Team &&
                            x.Modifiers.Any(z => z.Name == "modifier_kill")).ToList();
                if (_clones.Any())
                    Utils.Sleep(100, "Tempest.refresh");
                return _clones;
            }

            public static IEnumerable<Hero> GetFullyCloneList(Hero me)
            {
                if (!Utils.SleepCheck("Tempest.refresh.fully")) return _clonesF;
                _clonesF = ObjectManager.GetEntities<Hero>()
                    .Where(
                        x =>
                            !Equals(x, me) && x.IsControllable && x.Team == me.Team).ToList();
                if (_clonesF.Any())
                    Utils.Sleep(100, "Tempest.refresh.fully");
                return _clonesF;
            }

        }
        public class Necronomicon
        {
            private static IEnumerable<Unit> _necronomicon;
            private static IEnumerable<Unit> _necronomiconF;

            public static IEnumerable<Unit> GetNecronomicons(Unit me)
            {
                if (!Utils.SleepCheck("Necronomicon.refresh")) return _necronomicon;
                _necronomicon =
                    ObjectManager.GetEntities<Unit>()
                        .Where(x => x.IsAlive && x.IsControllable && x.Team == me.Team && x.IsSummoned)
                        .ToList();
                if (_necronomicon.Any())
                    Utils.Sleep(100, "Necronomicon.refresh");
                return _necronomicon;
            }
            public static IEnumerable<Unit> GetFullyNecronomicons(Unit me)
            {
                if (!Utils.SleepCheck("Necronomicon.refresh.fully")) return _necronomiconF;
                _necronomiconF =
                    ObjectManager.GetEntities<Unit>()
                        .Where(x => x.IsControllable && x.Team == me.Team && x.IsSummoned)
                        .ToList();
                if (_necronomiconF.Any())
                    Utils.Sleep(100, "Necronomicon.refresh.fully");
                return _necronomiconF;
            }
        }
        public class LaneCreeps
        {
            private static IEnumerable<Unit> _laneCreepsList;

            public static IEnumerable<Unit> GetCreeps()
            {
                if (!Utils.SleepCheck("LaneCreeps.refresh")) return _laneCreepsList;
                _laneCreepsList =
                    ObjectManager.GetEntitiesParallel<Unit>()
                        .Where(x => x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane && x.IsValid && x.IsAlive)
                        .ToList();
                if (_laneCreepsList.Any())
                    Utils.Sleep(100, "LaneCreeps.refresh");
                return _laneCreepsList;
            }
        }
        public class Fountains
        {
            private static Unit _ally;
            private static Unit _enemy;
            public static Unit GetAllyFountain()
            {
                if (_ally == null || !_ally.IsValid)
                {
                    _ally = ObjectManager.GetEntitiesParallel<Unit>()
                        .FirstOrDefault(x => x.Team == ObjectManager.LocalHero.Team && x.ClassID == ClassID.CDOTA_Unit_Fountain);
                }
                return _ally;
            }
            public static Unit GetEnemyFountain()
            {
                if (_enemy == null || !_enemy.IsValid)
                {
                    _enemy = ObjectManager.GetEntitiesParallel<Unit>()
                        .FirstOrDefault(x => x.Team != ObjectManager.LocalHero.Team && x.ClassID == ClassID.CDOTA_Unit_Fountain);
                }
                return _enemy;
            }
        }
    }
    
}
