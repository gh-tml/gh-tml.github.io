---
title: 让我们动起来-Mod弹幕与AI指南
author: 小天使
topic: modder-basic
description: 学习弹幕如何编写
order: 3
difficulty: beginner
time: 25分钟
next_chapter: Modder入门/第一个配置-可调开关.md
prev_chapter: Modder入门/第一把远程武器.md
source_cs:
  - ./code/firstproj.cs
colors:
  Mad: "#ff5430"
---
# 前言

> [!WARNING] 来自蟑螂皇帝:
>
> ***倾听我们阴暗的振翅声……我们在阴影中筑巢***
> ***贵族们永恒不变地前进着，即使他们被憎恨***
> ***然后，某一天，当你们沉浸在温暖中安然熟睡，我们就会从舒适的床上突然出现***
> ***……然后所有求生的声音开始喧哗，尖叫，扭动”，归于虚无……被我们的口器一片一片地吞噬***
> ***前进，前进，吞噬，生殖，抹除，再生，前进，前进，前进***
> - --
> {color:Mad}{弹幕如同战争一般，摧毁各种入门Modder的心脏}

上面当然只是开个玩笑（

不过弹幕这边，难的非常难，简单的非常简单。

{color:Mad}{我知道你们这些入门的想要什么，大部分都是冲着制作一把帅的武器来的}

{color:Mad}{做好心理准备，弹幕教程现在开始}

# 弹幕设置

## `SetDefaults()`

[弹幕设置](cs:./code/firstproj.cs#cs:m:ExMod.Content.Projectiles.FirstProj.SetDefaults())

## `AI()`

[弹幕AI](cs:./code/firstproj.cs#cs:m:ExMod.Content.Projectiles.FirstProj.AI())

当然，我们现在来模拟一下**Arrow**的AI

[Arrow模拟](cs:./code/firstproj.cs#cs:m:ExMod.Content.Projectiles.FirstProj.TryArrow())

# AI教学前置-数学中的向量详解

## 向量基础介绍

[动画1](anim:anims/vector-basic.anim.ts)

上面这个可交互的动画是向量的一个演示，如同你所见，向量会指向一个终点。

在数学上，向量是可以复用的，所以向量可以自由移动，所以判断两个向量是否一致，是通过判断长度和方向。

引入直角坐标系之后，向量就可以由起点O(固定(0,0))到终点A，然后我们会把终点A的坐标称呼为**向量OA**。

### 向量计算公式

- 关于指向一个目标: $ vector=终点坐标-起点坐标 $

这一块可能没接触向量的人有一点难理解，我们不妨设想一下：

> [!NOTE] 问题
>
> 如果我们已经用坐标表达的向量的话，那怎么样才能计算出起点到终点的向量？

根据上面所说的，向量是可以复用的，可以自由移动的，那么现在我们假设：

***起点就是(0,0)原点***

那么接下来坐标的加减法就是 $ (X_1-X_2,Y_1-Y_2) $

[箭的AI](anim:anims/arrow-easy-ai.anim.ts)

```quiz
type: choice
id: quiz-choice-k53nre
question: |
  嗨，来做题吧！我们知道了 $ (X_1-X_2,Y_1-Y_2) $ 是用坐标表达的向量加减法。
  那么，现在我们来算： $ Pos_1=(3,2);Pos_2=(1,2) $ ，求 $ Pos_1到Pos_2的向量 $
options:
  - id: A
    text: $ (4,4) $
  - id: B
    text: $ (2,0) $
  - id: C
    text: $ (-2,0) $
answer: C
explain: |
  Pos_1到Pos_2的向量是 $ Pos_2 - Pos_1 $
```

## 向量合成和分解

[动画2](anim:anims/vector-add-resolution.anim.ts)

这个动画通过点击可以进行交互。

然后你可以从这上面看到向量是怎么变化的（这边有点麻烦，我会细细的说明）

### 分解向量到坐标轴上

我知道这很难，但是你不要急。根据这个动画你也能发现，你只需要和坐标轴做个垂直关系，就能以最简单的方法分解出来了。

当然我们还有不同分解方法，但是大部分时候也不需要，你又不是做物理题，不用受力分析。

这样就会得到 X方向向量 $ vector_x=(x,0) $ 和 Y方向向量 $ vector_y=(0,y) $

### 把方向向量合成为一个向量

我知道这也很难（

有一个法则叫三角形法则，你可以把一个向量的起点移动到另一个向量上！

具体表达：

$$ vector = vector_1 + vector_2 $$

> 具体动画解释请自行搜索喵

> [!TIP] 温馨提示
>
> 你要这样玩只有用坐标表达可以，当然，计算机里面表达一个向量绝大多数时候不离开 X方向向量 + Y方向向量，它们刚刚好是一个坐标！ $ (x,y) $

# AI教学

[弹幕第一个AI](cs:./code/firstproj.cs#cs:m:ExMod.Content.Projectiles.FirstProj.AI_1())

上面是可以用在 `AI()` 方法里面的内容，里面调用的一个方法是下面的方法：

[弹幕第一个AI的运动](cs:./code/firstproj.cs#cs:m:ExMod.Content.Projectiles.FirstProj.AI_1_AttackNPC(NPC))