using System.Collections.Generic;

namespace ScoreCard.Models
{
    public class Player
    {
        public string Name { get; set; }
        public List<int?> Scores { get; set; } = new List<int?>();

        public Player(string name, int rounds)
        {
            Name = name;

            for (int i = 0; i < rounds; i++)
            {
                Scores.Add(null);
            }
        }

        public Player()
        {
        }
    }
}