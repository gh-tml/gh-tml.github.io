---
title: 项目结构与ExampleMod组织
author: AI协作者
topic: modder-basic
description: 从官方 ExampleMod 镜像理解 Mod 根类、内容目录与注册流程
order: 2
difficulty: beginner
time: 15分钟
prev_chapter: Modder入门/制作第一把武器.md
next_chapter: Modder入门/护甲、套装奖励与ModPlayer.md
source_cs:
  - ./code/project_examplemod.cs
  - ./code/project_exampleitem.cs
  - ./code/project_examplerecipes.cs
---

# 这一章的目标

你已经做完第一把武器，这一章不重复武器步骤，只做一件事，把功能代码放回官方项目结构里看清职责边界。

# 先看三个官方镜像入口

- [ExampleMod 根类](cs:./code/project_examplemod.cs)
- [ExampleItem 示例](cs:./code/project_exampleitem.cs)
- [ExampleRecipes 示例](cs:./code/project_examplerecipes.cs)

# 怎么读结构

先读 `ExampleMod` 根类，确认 Mod 的加载和全局入口。然后读 `ExampleItem`，看内容类型怎样继承并注册。最后读 `ExampleRecipes`，看配方相关代码如何被组织。

这三个文件合起来，刚好对应你后续最常写的三层，入口、内容类型、配方扩展。

# C# mini-lesson

读官方文件时优先盯三类语法点，`namespace` 决定归属，`class` 决定职责，`override` 决定你在 tML 生命周期里的挂点。

# 常见误区

不要把“能跑起来”当成结构正确。把入口逻辑塞进具体内容类，后面维护会很痛苦。先分清文件职责，再扩展功能，会稳定很多。
