﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EmptySet.Projectiles.Magic;

/// <summary>
/// 恒灰法球弹幕
/// </summary>
public class EternityAshMagicBallProj : ModProjectile
{
    Dust dust;
    private int count = 3;
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("Eternity Ash Magic Ball");
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // 要记录的旧位置长度
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // 记录模式
    }
    public override void SetDefaults()
    {
        Projectile.width = 26;
        Projectile.height = 26;
        //Projectile.aiStyle = 1; // The ai style of the projectile, please reference the source code of Terraria
        Projectile.friendly = true; // Can the projectile deal damage to enemies?
        Projectile.hostile = false; // Can the projectile deal damage to the player?
        Projectile.DamageType = DamageClass.Magic; 
        Projectile.penetrate = 7; // 射弹能穿透多少怪物。(下面的OnTileCollide也会减少弹跳穿透)
        Projectile.timeLeft = 600; // 弹丸的有效时间(60 = 1秒，所以600 = 10秒)
        Projectile.alpha = 0; // 射弹的透明度，255为完全透明。(aiStyle 1快速淡入投射物)如果你没有使用淡入的aiStyle，请确保删除这个。你会奇怪为什么你的投射物是隐形的。
        Projectile.light = 0; // 发射体周围发射出多少光
        Projectile.ignoreWater = true; // 水会影响弹丸的速度吗?
        Projectile.tileCollide = true; // 弹丸能与瓦片碰撞吗?
        Projectile.extraUpdates = 1; // 如果你想让投射物在一帧内更新多次，设置为0以上

        //AIType = ProjectileID.Bullet; // 就像默认的Bullet一样
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        count--;
        if (count <= 0)
        {
            Projectile.Kill();
        }
        else
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            // 如果弹丸击中瓷砖的左边或右边，反转X速度
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            // 如果弹丸击中瓷砖的顶部或底部，反转Y速度
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
        }

        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Projectile.rotation += 0.15f;
        dust = Main.dust[Dust.NewDust(Projectile.position, 26, 26, DustID.CorruptGibs)];
        dust.noGravity = true;
        Main.instance.LoadProjectile(Projectile.type);
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        // 用不受光线影响的颜色重新绘制投射体
        Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
            Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
            Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }

        return true;
    }
    public override void OnKill(int timeLeft)
    {
        //这段代码和上面类似的代码在“OnTileCollide”中产生的灰尘与瓷砖相撞。SoundID.Item10 就是你听到的弹跳声。
        Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
        SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
    }
}