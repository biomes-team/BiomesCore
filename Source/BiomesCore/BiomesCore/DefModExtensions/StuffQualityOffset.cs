using RimWorld;
using Verse;

namespace BiomesCore.DefModExtensions
{
    public class StuffQualityOffset : DefModExtension
    {
        public int qualityOffset;

        public QualityCategory minQuality = QualityCategory.Awful;
        public QualityCategory maxQuality = QualityCategory.Legendary;
    }
}
