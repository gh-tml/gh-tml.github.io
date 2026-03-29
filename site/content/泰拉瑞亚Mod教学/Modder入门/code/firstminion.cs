using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace ModDocProject.ModsSource.Modder入门;

/// <summary>
/// 召唤物 Buff - 控制召唤物的存在状态显示
/// 当玩家拥有这个 Buff 时，对应的召唤物会显示存在
/// Buff 的时间由召唤物自身逻辑管理
/// </summary>
public class FirstMinionBuff : ModBuff
{
    /// <summary>
    /// SetStaticDefaults 设置 Buff 的静态属性
    /// 这些属性对所有此类 Buff 实例生效
    /// </summary>
    public override void SetStaticDefaults()
    {
        // Main.buffNoSave 表示这个 Buff 不会在玩家退出世界时保存
        // 召唤物 Buff 通常需要这个，因为：
        // 1. 召唤物本身也不应该跨世界存在
        // 2. 避免重新进入世界时出现孤立 Buff
        Main.buffNoSave[Type] = true;

        // Main.buffNoTimeDisplay 表示不在 Buff 栏显示剩余时间
        // 召唤物 Buff 通常不需要显示倒计时
        // 因为召唤物Buff的时间由 Update 方法动态刷新
        Main.buffNoTimeDisplay[Type] = true;

        // 其他可选设置：
        // Main.debuff[Type] = true;     // 设为 debuff（负面影响）样式
        // BuffID.Sets.IsAnNPCSettingBuff[Type] = true;  // NPC 专属 Buff 样式
    }

    /// <summary>
    /// Update 在 Buff 每帧更新时调用
    /// 我们用它来检测召唤物是否存在，并控制 Buff 的显示
    /// </summary>
    /// <param name="player">拥有此 Buff 的玩家</param>
    /// <param name="buffIndex">此 Buff 在玩家 buffTime 数组中的索引</param>
    public override void Update(Player player, ref int buffIndex)
    {
        // player.ownedProjectileCounts 计算玩家拥有的特定类型投射物数量
        // 这是一个字典，键是投射物类型，值是数量
        // 如果数量 > 0，说明玩家的召唤物存在
        if (player.ownedProjectileCounts[ModContent.ProjectileType<FirstMinion>()] > 0)
        {
            // 重置 Buff 的剩余时间为最大值
            // 这样 Buff 就不会因为时间耗尽而消失
            // 18000 帧 ≈ 5 分钟，是游戏中的最大缓冲时间
            player.buffTime[buffIndex] = 18000;
        }
        else
        {
            // 召唤物不存在了，移除这个 Buff
            player.DelBuff(buffIndex);

            // 重要：递减 buffIndex
            // DelBuff 不会自动调整数组索引
            // 如果不减，下一个 Buff 的数据会被覆盖
            buffIndex--;
        }
    }
}

/// <summary>
/// 召唤物品 - 玩家使用后召唤出 Minion 的物品
/// 这个物品定义了什么情况下可以召唤、消耗多少资源
/// </summary>
public class FirstMinionItem : ModItem
{
    /// <summary>
    /// SetStaticDefaults 设置物品的静态属性
    /// 这些属性在游戏加载时设置，不会改变
    /// </summary>
    public override void SetStaticDefaults()
    {
        // ItemID.Sets.GamepadWholeScreenUseRange 允许玩家使用手柄时
        // 在屏幕任意位置召唤投射物
        // 默认只允许在玩家附近一定范围内
        ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;

        // ItemID.Sets.LockOnIgnoresCollision 启用"锁定"瞄准模式
        // 玩家可以用鼠标锁定屏幕外的敌人
        // 召唤物会无视障碍物，穿墙攻击锁定的目标
        ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;

        // ItemID.Sets.StaffMinionSlotsRequired 指定此召唤物占用的栏位数量
        // 玩家有总召唤栏位上限（默认 1），多个召唤物按此值分配
        // 设置为 1f 表示占用 1 格，0.5f 表示占用半格
        ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f;

        // 可选：设置图标在玩家选择界面的偏移
        // ItemID.Sets.ItemIconPulse[Item.type] = true;
    }

    /// <summary>
    /// SetDefaults 设置物品的基础属性
    /// 这是物品最核心的属性定义
    /// </summary>
    public override void SetDefaults()
    {
        // 基础物品属性
        Item.damage = 20;      // 物品/召唤物造成的伤害
        Item.knockBack = 2f;  // 击退力度
        Item.mana = 10;       // 使用此物品消耗的魔力（必须设置！）
        Item.width = 32;      // 物品图标宽度
        Item.height = 32;     // 物品图标高度
        Item.useTime = 30;    // 使用一次所需帧数（60帧 = 1秒）
        Item.useAnimation = 30; // 动画播放帧数
        Item.useStyle = ItemUseStyleID.Swing; // 使用动画样式

        // 物品价值（用于商人定价）
        // sellPrice 返回 ItemSellBar 格式：sellPrice(gold: X, silver: Y, copper: Z)
        Item.value = Item.sellPrice(gold: 5);

        // 物品稀有度
        // 稀有度影响颜色：白色 < 蓝色 < 绿色 < 橙色 < 红色 < 粉色 < 紫色
        Item.rare = ItemRarityID.Blue;

        // 使用声音
        Item.UseSound = SoundID.Item44;

        // ========================================================================
        // 召唤物特有属性
        // ========================================================================

        // Item.noMelee = true 表示这不是近战武器
        // 效果：
        // 1. 装备时的近战伤害加成不会应用
        // 2. 装备时的近战速度加成不会应用
        // 3. 不会触发某些近战专属效果
        Item.noMelee = true;

        // Item.DamageType 设置伤害类型
        // DamageClass.Summon 声明这是召唤物伤害
        // 效果：
        // 1. 受到召唤物伤害加成装备的影响
        // 2. 受到召唤物祝福/十字项链等加成的影响
        // 3. 某些敌人对"非召唤伤害"的抗性不会生效
        Item.DamageType = DamageClass.Summon;

        // Item.buffType 关联召唤物 Buff 类型
        // 当玩家使用此物品时，会自动给予这个 Buff
        // Buff 又会触发召唤物投射物的生成
        // 注意：buffTime 如果不设置，默认是 60 秒（3600帧）
        Item.buffType = ModContent.BuffType<FirstMinionBuff>();

        // Item.shoot 设置使用物品时生成的投射物类型
        // 游戏会根据这个值创建实际的召唤物实体
        // 如果不想自动生成投射物，可以在 Shoot() 中返回 false
        Item.shoot = ModContent.ProjectileType<FirstMinion>();

        // 可选：不显示 buff 时间提示
        // Item.buffTime 不需要特别设置，召唤物由自身逻辑控制
    }

    /// <summary>
    /// ModifyShootStats 在投射物生成前调用
    /// 可以修改生成的位置、速度、类型等参数
    /// 常用于自定义召唤物的生成位置
    /// </summary>
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        // Main.MouseWorld 是鼠标指针在世界坐标系中的位置
        // 我们让召唤物生成在鼠标位置，而不是玩家位置
        position = Main.MouseWorld;

        // LimitPointToPlayerReachableArea 确保生成位置在玩家可达范围内
        // 防止玩家在过远的位置召唤
        player.LimitPointToPlayerReachableArea(ref position);

        // 可以修改其他属性（示例）：
        // knockback *= 2f;  // 加倍击退
        // damage += 5;      // 额外伤害
    }

    /// <summary>
    /// Shoot 在投射物即将生成时调用
    /// 返回 false 阻止生成，返回 true 允许生成
    /// 可用于实现特殊召唤逻辑
    /// </summary>
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        // player.AddBuff 给玩家添加 Buff
        // 第二个参数是 Buff 持续时间（帧）
        // 设置为 2 帧几乎瞬时，因为召唤物会在 AI 中持续刷新
        // 如果不添加 Buff，召唤物会出现后立即消失
        player.AddBuff(Item.buffType, 2);

        // 返回 true 表示允许投射物生成
        // 如果返回 false，投射物不会创建，但 Buff 已经添加
        return true;
    }
}

/// <summary>
/// 召唤物本体 - 实际的跟随/战斗实体
/// Minion 本质上是一个 ModProjectile
/// 它不是"武器"，而是玩家的"宠物/随从"
/// </summary>
public class FirstMinion : ModProjectile
{
    /// <summary>
    /// SetStaticDefaults 设置投射物的静态属性
    /// 这些属性对所有此类投射物实例生效
    /// </summary>
    public override void SetStaticDefaults()
    {
        // Main.projFrames 设置动画帧数
        // 需要和投射物贴图的垂直帧数匹配
        Main.projFrames[Projectile.type] = 4;

        // ProjectileID.Sets.MinionTargettingFeature 启用右键锁定目标功能
        // 启用后，玩家可以右键点击一个敌人
        // 召唤物会优先攻击锁定的目标
        ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

        // Main.projPet 表示这是一个宠物/召唤物投射物
        // 效果：
        // 1. 在玩家死亡时会保留（普通投射物会消失）
        // 2. 在某些 UI 中会显示为"宠物"
        Main.projPet[Projectile.type] = true;

        // ProjectileID.Sets.MinionSacrificable 允许玩家使用 Q 键移除召唤物
        // 提供一种快速清理召唤物的方式
        // 对于需要手动控制的召唤物很有用
        ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;

        // ProjectileID.Sets.CultistIsResistantTo 设置对此投射物的抗性
        // 邪教徒会对这个投射物的攻击有减伤
        ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;

        // 可选：设置投射物在屏幕外的检测范围
        // ProjectileID.Sets.Screenadhese[Projectile.type] = true;
    }

    /// <summary>
    /// SetDefaults 设置投射物的基础属性
    /// 这是召唤物最核心的配置
    /// </summary>
    public override void SetDefaults()
    {
        // 碰撞箱大小
        Projectile.width = 18;
        Projectile.height = 28;

        // Projectile.tileCollide 控制投射物是否与物块碰撞
        // 设置为 false：召唤物可以穿过所有物块
        // 设置为 true：召唤物会被墙挡住
        // 对于飞行召唤物，通常设为 false
        Projectile.tileCollide = false;

        // Projectile.friendly 控制投射物是否对敌人造成伤害
        // true = 友方投射物，会攻击敌人
        // false = 中立投射物，不会造成伤害
        Projectile.friendly = true;

        // Projectile.minion = true 声明这是一个召唤物
        // 这是最关键的属性，必须设置！
        // 效果：
        // 1. 受召唤物栏位系统管理
        // 2. 受召唤物相关 Buff 影响
        // 3. 受召唤物专属伤害加成影响
        Projectile.minion = true;

        // Projectile.DamageType 确认伤害类型
        // 必须和召唤物品的 DamageType 一致
        Projectile.DamageType = DamageClass.Summon;

        // Projectile.minionSlots 设置占用的召唤栏位数量
        // 默认 1f，设为 0.5f 可以让玩家召唤更多
        Projectile.minionSlots = 1f;

        // Projectile.penetrate 控制穿透次数
        // 1 = 穿透一个敌人后消失
        // -1 = 不限制穿透次数
        // 大于 1 = 穿透指定次数后消失
        Projectile.penetrate = -1;
    }

    /// <summary>
    /// CanCutTiles 控制召唤物是否破坏物块
    /// 返回值：
    /// true = 可以破坏（如镐子、炸药）
    /// false = 不能破坏（如大部分召唤物）
    /// null = 使用默认值
    /// </summary>
    public override bool? CanCutTiles()
    {
        // 召唤物默认不应该破坏物块
        return false;
    }

    /// <summary>
    /// MinionContactDamage 控制是否通过接触造成伤害
    /// 返回 true：召唤物碰到敌人就会造成 Projectile.damage 的伤害
    /// 返回 false：需要通过其他方式造成伤害（如发射投射物）
    /// </summary>
    public override bool MinionContactDamage()
    {
        // 我们使用接触伤害，简单直接
        return true;
    }

    /// <summary>
    /// AI() 是召唤物的核心行为逻辑
    /// 控制跟随主人、寻找目标、移动、攻击
    /// 每帧（约 60 帧/秒）调用一次
    /// </summary>
    public override void AI()
    {
        // 获取召唤物的主人（玩家）引用
        // Projectile.owner 是投射物所有者的玩家索引
        Player owner = Main.player[Projectile.owner];

        // ========================================================================
        // 检测主人状态
        // ========================================================================

        // owner.dead = true 表示玩家已死亡
        // owner.active = false 表示玩家已离线
        // 如果主人不在线，召唤物应该消失
        if (!owner.dead && owner.active)
        {
            // 重置存活时间
            // 召唤物的 timeLeft 会每帧递减
            // 设置为 2 可以让它持续存活
            Projectile.timeLeft = 2;
        }

        // ========================================================================
        // 简单跟随逻辑
        // ========================================================================

        // 将召唤物位置设为跟随主人
        // 可以在主人周围调整偏移量，实现环绕效果
        Projectile.Center = owner.Center;

        // 由于使用位置跟随，速度设为零
        Projectile.velocity = Vector2.Zero;

        // 设置朝向主人的方向
        Projectile.direction = owner.direction;

        // ========================================================================
        // 攻击判定
        // ========================================================================

        // Projectile.friendly 控制是否攻击敌人
        // 可以根据是否有有效目标来切换
        Projectile.friendly = true;

        // ========================================================================
        // 动画处理
        // ========================================================================

        // 旋转角度（让召唤物有轻微倾斜）
        Projectile.rotation = Projectile.velocity.X * 0.05f;

        // 帧动画计数器
        Projectile.frameCounter++;

        // 每 6 帧切换一帧动画
        if (Projectile.frameCounter >= 6)
        {
            Projectile.frameCounter = 0;

            // 循环播放帧（0 -> 1 -> 2 -> 3 -> 0 -> 1...）
            Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
        }

        // ========================================================================
        // 可选：添加光照效果
        // ========================================================================

        // Lighting.AddLight 在召唤物位置添加光照
        // 参数：位置、RGB 值（0-1 范围）
        // Color.White.ToVector3() 返回 (1, 1, 1)
        // 乘以系数控制光照强度
        // Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.5f);
    }
}
