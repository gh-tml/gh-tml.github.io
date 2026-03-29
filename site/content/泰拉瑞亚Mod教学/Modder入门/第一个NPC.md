---
title: 第一个NPC-创建你的第一个敌怪
author: 小天使
topic: modder-basic
description: 学习如何创建NPC敌怪，包括基础属性、AI行为和攻击逻辑
order: 6
difficulty: beginner
time: 15分钟
next_chapter: Modder入门/合成表入门.md
prev_chapter: Modder入门/Buff与药水.md
source_cs:
  - ./code/firstnpc_npc.cs
colors:
  Mad: "#ff5430"
---

# 前言

当你深入泰拉瑞亚的世界，会遇到各种各样的敌人——史莱姆、僵尸、骷髅、克苏鲁之眼……

这些敌人统称为 NPC（Non-Player Character，非玩家角色）。

{color:Mad}{在这一章，你将学会如何创建属于自己的敌怪！}

NPC 是 Mod 开发中最复杂的部分之一，它涉及到：
- 基础属性的设置
- AI 行为逻辑
- 战斗系统的整合
- 掉落物的定义

# 创建基础NPC

## 继承 ModNPC

所有自定义 NPC 都需要继承 `ModNPC` 类：

[FirstNPC 类定义](cs:./code/firstnpc_npc.cs#cs:t:ModDocProject.ModsSource.Modder入门.FirstNPC)

`ModNPC` 是 tModLoader 提供的基类，封装了大部分 NPC 逻辑。

## SetDefaults - 基础属性

`SetDefaults` 是设置 NPC 核心属性的地方：

[SetDefaults 方法](cs:./code/firstnpc_npc.cs#cs:m:ModDocProject.ModsSource.Modder入门.FirstNPC.SetDefaults())

### 碰撞箱设置

```csharp
NPC.width = 36;
NPC.height = 36;
```

碰撞箱决定 NPC 可以被攻击的范围，应该与贴图大小匹配。

### 战斗属性

```csharp
NPC.damage = 12;      // 攻击伤害
NPC.defense = 5;      // 防御力
NPC.lifeMax = 50;     // 最大生命值
```

### 音效设置

```csharp
NPC.HitSound = SoundID.NPCHit1;    // 受伤音效
NPC.DeathSound = SoundID.NPCDeath1; // 死亡音效
```

### AI 复用技巧

[AI 方法](cs:./code/firstnpc_npc.cs#cs:m:ModDocProject.ModsSource.Modder入门.FirstNPC.AI())

```csharp
// 复用僵尸的 AI 行为
AIType = NPCID.Zombie;
```

使用 `AIType` 可以直接复用现有 NPC 的 AI，不用自己写复杂逻辑。

## SetStaticDefaults - 静态设置

`SetStaticDefaults` 在游戏加载时执行一次：

[SetStaticDefaults 方法](cs:./code/firstnpc_npc.cs#cs:m:ModDocProject.ModsSource.Modder入门.FirstNPC.SetStaticDefaults())

### 动画帧数

```csharp
Main.npcFrameCount[Type] = 5;
```

这个数字需要和贴图的垂直帧数匹配。

### Debuff 免疫

```csharp
NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
```

设置后，这个 NPC 不会中毒。

# AI 行为逻辑

## AI() 方法

`AI()` 每帧（约 60 帧/秒）都会被调用：

```csharp
public override void AI()
{
    // 面向最近的玩家
    NPC.TargetClosest(true);

    // 检测目标是否在范围内
    if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 500f)
    {
        // 移动
        NPC.velocity.X = NPC.direction * 1.5f;
        NPC.velocity.Y = -3f;
    }

    // 旋转
    NPC.rotation = NPC.velocity.X * 0.05f;
}
```

### aiStyle = -1

设置为 -1 表示完全自定义 AI，需要自己实现 `AI()` 方法。

# NPC 攻击玩家

## OnHitPlayer 方法

当 NPC 成功攻击玩家时触发：

[OnHitPlayer 方法](cs:./code/firstnpc_npc.cs#cs:m:ModDocProject.ModsSource.Modder入门.FirstNPC.OnHitPlayer(Player,Terraria.HitInfo))

```csharp
public override void OnHitPlayer(Player player, Player.HitInfo hurtInfo)
{
    // 给玩家添加中毒效果，持续3秒
    player.AddBuff(BuffID.Poisoned, 180);
}
```

> [!TIP] 常用 Debuff
>
> - `BuffID.Poisoned` - 中毒
> - `BuffID.OnFire` - 着火
> - `BuffID.Bleeding` - 流血
> - `BuffID.Confused` - 混乱
> - `BuffID.Slowed` - 缓慢

# 生成条件

## SpawnChance 方法

控制 NPC 在什么情况下会生成：

[SpawnChance 方法](cs:./code/firstnpc_npc.cs#cs:m:ModDocProject.ModsSource.Modder入门.FirstNPC.SpawnChance(Terraria.ModLoader.NPCSpawnInfo))

```csharp
public override float SpawnChance(NPCSpawnInfo spawnInfo)
{
    return SpawnCondition.OverworldDaySlime.Chance * 0.1f;
}
```

返回 0 表示不会自然生成，只能通过代码生成。

> [!NOTE] 生成条件参考
>
> - `SpawnCondition.OverworldDaySlime` - 白天史莱姆
> - `SpawnCondition.OverworldNightSlime` - 夜晚史莱姆
> - `SpawnCondition.Corruption` - 腐化之地
> - `SpawnCondition.Underground` - 地下

# 知识测验

```quiz
type: choice
id: quiz-npc-1
question: |
  aiStyle = -1 的作用是什么？
options:
  - id: A
    text: NPC 不会移动
  - id: B
    text: 使用完全自定义的 AI
  - id: C
    text: NPC 攻击无效
answer: B
explain: |
  aiStyle = -1 表示不使用预设 AI，
  需要自己实现 AI() 方法来定义行为。
```

```quiz
type: tf
id: quiz-npc-2
question: |
  NPC.lifeMax 设置的是 NPC 当前的生命值。
answer: false
explain: |
  NPC.lifeMax 设置的是 NPC 的最大生命值。
  life 才是当前生命值。
```

# 总结

{color:Mad}{恭喜你学会了创建第一个 NPC！}

今天学到的内容：
- 创建 `ModNPC` 子类
- 使用 `SetDefaults` 设置基础属性
- 使用 `AI()` 定义行为逻辑
- 使用 `OnHitPlayer` 实现攻击效果

> [!TIP] 下一步
>
> 接下来我们将学习**合成表入门**！
>
> 合成表是泰拉瑞亚的核心玩法，学会它你就可以让物品之间互相转换了。

{color:Mad}{敌怪在手，世界颤抖！}
