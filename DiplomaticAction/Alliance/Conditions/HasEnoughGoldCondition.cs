﻿using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace DiplomacyFixes.DiplomaticAction.Alliance.Conditions
{
    class HasEnoughGoldCondition : AbstractCostCondition
    {
        protected override TextObject FailedConditionText => new TextObject(StringConstants.NOT_ENOUGH_GOLD);

        protected override bool ApplyConditionInternal(Kingdom kingdom, Kingdom otherKingdom, ref TextObject textObject, bool forcePlayerCharacterCosts = false)
        {
            bool hasEnoughGold = DiplomacyCostCalculator.DetermineCostForFormingAlliance(kingdom, otherKingdom, forcePlayerCharacterCosts).GoldCost.CanPayCost();
            if (!hasEnoughGold)
            {
                textObject = FailedConditionText;
            }
            return hasEnoughGold;
        }
    }
}
