namespace Investigation
{
    /// <summary>Suspects in Murder in the Old Villa.</summary>
    public enum SuspectId
    {
        None = 0,
        Amy = 1,
        Coco = 2,
        Dean = 3,
        Ben = 4,
        Ella = 5,
        Felix = 6,
    }

    public static class SuspectNames
    {
        public static string DisplayName(SuspectId id) => id switch
        {
            SuspectId.Amy => "Amy",
            SuspectId.Ben => "Ben",
            SuspectId.Coco => "Coco",
            SuspectId.Dean => "Dean",
            SuspectId.Ella => "Ella",
            SuspectId.Felix => "Felix",
            _ => "Unknown",
        };
    }
}
