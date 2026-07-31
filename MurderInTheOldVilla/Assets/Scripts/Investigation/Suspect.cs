namespace Investigation
{
    /// <summary>Suspects in Murder in the Old Villa.</summary>
    public enum SuspectId
    {
        None,

        LinY,
        Su,
        Wang,
        LinH,
        Mei,
    }

    public static class SuspectNames
    {
        public static string DisplayName(SuspectId id) => id switch
        {
            SuspectId.LinY => "Lin-Y",
            SuspectId.Su => "Su",
            SuspectId.Wang => "Wang",
            SuspectId.LinH => "Lin-H",
            SuspectId.Mei => "Mei",
            _ => "Unknown",
        };
    }
}
