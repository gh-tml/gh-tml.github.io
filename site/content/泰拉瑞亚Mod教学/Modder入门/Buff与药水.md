---
title: Buff与药水-给玩家上状态
author: 小天使
topic: modder-basic
description: 学习如何创建Buff和药水，给玩家或敌人添加状态效果
order: 5
difficulty: beginner
time: 20分钟
next_chapter: Modder入门/第一个NPC.md
prev_chapter: Modder入门/制作第一把武器.md
source_cs:
  - ./code/firstbuff_buff.cs
  - ./code/firstbuff_potion.cs
colors:
  Mad: "#ff5430"
---

# 前言

> [!WARNING] 来自炼金术士的警告:
>
> ***药水不仅仅是颜色的混合……那是灵魂的烙印***
>
> ***每一瓶Buff，都承载着改变命运的力量***
>
> ***你准备好了吗，Modder？***

{color:Mad}{欢迎来到Buff与药水教程！}

当你玩泰拉瑞亚的时候，你一定喝过各种各样的药水——增加防御、增加伤害、加快移动速度……

这些效果是怎么实现的呢？答案就是 **Buff 系统**！

{color:Mad}{Buff就是"状态效果"，它可以影响玩家的属性、行为，甚至整个游戏世界！}

Buff 有三种主要类型：

| 类型 | 说明 | 例子 |
|------|------|------|
| 玩家增益 | 给玩家正面效果 | 防御力提升、伤害提升 |
| 玩家减益 | 给玩家负面效果 | 中毒、着火 |
| 召唤物标记 | 用于召唤物的攻击判定 | 鞭子的标记效果 |

# 创建一个Buff

## Buff类基础

Buff 是通过继承 `ModBuff` 来创建的。让我看看最基础的Buff是什么样的：

[Buff类定义](cs:./code/firstbuff_buff.cs#cs:t:ModDocProject.ModsSource.Modder入门.FirstDefenseBuff)

上面的代码展示了创建Buff的基本结构。让我来解释几个关键点：

### `SetStaticDefaults()` - 静态设置

```csharp
Main.buffNoSave[Type] = true;
```

> [!NOTE] 说明
>
> `buffNoSave = true` 表示这个Buff在玩家退出世界时不会被保存
>
> 通常用于临时Buff或测试用途

### `LocalizedText Description` - 本地化描述

```csharp
public override LocalizedText Description => base.Description.WithFormatArgs(DefenseBonus);
```

{color:Mad}{这是tModLoader 1.4.4+ 的新写法！}

使用 `WithFormatArgs` 可以让我们在本地化文件中使用 `{0}` 等占位符，动态插入数值。

## Buff效果 - `Update()`方法

[Buff效果实现](cs:./code/firstbuff_buff.cs#cs:m:ModDocProject.ModsSource.Modder入门.FirstDefenseBuff.Update(Player,ref int))

这就是Buff效果的核心！`Update` 方法每帧都会被调用。

> [!TIP] 常用玩家属性修改
>
> - `player.statDefense += 10;` - 增加防御
> - `player.statLifeMax2 += 20;` - 增加最大生命
> - `player.moveSpeed += 0.1f;` - 增加移动速度
> - `player.manaCost -= 0.1f;` - 减少魔力消耗

# 创建一个Buff药水

光有Buff还不够，我们需要一个物品来使用这个Buff！

[药水物品](cs:./code/firstbuff_potion.cs#cs:m:ModDocProject.ModsSource.Modder入门.FirstBuffPotion.SetStaticDefaults())

[药水物品设置](cs:./code/firstbuff_potion.cs#cs:m:ModDocProject.ModsSource.Modder入门.FirstBuffPotion.SetDefaults())

### 关键属性解读

```csharp
// 使用方式 - 像喝水一样喝下去
Item.useStyle = ItemUseStyleID.DrinkLiquid;

// 使用后给予的Buff类型
Item.buffType = ModContent.BuffType<FirstDefenseBuff>();

// Buff持续时间（帧）- 5400帧 = 90秒
Item.buffTime = 5400;

// 是否可消耗 - true表示使用后会减少数量
Item.consumable = true;
```

> [!IMPORTANT] ModContent.BuffType<T>()
>
> 要获取自定义Buff的类型，必须使用 `ModContent.BuffType<你的Buff类>()`
>
> 这在设置 `buffType` 时是必须的！

### 喝药水的粒子效果

```csharp
ItemID.Sets.DrinkParticleColors[Type] =
[
    new Color(240, 240, 240),
    new Color(200, 200, 200),
    new Color(140, 140, 140)
];
```

这个设置决定了喝药水时产生的粒子颜色，让你的药水看起来更加炫酷！

# 知识测验

```quiz
type: single
id: quiz-buff-1
question: |
  如果我想让Buff在玩家退出世界时被保存，应该把 `buffNoSave` 设置为？
options:
  - id: A
    text: true
  - id: B
    text: false
  - id: C
    text: 默认就是false，不需要设置
answer: B
explain: |
  `buffNoSave = false`（或不设置）表示Buff会被保存。
  只有设置为 `true` 时，Buff才会在退出世界时清除。
```

```quiz
type: tf
id: quiz-buff-2
question: |
  `buffTime = 5400` 的意思是Buff持续5400秒。
answer: false
explain: |
  5400是**帧**数，不是秒。
  泰拉瑞亚是60帧每秒，所以5400帧 = 90秒。
```

# 总结

{color:Mad}{恭喜你学会了创建Buff和药水！}

回顾一下今天学到的：

- ✅ 创建 `ModBuff` 类
- ✅ 使用 `SetStaticDefaults` 设置Buff属性
- ✅ 使用 `Update` 方法添加Buff效果
- ✅ 创建药水物品并关联Buff

> [!TIP] 下一步
>
> 接下来我们将学习**物块制作**！
>
> 物块是构成泰拉瑞亚世界的基础，学会它你就可以真正开始建造自己的世界了！

{color:Mad}{药水在手，天下我有！}
