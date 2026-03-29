using Terraria;
using Terraria.ModLoader;

namespace ModDocProject.ModsSource.Modder入门
{
    /// <summary>
    /// 第一个Buff：防御力增益
    /// 继承 ModBuff 来创建一个Buff
    /// </summary>
    public class FirstDefenseBuff : ModBuff
    {
        // 防御力增益数值
        public static readonly int DefenseBonus = 10;

        // 使用本地化文本，支持参数替换
        // Description 会自动读取 Localization 文件中的翻译
        public override LocalizedText Description => base.Description.WithFormatArgs(DefenseBonus);

        /// <summary>
        /// SetStaticDefaults：在Mod加载时设置Buff的静态属性
        /// 这里可以设置Buff图标、是否在退出世界时清除等
        /// </summary>
        public override void SetStaticDefaults()
        {
            // Main.buffNoSave[Type] = true 表示这个Buff不会在玩家退出世界时被保存
            // 通常用于临时Buff或测试用Buff
            Main.buffNoSave[Type] = true;

            // Main.buffNoTimeDisplay[Type] = true 会隐藏Buff的时间显示
            // 用于无限持续或通过其他方式显示时间的Buff
            // Main.buffNoTimeDisplay[Type] = true;
        }

        /// <summary>
        /// Update：每帧更新Buff效果
        /// Player player: 受到这个Buff影响的玩家
        /// ref int buffIndex: 当前Buff在玩家Buff列表中的索引
        /// </summary>
        public override void Update(Player player, ref int buffIndex)
        {
            // 直接修改玩家的属性
            player.statDefense += DefenseBonus;

            // 常用的玩家属性修改：
            // player.statLifeMax2 += 20;        // 最大生命值
            // player.manaCost -= 0.1f;         // 魔力消耗（负数减少消耗）
            // player.moveSpeed += 0.1f;         // 移动速度
            // player.GetDamage(DamageClass.Generic) += 0.1f;  // 全能伤害
        }
    }
}
