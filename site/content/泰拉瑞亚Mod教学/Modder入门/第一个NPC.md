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

所有自定义 NPC 都需要继承 `ModNPC` 类。`ModNPC` 是 tModLoader 提供的基类，封装了大部分 NPC 逻辑。

[FirstNPC 类定义](cs:./code/firstnpc_npc.cs#cs:t:ModDocProject.ModsSource.Modder入门.FirstNPC)

## SetDefaults - 基础属性

`SetDefaults` 是设置 NPC 核心属性的地方，包含以下关键属性：

[SetDefaults 实现](cs:./code/firstnpc_npc.cs#cs:m:ModDocProject.ModsSource.Modder入门.FirstNPC.SetDefaults())

### 碰撞箱设置

`NPC.width` 和 `NPC.height` 定义碰撞箱尺寸（像素），决定 NPC 可被攻击的范围，应该与贴图大小匹配。

### 战斗属性

`NPC.damage` 是攻击伤害，`NPC.defense` 是防御力，`NPC.lifeMax` 是最大生命值。

### 音效设置

`NPC.HitSound` 设置受伤音效，`NPC.DeathSound` 设置死亡音效，使用 `SoundID` 类中的预设 ID。

### AI 复用技巧

使用 `AIType` 可以直接复用现有 NPC 的 AI。例如 `AIType = NPCID.Zombie` 会让 NPC 使用僵尸的行为逻辑，不用自己写复杂代码。

## SetStaticDefaults - 静态设置

`SetStaticDefaults` 在游戏加载时执行一次，用于设置整个 NPC 类型共用的属性。

[SetStaticDefaults 实现](cs:./code/firstnpc_npc.cs#cs:m:ModDocProject.ModsSource.Modder入门.FirstNPC.SetStaticDefaults())

### 动画帧数

`Main.npcFrameCount[Type] = 5` 设置动画帧数，需要和贴图的垂直帧数匹配。

### Debuff 免疫

`NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true` 设置对特定 Debuff 的免疫。设置后，这个 NPC 不会中毒。

# AI 行为逻辑

## AI() 方法

`AI()` 每帧（约 60 帧/秒）都会被调用，是 NPC 行为逻辑的核心。

[AI 实现](cs:./code/firstnpc_npc.cs#cs:m:ModDocProject.ModsSource.Modder入门.FirstNPC.AI())

### aiStyle = -1

`NPC.aiStyle = -1` 表示完全自定义 AI，需要自己实现 `AI()` 方法。如果使用预设 AI，可以设置为其他数值。

### 目标锁定

`NPC.TargetClosest(true)` 让 NPC 面向并锁定最近的玩家。`NPC.HasValidTarget` 检查是否有有效目标。

### 移动与跳跃

`NPC.direction` 是 -1 或 1，表示面朝方向。设置 `NPC.velocity.X` 和 `NPC.velocity.Y` 控制移动和跳跃。

### 旋转效果

`NPC.rotation` 控制 NPC 的旋转角度，常用于让 NPC 随移动方向轻微倾斜。

# NPC 攻击玩家

## OnHitPlayer 方法

当 NPC 成功攻击玩家时触发。使用 `player.AddBuff()` 给玩家添加持续性 Debuff 效果。

[OnHitPlayer 实现](cs:./code/firstnpc_npc.cs#cs:m:ModDocProject.ModsSource.Modder入门.FirstNPC.OnHitPlayer(Player,Terraria.HitInfo))

常用 Debuff：
- `BuffID.Poisoned` - 中毒
- `BuffID.OnFire` - 着火
- `BuffID.Bleeding` - 流血
- `BuffID.Confused` - 混乱
- `BuffID.Slowed` - 缓慢

# 生成条件

## SpawnChance 方法

`SpawnChance` 控制 NPC 在什么情况下会生成。返回值是 0 到 1 之间的概率。

[SpawnChance 实现](cs:./code/firstnpc_npc.cs#cs:m:ModDocProject.ModsSource.Modder入门.FirstNPC.SpawnChance(Terraria.ModLoader.NPCSpawnInfo))

常用生成条件：
- `SpawnCondition.OverworldDaySlime` - 白天史莱姆
- `SpawnCondition.OverworldNightSlime` - 夜晚史莱姆
- `SpawnCondition.Corruption` - 腐化之地
- `SpawnCondition.Underground` - 地下

返回 0 表示不会自然生成，只能通过代码生成。

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
