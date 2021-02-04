using System;
using System.Collections.Generic;
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
        }

        protected override void OnEnable()
        {
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
                tboss = self.rng.NextElementUniform<PickupIndex>(self.bossDrops);

                if (TeleporterConfig.DropUniformity)    //only one item type
                {
                    PickupIndex itemPI;
                    int mult = 0;
                    switch (GetWeightedChance(this.WeightedTiers))
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

                    for (int i=0; i<baseDropCount; i++)
                    {
                        PickupIndex itemPI;
                        int mult = 0;
                        switch (GetWeightedChance(this.WeightedTiers))
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
            }

            WeightedTiers = tiers.ToArray();

            for(int i=0; i<tiers.Count; i++)
            {
                WeightedTiers[i].weight /= totalWeight;
            }
        }

        protected static ItemTier GetWeightedChance(params WeightedTier[] options)
        {
            float r = UnityEngine.Random.Range(0f, 1f);

            ItemTier tier;
            for (int i = 0; i<options.Length; i++)
            {
                if (options[i].weight <= r)
                {
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

        protected WeightedTier[] WeightedTiers = new WeightedTier[]
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
