using DAM.Domain.Entities;

namespace DAM.Domain.JsonData
{
    public class AutoMappingChannelEnemyCountData
    {
        public int? EnemyCount { get; set; }
        public ScreenPostType? Type { get; set; }

        public string channelid { get; set; }
    }
    public class AutoMappingChannelEnemyCountConfig
    {

        public string nodefchannelenemy { get; set; }
        public string defchannelenemy1 { get; set; }
        public string defchannelenemy2 { get; set; }
        public string defchannelenemy3 { get; set; }
        public string defchannelenemy4 { get; set; }
        public string defchannelenemy5 { get; set; }
        public string atkchannelenemy1 { get; set; }
        public string atkchannelenemy2 { get; set; }
        public string atkchannelenemy3 { get; set; }
        public string atkchannelenemy4 { get; set; }
        public string atkchannelenemy5 { get; set; }
        public List<AutoMappingChannelEnemyCountData> ToMappings()
        {
            var configs = new List<AutoMappingChannelEnemyCountData>()
            {
                new AutoMappingChannelEnemyCountData()
                {
                    channelid = nodefchannelenemy,
                    Type = ScreenPostType.Attack,
                    EnemyCount = 0
                },
                new AutoMappingChannelEnemyCountData()
                {
                    channelid = defchannelenemy1,
                    Type = ScreenPostType.Defense,
                    EnemyCount = 1
                },
                 new AutoMappingChannelEnemyCountData()
                {
                    channelid = defchannelenemy2,
                    Type = ScreenPostType.Defense,
                    EnemyCount = 2
                },
                new AutoMappingChannelEnemyCountData()
                {
                    channelid = defchannelenemy3,
                    Type = ScreenPostType.Defense,
                    EnemyCount = 3
                },
                new AutoMappingChannelEnemyCountData()
                {
                    channelid = defchannelenemy4,
                    Type = ScreenPostType.Defense,
                    EnemyCount = 4
                },
                new AutoMappingChannelEnemyCountData()
                {
                    channelid = defchannelenemy5,
                    Type = ScreenPostType.Defense,
                    EnemyCount = 5
                },
                new AutoMappingChannelEnemyCountData()
                {
                    channelid = atkchannelenemy1,
                    Type = ScreenPostType.Attack,
                    EnemyCount = 1
                },
                new AutoMappingChannelEnemyCountData()
                {
                    channelid = atkchannelenemy2,
                    Type = ScreenPostType.Attack,
                    EnemyCount = 2
                },
                new AutoMappingChannelEnemyCountData()
                {
                    channelid = atkchannelenemy3,
                    Type = ScreenPostType.Attack,
                    EnemyCount = 3
                },
                new AutoMappingChannelEnemyCountData()
                {
                    channelid = atkchannelenemy4,
                    Type = ScreenPostType.Attack,
                    EnemyCount = 4
                },
                 new AutoMappingChannelEnemyCountData()
                {
                    channelid = atkchannelenemy5,
                    Type = ScreenPostType.Attack,
                    EnemyCount = 5
                }
            };

            return configs;
        }

        public AutoMappingChannelEnemyCountData GetCountFromChannel(string channelid)
        {
            var result = new AutoMappingChannelEnemyCountData();
            if (nodefchannelenemy == channelid)
            {
                result.Type = ScreenPostType.Attack;
                result.EnemyCount = 0;
                return result;
            }
            if (defchannelenemy1 == channelid)
            {
                result.Type = ScreenPostType.Defense;
                result.EnemyCount = 1;
                return result;
            }
            if (defchannelenemy2 == channelid)
            {
                result.Type = ScreenPostType.Defense;
                result.EnemyCount = 2;
                return result;
            }
            if (defchannelenemy3 == channelid)
            {
                result.Type = ScreenPostType.Defense;
                result.EnemyCount = 3;
                return result;
            }
            if (defchannelenemy4 == channelid)
            {
                result.Type = ScreenPostType.Defense;
                result.EnemyCount = 4;
                return result;
            }
            if (defchannelenemy5 == channelid)
            {
                result.Type = ScreenPostType.Defense;
                result.EnemyCount = 5;
                return result;
            }
            if (atkchannelenemy1 == channelid)
            {
                result.Type = ScreenPostType.Attack;
                result.EnemyCount = 1;
                return result;
            }
            if (atkchannelenemy2 == channelid)
            {
                result.Type = ScreenPostType.Attack;
                result.EnemyCount = 2;
                return result;
            }
            if (atkchannelenemy3 == channelid)
            {
                result.Type = ScreenPostType.Attack;
                result.EnemyCount = 3;
                return result;
            }
            if (atkchannelenemy4 == channelid)
            {
                result.Type = ScreenPostType.Attack;
                result.EnemyCount = 4;
                return result;
            }
            if (atkchannelenemy5 == channelid)
            {
                result.Type = ScreenPostType.Attack;
                result.EnemyCount = 5;
                return result;
            }
            return null;
        }
    }
}
