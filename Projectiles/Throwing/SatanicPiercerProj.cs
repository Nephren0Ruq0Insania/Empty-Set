﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EmptySet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;


namespace EmptySet.Projectiles.Throwing
{	
    public class SatanicPiercerProj : ModProjectile
    {
		public bool IsStickingToTarget
		{
			get => Projectile.ai[0] == 1f;
			set => Projectile.ai[0] = value ? 1f : 0f;
		}

		// Index of the current target
		public int TargetWhoAmI
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}
		private readonly Point[] _stickingJavelins = new Point[3]; // The point array holding for sticking javelins
		public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Satanic Piercer");
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.timeLeft = 600;
			Projectile.friendly = true;
			Projectile.penetrate = 3;
			Projectile.hide = true;
		}
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			if (IsStickingToTarget)
				if (TargetWhoAmI >= 0 && TargetWhoAmI < 200 && Main.npc[TargetWhoAmI].active)
				{
					if (Main.npc[TargetWhoAmI].behindTiles)
						behindNPCsAndTiles.Add(index);
					else
						behindNPCs.Add(index);
					return;

				}
			behindProjectiles.Add(index);
		}
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {			
			width = height = 10;
			return true;
		}
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
				targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
			return projHitbox.Intersects(targetHitbox);
		}
		public override void OnKill(int timeLeft)
		{
			if (IsStickingToTarget)
				Projectile.NewProjectileDirect(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SatanicExplosion>(), 10, 1f, Projectile.owner);
		}
		public override void AI()
		{
			UpdateAlpha();
			if (IsStickingToTarget) StickyAI();
			else NormalAI();
		}
		private void UpdateAlpha()
        {               
			Projectile.alpha -= 25;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
        }
		private void NormalAI()
		{
			Projectile.ai[1]++;
			if (Projectile.ai[1] >= 40)
			{
				Projectile.ai[1] = 40;
				Projectile.velocity.X *= 0.98f;
				Projectile.velocity.Y += 0.35f;
			}
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, DustID.Demonite,
					Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 200, Scale: 1.2f);
				dust.velocity += Projectile.velocity * 0.3f;
				dust.velocity *= 0.2f;
			}
			Projectile.localAI[1] = Projectile.damage;
		}
		private void StickyAI()
		{
			Projectile.damage = 0;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.localAI[0]++;
			int projTargetIndex = TargetWhoAmI;
			if (Projectile.localAI[0] >= 180 || projTargetIndex < 0 || projTargetIndex >= 200)
				Projectile.Kill();
			else if (Main.npc[projTargetIndex].active && !Main.npc[projTargetIndex].dontTakeDamage)
			{
				Projectile.Center = Main.npc[projTargetIndex].Center - Projectile.velocity * 2f;
				Projectile.gfxOffY = Main.npc[projTargetIndex].gfxOffY;
			}
			else
				Projectile.Kill();
		}
		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			IsStickingToTarget = true;
			TargetWhoAmI = target.whoAmI;
			Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
			Projectile.netUpdate = true;
			UpdateStickyJavelins(target);
		}
		private void UpdateStickyJavelins(NPC target)
		{
			int currentJavelinIndex = 0;
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile currentProjectile = Main.projectile[i];
				if (i != Projectile.whoAmI && currentProjectile.active && currentProjectile.owner == Main.myPlayer && currentProjectile.type == Projectile.type && currentProjectile.ModProjectile is SatanicPiercerProj javelinProjectile && javelinProjectile.IsStickingToTarget && javelinProjectile.TargetWhoAmI == target.whoAmI)
				{
					_stickingJavelins[currentJavelinIndex++] = new Point(i, currentProjectile.timeLeft);
					if (currentJavelinIndex >= _stickingJavelins.Length)
						break;
				}
			}
			if (currentJavelinIndex >= 3)
			{
				int oldJavelinIndex = 0;
				for (int i = 1; i < 3; i++)
					if (_stickingJavelins[i].Y < _stickingJavelins[oldJavelinIndex].Y)
						oldJavelinIndex = i;
				Main.projectile[_stickingJavelins[oldJavelinIndex].X].Kill();
			}
		}
    }
}
