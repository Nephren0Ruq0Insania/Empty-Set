﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using EmptySet.Extensions;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace EmptySet.Projectiles.Melee;

/// <summary>
/// 影球弹幕(右侧)
/// </summary>
public class BigShadowBall : ModProjectile
{
    public override void SetStaticDefaults()
    {   
        Main.projFrames[Type] = 4;
    }
    public override void SetDefaults()
    {
        Projectile.DamageType = DamageClass.Melee;
        Projectile.width = 36;
        Projectile.height = 80;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        
        Projectile.timeLeft = (int)(8 * EmptySet.Frame);
    }
    public override void AI()
    {
        //Projectile.DirectionalityRotation(0.27f);
        Projectile.FrameControl();
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
            DustID.PurpleTorch, 0, 0, 0, Color.Purple);

    }
    
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        SoundEngine.PlaySound(SoundID.Item167, Projectile.position);
        Projectile.localAI[0]++;
        if (Projectile.localAI[0] > 1)
            return;
        //三弹幕
        //Vector2 velocity = new Vector2(0, 20);
        //Vector2 startPos = Projectile.position + new Vector2(0, -700);
        //Vector2 velo = (Projectile.position - startPos).SafeNormalize(Vector2.UnitY) * velocity.Length();
        //Projectile.NewProjectile(Projectile.GetSource_FromAI(), startPos, velo, ModContent.ProjectileType<ShadowBall>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Main.MouseWorld.Y);
        //for (int i = 0; i < 2; i++)
        //{
        //    startPos = Projectile.position + new Vector2((i == 1 ? -1 : 1) * Main.rand.Next(200, 500), -700);
        //    velo = (Projectile.position - startPos).SafeNormalize(Vector2.UnitY) * velocity.Length();
        //    Projectile.NewProjectile(Projectile.GetSource_FromAI(), startPos, velo, ModContent.ProjectileType<ShadowBall>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Main.MouseWorld.Y);
        //}

        //Vector2 velocity = new Vector2(0, 20);
        var startPos = Main.player[Projectile.owner].position;
        var rand = Main.rand;
        var velocity = Vector2.One * 16f;
        for (int i = 0; i < 3; i++)
        {
            var range = 60;
            var randPos = new Vector2(startPos.X + rand.Next(-range,range), startPos.Y + rand.Next(-range,range));
            Vector2 velo = (Projectile.Center - randPos).SafeNormalize(Vector2.UnitY) * velocity.Length();
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), randPos, velo,
                ModContent.ProjectileType<ShadowBall>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

        }
       
        Projectile.LetExplodeWith(60, () =>
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width-20, Projectile.height-20, DustID.Smoke, 0f, 0f, 100,
                    default, 2f);
                Main.dust[dust].velocity *= 1.4f;
            }//生成短暂烟雾粒子
            //for (int l = 0; l < 4; l++)
            //{
            //    int gore = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y), default,
            //        Main.rand.Next(61, 64), 1f);
            //    Main.gore[gore].velocity *= 0.5f;
            //    Gore gore2 = Main.gore[gore];
            //    gore2.velocity.X += 1f;
            //    Gore gore3 = Main.gore[gore];
            //    gore3.velocity.Y += 1f;
            //}//生成长时段四片烟
        }, Projectile.damage, true, false);


        Projectile.Kill();
    }
    public override void ModifyDamageHitbox(ref Rectangle hitbox)
    {
        //hitbox.X += 3;
        //hitbox.Y += 3;
        //hitbox.Width -= 6;
        //hitbox.Height -= 6;
        //46*46= 2116 /3.14*25*25=196x/ 50*50=2500  /44best
        //Vector2 vec = Projectile.velocity.SafeNormalize(Vector2.Zero) * 12f * Projectile.scale;
    }

    public override void OnKill(int timeLeft)
    {

        


    }
}

/// <summary>
/// 影球弹幕(左侧)
/// </summary>
public class BigShadowBall2 : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.DamageType = DamageClass.Melee;
        Projectile.width = 36;
        Projectile.height = 80;
        Projectile.friendly = true;
        Projectile.tileCollide = false;

        Projectile.timeLeft = (int) (8 * EmptySet.Frame);
    }

    public override void AI()
    {
        //Projectile.DirectionalityRotation(0.27f);
        Projectile.FrameControl();
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
            DustID.PurpleTorch, 0, 0, 0, Color.Purple);

    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.localAI[0]++;
        if (Projectile.localAI[0] > 1)
            return;
        var startPos = Main.player[Projectile.owner].position;
        var rand = Main.rand;
        var velocity = Vector2.One * 16f;
        for (int i = 0; i < 3; i++)
        {
            var range = 60;
            var randPos = new Vector2(startPos.X + rand.Next(-range, range), startPos.Y + rand.Next(-range, range));
            Vector2 velo = (Projectile.Center - randPos).SafeNormalize(Vector2.UnitY) * velocity.Length();
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), randPos, velo,
                ModContent.ProjectileType<ShadowBall>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

        }

        Projectile.LetExplodeWith(60, () =>
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width - 20, Projectile.height - 20,
                    DustID.Smoke, 0f, 0f, 100,
                    default, 2f);
                Main.dust[dust].velocity *= 1.4f;
            } //生成短暂烟雾粒子
        }, Projectile.damage, true, false);


        Projectile.Kill();
    }

    public override void ModifyDamageHitbox(ref Rectangle hitbox)
    {

    }

    public override void OnKill(int timeLeft)
    {

    }
}
