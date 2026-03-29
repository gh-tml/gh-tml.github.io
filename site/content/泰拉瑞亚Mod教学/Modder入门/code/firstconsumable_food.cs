using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModDocProject.ModsSource.Modder入门
{
    /// <summary>
    /// 第一个食物物品
    /// 食物有一些特殊的属性来模拟进食效果
    /// </summary>
    public class FirstFood : ModItem
    {
        public override void SetStaticDefaults()
        {
            // ResearchUnlockCount：研究模式需要的解锁数量
            // 食物通常设置为5，因为研究一个就能解锁
            Item.ResearchUnlockCount = 5;

            // Main.RegisterItemAnimation：注册物品动画
            // DrawAnimationVertical：垂直方向的动画
            // 第一个参数是切换到下一帧的时间（int.MaxValue = 几乎不切换）
            // 第二个参数是总帧数（3帧：背包、持有、放置）
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

            // FoodParticleColors：进食时产生的粒子颜色
            // 食物粒子会向外飞出，饮料粒子会下落
            ItemID.Sets.FoodParticleColors[Item.type] =
            [
                new Color(249, 230, 136),
                new Color(152, 93, 95),
                new Color(174, 192, 192)
            ];

            // ItemID.Sets.IsFood：标记这个物品为食物
            // 允许它被放置在盘子上或正确地被持有
            ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            // DefaultToFood：设置食物的默认属性
            // 参数：宽度, 高度, 主Buff类型, Buff持续时间
            // Buff持续时间以帧为单位，57600帧 = 16分钟
            Item.DefaultToFood(22, 22, BuffID.WellFed3, 57600);

            // 设置价值和稀有度
            Item.value = Item.buyPrice(gold: 3);
            Item.rare = ItemRarityID.Blue;
        }

        /// <summary>
        /// OnConsumeItem：消耗物品时触发
        /// 用于添加额外的Buff效果
        /// 这个方法在食物Buff生效后调用
        /// </summary>
        public override void OnConsumeItem(Player player)
        {
            // 添加额外的Buff
            // 糖化Buff会增加一点点移动速度
            player.AddBuff(BuffID.SugarRush, 3600);
        }
    }
}
