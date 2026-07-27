namespace Investigation
{
    /// <summary>
    /// Placeholder roster blending Alex's and Michelle's storylines under one
    /// shared ID set. Not tied to either story exclusively — the state machine
    /// discussion will decide which subset ships in the first playable slice.
    /// </summary>
    public enum SuspectId
    {
        None,

        // Michelle — The Substitute Teacher
        Mira,
        Wonjin,
        TheStandIn,
        MrsHan,
        Jae,
        MrHan,

        // Alex — Murder in the Old Villa
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
            SuspectId.Mira => "Mira",
            SuspectId.Wonjin => "Wonjin",
            SuspectId.TheStandIn => "The Stand-in",
            SuspectId.MrsHan => "Mrs. Han",
            SuspectId.Jae => "Jae",
            SuspectId.MrHan => "Mr. Han",
            SuspectId.LinY => "Lin-Y",
            SuspectId.Su => "Su",
            SuspectId.Wang => "Wang",
            SuspectId.LinH => "Lin-H",
            SuspectId.Mei => "Mei",
            _ => "Unknown",
        };
    }
}
