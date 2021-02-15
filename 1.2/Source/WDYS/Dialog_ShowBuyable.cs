using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace WDYS
{
    public class Dialog_ShowBuyable : Window
    {
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(1024f, (float)Mathf.Min(UI.screenHeight, 1000));
			}
		}

		private ITrader trader;

		private Vector2 scrollPosition = Vector2.zero;

		private string search = "";

		public Dialog_ShowBuyable(ITrader trader)
		{
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.trader = trader;
		}

		public override void DoWindowContents(Rect inRect)
        {
			float num = 0f;

			// Window title
			Rect titleRect = new Rect(0f, 0f, inRect.width, 60f);
			Text.Font = GameFont.Medium;
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(titleRect, "WDYS.WindowTitle".Translate(this.trader.TraderName));
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;
			num += 60f;

			// Search bar
			Text.Anchor = TextAnchor.MiddleLeft;
			Rect searchRect = new Rect(0f, 10f + num, 500f, 30f);
			search = Widgets.TextEntryLabeled(searchRect, "WDYS.SearchBar".Translate(), search);
			num += 40f;

			// Draw settlement money
			Rect settlementMoneyRect = new Rect(0f, 10f + num, inRect.width, 30f);
			this.DrawTradeableRow(settlementMoneyRect, new Tradeable(null, this.trader.Goods.ToList().Find(t => t.def == ThingDefOf.Silver)), 0);
			num += 40f;

			// Draw goods
			Rect mainRect = new Rect(0f, 10f + num, inRect.width, inRect.height - num - 100f);
			num += this.FillMainRect(mainRect, search);

			Rect rect4 = new Rect((mainRect.width / 2f) - 80f, mainRect.y + mainRect.height + 40f, 160f, 40f);
			if (Widgets.ButtonText(rect4, "WDYS.Quit".Translate(), true, true, true))
			{
				this.Close(true);
				Event.current.Use();
			}
		}

		private float FillMainRect(Rect mainRect, string search)
		{
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleLeft;
			float height = (float)(this.trader.Goods.Count() - 1) * 30f;
			Rect viewRect = new Rect(0f, 0f, mainRect.width - 16f, height);
			Widgets.BeginScrollView(mainRect, ref this.scrollPosition, viewRect, true);
			int index = 0;

			float num = 0f;
			float num2 = this.scrollPosition.y - 30f;
			float num3 = this.scrollPosition.y + mainRect.height;

			for (int i = 0; i < this.trader.Goods.Count(); i++)
			{
				if (this.trader.Goods.ElementAt(i).LabelCapNoCount.ToLower().Contains(search.ToLower()) && this.trader.Goods.ElementAt(i).def != ThingDefOf.Silver)
				{
					if (num > num2 && num < num3)
					{
						Rect rect = new Rect(0f, num, viewRect.width, 30f);
						this.DrawTradeableRow(rect, new Tradeable(null, this.trader.Goods.ElementAt(i)), index);
					}
					num += 30f;
					index++;
				}				
			}

			Widgets.EndScrollView();
			Text.Anchor = TextAnchor.UpperLeft;

			return num;
		}

		public void DrawTradeableRow(Rect rect, Tradeable trad, int index)
		{
			if (index % 2 == 1)
			{
				Widgets.DrawLightHighlight(rect);
			}
			Text.Font = GameFont.Small;
			GUI.BeginGroup(rect);
			float num = rect.width;
			int num2 = trad.CountHeldBy(Transactor.Trader);
			if (num2 != 0 && trad.IsThing)
			{
				Rect rect2 = new Rect(num - 75f, 0f, 75f, rect.height);
				if (Mouse.IsOver(rect2))
				{
					Widgets.DrawHighlight(rect2);
				}
				Text.Anchor = TextAnchor.MiddleRight;
				Rect rect3 = rect2;
				rect3.xMin += 5f;
				rect3.xMax -= 5f;
				Widgets.Label(rect3, num2.ToStringCached());
				TooltipHandler.TipRegionByKey(rect2, "TraderCount");
				Rect rect4 = new Rect(rect2.x - 100f, 0f, 100f, rect.height);
				Text.Anchor = TextAnchor.MiddleRight;
				this.DrawPrice(rect4, trad, TradeAction.PlayerBuys);
			}
			num -= 175f;
			num -= 240f;
			num -= 175f;
			TransferableUIUtility.DoExtraAnimalIcons(trad, rect, ref num);
			Rect idRect = new Rect(0f, 0f, num, rect.height);
			TransferableUIUtility.DrawTransferableInfo(trad, idRect, Color.white);
			GenUI.ResetLabelAlign();
			GUI.EndGroup();
		}

		private void DrawPrice(Rect rect, Tradeable trad, TradeAction action)
		{
			if (trad.thingsTrader.First().def == ThingDefOf.Silver)
			{
				return;
			}
			rect = rect.Rounded();
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
			}
			TooltipHandler.TipRegionByKey(rect, "WDYS.MarketValue");
			float priceFor = trad.thingsTrader.First().MarketValue;
			string label = priceFor.ToStringMoney(null);
			Rect rect2 = new Rect(rect);
			rect2.xMax -= 5f;
			rect2.xMin += 5f;
			if (Text.Anchor == TextAnchor.MiddleLeft)
			{
				rect2.xMax += 300f;
			}
			if (Text.Anchor == TextAnchor.MiddleRight)
			{
				rect2.xMin -= 300f;
			}
			Widgets.Label(rect2, label);
			GUI.color = Color.white;
		}
	}
}
