namespace BiomesCore.Patches
{
    //[HarmonyPatch(typeof(PawnGraphicSet), nameof(PawnGraphicSet.ResolveAllGraphics))]
    //static class AlternativePawnGraphics
    //{
    //    public static void Postfix(Pawn ___pawn, Graphic ___nakedGraphic)
    //    {
    //        PawnGraphicSet graphics = new PawnGraphicSet(___pawn);
    //        graphics.ClearCache();
    //        PawnKindDef pawnDef = ___pawn.kindDef;
    //        Biomes_AnimalControl animalControl = pawnDef.GetModExtension<Biomes_AnimalControl>();
    //        string path = ___pawn.story.bodyType.bodyNakedGraphicPath;
    //        //foreach (string altPath in animalControl.biomesAlternateGraphics)
    //        //{
    //        //    path = path + 2;
    //        //}
    //        //Log.Error("path = " + path);
    //        //___nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.CutoutSkin, Vector2.one, ___pawn.story.SkinColor);
    //        //Rand.PopState();
    //    }
    //}
}
