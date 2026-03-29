using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ModDocProject.ModsSource.Modder入门
{
    /// <summary>
    /// 第一个家具：工作台
    /// 家具物块需要更多的设置来正确显示和工作
    /// </summary>
    public class FirstWorkbench : ModTile
    {
        public override void SetStaticDefaults()
        {
            // ========== 家具基础属性 ==========

            // Main.tileTable[Type] = true
            // 标记为桌子类家具
            // 影响是否能放置物品、是否影响NPC移动等
            Main.tileTable[Type] = true;

            // Main.tileSolidTop[Type] = true
            // 顶部是实心的，可以放置物品
            Main.tileSolidTop[Type] = true;

            // Main.tileNoAttach[Type] = true
            // 这个物块不能被粘性活塞等推动
            Main.tileNoAttach[Type] = true;

            // Main.tileLavaDeath[Type] = true
            // 岩浆接触会销毁这个物块
            Main.tileLavaDeath[Type] = true;

            // Main.tileFrameImportant[Type] = true
            // 物块有多个帧（如门需要开/关两帧）
            // 设置为 true 后物块不会随机变化
            Main.tileFrameImportant[Type] = true;

            // ========== 智能光标禁用 ==========

            // TileID.Sets.DisableSmartCursor[Type] = true
            // 禁用智能光标放置
            // 推荐用于复杂的家具
            TileID.Sets.DisableSmartCursor[Type] = true;

            // TileID.Sets.IgnoredByNpcStepUp[Type] = true
            // NPC不会尝试踏上这个物块
            // 推荐用于所有有顶部实心的家具
            TileID.Sets.IgnoredByNpcStepUp[Type] = true;

            // ========== 尘埃和音效 ==========

            DustType = DustID.Platinum;

            // AdjTiles：相邻时被视为工作站的物块类型数组
            // 影响哪些合成可以在这个物块旁边进行
            // 这里设置为工作台，所以玩家可以在它旁边合成
            AdjTiles = [TileID.WorkBenches];

            // ========== 物块尺寸和坐标 ==========

            // TileObjectData.newTile：创建新的物块对象数据
            // CopyFrom：复制一个已有的样式作为基础
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);

            // CoordinateHeights：每帧的高度（像素）
            // Style2x1 默认是 20，这里改为 18
            TileObjectData.newTile.CoordinateHeights = [18];

            // 将物块数据添加到游戏
            TileObjectData.addTile(Type);

            // ========== 房间需求 ==========

            // AddToArray：将这个物块添加到房间需要的家具列表
            // TileID.Sets.RoomNeeds.CountsAsTable 表示这个物块算作桌子
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

            // ========== 地图显示 ==========

            // Language.GetText("ItemName.WorkBench") 使用原版工作台的本地化名称
            // 你也可以使用 CreateMapEntryName() 创建自定义名称
            AddMapEntry(new Color(200, 200, 200), Language.GetText("ItemName.WorkBench"));
        }

        /// <summary>
        /// NumDust：计算挖掘时产生的尘埃数量
        /// </summary>
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

    /// <summary>
    /// 工作台对应的物品
    /// </summary>
    public class FirstWorkbenchItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 14;

            // DefaultToPlaceableTile：快速设置放置物品的默认属性
            Item.DefaultToPlaceableTile(ModContent.TileType<FirstWorkbench>());

            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 10);
        }
    }
}
