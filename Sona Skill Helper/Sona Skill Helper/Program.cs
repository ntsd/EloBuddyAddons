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
            R = new Spell.Skillshot(SpellSlot.R, 1000, SkillShotType.Circular, 250, 2400, 140);
            Exhaust = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerexhaust"), 650);

            SonaHelperMenu = MainMenu.AddMenu("Sona Skill Helper", "Sona Skill Helper");
            SkillMenu = SonaHelperMenu.AddSubMenu("SkillMenu", "skillmenu");
            SkillMenu.AddGroupLabel("Skill Settings");
            SkillMenu.AddSeparator();
            SkillMenu.Add("useskillq", new CheckBox("Auto Q"));
            SkillMenu.Add("useskillr", new CheckBox("Auto R"));
            SkillMenu.Add("rslider", new Slider("Minimum Enemy for Auto R", 1, 0, 5));
            //SkillMenu.Add("comboOnlyExhaust", new CheckBox("Auto Exhaust"));
            SkillMenu.AddSeparator();


            Game.OnUpdate += Game_OnTick;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                AutoSkill();
        }

        public static
        void AutoSkill()
        {
            var target = TargetSelector.GetTarget(1000, DamageType.Magical);
            if (target == null || !target.IsValid())
            {
                return;
            }
            if (SkillMenu["useskillq"].Cast<CheckBox>().CurrentValue)
                if (Q.IsReady() && Sona.CountEnemiesInRange(Q.Range) >= 1)
                {
                    Q.Cast();
                }
            if (SkillMenu["useskillr"].Cast<CheckBox>().CurrentValue)
                if (R.IsReady())
                {
                    var predR = R.GetPrediction(target).CastPosition;
                    if (target.CountEnemiesInRange(R.Width) >= SkillMenu["rslider"].Cast<Slider>().CurrentValue)
                        R.Cast(predR);
                }
        }
    }
}

