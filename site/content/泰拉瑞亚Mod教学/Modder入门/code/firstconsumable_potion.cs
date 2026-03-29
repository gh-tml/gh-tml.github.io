using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModDocProject.ModsSource.Modder入门
{
    /// <summary>
    /// 第一个Buff药水
    /// 这个物品使用后会给予玩家一个Buff效果
    /// </summary>
    public class FirstPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            // ResearchUnlockCount：研究模式需要的解锁数量
            // 设置为 20 表示需要研究20个才能解锁批量复制
            Item.ResearchUnlockCount = 20;

            // DrinkParticleColors：喝药水时产生的粒子颜色
            // 数组中每个颜色代表一个粒子
            ItemID.Sets.DrinkParticleColors[Type] =
            [
                new Color(240, 240, 240),
                new Color(200, 200, 200),
                new Color(140, 140, 140)
            ];
        }

        public override void SetDefaults()
        {
            // ========== 基础属性 ==========

            Item.width = 20;
            Item.height = 26;

            // useStyle：使用方式
            // ItemUseStyleID.DrinkLiquid 表示像药水一样喝下去
            // 玩家会有喝药水的动画
            Item.useStyle = ItemUseStyleID.DrinkLiquid;

            // useAnimation / useTime：使用动画时间和实际使用时间
            // 这里都设置为15帧（0.25秒）
            Item.useAnimation = 15;
            Item.useTime = 15;

            // useTurn：使用时允许玩家转向
            Item.useTurn = true;

            // UseSound：使用时的声音
            Item.UseSound = SoundID.Item3;

            // ========== 消耗品属性 ==========

            // maxStack：最大堆叠数量
            // Item.CommonMaxStack = 9999，是大多数物品的默认值
            Item.maxStack = Item.CommonMaxStack;

            // consumable：是否可消耗
            // 设置为 true 后，使用物品会减少数量
            Item.consumable = true;

            // rare：稀有度，影响物品名称颜色
            Item.rare = ItemRarityID.Orange;

            // value：物品价值
            // buyPrice(gold: 1) = 1金
            // sellPrice 是 buyPrice 的 1/5
            Item.value = Item.buyPrice(gold: 1);

            // ========== 药水特殊属性 ==========

            // buffType：使用后给予的Buff类型
            // ModContent.BuffType<T>() 获取我们自定义Buff的类型
            Item.buffType = ModContent.BuffType<FirstDefenseBuff>();

            // buffTime：Buff持续时间（帧）
            // 5400帧 = 90秒 (5400 / 60 = 90)
            Item.buffTime = 5400;
        }
    }
}
