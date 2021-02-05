using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TeleporterDropTierEditor.Configuration;

using RoR2;

using BepInEx.Extensions.Configuration;
using UnityEngine;

namespace TeleporterDropTierEditor.Controllers
{
    class TeleporterController :  ControllerBase
    {
        protected override void OnInit()
        {
            Config.BindModel<TeleporterConfig>(Logger);
        }

        protected override void OnPostInit()
        {
            InitTierWeights();
            TeleporterConfig.Tier1Drops.OnSettingChanged += (object sender, EventArgs e) => { this.InitTierWeights(); };
            TeleporterConfig.Tier1DropWeightedChance.OnSettingChanged += (object sender, EventArgs e) => { this.InitTierWeights(); };
            TeleporterConfig.Tier2Drops.OnSettingChanged += (object sender, EventArgs e) => { this.InitTierWeights(); };
            TeleporterConfig.Tier2DropWeightedChance.OnSettingChanged += (object sender, EventArgs e) => { this.InitTierWeights(); };
            TeleporterConfig.Tier3Drops.OnSettingChanged += (object sender, EventArgs e) => { this.InitTierWeights(); };
            TeleporterConfig.Tier3DropWeightedChance.OnSettingChanged += (object sender, EventArgs e) => { this.InitTierWeights(); };
            TeleporterConfig.TierBossDrops.OnSettingChanged += (object sender, EventArgs e) => { this.InitTierWeights(); };
            TeleporterConfig.TierBossDropWeightedChance.OnSettingChanged += (object sender, EventArgs e) => { this.InitTierWeights(); };
        }

        protected override void OnEnable()
        {
            if (TeleporterConfig.Enabled)
                On.RoR2.BossGroup.DropRewards += BossGroup_DropRewards;
        }

        protected override void OnDisable()
        {
            On.RoR2.BossGroup.DropRewards -= BossGroup_DropRewards;
        }

        protected override void OnDispose()
        {
            WeightedTiers = null;
        }

        private void BossGroup_DropRewards(On.RoR2.BossGroup.orig_DropRewards orig, BossGroup self)
        {
            if (Run.instance.participatingPlayerCount > 0 && self.dropPosition)
            {
                Stack<PickupIndex> itemDrops = new Stack<PickupIndex>();    //one entry per item to be dropped.
                int baseDropCount = (1 + self.bonusRewardCount) * (self.scaleRewardsByPlayerCount ? Run.instance.participatingPlayerCount : 1); //base drop count

                //Add items to the drop table;

                PickupIndex t1, t2, t3, tboss;
                t1 = self.rng.NextElementUniform<PickupIndex>(Run.instance.availableTier1DropList);
                t2 = self.rng.NextElementUniform<PickupIndex>(Run.instance.availableTier2DropList);
                t3 = self.rng.NextElementUniform<PickupIndex>(Run.instance.availableTier3DropList);
                if (self.bossDrops.Count > 0)   //Boss drops not guaranteed.
                    tboss = self.rng.NextElementUniform<PickupIndex>(self.bossDrops);
                else
                    tboss = t2;

#if DEBUG
                Logger.LogWarning($"TELEPDROP::BossGroup..()| t1.Value: {t1.pickupDef.nameToken}");
                Logger.LogWarning($"TELEPDROP::BossGroup..()| t2.Value: {t2.pickupDef.nameToken}");
                Logger.LogWarning($"TELEPDROP::BossGroup..()| t3.Value: {t3.pickupDef.nameToken}");
                Logger.LogWarning($"TELEPDROP::BossGroup..()| tboss.Value: {tboss.pickupDef.nameToken}");
#endif

                if (TeleporterConfig.DropUniformity)    //only one item type
                {
#if DEBUG
                    Logger.LogWarning($"TELEPDROP::BossGroup..()| Using DropUniformity");
#endif
                    PickupIndex itemPI;
                    int mult = 0;
                    switch (GetWeightedChance(WeightedTiers))
                    {
                        case ItemTier.Tier1:
                            itemPI = t1;
                            mult = TeleporterConfig.Tier1DropMultiplier;
                            break;

                        case ItemTier.Tier2:
                            itemPI = t2;
                            mult = TeleporterConfig.Tier2DropMultiplier;
                            break;

                        case ItemTier.Tier3:
                            itemPI = t3;
                            mult = TeleporterConfig.Tier3DropMultiplier;
                            break;

                        case ItemTier.Boss:
                            itemPI = tboss;
                            mult = TeleporterConfig.TierBossDropMultiplier;
                            break;

                        default:
                            itemPI = t2;
                            mult = TeleporterConfig.Tier2DropMultiplier;
                            break;
                    }


                    for (int i = 0; i < baseDropCount * mult; i++)
                        itemDrops.Push(itemPI);
                }
                else
                {
#if DEBUG
                    Logger.LogWarning($"TELEPDROP::BossGroup...()| Not-using DropUniformity.");
#endif
                    for (int i=0; i<baseDropCount; i++)
                    {
                        PickupIndex itemPI;
                        int mult = 0;
                        switch (GetWeightedChance(WeightedTiers))
                        {
                            case ItemTier.Tier1:
                                itemPI = t1;
                                mult = TeleporterConfig.Tier1DropMultiplier;
                                break;

                            case ItemTier.Tier2:
                                itemPI = t2;
                                mult = TeleporterConfig.Tier2DropMultiplier;
                                break;

                            case ItemTier.Tier3:
                                itemPI = t3;
                                mult = TeleporterConfig.Tier3DropMultiplier;
                                break;

                            case ItemTier.Boss:
                                itemPI = tboss;
                                mult = TeleporterConfig.TierBossDropMultiplier;
                                break;

                            default:
                                itemPI = t2;
                                mult = TeleporterConfig.Tier2DropMultiplier;
                                break;
                        }
                        for (int j = 0; j < mult; j++)
                            itemDrops.Push(itemPI);
                    }
                }

                float angle = 360f / (float)itemDrops.Count;
                Vector3 vector = Quaternion.AngleAxis((float)UnityEngine.Random.Range(0, 360), Vector3.up) * (Vector3.up * 40f + Vector3.forward * 5f);
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);

                while(itemDrops.Count > 0)
                {
#if DEBUG
                    Logger.LogWarning($"TELEPDROP::BossGroup...()| Item to spawn: {itemDrops.Peek().pickupDef.nameToken}");
#endif
                    PickupDropletController.CreatePickupDroplet(itemDrops.Pop(), self.dropPosition.position, vector);
                    vector = rotation * vector;
                }
            }
        }


        protected void InitTierWeights()
        {
            float totalWeight = 0f;
            List<WeightedTier> tiers = new List<WeightedTier>();
            if (TeleporterConfig.Tier1Drops)
            {
                tiers.Add(
                    new WeightedTier()
                    {
                        weight = TeleporterConfig.Tier1DropWeightedChance,
                        tier = ItemTier.Tier1,
                        multiplier = TeleporterConfig.Tier2DropMultiplier
                    });
                totalWeight += TeleporterConfig.Tier1DropWeightedChance;
#if DEBUG
                Logger.LogWarning($"TELEPDROP::InitTierWeights() | Tier 1 Weight: {tiers[tiers.Count-1].weight}. TotalWeight-Post: {totalWeight}");
#endif
            }
            if (TeleporterConfig.Tier2Drops)
            {
                tiers.Add(
                    new WeightedTier()
                    {
                        weight = TeleporterConfig.Tier2DropWeightedChance + totalWeight,
                        tier = ItemTier.Tier2,
                        multiplier = TeleporterConfig.Tier2DropMultiplier
                    });
                totalWeight += TeleporterConfig.Tier2DropWeightedChance;

#if DEBUG
                Logger.LogWarning($"TELEPDROP::InitTierWeights() | Tier 2 Weight: {tiers[tiers.Count - 1].weight}. TotalWeight-Post: {totalWeight}");
#endif
            }
            if (TeleporterConfig.Tier3Drops)
            {
                tiers.Add(
                    new WeightedTier()
                    {
                        weight = TeleporterConfig.Tier3DropWeightedChance + totalWeight,
                        tier = ItemTier.Tier3,
                        multiplier = TeleporterConfig.Tier3DropMultiplier
                    });
                totalWeight += TeleporterConfig.Tier3DropWeightedChance;

#if DEBUG
                Logger.LogWarning($"TELEPDROP::InitTierWeights() | Tier 3 Weight: {tiers[tiers.Count - 1].weight}. TotalWeight-Post: {totalWeight}");
#endif
            }
            if (TeleporterConfig.TierBossDrops)
            {
                tiers.Add(
                    new WeightedTier()
                    {
                        weight = TeleporterConfig.TierBossDropWeightedChance + totalWeight,
                        tier = ItemTier.Boss,
                        multiplier = TeleporterConfig.TierBossDropMultiplier
                    });
                totalWeight += TeleporterConfig.TierBossDropWeightedChance;

#if DEBUG
                Logger.LogWarning($"TELEPDROP::InitTierWeights() | Tier Boss Weight: {tiers[tiers.Count - 1].weight}. TotalWeight-Post: {totalWeight}");
#endif
            }

            WeightedTier[] arr = tiers.OrderBy(t => t.weight).ToArray();

            for (int i=0; i<arr.Length; i++)
            {
                arr[i].weight /= totalWeight;
#if DEBUG
                Logger.LogWarning($"TELEPDROP::InitTierWeights() | Adjusted Tier Weights: Tier: {arr[i].tier} Weight: {arr[i].weight}");
#endif
            }

            if (arr.Length > 0)    //Else use defaults
                WeightedTiers = arr;
            else
                WeightedTiers = new WeightedTier[]
                {
                    //Defaults
                    new WeightedTier()
                    {
                        weight = 1f,
                        tier = ItemTier.Tier2,
                        multiplier = 1
                    }
                };

#if DEBUG
            Logger.LogWarning($"TELEPDROP::InitTierWeights()|arr.arrlen: {arr.Length}");
            Logger.LogWarning($"TELEPDROP::InitTierWeights()|wtiers.arrlen: {WeightedTiers.Length}");
#endif
        }

        protected ItemTier GetWeightedChance(params WeightedTier[] options)
        {
            float r = UnityEngine.Random.Range(0f, 1f);

#if DEBUG
            Logger.LogWarning($"TELEPDROP::GetWeightedChance() | RandomGen: {r}");
            Logger.LogWarning($"TELEPDROP::GetWeightedChance() | options.arrlen: {options.Length}");
#endif

            for (int i = 0; i<options.Length; i++)
            {
#if DEBUG
                Logger.LogWarning($"TELEPDROP::GetWeightedChance() | OptionData: tier: {options[i].tier} weight: {options[i].weight}");
#endif

                if (options[i].weight >= r)
                {
#if DEBUG
                    Logger.LogWarning($"TELEPDROP::GetWeightedChance() | SelectedTier: {options[i].tier}");
#endif
                    return options[i].tier;
                }
            }

            return ItemTier.Tier2;  //Default on failure
        }

        protected struct WeightedTier
        {
            public float weight;
            public RoR2.ItemTier tier;
            public int multiplier;
        }

        protected static WeightedTier[] WeightedTiers = new WeightedTier[]
        {
            //Defaults
            new WeightedTier()
            {
                weight = 1f,
                tier = ItemTier.Tier1,
                multiplier = 1
            }
        };
    }
}
