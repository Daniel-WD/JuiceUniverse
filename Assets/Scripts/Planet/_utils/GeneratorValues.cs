using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class GeneratorValues {

    public static readonly float BOTTLE_FILLER_COST_INCREMENT = 1.16f;
    public static readonly float BOTTLE_FILLER_RESULT_INCREMENT = 1.096f;

    public static readonly int[] upgradeStages = {
        1, 10, 25, 50, 100
    };

    private static readonly int[] bottleFillerBoostRewards = {
        10, 25, 50, 100, 200
    };

    private static readonly int[] moneyCollectorBoostRewards = {
        20, 50, 100, 200, 400, 600, 800
    };

    private static readonly int[] bottleDistributerBoostRewards = {
        20, 50, 100, 200, 400, 600, 800
    };

    private static readonly int[] bottleFillerBoostLevels = {
        10, 25, 50, 100, 200
    };

    private static readonly int[] moneyCollectorBoostLevels = {
        20, 50, 100, 200, 400, 600, 800
    };

    private static readonly int[] bottleDistributerBoostLevels = {
        20, 50, 100, 200, 400, 600, 800
    };

    private static readonly int[] costIncrement = { 100, 30, 20 };

    private static readonly int[] incrementLevels = { 42, 26 };

    public static readonly BigDecimal MONEY_COLLECTOR_INIT_COST = 300;
    public static readonly BigDecimal BOTTLE_DISTRIBUTER_INIT_COST = 300;
    public static readonly BigDecimal BOTTLE_FILLER_FIRST_INIT_COST = 10;

    public static readonly BigDecimal MONEY_COLLECTOR_INIT_RATE = 20;
    public static readonly BigDecimal BOTTLE_DISTRIBUTER_INIT_RATE = 20;
    public static readonly BigDecimal BOTTLE_FILLER_INIT_RATE = 3;

    public delegate BigDecimal CostDelegate(int level, int x);
    public delegate BigDecimal BulkCostDelegate(int level, int count, int x);


    public static BigDecimal moneyCollectorCost(int level, int ignoreMe = 0) {
        return MONEY_COLLECTOR_INIT_COST * BigDecimal.Pow(1.2f, level);
    }

    public static BigDecimal bottleDistributerCost(int level, int ignoreMe = 0) {
        return BOTTLE_DISTRIBUTER_INIT_COST * BigDecimal.Pow(1.2f, level);
    }

    public static BigDecimal bottleFillerCost(int level, int dex) {
        return GeneratorValues.calcProducerCost(dex) * BigDecimal.Pow(1.16f, level);
    }


    public static BigDecimal moneyCollectorBulkCost(int level, int count, int ignoreMe = 0) {
        if(count <= 1) return moneyCollectorCost(level);
        float growthRate = 1.2f;
        return MONEY_COLLECTOR_INIT_COST * (BigDecimal.Pow(growthRate, level) * (BigDecimal.Pow(growthRate, count) - 1)) / (growthRate - 1);
    }

    public static BigDecimal bottleDistributerBulkCost(int level, int count, int ignoreMe = 0) {
        if(count <= 1) return bottleDistributerCost(level);
        float growthRate = 1.2f;
        return BOTTLE_DISTRIBUTER_INIT_COST * (BigDecimal.Pow(growthRate, level) * (BigDecimal.Pow(growthRate, count) - 1)) / (growthRate - 1);
    }

    public static BigDecimal bottleFillerBulkCost(int level, int count, int dex) {
        if(count <= 1) return bottleFillerCost(level, dex);
        float growthRate = 1.16f;
        return GeneratorValues.calcProducerCost(dex) * (BigDecimal.Pow(growthRate, level) * (BigDecimal.Pow(growthRate, count) - 1)) / (growthRate - 1);
    }
    
    
    public static void moneyCollectorStagesCost(int level, ref BigDecimal[] output) {
        for(int i = 0; i < output.Length; i++) {
            output[i] = moneyCollectorBulkCost(level, upgradeStages[i]);
        }
    }
    
    public static void bottleDistributerStagesCost(int level, ref BigDecimal[] output) {
        for(int i = 0; i < output.Length; i++) {
            output[i] = bottleDistributerBulkCost(level, upgradeStages[i]);
        }
    }
    
    public static void bottleFillerStagesCost(int level, int dex, ref BigDecimal[] output) {
        for(int i = 0; i < output.Length; i++) {
            output[i] = bottleFillerBulkCost(level, upgradeStages[i], dex);
        }
    }


    public static BigDecimal moneyCollectorRate(int level, int ignoreMe = 0) {
        return MONEY_COLLECTOR_INIT_RATE * BigDecimal.Pow(1.4f, level - 1);
    }

    public static BigDecimal bottleDistributerRate(int level, int ignoreMe = 0) {
        return BOTTLE_DISTRIBUTER_INIT_RATE * BigDecimal.Pow(1.4f, level - 1);
    }

    public static BigDecimal bottleFillerRate(int level, int dex) {
        return BOTTLE_FILLER_INIT_RATE * BigDecimal.Pow(1.096, level + firstResultJumpedLevels(dex) - 1);
    }


    public static int moneyCollectorUpgradeCountForCurrentPrimMoney(int level, int maxCount) {
        return upgradeCountForCurrentPrimMoney(level, maxCount, moneyCollectorCost);
    }

    public static int bottleDistributerUpgradeCountForCurrentPrimMoney(int level, int maxCount) {
        return upgradeCountForCurrentPrimMoney(level, maxCount, bottleDistributerCost);
    }

    public static int bottleFillerUpgradeCountForCurrentPrimMoney(int level, int maxCount, int dex) {
        return upgradeCountForCurrentPrimMoney(level, maxCount, bottleFillerCost, dex);
    }

    private static int upgradeCountForCurrentPrimMoney(int level, int maxCount, CostDelegate costDelegate, int dex = 0) {
        int count = 0;
        BigDecimal money = Database.inst.getPrimaryMoney();
        BigDecimal cost;
        while (money - (cost = costDelegate(level + count, dex)) >= 0) {
            money -= cost;
            count++;
            if (count >= maxCount) break;
        }
        return count;
    }


    public static BigDecimal calcProducerCost(int index) {
        if (index <= 0) {
            return BOTTLE_FILLER_FIRST_INIT_COST;
        } else {
            int increment = costIncrement.Length >= index ? costIncrement[index - 1] : costIncrement[costIncrement.Length - 1];
            return calcProducerCost(index - 1) * increment;
        }
    }

    // public static BigDecimal calcFirstResult(int index) {
    //     if (index <= 0) {
    //         return firstResult;
    //     } else {
    //         int incrementLevel = incrementLevels.Length >= index ? incrementLevels[index - 1] : incrementLevels[incrementLevels.Length - 1];
    //         return calcFirstResult(index - 1) * BigDecimal.Pow(BOTTLE_FILLER_RESULT_INCREMENT, incrementLevel);
    //     }
    // }

    public static int firstResultJumpedLevels(int index) {
        if (index <= 0) {
            return 0;
        } else {
            return firstResultJumpedLevels(index - 1) + (incrementLevels.Length >= index ? incrementLevels[index - 1] : incrementLevels[incrementLevels.Length - 1]);
        }
    }

    public static int bottleDistributerBoostCount(int lvl) {
        return lowerOrEqualsCount(lvl, bottleDistributerBoostLevels);
    }

    public static int moneyCollectorBoostCount(int lvl) {
        return lowerOrEqualsCount(lvl, moneyCollectorBoostLevels);
    }

    public static int bottleFillerBoostCount(int lvl) {
        return lowerOrEqualsCount(lvl, bottleFillerBoostLevels);
    }

    private static int lowerOrEqualsCount(int lvl, int[] boostLevels) {
        int count = 0;
        foreach (var boostLvl in boostLevels) {
            if (boostLvl <= lvl) count++;
            else break;
        }
        return count;
    }

    public static int bottleDistributerNextBoostLvl(int lvl) {
        return nextBoostLevel(lvl, bottleDistributerBoostLevels);
    }

    public static int moneyCollectorNextBoostLvl(int lvl) {
        return nextBoostLevel(lvl, moneyCollectorBoostLevels);
    }

    public static int bottleFillerNextBoostLvl(int lvl) {
        return nextBoostLevel(lvl, bottleFillerBoostLevels);
    }

    private static int nextBoostLevel(int lvl, int[] boostLevels) {
        int next = 0;
        for (int i = 0; i < boostLevels.Length; i++) {
            if (boostLevels[i] > lvl) {
                if (i < boostLevels.Length) {
                    return boostLevels[i];
                } else {
                    return -1;
                }
            }
        }
        return next;
    }

    public static int bottleDistributerNextBoostReward(int lvl) {
        int boostCount = bottleDistributerBoostCount(lvl);
        if (boostCount >= bottleDistributerBoostRewards.Length) {
            return -1;
        } else {
            return bottleDistributerBoostRewards[boostCount];
        }
    }

    public static int moneyCollectorNextBoostReward(int lvl) {
        int boostCount = moneyCollectorBoostCount(lvl);
        if (boostCount >= moneyCollectorBoostRewards.Length) {
            return -1;
        } else {
            return moneyCollectorBoostRewards[boostCount];
        }
    }

    public static int bottleFillerNextBoostReward(int lvl) {
        int boostCount = bottleFillerBoostCount(lvl);
        if (boostCount >= bottleFillerBoostRewards.Length) {
            return -1;
        } else {
            return bottleFillerBoostRewards[boostCount];
        }
    }

    public static int bottleDistributerLastBoostLvl(int lvl) {
        return lastBoostLevel(lvl, bottleDistributerBoostLevels);
    }

    public static int moneyCollectorLastBoostLvl(int lvl) {
        return lastBoostLevel(lvl, moneyCollectorBoostLevels);
    }

    public static int bottleFillerLastBoostLvl(int lvl) {
        return lastBoostLevel(lvl, bottleFillerBoostLevels);
    }

    private static int lastBoostLevel(int lvl, int[] boostLevels) {
        int result = 0;
        foreach (var boostLvl in boostLevels) {
            if (boostLvl <= lvl) result = boostLvl;
        }
        return result;
    }

    public static int plusCountForPossibleUpgrades(int pUpgrades) {
        int res = lowerOrEqualsCount(pUpgrades, upgradeStages);
        return res;
    }

}