﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EmptySet.Projectiles.Throwing;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace EmptySet.Projectiles.Magic;

public class DarklingCrossing : ModProjectile
{
    private Player player => Main.player[Projectile.owner];

    private int Timer
    {
        get => (int) Projectile.ai[0];
        set => Projectile.ai[0] = value;
    }

    private bool shoot = false;
    private Vector2 velo = Vector2.UnitX;
    public override string Texture => "EmptySet/Items/Weapons/Magic/DarklingCrux";

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.scale = 0.5f;
        Projectile.alpha = 255;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.knockBack = 6f;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }

    public override bool? CanDamage() => !player.channel;

    public override void AI()
    {
        switch (Projectile.ai[1])
        {
            case 0:
                UpdatePlayer();
                Timer++;
                if (player.dead)
                {
                    Projectile.timeLeft = 1;
                }
                else if (player.channel)
                {
                    player.itemAnimation = 2;
                    player.itemTime = 2;
                    UpdateProj();
                    if (Timer % 6 == 0)
                    {
                        int damage = (int) (Projectile.damage * Projectile.scale / 2);
                        int type = ModContent.ProjectileType<DarklingCrossingShard>();
                        Vector2 vel = new Vector2(0, -10f).RotatedBy(Timer).SafeNormalize(Vector2.Zero) * 10f;
                        Vector2 pos = Projectile.Center;
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, vel, type, damage, 1f, Projectile.owner);
                        if (Main.rand.NextBool(2))
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos,
                                (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 15f, type, damage,
                                1f, Projectile.owner);
                        }

                        if (!player.CheckMana(4, true)) player.channel = false;
                    }
                }
                else Projectile.ai[1] = 1;

                break;
            case 1:
                if (!shoot)
                {
                    shoot = true;
                    Timer = 0;
                    Projectile.localAI[0] = (Main.MouseWorld - Projectile.Center).ToRotation() - 1.57f;
                    Projectile.timeLeft = 600;
                    Projectile.damage = (int) (Projectile.scale * Projectile.damage);
                }

                if (Projectile.scale <= 1f)
                {
                    Projectile.timeLeft = 1;
                    return;
                }

                float rot = Projectile.localAI[0] - Projectile.rotation;
                Timer++;
                if (Timer == 30)
                {
                    velo = new Vector2(0, 1f).RotatedBy(Projectile.localAI[0]);
                    Projectile.velocity = velo * -5;
                }
                else if (Timer > 30) Projectile.velocity += velo * 0.5f;

                if (Timer < 15)
                {
                    Projectile.rotation += (rot) * 0.3f;
                }
                else if (Timer == 15)
                {
                    Projectile.tileCollide = true;
                    Projectile.friendly = true;
                }
                else
                {
                    if (Projectile.velocity.Length() >= 20f)
                        Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 20f;
                }

                break;
        }
    }

    private void UpdatePlayer()
    {
        int dir = Main.MouseWorld.X > player.position.X ? 1 : -1;
        player.ChangeDir(dir);
        player.itemRotation = 0.01f * dir;
        Projectile.timeLeft = 2;
        //Projectile.damage = (int) (player.HeldItem.damage * player.GetDamage(DamageClass.Magic));
    }

    private void UpdateProj()
    {
        Projectile.alpha -= 20;
        if (Projectile.alpha < 0) Projectile.alpha = 0;
        Projectile.light += 0.05f;
        if (Projectile.light > 1.2f) Projectile.light = 1.2f;
        Projectile.scale += 0.01f;
        if (Projectile.scale > 2.5f) Projectile.scale = 2.5f;
        Projectile.Center = player.Center + new Vector2(0, -80f) * Projectile.scale;
        Projectile.rotation = (float) Math.Sin(Timer / 20f) / 7f;
    }

    public override void OnKill(int timeLeft)
    {
        if (!player.dead && Projectile.scale > 1f)
        {
            Projectile projectile = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.position, Vector2.Zero,
                ModContent.ProjectileType<ShadowlightExplosion>(), Projectile.damage, 1f, Projectile.owner);
            projectile.DamageType = DamageClass.Magic;
            projectile.width = projectile.height = (int) (80 * Projectile.scale);
            projectile.usesLocalNPCImmunity = false;
            projectile.timeLeft = 60;
            projectile.Center = new Vector2(Projectile.Hitbox.X, Projectile.Hitbox.Y);
            SoundEngine.PlaySound(SoundID.Item74);
        }
        else
            for (int i = 0; i < 30; i++)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame);
    }

    public override void ModifyDamageHitbox(ref Rectangle hitbox)
    {
        if (Timer >= 15 && Projectile.ai[1] == 1)
        {
            Vector2 vec = Projectile.velocity.SafeNormalize(Vector2.Zero) * 12f * Projectile.scale;
            hitbox.X = (int) (hitbox.X + vec.X);
            hitbox.Y = (int) (hitbox.Y + vec.Y);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Main.instance.LoadProjectile(Projectile.type);
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Vector2 pos = new(Projectile.position.X - Main.screenPosition.X +  (Projectile.width / 2f),
            Projectile.position.Y - Main.screenPosition.Y + (Projectile.height / 2f));
        Rectangle rectangle = texture.Bounds;
        Vector2 drawOrigin = new(texture.Width / 2f, texture.Height / 2f);
        if (Timer >= 60 && Projectile.ai[1] == 1)
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Color color = Color.Purple * Projectile.Opacity * 0.5f;
                color *= (float) (ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) /
                         ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 drawPos = (Projectile.oldPos[i] + Projectile.position * 2) / 3 - Main.screenPosition +
                                  new Vector2( Projectile.width / 2f,  Projectile.height / 2f);
                Main.EntitySpriteDraw(texture, drawPos, rectangle, color, Projectile.rotation, drawOrigin,
                    Projectile.scale, SpriteEffects.None, 0);
            }

        Main.EntitySpriteDraw(texture, pos, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin,
            Projectile.scale, SpriteEffects.None, 0);
        return false;
    }
}