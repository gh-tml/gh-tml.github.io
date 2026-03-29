using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModDocProject.ModsSource.Modder入门
{
    /// <summary>
    /// 第一个矿石物块
    /// 矿石有一些特殊属性，让它们在游戏中更容易被找到
    /// </summary>
    public class FirstOre : ModTile
    {
        public override void SetStaticDefaults()
        {
            // ========== 矿石特殊属性 ==========

            // TileID.Sets.Ore[Type] = true
            // 标记这个物块为矿石
            // 影响某些原版逻辑和探测装备
            TileID.Sets.Ore[Type] = true;

            // Main.tileSpelunker[Type] = true
            // 洞穴探险药水高亮显示这个矿石
            // 强烈建议设置为 true
            Main.tileSpelunker[Type] = true;

            // Main.tileOreFinderPriority[Type] = value
            // 矿石探测器（金属探测器）的优先级
            // 数值越高越容易被探测到
            // 参考值：铁=410, 金=500, 铂金=600, 钨=310, 钛金=700
            Main.tileOreFinderPriority[Type] = 410;

            // Main.tileShine2[Type] = true
            // 矿石会在光源下微微闪烁
            // 增加视觉吸引力
            Main.tileShine2[Type] = true;

            // Main.tileShine[Type] = value
            // 矿石微小尘埃出现的频率
            // 数值越大出现越频繁（看起来越亮）
            // 参考值：1-400 = 非常少，400-800 = 适中，800+ = 非常频繁
            Main.tileShine[Type] = 975;

            // ========== 基础物块属性 ==========

            // 矿石通常是可挖掘的实心物块
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMergeDirt[Type] = true;

            DustType = DustID.Platinum;
            AddMapEntry(new Color(152, 171, 198));

            // MineRespect = value
            // 挖掘这个物块需要的最低镐力
            // 如果设置，需要 >= 这个值的镐子才能挖掘
            // MinePick = 200;

            // 替代删除后的物块
            VanillaFallbackOnModDeletion = TileID.Silver;

            HitSound = SoundID.Tink;
        }

        /// <summary>
        /// IsTileBiomeSightable：
        /// 生物群落 sight 药水是否能看到这个矿石
        /// </summary>
        public override bool IsTileBiomeSightable(int i, int j, ref Color sightColor)
        {
            // sightColor：矿石在sight模式下的颜色
            sightColor = Color.Blue;

            // 返回 true 表示这个矿石可以被看到
            return true;
        }
    }

    /// <summary>
    /// 矿石对应的物品
    /// </summary>
    public class FirstOreItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;

            // 放置矿石物块
            Item.createTile = ModContent.TileType<FirstOre>();

            Item.maxStack = 999;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 30);
        }
    }
}
