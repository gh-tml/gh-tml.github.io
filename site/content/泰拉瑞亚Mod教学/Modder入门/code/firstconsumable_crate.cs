using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModDocProject.ModsSource.Modder入门
{
    /// <summary>
    /// 第一个钓鱼箱
    /// 钓鱼时有几率获得这个箱子
    /// 右键可以打开获得各种物品
    /// </summary>
    public class FirstCrate : ModItem
    {
        public override void SetStaticDefaults()
        {
            // ItemID.Sets.IsFishingCrate：标记为钓鱼箱
            // 这会让钓鱼时有机会获得这个物品
            ItemID.Sets.IsFishingCrate[Type] = true;

            // ItemID.Sets.IsFishingCrateHardmode：标记为困难模式钓鱼箱
            // 如果你的箱子在困难模式才出现，设置这个
            // ItemID.Sets.IsFishingCrateHardmode[Type] = true;

            // ResearchUnlockCount：研究模式需要的解锁数量
            Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults()
        {
            // DefaultToPlaceableTile：快速设置为可放置物品
            // 箱子放置后会变成一个可以右键打开的物块
            Item.DefaultToPlaceableTile(ModContent.TileType<FirstCrateTile>());

            // 设置尺寸（通常较小）
            Item.width = 12;
            Item.height = 12;

            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 2);
        }

        /// <summary>
        /// ModifyResearchSorting：修改研究排序
        /// 确保这个物品在研究菜单中显示在正确的分类
        /// </summary>
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            // Crates 分组会让它出现在"环境"分类
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Crates;
        }

        /// <summary>
        /// CanRightClick：是否能右键使用
        /// 返回 true 会让物品在右键时被消耗并调用 ModifyItemLoot
        /// </summary>
        public override bool CanRightClick()
        {
            return true;
        }

        /// <summary>
        /// ModifyItemLoot：修改物品掉落
        /// 当玩家右键打开箱子时，会根据这个方法的规则掉落物品
        /// </summary>
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // IItemDropRule：物品掉落规则的接口
            // ItemDropRule.Common：常见掉落规则

            // ========== 添加一个主题物品 ==========
            // OneFromOptionsNotScalingWithLuck：从指定选项中随机获得一个
            // 第一个参数是几率的倒数（1 = 100%）
            itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(1,
                [ModContent.ItemType<ExampleItem>()]));

            // ========== 添加金币 ==========
            // 4/5 的概率获得1-13个金 coins
            itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 4, 5, 13));

            // ========== 添加矿石 ==========
            // 从多个选项中随机选择一个
            IItemDropRule[] oreTypes =
            [
                ItemDropRule.Common(ItemID.CopperOre, 1, 30, 50),
                ItemDropRule.Common(ItemID.IronOre, 1, 30, 50),
                ItemDropRule.Common(ItemID.SilverOre, 1, 30, 50),
                ItemDropRule.Common(ItemID.GoldOre, 1, 30, 50),
            ];

            // OneFromRules：每7个单位有1个单位掉落
            itemLoot.Add(new OneFromRulesRule(7, oreTypes));

            // ========== 添加药水 ==========
            IItemDropRule[] potions =
            [
                ItemDropRule.Common(ItemID.SpelunkerPotion, 1, 2, 5),
                ItemDropRule.Common(ItemID.HunterPotion, 1, 2, 5),
                ItemDropRule.Common(ItemID.MiningPotion, 1, 2, 5),
            ];
            itemLoot.Add(new OneFromRulesRule(4, potions));

            // ========== 添加燃料 ==========
            IItemDropRule[] bait =
            [
                ItemDropRule.Common(ItemID.JourneymanBait, 1, 2, 7),
                ItemDropRule.Common(ItemID.MasterBait, 1, 2, 7),
            ];
            itemLoot.Add(new OneFromRulesRule(2, bait));
        }
    }

    /// <summary>
    /// 钓鱼箱对应的物块
    /// </summary>
    public class FirstCrateTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            // ... 其他物块设置
        }
    }
}
