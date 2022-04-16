using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;


namespace UltimateScaling
{
	// Need one class that extends "Mod"
	public class UltimateScaling : Mod
	{
	}

	// Allows classes to save and share the variables that are made here
	public static class ShowDamage
	{
		private static float boostedDmg = 0f;
		public static float BoostedDmg
		{
			get { return boostedDmg; }
			set { boostedDmg = value; }
		}

		private static bool isBoosted;
		public static bool IsBoosted
        {
			get { return isBoosted; }
			set { isBoosted = value; }
        }
	}

	//Extends "ModPlayer" in order to use ModifyWeaponDamage
	public class BoostDamage : ModPlayer
	{
		// Send the item to be checked if it can be boosted
		public override void ModifyWeaponDamage(Item item, ref StatModifier damage, ref float flat)
		{
			flat += BoostDmg(item);
		}

		// Check the sent item to determine total boss kills and if it falls under DPS value to boost
		private static float BoostDmg(Item item)
		{
			List<bool> bosses = new()
			{
				NPC.downedSlimeKing,
				NPC.downedBoss1,
				NPC.downedBoss2,
				NPC.downedBoss3,
				NPC.downedQueenBee,
				NPC.downedMechBoss1,
				NPC.downedMechBoss2,
				NPC.downedMechBoss3,
				NPC.downedQueenSlime,
				NPC.downedFishron,
				NPC.downedPlantBoss,
				NPC.downedGolemBoss,
				NPC.downedAncientCultist,
				NPC.downedEmpressOfLight,
				NPC.downedMoonlord
			};

			float boost = 0.5f;
			int total = 0;
			int max = bosses.Count;


			// Get all bosses that have been downed and add to the 'total'
			foreach (bool downed in bosses)
			{
				if (downed)
				{
					total++;
				}
			}

			// Exponentially add damage onto the weapon based on world bosses killed
			for (int i = 0; i < total; i++)
			{
				boost *= 1.125f;
			}

			// Calculate the DPS after adding the boost and if it "outscales" the world progress
			if ((item.damage * boost) * (60 / item.useTime) >= Math.Pow(total, 3) / 1.75)
			{
				ShowDamage.IsBoosted = false;
				return 0f;
			}

			ShowDamage.BoostedDmg = item.damage * boost;
			ShowDamage.IsBoosted = true;
			return item.damage * boost;
		}
	}

	// Add Tooltips to show current boost of weapons and add a simple DPS line
	public class ShowBoostedDamage : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			int dmg = 0;
			float dps = item.damage * (60 / item.useTime);

			if (ShowDamage.IsBoosted)
            {
				dmg = (int)ShowDamage.BoostedDmg;
				dps = (dmg + item.damage) * (60 / item.useTime);
			}
			
			if (item.damage > 0)
			{
					var curBoost = new TooltipLine(Mod, "Boost", "[Boost] +" + $"{dmg} Damage");
					var curDPS = new TooltipLine(Mod, "DPS", "[DPS] " + $"{dps}/s");
					//curBoost.overrideColor = Color.Yellow;
					//curDPS.overrideColor = Color.Yellow;
					tooltips.Add(curBoost);
					tooltips.Add(curDPS);
			}
		}
	}
}