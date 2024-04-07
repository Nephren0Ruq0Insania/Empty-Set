﻿using Microsoft.Xna.Framework;
using EmptySet.Extensions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EmptySet.Projectiles.Weapons.Throwing;

/// <summary>
/// 微型彗星雷弹幕
/// </summary>
public class MiniMeteoriteThunderProj : ModProjectile
{
    //private float _scale = 1f;

    public override void SetDefaults()
    {
        Projectile.width = 60; //已精确测量 //(int)(50* _scale);
        Projectile.height = 40; //(int)(50*_scale);
        Projectile.aiStyle = 14;
        Projectile.friendly = true;
        //Projectile.hostile = true;
        Projectile.DamageType = DamageClass.Throwing;
        //Projectile.scale = _scale;
        Projectile.timeLeft = 4 * EmptySet.Frame;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        //Projectile.velocity *= 0.97f;
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

        for (int i = 1; i <= 4; i++)
        {
            var star = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center,
                Projectile.velocity + (new Vector2(20).Length() + ((150f+i*5f) * MathHelper.Pi / 36f)).ToRotationVector2() * 4,
                ProjectileID.FallingStar, Projectile.damage, Projectile.knockBack, Projectile.owner, 1);
            star.aiStyle = 2;
            star.scale = 0.75f;
        }//生成坠星

        if (Projectile.penetrate == 0) //触发造成伤害
        {
            Projectile.LetExplode(100, Projectile.damage);
        }

        Projectile.position.X += Projectile.width / 2f;
        Projectile.position.Y += Projectile.height / 2f;
        Projectile.position.X -= Projectile.width / 2f;
        Projectile.position.Y -= Projectile.height / 2f;
        for (int i = 0; i < 50; i++)
        {
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100,
                default, 2f);
            Main.dust[dust].velocity *= 1.4f;
        }//生成短暂烟雾粒子
        for (int j = 0; j < 30; j++)
        {
            int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.YellowTorch, 0f,
                0f, 100, default, 3.5f);
            Main.dust[dust2].noGravity = true;
            Main.dust[dust2].velocity *= 7f;
            dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.YellowTorch, 0f,
                0f, 100,
                default, 1.5f);
            Main.dust[dust2].velocity *= 3f;
        }//黄色粒子
        for (int l = 0; l < 4; l++)
        {
            int gore = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y), default,
                Main.rand.Next(61, 64), 1f);
            Main.gore[gore].velocity *= 0.5f;
            Gore gore2 = Main.gore[gore];
            gore2.velocity.X += 1f;
            Gore gore3 = Main.gore[gore];
            gore3.velocity.Y += 1f;
        }//生成长时段四片烟
    }
    public override void AI()
    {
        //var dust = Dust.NewDustDirect(Projectile.position + Projectile.velocity*Projectile.rotation, Projectile.width -16 , Projectile.height -16 ,//- 16
        //    DustID.Torch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);//* 0.5f
        //dust.noGravity = true;
        //dust.velocity *= 0f;
        //dust.scale = 0.8f;
    }
}