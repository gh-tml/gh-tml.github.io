using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModDocProject.ModsSource.Modder入门;

/// <summary>
/// 合成配方系统需要放在 ModSystem 的子类中
/// ModSystem 是游戏系统的基类，提供了多个可重写的生命周期方法
/// 配方相关的方法会在游戏加载时自动调用
/// </summary>
public class FirstRecipe : ModSystem
{
    /// <summary>
    /// AddRecipes 是添加合成配方的入口方法
    /// 游戏加载时会自动调用这个方法
    /// 所有自定义合成配方都应该在这里注册
    /// 
    /// 推荐使用链式调用风格，代码更简洁易读
    /// </summary>
    public override void AddRecipes()
    {
        // ========================================================================
        // 方式一：链式调用风格（现代推荐写法）
        // ========================================================================

        // Recipe.Create() 创建配方，参数是结果物品和数量
        // 结果物品可以是 ItemID（整数）或 ModContent.ItemType<T>()
        // 第二个参数可选，默认为 1
        Recipe.Create(ItemID.IronBar, 5)
            // AddIngredient 添加一个材料需求
            // 第一个参数是材料物品 ID
            // 第二个可选参数是需要的数量，默认 1
            .AddIngredient(ItemID.IronOre, 10)
            // AddTile 添加一个工作站需求（工作台、熔炉等）
            // 玩家必须在指定的工作站旁才能合成
            // 设为 TileID.Dirt2xx 表示不需要工作站
            .AddTile(TileID.Furnaces)
            // Register() 必须调用，表示完成配方注册
            // 没有调用 Register() 的配方不会生效
            .Register();

        // ========================================================================
        // 使用合成组（RecipeGroup）
        // ========================================================================

        // RecipeGroupID 包含所有原版合成组的 ID
        // 合成组允许多种物品作为同一个材料槽的输入
        // 例如：RecipeGroupID.Wood 包含所有木头变体
        Recipe.Create(ItemID.Wood, 20)
            // AddRecipeGroup 添加一个合成组需求
            // 系统会接受组内任意物品作为材料
            .AddRecipeGroup(RecipeGroupID.Wood)
            // 可以同时添加多个 AddTile
            // 玩家只需要满足其中一个工作站即可
            .AddTile(TileID.WorkBenches)
            .Register();

        // ========================================================================
        // 方式二：变量保存风格（适合复杂配方）
        // ========================================================================

        // 有些情况下需要保存配方引用以便后续操作
        Recipe goldBarRecipe = Recipe.Create(ItemID.GoldBar, 1);
        goldBarRecipe.AddIngredient(ItemID.GoldOre, 8);
        goldBarRecipe.AddTile(TileID.Furnaces);
        goldBarRecipe.Register();

        // ========================================================================
        // 使用 ModContent.ItemType 引用自制物品
        // ========================================================================

        // 如果要引用自己 mod 中的物品，使用 ModContent.ItemType<T>()
        // 泛型参数 T 是物品类的类型
        // 例如：ModContent.ItemType<MyCustomItem>()

        // 示例（伪代码）：
        // Recipe.Create(ModContent.ItemType<ExampleSword>())
        //     .AddIngredient(ModContent.ItemType<ExampleMaterial>(), 5)
        //     .AddTile(ModContent.TileType<ExampleAnvil>())
        //     .Register();
    }

    /// <summary>
    /// AddRecipeGroups 用于注册自定义合成组
    /// 合成组让你可以定义"任意 X 都能用"的材料需求
    /// 
    /// 使用场景：
    /// - 你想让银锭和金锭可以互相替代作为材料
    /// - 你想让所有稀有矿石都能作为某种材料
    /// - 整合其他 mod 的物品到你的配方中
    /// </summary>
    public override void AddRecipeGroups()
    {
        // ========================================================================
        // 创建自定义合成组
        // ========================================================================

        // new RecipeGroup() 构造函数：
        // 第一个参数：Func<string> 验证器回调，用于 UI 显示
        //   这个函数返回一个字符串，会在配方界面显示
        //   通常格式是："任意 X" 或 "X 或 Y"
        // 其余参数：params int[] 可变数量的物品 ID
        //   这些物品都会被算作同一组

        RecipeGroup SilverBarRecipeGroup = new RecipeGroup(
            () => $"{Lang.GetItemNameValue(ItemID.SilverBar)} 或 {Lang.GetItemNameValue(ItemID.TungstenBar)}",
            // 第一个物品会用作组名显示，非常重要
            ItemID.SilverBar,
            // 后续的物品都是可选的替代品
            ItemID.TungstenBar
        );

        // RegisterGroup() 注册合成组
        // 参数是组的唯一标识符，建议格式："ModName:GroupName"
        // 如果使用原版物品名称作为标识符，系统会自动合并
        RecipeGroup.RegisterGroup("SilverBarGroup", SilverBarRecipeGroup);

        // ========================================================================
        // 常用原版合成组 ID 参考
        // ========================================================================

        // RecipeGroupID.Wood         // 所有木头类型
        // RecipeGroupID.IronBar      // 所有铁锭/铅锭
        // RecipeGroupID.SilverBar    // 所有银锭/钨锭
        // RecipeGroupID.GoldBar      // 所有金锭/铂金锭
        // RecipeGroupID.Dirt         // 所有泥土类型
        // RecipeGroupID.EverythingCraftedByFurnaceGroup  // 所有可熔炼物品
        // RecipeGroupID.WallTypes    // 所有墙类型
        // RecipeGroupID.SoulGroup    // 所有魂（蓝魂、红魂等）

        // ========================================================================
        // 添加物品到现有合成组
        // ========================================================================

        // 如果你想让自己的物品加入某个原版合成组
        // RecipeGroup.recipeGroups 是所有合成组的字典
        // 使用组 ID 作为键访问，然后向 ValidItems 列表添加

        // 示例：将自定义矿石加入"任意铁锭"组（不建议，仅演示）
        // RecipeGroup.recipeGroups[RecipeGroupID.IronBar].ValidItems
        //     .Add(ModContent.ItemType<MyCustomOre>());
    }
}
