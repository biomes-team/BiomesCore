using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BiomesCore.Defs
{
	public class BMT_GeneDef : GeneDef
	{

        [NoTranslate]
        public string backgroundPath = null;

		private CachedTexture cachedBackgroundPath;
		public CachedTexture GeneralBackground
		{
			get
			{
				if (cachedBackgroundPath == null)
				{
					cachedBackgroundPath = new(backgroundPath);
				}
				return cachedBackgroundPath;
			}
		}

		[NoTranslate]
        public string backgroundEndogenePath;
        [NoTranslate]
        public string backgroundXenogenePath;
        [NoTranslate]
        public string backgroundArchiteEndogenePath;
        [NoTranslate]
        public string backgroundArchiteXenogenePath;

        private CachedTexture cachedBackgroundEndogene;
		public CachedTexture BackgroundEndogene
		{
			get
			{
                if (cachedBackgroundEndogene == null)
                {
                    cachedBackgroundEndogene = new(backgroundEndogenePath);
				}
				return cachedBackgroundEndogene;
			}
		}

		private CachedTexture cachedBackgroundXenogene;
		public CachedTexture BackgroundXenogene
		{
			get
			{
				if (cachedBackgroundXenogene == null)
				{
					cachedBackgroundXenogene = new(backgroundXenogenePath);
				}
				return cachedBackgroundXenogene;
			}
		}

		private CachedTexture cachedBackgroundArchiteEndogene;
		public CachedTexture BackgroundArchiteEndogene
		{
			get
			{
				if (cachedBackgroundArchiteEndogene == null)
				{
					cachedBackgroundArchiteEndogene = new(backgroundArchiteEndogenePath);
				}
				return cachedBackgroundArchiteEndogene;
			}
		}

		private CachedTexture cachedBackgroundArchiteXenogene;
		public CachedTexture BackgroundArchiteXenogene
		{
			get
			{
				if (cachedBackgroundArchiteXenogene == null)
				{
					cachedBackgroundArchiteXenogene = new(backgroundArchiteXenogenePath);
				}
				return cachedBackgroundArchiteXenogene;
			}
		}

		public CachedTexture BackgroundTexture(GeneDef gene, GeneType geneType)
        {
            if (!backgroundPath.NullOrEmpty())
            {
                return GeneralBackground;
            }
            CachedTexture cachedTexture = GeneralBackground;
            if (gene.biostatArc == 0)
            {
                switch (geneType)
                {
                    case GeneType.Endogene:
                        cachedTexture = BackgroundEndogene;
                        break;
                    case GeneType.Xenogene:
                        cachedTexture = BackgroundXenogene;
                        break;
                }
            }
            else
            {
                switch (geneType)
                {
                    case GeneType.Endogene:
                        cachedTexture = BackgroundArchiteEndogene;
                        break;
                    case GeneType.Xenogene:
                        cachedTexture = BackgroundArchiteXenogene;
                        break;
                }
            }
            return cachedTexture;
        }

		public override void ResolveReferences()
		{
			base.ResolveReferences();
			if (backgroundPath.NullOrEmpty() && backgroundEndogenePath.NullOrEmpty() && backgroundXenogenePath.NullOrEmpty() && backgroundArchiteXenogenePath.NullOrEmpty() && backgroundArchiteEndogenePath.NullOrEmpty())
			{
				backgroundPath = "UI/Icons/Genes/Gene_BodyFat";
				Log.Error("Error in def: " + defName + ". Please set any backgroundPath.");
			}
		}

	}

}