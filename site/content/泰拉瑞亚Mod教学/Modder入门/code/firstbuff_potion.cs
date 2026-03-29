using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModDocProject.ModsSource.Modder入门
{
    /// <summary>
    /// 第一个Buff药水
    /// 使用这个物品后会给玩家添加一个Buff
    /// </summary>
    public class FirstBuffPotion : ModItem
    {
        /// <summary>
        /// SetStaticDefaults：设置物品的静态属性
        /// </summary>
        public override void SetStaticDefaults()
        {
            // ResearchUnlockCount：研究模式需要的解锁数量
            Item.ResearchUnlockCount = 20;

            // DrinkParticleColors：喝药水时产生的粒子颜色
            // 用于 ItemUseStyleID.DrinkLiquid 样式
            ItemID.Sets.DrinkParticleColors[Type] =
            [
                new Color(240, 240, 240),
                new Color(200, 200, 200),
                new Color(140, 140, 140)
            ];
        }

        /// <summary>
        /// SetDefaults：设置物品的默认属性
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 20;       // 物品在背包中的宽度
            Item.height = 26;     // 物品在背包中的高度

            // useStyle：使用方式
            // ItemUseStyleID.DrinkLiquid 表示像药水一样喝下去
            Item.useStyle = ItemUseStyleID.DrinkLiquid;

            Item.useAnimation = 15;  // 使用动画时间（帧）
            Item.useTime = 15;       // 实际使用时间（帧）

            Item.useTurn = true;     // 使用时允许转向

            // UseSound：使用时播放的声音
            Item.UseSound = SoundID.Item3;

            // maxStack：最大堆叠数量
            // Item.CommonMaxStack 是大多数物品的默认值 (9999)
            Item.maxStack = Item.CommonMaxStack;

            // consumable：是否可消耗
            // true 表示使用后会减少数量
            Item.consumable = true;

            // rare：稀有度
            // 影响物品名称的颜色
            Item.rare = ItemRarityID.Orange;

            // value：物品的价值
            // buyPrice(gold: 1) = 1金
            Item.value = Item.buyPrice(gold: 1);

            // buffType：使用后给予的Buff类型
            // 使用 ModContent.BuffType<T>() 来获取自定义Buff
            Item.buffType = ModContent.BuffType<FirstDefenseBuff>();

            // buffTime：Buff持续时间（帧）
            // 5400帧 = 90秒 (5400 / 60 = 90)
            // 如果不设置或设置为0，Buff将永久持续直到被移除
            Item.buffTime = 5400;
        }
    }
}
