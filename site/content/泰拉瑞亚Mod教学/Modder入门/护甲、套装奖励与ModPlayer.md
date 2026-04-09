---
title: 护甲、套装奖励与ModPlayer
author: AI协作者
topic: modder-basic
description: 用官方 ExampleMod 玩家状态文件讲清单件护甲、套装奖励与 ModPlayer 分工
order: 3
difficulty: beginner
time: 18分钟
prev_chapter: Modder入门/项目结构与ExampleMod组织.md
next_chapter: Modder入门/本地化入门.md
source_cs:
  - ./code/armor_examplehelmet.cs
  - ./code/armor_examplebreastplate.cs
  - ./code/armor_exampleleggings.cs
  - ./code/player_simplemodplayer.cs
  - ./code/player_exampleinventoryplayer.cs
  - ./code/buff_exampledefensebuff.cs
---

# 这一章解决什么问题

新手最常见的问题是，哪些效果放 `ModItem`，哪些状态放 `ModPlayer`。这章直接用官方 ExampleMod 的本地镜像给你一个可追溯答案。

# 官方玩家状态核心来源

- [SimpleModPlayer.cs](cs:./code/player_simplemodplayer.cs)
- [ExampleInventoryPlayer.cs](cs:./code/player_exampleinventoryplayer.cs)
- [ExampleDefenseBuff.cs](cs:./code/buff_exampledefensebuff.cs)

这三份文件是本章的核心依据，分别对应基础玩家状态挂点、背包驱动状态、Buff 触发状态。

# 护甲文件怎么配合看

- [ExampleHelmet](cs:./code/armor_examplehelmet.cs)
- [ExampleBreastplate](cs:./code/armor_examplebreastplate.cs)
- [ExampleLeggings](cs:./code/armor_exampleleggings.cs)

先看单件 `UpdateEquip`，再看套装 `IsArmorSet` 和 `UpdateArmorSet`，最后回到 `ModPlayer` 做跨帧状态收口。

# C# mini-lesson

本章最常用的是字段、`bool`、`if` 和生命周期方法。判断状态时先用 `if` 早退，状态位按帧重置，避免脱装备后效果残留。

# 常见误区

把整套逻辑写在单件 `UpdateEquip`，或者只设状态不在玩家类里重置，都会导致后续扩展困难。按官方文件拆层写，问题会少很多。
