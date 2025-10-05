using Godot;
using System;
//遗物模版
public partial class coupons : Relic
{

	[Export(PropertyHint.Range, "1,100")]
	public int discount = 50;

	public RelicUi relic_ui;

	public override void InitializeRelic(RelicUi owner)
	{
		events.instance.ShopEnterd += AddShopModifier;
		relic_ui = owner;
	}


    public override void DeactivateRelic(RelicUi owner)
	{
		events.instance.ShopEnterd -=AddShopModifier;
	}

	public void AddShopModifier(Shop shop)
	{
		relic_ui.Flash();
		var shop_cost_modifier = shop.modifier_handler.GetModifier(Modifier.Type.SHOP_COST);


		var coupons_modifier_value = shop_cost_modifier.GetValue("coupons");
		if (coupons_modifier_value == null)
		{
			coupons_modifier_value = ModifierValue.CreateNewModifier("coupons", ModifierValue.Type.PERCENT_BASED);
			coupons_modifier_value.percent_value = (float)(-1 * discount / 100.0);
			shop_cost_modifier.AddNewValue(coupons_modifier_value);
		}
	}
	






}

