﻿using DiplomacyFixes.Costs;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace DiplomacyFixes.DiplomaticAction.WarPeace
{
    class KingdomPeaceAction
    {

        private static void AcceptPeace(Kingdom kingdomMakingPeace, Kingdom otherKingdom, DiplomacyCost diplomacyCost)
        {
            diplomacyCost.ApplyCost();
            MakePeaceAction.Apply(kingdomMakingPeace, otherKingdom);
        }

        private static string CreateMakePeaceInquiryText(Kingdom kingdomMakingPeace, Kingdom otherKingdom, int payment)
        {
            TextObject peaceText;
            if (Settings.Instance.EnableWarExhaustion && WarExhaustionManager.Instance.HasMaxWarExhaustion(kingdomMakingPeace, otherKingdom))
            {
                peaceText = new TextObject("{=HWiDa4R1}Exhausted from the war, the {KINGDOM} is proposing peace with the {PLAYER_KINGDOM}.");
                peaceText.SetTextVariable("KINGDOM", kingdomMakingPeace.Name.ToString());
                peaceText.SetTextVariable("PLAYER_KINGDOM", otherKingdom.Name.ToString());
            }
            else
            {
                peaceText = new TextObject("{=t0ZS9maD}{KINGDOM_LEADER} of the {KINGDOM} is proposing peace with the {PLAYER_KINGDOM}.");
                peaceText.SetTextVariable("KINGDOM_LEADER", kingdomMakingPeace.Leader.Name.ToString());
                peaceText.SetTextVariable("KINGDOM", kingdomMakingPeace.Name.ToString());
                peaceText.SetTextVariable("PLAYER_KINGDOM", otherKingdom.Name.ToString());
            }
            List<string> inquiryText = new List<string>();
            inquiryText.Add(peaceText.ToString());

            if (payment > 0)
            {
                TextObject warReparationText = new TextObject("{=ZrwszZww} They are willing to pay war reparations of {DENARS} denars.");
                warReparationText.SetTextVariable("DENARS", payment);
                inquiryText.Add(warReparationText.ToString());
            }
            return string.Concat(inquiryText);
        }

        private static void CreatePeaceInquiry(Kingdom kingdom, Kingdom faction, HybridCost diplomacyCost)
        {
            int payment = (int)diplomacyCost.GoldCost.Value;
            InformationManager.ShowInquiry(new InquiryData(new TextObject("{=BkGSVccZ}Peace Proposal").ToString(), CreateMakePeaceInquiryText(kingdom, faction, payment), true, true, new TextObject("{=3fTqLwkC}Accept").ToString(), new TextObject("{=dRoMejb0}Decline").ToString(), () =>
            {
                AcceptPeace(kingdom, faction, diplomacyCost);
            }, () =>
            {
                Events.Instance.OnPeaceProposalSent(kingdom);
            }, ""), true);
        }

        public static void ApplyPeace(Kingdom kingdomMakingPeace, Kingdom otherKingdom, bool forcePlayerCharacterCosts = false)
        {

            HybridCost diplomacyCost = DiplomacyCostCalculator.DetermineCostForMakingPeace(kingdomMakingPeace, otherKingdom, forcePlayerCharacterCosts);
            if (!otherKingdom.Leader.IsHumanPlayerCharacter)
            {
                AcceptPeace(kingdomMakingPeace, otherKingdom, diplomacyCost);
            }
            else if (!CooldownManager.HasPeaceProposalCooldownWithPlayerKingdom(kingdomMakingPeace))
            {
                CreatePeaceInquiry(kingdomMakingPeace, otherKingdom, diplomacyCost);
            }
        }
    }
}
