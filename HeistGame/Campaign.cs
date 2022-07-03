namespace HeistGame
{
    class Campaign
    {
        public string Name { get; private set; }
        public Level[] Levels { get; private set; }

        public Campaign(string name, Level[] levels)
        {
            Name = name;
            Levels = levels;
        }
    }

    class CampaignConfig
    {
        public string Name { get; set; }
        public string[] LevelFiles { get; set; }

        public CampaignConfig()
        {
            //The deserialization requires a default, parameterless constructor
        }
    }
}
