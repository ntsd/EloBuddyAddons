using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace Sona_Skill_Helper
{
    internal class Program
    {
        public static Spell.Active Q;
        public static Spell.Active W;
        public static Spell.Active E;
        public static Spell.Skillshot R;
        public static Spell.Targeted Exhaust;
        public static AIHeroClient Sona = ObjectManager.Player;

        public static Menu SonaHelperMenu,
            SkillMenu,
            DrawMenu;

        public static AIHeroClient SelectedHero { get; set; }

        private static Vector3 MousePos
        {
            get { return Game.CursorPos; }
        }

        public static bool HasSpell(string s)
        {
            return Player.Spells.FirstOrDefault(o => o.SData.Name.Contains(s)) != null;
        }

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoaded;
        }

        private static void OnLoaded(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Sona")
                return;
            Bootstrap.Init(null);
            Q = new Spell.Active(SpellSlot.Q, 850);
            W = new Spell.Active(SpellSlot.W, 1000);
            E = new Spell.Active(SpellSlot.E, 350);
            R = new Spell.Skillshot(SpellSlot.R, 900, SkillShotType.Circular, 250, 2400, 140);
            Exhaust = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerexhaust"), 650);

            SonaHelperMenu = MainMenu.AddMenu("Sona Skill Helper", "Sona Skill Helper");
            SkillMenu = SonaHelperMenu.AddSubMenu("SkillMenu", "skillmenu");
            SkillMenu.AddGroupLabel("Skill Settings");
            SkillMenu.AddSeparator();
            SkillMenu.Add("autoskill", new CheckBox("Auto Skill"));
            SkillMenu.AddSeparator();
            SkillMenu.Add("autoskillq", new CheckBox("Auto Q"));
            SkillMenu.Add("qslider", new Slider("Numbor of Opponent in range for Auto Q", 2, 1, 2));
            SkillMenu.Add("autoskillr", new CheckBox("Auto R"));
            SkillMenu.Add("rslider", new Slider("Numbor of Opponent in range for Auto R", 3, 1, 5));
            //SkillMenu.Add("comboOnlyExhaust", new CheckBox("Auto Exhaust"));

            DrawMenu = SonaHelperMenu.AddSubMenu("DrawMenu", "drawmenu");
            SkillMenu.AddGroupLabel("Draw Settings");
            SkillMenu.AddSeparator();
            DrawMenu.Add("drawq", new CheckBox("Draw Q"));
            DrawMenu.Add("draww", new CheckBox("Draw W"));
            DrawMenu.Add("drawr", new CheckBox("Draw R"));


            Game.OnUpdate += Game_OnTick;
            Drawing.OnDraw += OnDraw;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (SkillMenu["autoskill"].Cast<CheckBox>().CurrentValue)
            {
                AutoSkill();
            } 
        }

        public static
        void AutoSkill()
        {
            var target = TargetSelector.GetTarget(900, DamageType.Magical);
            if (target == null || !target.IsValid())
            {
                return;
            }
            if (SkillMenu["autoskillq"].Cast<CheckBox>().CurrentValue)
                if (Q.IsReady() && Sona.CountEnemiesInRange(Q.Range) >= SkillMenu["qslider"].Cast<Slider>().CurrentValue)
                {
                    Q.Cast();
                }
            if (SkillMenu["autoskillr"].Cast<CheckBox>().CurrentValue)
                if (R.IsReady())
                {
                    var predR = R.GetPrediction(target).CastPosition;
                    if (target.CountEnemiesInRange(R.Width) >= SkillMenu["rslider"].Cast<Slider>().CurrentValue)
                        R.Cast(predR);
                }
        }

        private static void OnDraw(EventArgs args)
        {
            if (!Sona.IsDead)
            {
                if (DrawMenu["drawq"].Cast<CheckBox>().CurrentValue && Q.IsLearned)
                {
                    Circle.Draw(Color.Blue, Q.Range, Player.Instance.Position);
                }
                if (DrawMenu["draww"].Cast<CheckBox>().CurrentValue && W.IsLearned)
                {
                    Circle.Draw(Color.Green, W.Range, Player.Instance.Position);
                }
                if (DrawMenu["drawr"].Cast<CheckBox>().CurrentValue && W.IsLearned)
                {
                    Circle.Draw(Color.Yellow, R.Range, Player.Instance.Position);
                }
            }
        }
    }
}

