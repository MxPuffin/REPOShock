﻿using BepInEx.Configuration;
using PiShockClassLibrary.Interfaces;
using PiShockClassLibrary.Models;
using PiShockClassLibrary.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.FullSerializer;

namespace REPOShock.src
{
    internal class ConfigHandler
    {
        internal static ConfigHandler Instance { get; private set; } = null!;

        // Config
        public static ConfigEntry<int>? ConfigMaxIntensity { get; private set; }
        public static ConfigEntry<int>? ConfigDeathIntensity { get; private set; }
        public static ConfigEntry<int>? ConfigDeathDuration { get; private set; }
        public static ConfigEntry<float>? ConfigDamageInteravl { get; private set; }
        public static ConfigEntry<bool>? ConfigEnableBreakShock { get; private set; }
        public static ConfigEntry<bool>? ConfigBreakIgnoreEnemy { get; private set; }
        public static ConfigEntry<bool>? ConfigMapValueToShock { get; private set; }
        public static ConfigEntry<float>? ConfigBreakValueDmgMult { get; private set; }
        public static ConfigEntry<int>? ConfigBreakShockWindow { get; private set; }


        public static ConfigEntry<float>? ThrownOffensiveGracePeriod { get; private set; }
        public static ConfigEntry<float>? HitOffensiveGracePeriod { get; private set; }

        public static void InitConfig()
        {
            // Settings Config
            ConfigMaxIntensity = REPOShock.CFG.Bind("Settings",
                "Max_Intensity",
                100,
                new ConfigDescription(
                    "The maximum amount of an intensity a shock will ever be.",
                    new AcceptableValueRange<int>(1,100)));

            ConfigDamageInteravl = REPOShock.CFG.Bind("Settings",
                "Damage_Interval",
                1.0f,
                new ConfigDescription(
					"The intensity for each damage dealt to you. Shock will always be rounded up.",
                    new AcceptableValueRange<float>(0.1f, 100)));

            ConfigDeathIntensity = REPOShock.CFG.Bind("Settings",
                "Death_Intensity",
                50,
                new ConfigDescription(
					"The intensity when you die. (1-100)",
                    new AcceptableValueRange<int>(1,100)));

            ConfigDeathDuration = REPOShock.CFG.Bind("Settings",
                "Max_Duration",
                3,
                new ConfigDescription(
                    "The duration when you die. (1-15)",
                    new AcceptableValueRange<int>(1, 15)));

            // Config for breaking items

            ConfigEnableBreakShock = REPOShock.CFG.Bind("Settings_Items",
                "Enable_Breaking_Item_Shocks",
                true,
                "If enabled, will shock you whenever a valuable item you're holding takes damage.");

            ConfigMapValueToShock = REPOShock.CFG.Bind("Settings_Items",
                "Map_Value_to_Shock",
                false,
                "If enabled, shock intensity will be mapped to valuable objects original value.\n" +
                "If you break lose 250 durability on an item that started with 500, you would get\n" +
                "shocked for 50% of your max intensity.");

            ConfigBreakValueDmgMult = REPOShock.CFG.Bind("Settings_Items",
                "Value_Loss_Multiplier",
                0.1f,
                new ConfigDescription(
				"The intensity of the shock based on value lost. Damage an item for 200 value? at 0.1,\n" +
				"this will shock you for 20%. Caps at MaxIntensity",
                new AcceptableValueRange<float>(0.1f, 100)));

            ConfigBreakShockWindow = REPOShock.CFG.Bind("Settings_Items",
                "Break_Shock_Window",
                15,
                new ConfigDescription(
					"The amount of time a Valuable Object you have touched is tied to you.\n" +
				    "If it gets damaged during this period, you will get shocked.",
                    new AcceptableValueRange<int>(1,120)));

            // Config for breaking items offensive

            ConfigBreakIgnoreEnemy = REPOShock.CFG.Bind("Settings_Items",
                "Ignore_Hitting_Enemies",
                true,
                "If enabled, hitting an enemy with an item won't shock you.");
            ThrownOffensiveGracePeriod = REPOShock.CFG.Bind("Settings_Items",
                "Thrown_Offensive_Grace_Period",
                2.0f,
                new ConfigDescription(
					"The amount of time an object you let go of has to hit an enemy before it shocks you.\n" +
				    "This also starts the HitOffensiveGracePeriod, so you can keep using it as a weapon.",
                    new AcceptableValueRange<float>(1f, 5f)));
            HitOffensiveGracePeriod = REPOShock.CFG.Bind("Settings_Items",
                "Hit_Offensive_Grace_Period",
                3.0f,
                new ConfigDescription(
					"The amount of time an object you use as a weapon has before it causes you to get\n" +
				    "shocked again. After this times up, if you don't hit an enemy again, the item goes\n" +
				    "back to shocking you if it takes damage.",
                    new AcceptableValueRange<float>(1f, 5f)));
        }
    }

}

