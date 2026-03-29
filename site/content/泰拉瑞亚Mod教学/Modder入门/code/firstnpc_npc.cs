using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModDocProject.ModsSource.Modder入门;

/// <summary>
/// 这是我们创建的第一个NPC类
/// 所有自定义NPC都需要继承 ModNPC
/// ModNPC 是 tModLoader 提供的基类，封装了大部分 NPC 逻辑
/// </summary>
public class FirstNPC : ModNPC
{
    /// <summary>
    /// SetStaticDefaults 用于设置静态默认值
    /// 这些设置在游戏加载时只执行一次，适用于整个 NPC 类型
    /// 注意：此方法在 ModNPC 实例化前调用，因此不能访问实例字段
    /// </summary>
    public override void SetStaticDefaults()
    {
        // Main.npcFrameCount 设置 NPC 的动画帧数
        // 这个数字需要和你的 NPC 贴图的垂直帧数匹配
        // 例如：贴图有 5 行动画，就设置为 5
        Main.npcFrameCount[Type] = 5;

        // NPCID.Sets.SpecificDebuffImmunity 设置 NPC 对特定 Debuff 的免疫
        // 第一个参数是当前 NPC 类型，第二个是 Debuff ID
        // 格式：[NPC类型][Debuff类型] = true 表示免疫
        // 这样设置后，这个 NPC 不会中毒
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;

        // 其他常用的免疫设置示例：
        // NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;     // 免疫着火
        // NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;  // 免疫混乱
        // NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Cursed] = true;    // 免疫诅咒
    }

    /// <summary>
    /// SetDefaults 是 NPC 的核心属性设置
    /// 每个 NPC 实例生成时都会调用这个方法
    /// 这是设置 NPC 基本属性的主要地方
    /// </summary>
    public override void SetDefaults()
    {
        // NPC.width 和 NPC.height 定义 NPC 的碰撞箱尺寸（像素）
        // 碰撞箱决定 NPC 可以被攻击的范围，应该与贴图大小匹配
        // 如果设置太小，玩家可能打不到；如果太大，可能会卡住
        NPC.width = 36;
        NPC.height = 36;

        // NPC.aiStyle 定义 NPC 使用的 AI 行为模式
        // -1 表示完全自定义 AI，需要自己实现 AI() 方法
        // 其他数值对应不同的预设 AI，详见 wiki 的 AI 样式表
        // 常见预设：
        //   0 = 站立，面向玩家
        //   1 = 跟随地面
        //   3 = 飞行
        //   7 = 城镇 NPC
        //   39 = 射弹攻击
        NPC.aiStyle = -1;

        // NPC.damage 是 NPC 的基础攻击力
        // 当 NPC 接触玩家或使用攻击时造成此数值的伤害
        NPC.damage = 12;

        // NPC.defense 是 NPC 的防御力
        // 实际受到的伤害 = 基础伤害 * (100 / (100 + 防御力))
        // 例如：100 伤害打 10 防御的 NPC，实际伤害 = 100 * (100/110) ≈ 91
        NPC.defense = 5;

        // NPC.lifeMax 是 NPC 的最大生命值
        // 这个值决定 NPC 能承受多少伤害才会死亡
        NPC.lifeMax = 50;

        // NPC.value 是 NPC 死亡时掉落的铜币数量
        // 游戏会自动根据这个数值掉落对应价值的硬币
        // 可以使用 Item.buyPrice(gold: X) 或 Item.sellPrice(gold: X) 转换
        NPC.value = 25f;

        // NPC.HitSound 是 NPC 受到攻击时播放的声音
        // SoundID 类包含所有预设声音的 ID
        NPC.HitSound = SoundID.NPCHit1;

        // NPC.DeathSound 是 NPC 死亡时播放的声音
        NPC.DeathSound = SoundID.NPCDeath1;

        // AIType 允许你复用现有 NPC 的 AI 行为
        // 设置后，当前 NPC 会使用目标 NPC 的 AI 逻辑
        // 这样可以省去自己写 AI 的麻烦
        // 注意：同时设置 AnimationType 可以复用动画
        AIType = NPCID.Zombie;
    }

    /// <summary>
    /// SpawnChance 决定这个 NPC 在什么情况下会生成
    /// 返回值是 0 到 1 之间的概率系数
    /// 返回 0 表示不会自然生成，只能通过代码生成
    /// </summary>
    /// <param name="spawnInfo">包含生成环境信息的结构体</param>
    /// <returns>生成概率，0.0 到 1.0 之间</returns>
    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        // SpawnCondition.OverworldDaySlime.Chance 是白天史莱姆的生成概率
        // 乘以 0.1f 表示只有 10% 的机会被替换成我们的 NPC
        // 建议控制在 0.01 到 0.2 之间，太高会导致 NPC 泛滥
        return SpawnCondition.OverworldDaySlime.Chance * 0.1f;

        // 其他常用的 SpawnCondition：
        // SpawnCondition.OverworldDaySlime.Chance      // 白天史莱姆
        // SpawnCondition.OverworldNightSlime.Chance    // 夜晚史莱姆
        // SpawnCondition.Corruption.Chance             // 腐化之地
        // SpawnCondition.Crimson.Chance                // 血腥之地
        // SpawnCondition.Hallow.Chance                 // 神圣之地
        // SpawnCondition.Underground.Chance            // 地下
        // SpawnCondition.Overworld.Chance              // 主世界（所有地形）
    }

    /// <summary>
    /// AI() 是 NPC 的行为逻辑核心
    /// 每帧（约 60 帧/秒）都会被调用
    /// 你可以在这里实现任何自定义行为
    /// </summary>
    public override void AI()
    {
        // NPC.TargetClosest() 让 NPC 面向并锁定最近的玩家作为目标
        // 参数 true 表示考虑墙壁等障碍物
        // 如果设为 false，NPC 可能锁定视野外的玩家
        NPC.TargetClosest(true);

        // NPC.HasValidTarget 检查 NPC 是否有一个有效的目标
        // 如果玩家死亡、离线或超出范围，目标会失效
        if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 500f)
        {
            // NPC.direction 是 -1 或 1，表示 NPC 面朝的方向
            // -1 = 左，1 = 右
            // 结合速度可以实现追向玩家
            NPC.velocity.X = NPC.direction * 1.5f;

            // NPC.velocity.Y 控制垂直速度
            // 注意：屏幕坐标系 Y 轴向下为正
            // 所以负数表示向上移动
            // 这里设置为 -3f 实现跳跃
            NPC.velocity.Y = -3f;
        }

        // NPC.rotation 控制 NPC 的旋转角度（弧度）
        // 通常用于让 NPC 随移动方向倾斜
        // 乘以一个小的系数实现轻微旋转效果
        NPC.rotation = NPC.velocity.X * 0.05f;
    }

    /// <summary>
    /// OnHitPlayer 在 NPC 成功攻击玩家时调用
    /// 注意：这个方法只在玩家实际受到伤害时触发
    /// </summary>
    /// <param name="player">被攻击的玩家</param>
    /// <param name="hurtInfo">包含伤害信息的结构体</param>
    public override void OnHitPlayer(Player player, Player.HitInfo hurtInfo)
    {
        // Player.AddBuff 给玩家添加一个持续性的 Debuff 效果
        // 第一个参数是 Debuff 的类型（BuffID）
        // 第二个参数是持续时间，单位是帧（60 帧 ≈ 1 秒）
        // 例如：180 帧 ≈ 3 秒
        player.AddBuff(BuffID.Poisoned, 180);

        // 常用的 Debuff 效果：
        // BuffID.Poisoned    // 中毒
        // BuffID.OnFire      // 着火
        // BuffID.Bleeding    // 流血
        // BuffID.Confused    // 混乱
        // BuffID.Slowed      // 缓慢
        // BuffID.Weak        // 虚弱
        // BuffID.Cursed      // 诅咒之火
    }

    /// <summary>
    /// HitEffect 在 NPC 受到伤害时调用
    /// 常用于创建击中特效、粒子效果
    /// </summary>
    public override void HitEffect(NPC.HitInfo hit)
    {
        // NPC.life 是当前生命值
        // 当生命值 <= 0 时表示 NPC 已死亡
        if (NPC.life <= 0)
        {
            // 在死亡位置生成灰尘粒子效果
            for (int i = 0; i < 6; i++)
            {
                // Dust.NewDustDirect 在指定位置创建灰尘
                // 参数：位置、碰撞箱宽度、碰撞箱高度、灰尘类型
                Dust dust = Dust.NewDustDirect(
                    NPC.position,
                    NPC.width,
                    NPC.height,
                    DustID.Blood,
                    // hit.HitDirection 是击中方向，用于粒子飞散
                    2 * hit.HitDirection,
                    -2f
                );

                // 随机设置灰尘大小
                dust.scale = Main.rand.NextFloat(0.7f, 1.2f);
            }
        }
    }
}
