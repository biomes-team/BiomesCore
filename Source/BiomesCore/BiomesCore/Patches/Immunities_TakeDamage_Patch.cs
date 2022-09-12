using RimWorld;
using Verse;
using HarmonyLib;

namespace BiomesCore.Patches
{
	[HarmonyPatch(typeof(Thing), "TakeDamage")]
	public static class DI_TakeDamage_Patch
	{
		[HarmonyPrefix]
		public static bool DI_TakeDamage(Thing __instance, ref DamageWorker.DamageResult __result, DamageInfo dinfo)
		{
			if (__instance is Building || __instance.Destroyed)
			{
				return true;
			}

			if (!__instance.def.HasComp(typeof(CompDamageImmunities)))
			{
				return true;
			}

			if (__instance.TryGetComp<CompDamageImmunities>().Props.damageDefs.Contains(dinfo.Def))
			{
				var props = __instance.TryGetComp<CompDamageImmunities>().Props;
				__result = new DamageWorker.DamageResult();
				if (props.throwText)
					MoteMaker.ThrowText(__instance.Position.ToVector3(), __instance.Map, "DI_Immune".Translate(dinfo.Def.label), props.textColor, props.textDuration);

				return false;
			}

			return true;
		}
	}
}
