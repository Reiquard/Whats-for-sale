using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WDYS
{
    [HarmonyPatch(typeof(Settlement), "GetGizmos")]
    public static class Settlement_Gizmo_Patch
    {
        public static void Postfix(ref IEnumerable<Gizmo> __result, Settlement __instance)
        {
            if (__instance.CanTradeNow && !__instance.Faction.HostileTo(Faction.OfPlayer))
            {
				if ((WDYS_Mod.settings.needTradeConsole && WDYS_Mod.settings.onlyIndustrialAndHigher && __instance.Faction.def.techLevel < TechLevel.Industrial) ||
					(WDYS_Mod.settings.needTradeConsole && !Find.Maps.FindAll(map => map.ParentFaction == Faction.OfPlayer && map.listerBuildings.AllBuildingsColonistOfDef(ThingDefOf.CommsConsole).ToList().FindAll(c => c.GetComp<CompPowerTrader>().PowerOn).Any()).Any()))
                {
					return;
                }

				List<Gizmo> gizmoList = __result.ToList();
				gizmoList.Add(new Command_Action
				{
					defaultLabel = "WDYS.CommandShowSettlementGoods".Translate(),
					defaultDesc = "WDYS.CommandShowSettlementGoodsDesc".Translate(),
					icon = Settlement.ShowSellableItemsCommand,
					action = delegate ()
					{
						Find.WindowStack.Add(new Dialog_ShowBuyable(__instance));
						RoyalTitleDef titleRequiredToTrade = __instance.TraderKind.TitleRequiredToTrade;
						if (titleRequiredToTrade != null)
						{
							TutorUtility.DoModalDialogIfNotKnown(ConceptDefOf.TradingRequiresPermit, new string[]
							{
								titleRequiredToTrade.GetLabelCapForBothGenders()
							});
						}
					}
				});
				__result = gizmoList;
			}
        }
    }
}
