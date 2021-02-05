using System;
using System.Collections.Generic;
using System.Text;

using BepInEx.Configuration;
using BepInEx.Extensions.Configuration;

namespace TeleporterDropTierEditor.Configuration
{
    public class TeleporterConfig : ConfigDataModel
    {
        public override void SetDefaults()
        {
            SectionName = "General";
        }

        #region GENERAL
        public static ConfigData<bool> Enabled = new ConfigData<bool>()
        {
            DescriptionString = "Enables/Disables the entire module",
            DefaultValue = true
        };

        public static ConfigData<bool> DropUniformity = new ConfigData<bool>()
        {
            DescriptionString = "Whether or not all drops from the boss-teleporter event will be the same.",
            DefaultValue = true
        };
        #endregion

        #region TIER1
        public static ConfigData<bool> Tier1Drops = new ConfigData<bool>()
        {
            DescriptionString = "Allow Tier 1 (White) item drops from the teleporter",
            DefaultValue = false
        };

        public static ConfigData<float> Tier1DropWeightedChance = new ConfigData<float>()
        {
            DescriptionString = "The relative weighted chance that the drops are Tier 1 (White), the weight is compared to all other enabled tiers' weights",
            DefaultValue = 1f,
            AcceptableValues = new AcceptableValueRange<float>(0.1f, 100f)
        };

        public static ConfigData<int> Tier1DropMultiplier = new ConfigData<int>()
        {
            DescriptionString = "Multiplies the total amount of items dropped (capped at 100 for sanity)",
            DefaultValue = 1,
            AcceptableValues = new AcceptableValueRange<int>(1, 100)
        };
        #endregion

        #region TIER2
        public static ConfigData<bool> Tier2Drops = new ConfigData<bool>()
        {
            DescriptionString = "Allow Tier 2 (Green) item drops from the teleporter",
            DefaultValue = true
        };

        public static ConfigData<float> Tier2DropWeightedChance = new ConfigData<float>()
        {
            DescriptionString = "The relative weighted chance that the drops are Tier 2 (Green), the weight is compared to all other enabled tiers' weights",
            DefaultValue = 1f,
            AcceptableValues = new AcceptableValueRange<float>(0.1f, 100f)
        };

        public static ConfigData<int> Tier2DropMultiplier = new ConfigData<int>()
        {
            DescriptionString = "Multiplies the total amount of items dropped (capped at 100 for sanity)",
            DefaultValue = 1,
            AcceptableValues = new AcceptableValueRange<int>(1, 100)
        };
        #endregion

        #region TIER3
        public static ConfigData<bool> Tier3Drops = new ConfigData<bool>()
        {
            DescriptionString = "Allow Tier 3 (Red) item drops from the teleporter",
            DefaultValue = false
        };

        public static ConfigData<float> Tier3DropWeightedChance = new ConfigData<float>()
        {
            DescriptionString = "The relative weighted chance that the drops are Tier 3 (Red), the weight is compared to all other enabled tiers' weights",
            DefaultValue = 1f,
            AcceptableValues = new AcceptableValueRange<float>(0.1f, 100f)
        };

        public static ConfigData<int> Tier3DropMultiplier = new ConfigData<int>()
        {
            DescriptionString = "Multiplies the total amount of items dropped (capped at 100 for sanity)",
            DefaultValue = 1,
            AcceptableValues = new AcceptableValueRange<int>(1, 100)
        };
        #endregion

        #region TIER_BOSS
        public static ConfigData<bool> TierBossDrops = new ConfigData<bool>()
        {
            DescriptionString = "Allow Tier Boss (Yellow) item drops from the teleporter",
            DefaultValue = true
        };

        public static ConfigData<float> TierBossDropWeightedChance = new ConfigData<float>()
        {
            DescriptionString = "The relative weighted chance that the drops are Tier Boss (Yellow), the weight is compared to all other enabled tiers' weights",
            DefaultValue = 1f,
            AcceptableValues = new AcceptableValueRange<float>(0.1f, 100f)
        };

        public static ConfigData<int> TierBossDropMultiplier = new ConfigData<int>()
        {
            DescriptionString = "Multiplies the total amount of items dropped (capped at 100 for sanity)",
            DefaultValue = 1,
            AcceptableValues = new AcceptableValueRange<int>(1, 100)
        };
        #endregion

    }
}
