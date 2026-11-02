using BepInEx.Configuration;
using BepInEx.Logging;
using REPOShock.src.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.FullSerializer;

namespace REPOShock.src
{
    internal class ConfigHandler
    {
        // Config
        public static ConfigEntry<int> MaxIntensity { get; private set; } = null!;
        public static ConfigEntry<int> DeathIntensity { get; private set; } = null!;
        public static ConfigEntry<int> DeathDuration { get; private set; } = null!;
        public static ConfigEntry<int> HurtDuration { get; private set; } = null!;
        public static ConfigEntry<float> DamageInteravl { get; private set; } = null!;
        public static ConfigEntry<bool> EnableBreakShock { get; private set; } = null!;
        public static ConfigEntry<bool> BreakIgnoreEnemy { get; private set; } = null!;
        public static ConfigEntry<bool> MapValueToShock { get; private set; } = null!;
        public static ConfigEntry<float> BreakValueDmgMult { get; private set; } = null!;
        public static ConfigEntry<int> BreakShockWindow { get; private set; } = null!;


        public static ConfigEntry<float> ThrownOffensiveGracePeriod { get; private set; } = null!;
        public static ConfigEntry<float> HitOffensiveGracePeriod { get; private set; } = null!;

        public static void InitConfig(ConfigFile cfg)
        {
            // Settings Config
            BindConfigEntries(cfg);
            CleanupLegacyConfig(cfg);
        }

        private static void BindConfigEntries(ConfigFile cfg)
        {
            MaxIntensity = cfg.Bind
            (
                "Settings",
                "Max_Intensity",
                100,
                new ConfigDescription
                (
                    "The maximum amount of an intensity a shock will ever be.",
                    new AcceptableValueRange<int>
                    (
                        1,
                        100
                    )
                )
            );

            DamageInteravl = cfg.Bind
            (
                "Settings",
                "Damage_Interval",
                1.0f,
                new ConfigDescription
                (
                    "The intensity for each damage dealt to you. Shock will always be rounded up.",
                    new AcceptableValueRange<float>
                    (
                        0.1f,
                        100
                    )
                )
            );

            DeathIntensity = cfg.Bind
            (
                "Settings",
                "Death_Intensity",
                50,
                new ConfigDescription
                (
                    "The intensity when you die. (1-100)",
                    new AcceptableValueRange<int>
                    (
                        1,
                        100
                    )
                )
            );

            DeathDuration = cfg.Bind
            (
                "Settings",
                "Death_Duration",
                3,
                new ConfigDescription
                (
                    "The duration when you die. (1-15)s or (16-15,000)ms\n"
                    + "Any value below between 1-15 will be treated in seconds, anything over 15 will be ms",
                    new AcceptableValueRange<int>
                    (
                        1,
                        15_000
                    )
                )
            );

            HurtDuration = cfg.Bind
            (
                "Settings",
                "Hurt_Duration",
                500,
                new ConfigDescription
                (
                    "The duration when you get hurt/break something. (1-15)s or (16-15,000)ms\n"
                    + "Any value below between 1-15 will be treated in seconds, anything over 15 will be ms",
                    new AcceptableValueRange<int>
                    (
                        1,
                        15_000
                    )
                )
            );

            // Config for breaking items

            EnableBreakShock = cfg.Bind
            (
                "Settings_Items",
                "Enable_Breaking_Item_Shocks",
                true,
                new ConfigDescription
                (
                    "If enabled, will shock you whenever a valuable item you're holding takes damage."
                )
            );

            MapValueToShock = cfg.Bind
            (
                "Settings_Items",
                "Map_Value_to_Shock",
                false,
                new ConfigDescription
                (
                    "If enabled, shock intensity will be mapped to valuable objects original value.\n"
                    + "If you break lose 250 durability on an item that started with 500, you would get\n"
                    + "shocked for 50% of your max intensity."
                )
            );

            BreakValueDmgMult = cfg.Bind
            (
                "Settings_Items",
                "Value_Loss_Multiplier",
                0.1f,
                new ConfigDescription
                (
                    "The intensity of the shock based on value lost. Damage an item for 200 value? at 0.1,\n"
                    + "this will shock you for 20%. Caps at MaxIntensity",
                    new AcceptableValueRange<float>
                    (
                        0.1f,
                        100
                    )
                )
            );

            BreakShockWindow = cfg.Bind
            (
                "Settings_Items",
                "Break_Shock_Window",
                15,
                new ConfigDescription
                (
                    "The amount of time a Valuable Object you have touched is tied to you.\n"
                    + "If it gets damaged during this period, you will get shocked.",
                    new AcceptableValueRange<int>
                    (
                        1,
                        120
                    )
                )
            );

            // Config for breaking items offensive

            BreakIgnoreEnemy = cfg.Bind
            (
                "Settings_Items",
                "Ignore_Hitting_Enemies",
                true,
                new ConfigDescription
                (
                    "If enabled, hitting an enemy with an item won't shock you."
                )
            );

            ThrownOffensiveGracePeriod = cfg.Bind
            (
                "Settings_Items",
                "Thrown_Offensive_Grace_Period",
                2.0f,
                new ConfigDescription
                (
                    "The amount of time an object you let go of has to hit an enemy before it shocks you.\n"
                    + "This also starts the HitOffensiveGracePeriod, so you can keep using it as a weapon.",
                    new AcceptableValueRange<float>
                    (
                        1f,
                        5f
                    )
                )
            );

            HitOffensiveGracePeriod = cfg.Bind
            (
                "Settings_Items",
                "Hit_Offensive_Grace_Period",
                3.0f,
                new ConfigDescription
                (
                    "The amount of time an object you use as a weapon has before it causes you to get\n"
                    + "shocked again. After this times up, if you don't hit an enemy again, the item goes\n"
                    + "back to shocking you if it takes damage.",
                    new AcceptableValueRange<float>
                    (
                        1f,
                        5f
                    )
                )
            );
        }

        private static void CleanupLegacyConfig(ConfigFile cfg)
        {
            const string defaultValue = "__";

            ConfigDefinition[] defs =
            {
                new("Auth_PiShock", "APIKey"),
                new("Auth_PiShock", "ShareCodes"),
                new("Auth_PiShock", "UserName"),
            };

            var changed = false;
            var saveOnSet = cfg.SaveOnConfigSet;
            cfg.SaveOnConfigSet = false;

            try
            {
                foreach (var def in defs)
                {
                    var entry = cfg.Bind(def, defaultValue);

                    if (entry.Value != defaultValue)
                    {
                        if (def.Key == "APIKey")
                        {
                            REPOShock.Logger.LogInfo($"Migrating existing {def.Key} to new system.");
                            ApiKeyStore.SaveKey(entry.Value);
                        }

                        REPOShock.Logger.LogInfo($"Removed legacy config {def.Section}/{def.Key}");
                        changed = true;
                    }

                    cfg.Remove(def);
                }
            }
            finally
            {
                cfg.SaveOnConfigSet = saveOnSet;
            }

            if (changed) { cfg.Save(); }
        }
    }

}

