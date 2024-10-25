namespace DAM.Tool.DataMigrations.Constants
{
    public static class DiscordMigrationConsts
    {
        public const ulong SPAM_HELLMINA_SERVER = 1189700553635278969;
        //Key is ChannelId, value is messageid
        public static Dictionary<ulong, ulong> SPAM_HELLMINA_ChannelMessageMapping = new Dictionary<ulong, ulong>()
        {
            { 1197021989370609734,1200500517953093672 },//def v1
            { 1197022022732091463,1200500529445474376 },//def v2
            { 1197022049814712370,1200500542611411075 },//def v3
            { 1197022066713567282,1200500553575317554 },//def v4
            { 1197022083855699998,1200500565101265037 },//def v5
            { 1197026950896701582,1201678286376415334 },//No def , Atk
            { 1197022118982991893,1201678259943919626 },//Atk , v1
        };
    }
}
