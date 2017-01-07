using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;

namespace MyMonkey
{
    class Program
    {
        public static Hero target;
        public static Hero me;
        private static readonly Menu Menu = new Menu("Lancer", "Lancer", true, "npc_dota_hero_phantom_lancer", true);

        private static List<Nagaboy> NagaBoys = new List<Nagaboy>();
        
        
        static void Main(string[] args)
        {
            //Menu.AddItem(new MenuItem("FFF", "FFF").SetValue(new KeyBind('J', KeyBindType.Press)));
            //Menu.AddItem(new MenuItem("KillJungleFirst", "KillJungleFirst").SetValue(new KeyBind('J', KeyBindType.Toggle)));
            Menu.AddItem(new MenuItem("Fight", "Fight").SetValue(new KeyBind('J', KeyBindType.Toggle)));
            Menu.AddItem(new MenuItem("PushSingleLane", "PushSingleLane").SetValue(new KeyBind('J', KeyBindType.Toggle)));
            Menu.AddToMainMenu();
            Game.OnIngameUpdate += Game_OnIngameUpdate;
            Drawing.OnDraw += Esp;
            
            
        }
        private static void Esp(EventArgs args)
        {
        }
        private static void Game_OnIngameUpdate(EventArgs args)
        {
            if (Utils.SleepCheck("wbw_getlocalHero"))
            {
                me = ObjectManager.LocalHero;
                Utils.Sleep(1000, "wbw_getlocalHero");
            }
            Nagaboy.UpdateBoys(me, NagaBoys);   

            
            foreach(var nagaboy in NagaBoys)
            {
                //if (Menu.Item("LinePush").GetValue<KeyBind>().Active)
                //{
                //    nagaboy.PushLane();
                //}
                //if (Menu.Item("OnLane").GetValue<KeyBind>().Active)
                //{
                //    //nagaboy.DoLaneWork();

                //}
                //if (Menu.Item("CCC").GetValue<KeyBind>().Active)
                //{
                //    if (Utils.SleepCheck("HitClosest"))
                //    {

                //        if (target != null)
                //        {
                //            nagaboy.Boy.Attack(target);
                //        }
                //        Utils.Sleep(200, "HitClosest");
                //    }
                //    return;
                //}
                if (nagaboy.Boy.IsAlive)
                {

                    if (Menu.Item("PushSingleLane").GetValue<KeyBind>().Active)
                    {
                        nagaboy.PushSingleLane();
                        Console.WriteLine("push single lane");
                    }
                    if (Menu.Item("Fight").GetValue<KeyBind>().Active && !Menu.Item("PushSingleLane").GetValue<KeyBind>().Active)
                    {

                        nagaboy.FightFirst();
                    }
                }
            }
            

            


            if (Utils.SleepCheck("MyNageInfo"))
            {

                foreach(var nagaboy in NagaBoys)
                {
                    Console.WriteLine(nagaboy.Boy.Handle+"  "+nagaboy.Lane);
                    
                }
                Console.WriteLine("-----------------");
                Utils.Sleep(1500, "MyNageInfo");
            }
        }
    }
}
