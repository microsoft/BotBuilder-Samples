namespace RollerSkillBot
{
    using System;

    [Serializable]
    public class GameData
    {
        public GameData()
        {
            this.Type = "Custom";
        }

        public GameData(string type, int sides, long count, int turns)
        {
            this.Type = type;
            this.Sides = sides;
            this.Count = count;
            this.Turns = turns;
        }

        public string Type { get; set; }

        public int Sides { get; set; }

        public long Count { get; set; }

        public int Turns { get; set; }
    }
}