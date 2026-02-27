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

        [NoTranslate]
        public string backgroundEndogenePath;
        [NoTranslate]
        public string backgroundXenogenePath;
        [NoTranslate]
        public string backgroundArchiteEndogenePath;
        [NoTranslate]
        public string backgroundArchiteXenogenePath;

        public CachedTexture BackgroundTexture(GeneDef gene, GeneType geneType)
        {
            if (!backgroundPath.NullOrEmpty())
            {
                return new CachedTexture(backgroundPath);
            }
            CachedTexture cachedTexture = new(backgroundEndogenePath);
            if (gene.biostatArc == 0)
            {
                switch (geneType)
                {
                    case GeneType.Endogene:
                        cachedTexture = new(backgroundEndogenePath);
                        break;
                    case GeneType.Xenogene:
                        cachedTexture = new(backgroundXenogenePath);
                        break;
                }
            }
            else
            {
                switch (geneType)
                {
                    case GeneType.Endogene:
                        cachedTexture = new(backgroundArchiteEndogenePath);
                        break;
                    case GeneType.Xenogene:
                        cachedTexture = new(backgroundArchiteXenogenePath);
                        break;
                }
            }
            return cachedTexture;
        }


    }

}