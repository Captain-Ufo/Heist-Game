namespace HeistGame
{
    class MissionConfig
    {
        public string Name { get; set; }
        public string[] Briefing { get; set; }
        public string[] LevelMap { get; set; }
        public string[] Messages { get; set; }
        public string[] Outro { get; set; }

        public MissionConfig()
        {
            //empty, parameterless constructor required by deserialization
        }
    }
}
