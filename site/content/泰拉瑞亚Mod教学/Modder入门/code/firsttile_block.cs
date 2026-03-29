using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModDocProject.ModsSource.Modder入门
{
    /// <summary>
    /// 第一个物块
    /// 继承 ModTile 来创建一个可以放置的方块
    /// </summary>
    public class FirstBlock : ModTile
    {
        /// <summary>
        /// SetStaticDefaults：设置物块的静态属性
        /// 这个方法在物块加载时被调用一次
        /// </summary>
        public override void SetStaticDefaults()
        {
            // Main.tileSolid[Type] = true 表示这个物块是实心的
            // 玩家不能通过它，需要镐子挖掘
            Main.tileSolid[Type] = true;

            // Main.tileBlockLight = true 表示这个物块会阻挡光
            // 设置为 false 会让光通过（如玻璃）
            Main.tileBlockLight[Type] = true;

            // Main.tileMergeDirt[Type] = true 表示这个物块会和泥土合并
            // 矿石通常设置为 true，装饰物通常设置为 false
            Main.tileMergeDirt[Type] = true;

            // DustType：挖掘这个物块时产生的尘埃类型
            // ModContent.DustType<Sparkle>() 使用自定义尘埃
            // DustID.Platinum/Dirt 等是原版尘埃
            DustType = DustID.Platinum;

            // AddMapEntry：添加地图条目（游戏中显示的名称和颜色）
            // 第一个参数是颜色，第二个参数是本地化文本
            AddMapEntry(new Color(200, 200, 200));

            // VanillaFallbackOnModDeletion：
            // 当这个Mod被删除时，替代的原版物块
            VanillaFallbackOnModDeletion = TileID.DiamondGemspark;

            // HitSound：挖掘时播放的声音
            HitSound = SoundID.Tink;
        }

        /// <summary>
        /// NumDust：计算挖掘时产生的尘埃数量
        /// </summary>
        /// <param name="i">物块的X坐标</param>
        /// <param name="j">物块的Y坐标</param>
        /// <param name="fail">是否挖掘失败（镐子威力不足）</param>
        /// <param name="num">输出的尘埃数量</param>
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            // fail 为 true 时表示镐子威力不足，只产生少量尘埃
            num = fail ? 1 : 3;
        }
    }

    /// <summary>
    /// 对应的物品类
    /// 玩家用这个物品来放置物块
    /// </summary>
    public class FirstBlockItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;

            // item.createTile：放置后创建的物块类型
            Item.createTile = ModContent.TileType<FirstBlock>();

            // item.placeStyle：放置的样式（用于多帧物块如门）
            // Item.placeStyle = 0;

            // item.maxStack：最大堆叠数
            Item.maxStack = 999;

            // 稀有度和价值
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 10);
        }
    }
}
