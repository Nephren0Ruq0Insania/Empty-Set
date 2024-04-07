﻿using EmptySet.Utils;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EmptySet.Items.Weapons.Magic;

/// <summary>
/// 飞叶
/// </summary>
public class FlightLeaves : ModItem
{
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("FlightLeaves");

        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useTime = UseSpeedLevel.FastSpeed;
        Item.useAnimation = UseSpeedLevel.FastSpeed; // 武器使用动画的时间跨度，建议与useTime设置相同。
        Item.autoReuse = true; // 自动挥舞
        Item.DamageType = DamageClass.Magic; // 伤害类型
        Item.noMelee = true;
        Item.damage = 7;
        Item.mana = 6;
        Item.knockBack = KnockBackLevel.None;
        Item.crit = 4;
        Item.value = Item.sellPrice(0, 0, 0, 50);
        Item.rare = ItemRarityID.Blue;
        Item.UseSound = SoundID.Item8;

        Item.shoot = ProjectileID.Leaf;
        Item.shootSpeed = 11;
    }

    public override void AddRecipes() => CreateRecipe()
        .AddIngredient(ItemID.Daybloom, 3)
        .AddIngredient(ItemID.Gel, 20)
        .AddIngredient(ItemID.FallenStar)
        .AddTile(TileID.WorkBenches)
        .Register();
}