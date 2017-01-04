using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using System.Diagnostics.CodeAnalysis;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Objects;

namespace MyMonkey
{
    public class Nagaboy
    {
        public Nagaboy(Hero boy)
        {
            this.Boy = boy;
            handle = boy.Handle;
            me = ObjectManager.LocalHero;
            //qLevel = GetQlevel();
            //boyAttackDamage = GetBoyAttackDamage(qLevel);

            // 分路结束
            myLanePoints = MapInfo.LanePoints.Where(x => x.Lane == Lane).ToList();
            fountain = Objects.Fountains.GetAllyFountain();

        }
        public static List<Unit> tempUnits = new List<Unit>();
        public static List<Hero> tempHeros = new List<Hero>();
        private static Unit enemyFountain;
        private static Hero enemyHeroNearMyBoss;

        public TargetView TargetEsp = new TargetView();
        public Hero Boy { get; set; }
        public Hero Target { get; set; }
        public Entity EntityTarget { get; set; }
        public Lane Lane { get; set; }
        public uint handle;

        private List<Unit> myCreeps;
        private List<LanePoint> myLanePoints;
        private Hero enemyHero;
        private Unit targetCreep;
        private Unit moveCreep;
        private Unit rangeCreep;
        private Unit fountain;
        private Unit neuture;
        private Unit nearestTower;
        private Hero me;

        //private uint qLevel;
        //private int boyAttackDamage;





        //private static List<Nagaboy> TopBoys = new List<Nagaboy>();
        //private static List<Nagaboy> MidBoys = new List<Nagaboy>();
        //private static List<Nagaboy> BotBoys = new List<Nagaboy>();

        //private double BoysAttackTiming(Unit unit)
        //{
        //    if (unit.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege)
        //    {
        //        return (3 * (1 - targetCreep.DamageResist) * boyAttackDamage)*0.5 + 5;
        //    }
        //    else
        //    {
        //        return (3 * (1 - targetCreep.DamageResist) * boyAttackDamage) + 15;
        //    }
        //}
        //private uint GetQlevel()
        //{
        //    return me.Spellbook.SpellQ.Level;
        //}
        //private int GetBoyAttackDamage(uint qLevel)
        //{
        //    switch (qLevel)
        //    {
        //        case 1:
        //            return (int)(0.25 * me.MinimumDamage);
        //        case 2:
        //            return (int)(0.3 * me.MinimumDamage);
        //        case 3:
        //            return (int)(0.35 * me.MinimumDamage);
        //        case 4:
        //            return (int)(0.4 * me.MinimumDamage);
        //        default:
        //            return (int)(0.25 * me.MinimumDamage);
        //    }
        //}
        public static void UpdateBoys(Hero Naga, List<Nagaboy> NagaBoys)
        {


            if (Utils.SleepCheck("UpdateBoys_MAIN"))
            {

                tempUnits = ObjectManager.GetEntitiesParallel<Unit>().ToList();
                tempHeros = ObjectManager.GetEntitiesParallel<Hero>().ToList();
                enemyHeroNearMyBoss = tempHeros.Where(x => !x.IsIllusion && x.Team != Program.me.Team && x.IsAlive && x.IsVisible && x.Distance2D(Program.me) < 150).OrderBy(x => x.Health).FirstOrDefault();
                Program.target = tempHeros.Where(x => x.Team != Program.me.Team && !x.IsIllusion && x.IsAlive && x.Distance2D(Program.me) < 1500).OrderBy(x => x.Distance2D(Program.me)).FirstOrDefault();
                enemyFountain = tempUnits.FirstOrDefault(x => x.Team != ObjectManager.LocalHero.Team && x.ClassID == ClassID.CDOTA_Unit_Fountain);
                var illusions = ObjectManager.GetEntitiesParallel<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_PhantomLancer && x.IsIllusion && x.IsVisible && x.IsAlive && x.Team == Naga.Team).ToList();
                if (illusions.Count() > 0)
                {
                    foreach (var boy in illusions)
                    {
                        if (NagaBoys.All(x => x.Boy.Handle != boy.Handle))
                        {
                            Nagaboy newnagaboy = new Nagaboy(boy);

                            NagaBoys.Add(newnagaboy);
                            Console.WriteLine("added");
                        }
                    }
                }
                if (NagaBoys.Count() > 0)
                {
                    foreach (var nagaboy in NagaBoys)
                    {
                        if (!nagaboy.Boy.IsAlive)
                        {

                            NagaBoys.Remove(nagaboy);
                            //if (TopBoys.Contains(nagaboy))
                            //    TopBoys.Remove(nagaboy);
                            //if (MidBoys.Contains(nagaboy))
                            //    MidBoys.Remove(nagaboy);
                            //if (BotBoys.Contains(nagaboy))
                            //    BotBoys.Remove(nagaboy);

                        }
                        //if (nagaboy.Target == null || nagaboy.Target.Distance2D(nagaboy.Boy) > 1500 || !nagaboy.Target.IsAlive || !nagaboy.Target.IsVisible)
                        //{
                        //    nagaboy.Target = TargetSelector.BestAutoAttackTarget(nagaboy.Boy, 500);
                        //}
                    }

                }
                Utils.Sleep(200, "UpdateBoys_MAIN");
            }
        }
        public void GetNearestInfo()
        {

            enemyHero = tempHeros.Where(x => !x.IsIllusion && x.Team != Boy.Team && x.IsAlive && x.IsVisible && x.Distance2D(Boy) < 1500).OrderBy(x => x.Health).FirstOrDefault();
            targetCreep = tempUnits.Where(x => x.Team != Boy.Team &&!x.IsWaitingToSpawn&& x.IsAlive && (x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege) && x.Distance2D(Boy) < 900).OrderBy(x => x.Health).FirstOrDefault();
            //rangeCreep= Objects.LaneCreeps.GetCreeps().Where(x => x.Team != Boy.Team&&x.IsAlive && x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane && x.AttackRange==500 && x.Distance2D(Boy) < 1000).OrderBy(x => x.Health).FirstOrDefault();
            moveCreep = tempUnits.Where(x => x.Team != Boy.Team &&!x.IsWaitingToSpawn&& x.IsAlive && (x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege) && x.Distance2D(Boy) < 1700).OrderBy(x => x.Health).FirstOrDefault();
            neuture = tempUnits.Where(x => x.Team != Boy.Team &&!x.IsWaitingToSpawn&& x.IsAlive&&x.IsValid && (x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral) && x.Distance2D(Boy) < 300).OrderBy(x => x.Health).FirstOrDefault();
            nearestTower = tempUnits.Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_Tower || x.ClassID == ClassID.CDOTA_BaseNPC_Barracks) && x.IsValid && x.IsAlive && !x.IsInvul() && x.Team == Boy.GetEnemyTeam()).OrderBy(y => y.Distance2D(Boy)).FirstOrDefault() ?? enemyFountain;
            
        }
        
        private void SmartPush()
        {
            if (targetCreep == null && moveCreep == null)
            {

                Boy.Attack(nearestTower);
            }
            if (targetCreep == null && moveCreep != null)
            {
                Boy.Attack(moveCreep);
            }
            else
            {
                if (targetCreep != null)
                {
                    Boy.Attack(targetCreep);
                }
            }
        }
        public static void SmartAttack(Unit me, List<Unit> myCreeps, Unit nearestTower, Vector3 pos)
        {
            var name = me.StoredName();
            if (myCreeps.Any(x => x.Distance2D(nearestTower) <= 800) && me.Distance2D(nearestTower) <= 1000)
            {
                var hpwasChanged = CheckForChangedHealth(me);
                if (hpwasChanged)
                {
                    var allyCreep = myCreeps.OrderBy(x => x.Distance2D(me)).First();
                    if (allyCreep != null)
                    {
                        var towerPos = nearestTower.Position;
                        var ang = allyCreep.FindAngleBetween(towerPos, true);
                        var p = new Vector3((float)(allyCreep.Position.X - 250 * Math.Cos(ang)),
                            (float)(allyCreep.Position.Y - 250 * Math.Sin(ang)), 0);
                        me.Move(p);
                        me.Attack(allyCreep, true);
                        Utils.Sleep(1200, name + "attack");
                    }
                    else
                    {
                        var towerPos = nearestTower.Position;
                        var ang = me.FindAngleBetween(towerPos, true);
                        var p = new Vector3((float)(me.Position.X - 1000 * Math.Cos(ang)),
                            (float)(me.Position.Y - 1000 * Math.Sin(ang)), 0);
                        me.Move(p);
                        Utils.Sleep(500, name + "attack");
                    }
                }
                else
                {
                    me.Attack(pos);
                    Utils.Sleep(500, name + "attack");
                }
            }
            else
            {
                me.Attack(pos);
                Utils.Sleep(500, name + "attack");
            }
        }
        private static bool CheckForChangedHealth(Unit me)
        {
            uint health;
            if (!LastCheckedHp.TryGetValue(me, out health))
            {
                LastCheckedHp.Add(me, me.Health);
            }
            var boolka = health > me.Health;
            LastCheckedHp[me] = me.Health;
            return boolka;
        }
        private static readonly Dictionary<Unit, uint> LastCheckedHp = new Dictionary<Unit, uint>();

        //带线
        public void PushLane()
        {
            if (!Boy.IsAlive)
                return;
            handle = Boy.Handle;
            myCreeps = Objects.LaneCreeps.GetCreeps().Where(x => x.Team == Boy.Team).ToList();
            enemyHero = ObjectManager.GetEntitiesParallel<Hero>().Where(x => x.Team != Boy.Team && !x.IsIllusion && x.IsAlive && x.IsVisible && x.Health < 500 && x.Distance2D(Boy) < 800).OrderBy(x => x.Health).FirstOrDefault();
            var clospoint = MapInfo.GetClosestPoint(Lane);
            nearestTower =
                Objects.Towers.GetTowers()
                    .Where(x => x.Team == Boy.GetEnemyTeam())
                    .OrderBy(y => y.Distance2D(clospoint))
                    .FirstOrDefault() ?? Objects.Fountains.GetEnemyFountain();
            var nearestCreep =
                Objects.LaneCreeps.GetCreeps()
                .Where(x => x.Team == Boy.GetEnemyTeam())
                .OrderBy(y => y.Distance2D(clospoint))
                .FirstOrDefault() ?? nearestTower;
            // 分身在己方半场
            var useThisShit = clospoint.Distance2D(fountain) - 250 > Boy.Distance2D(fountain);


            //攻击血量小于X的敌人
            if (enemyHero != null && enemyHero.Distance2D(Boy) < 800)
            {
                if (Utils.SleepCheck(handle + "chase"))
                {
                    Boy.Attack(enemyHero);
                    Utils.Sleep(200, handle + "chase");
                }
            }
            else
            {
                if (nearestTower != null)
                {//在中路或者在对方半场    pos为最近塔的位置    否则为此线中心点
                    var pos = (Lane == Lane.Mid || !useThisShit) ? nearestTower.Position : clospoint;

                    //if (Utils.SleepCheck("nagaboy.Attack.Tower.Cd" + handle))
                    //{

                    //    if (!Boy.IsAttacking())
                    //    {
                    //        Boy.Attack(nearestCreep);
                    //    }
                    //    Utils.Sleep(1000, "nagaboy.Attack.Tower.Cd" + handle);
                    //}


                    if (Utils.SleepCheck("nagaboy.Attack.Cd" + handle) && !Boy.IsAttacking())
                    {
                        if (myLanePoints.All(x => x.Position.Distance2D(nearestCreep.Position) > 2000))
                        {
                            Boy.Attack(nearestTower);
                        }
                        else
                        {
                            if (Boy.Distance2D(nearestCreep.Position) < 800)
                            {
                                Boy.Attack(nearestCreep.Position);
                            }
                            else
                            {
                                Boy.Move(nearestCreep.Position);
                            }
                        }
                        Utils.Sleep(1000, "nagaboy.Attack.Cd" + handle);
                    }
                    // smart attack for necrobook (unaggro under tower)
                    if (Utils.SleepCheck(Boy.StoredName() + "attack"))
                    {
                        if (Boy.Distance2D(nearestCreep.Position) < 800)
                            SmartAttack(Boy, myCreeps, nearestTower, pos);
                    }


                }
            }
            if (Utils.SleepCheck(handle + "move"))
            {

                Utils.Sleep(500, handle + "move");
            }
        }
        //线上帮助
        //public void DoLaneWork()
        //{
        //    if (Utils.SleepCheck(handle + "update_near_info"))
        //    {
        //        GetNearestInfo();
        //        Utils.Sleep(300, handle + "update_near_info");
        //    }

        //    if (targetCreep != null&&Utils.SleepCheck(handle + "attacklowesthp"))
        //    {

        //        if (targetCreep.Health < BoysAttackTiming(targetCreep))
        //        {
        //            Boy.Attack(targetCreep);

        //        }
        //        else
        //        {


        //            Boy.Move(targetCreep.Position);

        //                Console.WriteLine(BoysAttackTiming(targetCreep));

        //            //Console.WriteLine();
        //        }
        //        Utils.Sleep(400, handle + "attacklowesthp");
        //    }
        //}
        public void PushSingleLane()
        {


            if (Utils.SleepCheck(handle + "update_near_info"))
            {
                GetNearestInfo();
                if (enemyHeroNearMyBoss != null)
                {
                    Boy.Attack(enemyHeroNearMyBoss);
                }
                else
                {
                    if (neuture != null)
                    {
                        Boy.Attack(neuture);
                    }
                    else
                    {
                        SmartPush();
                    }
                }
                Utils.Sleep(200, handle + "update_near_info");
            }


            //if (targetCreep != null && Utils.SleepCheck(handle + "attacklowesthp"))
            //{

            //    if (targetCreep.Health < BoysAttackTiming(targetCreep))
            //    {
            //        Boy.Attack(targetCreep);

            //    }
            //    else
            //    {


            //        Boy.Move(targetCreep.Position);

            //        Console.WriteLine(BoysAttackTiming(targetCreep));

            //        //Console.WriteLine();
            //    }
            //    Utils.Sleep(400, handle + "attacklowesthp");
            //}
        }
        public void FightFirst()
        {
            if (Utils.SleepCheck(handle + "update_near_info"))
            {
                GetNearestInfo();
                if (enemyHeroNearMyBoss != null)
                {
                    Boy.Attack(enemyHeroNearMyBoss);
                }
                else
                {
                    if (neuture != null)
                    {
                        Boy.Attack(neuture);

                    }
                    else
                    {
                        if (enemyHero != null)
                        {
                            Boy.Attack(enemyHero);
                        }
                        else
                        {
                            SmartPush();
                        }
                    }
                }
                Utils.Sleep(200, handle + "update_near_info");
            }

            //if (Utils.SleepCheck(handle + "attacklowesthp"))
            //{


            //Utils.Sleep(300, handle + "attacklowesthp");
        }

        //if (targetCreep != null && Utils.SleepCheck(handle + "attacklowesthp"))
        //{

        //    if (targetCreep.Health < BoysAttackTiming(targetCreep))
        //    {
        //        Boy.Attack(targetCreep);

        //    }
        //    else
        //    {


        //        Boy.Move(targetCreep.Position);

        //        Console.WriteLine(BoysAttackTiming(targetCreep));

        //        //Console.WriteLine();
        //    }
        //    Utils.Sleep(400, handle + "attacklowesthp");
        //}


        public class TargetView
        {
            private ParticleEffect targetPartical;
            public void TargetEsp(Hero hunter, Hero target)
            {
                if (!Game.IsInGame || Game.IsWatchingGame)
                    return;

                if (hunter == null)
                    return;
                if (targetPartical == null && target != null)
                {
                    targetPartical = new ParticleEffect(@"particles\ui_mouseactions\range_finder_tower_aoe.vpcf", target);
                }
                if ((target == null || !target.IsVisible || !target.IsAlive || !hunter.IsAlive) && targetPartical != null)
                {
                    targetPartical.Dispose();
                    targetPartical = null;
                }
                if (target != null && targetPartical != null)
                {
                    targetPartical.SetControlPoint(2, hunter.Position);
                    targetPartical.SetControlPoint(6, new Vector3(1, 0, 0));
                    targetPartical.SetControlPoint(7, target.Position);
                }
            }

        }
    }
}
