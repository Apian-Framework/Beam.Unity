using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeamBackend
{
    public class Player
    {
        public string ID { get; private set;}    
        public string ScreenName { get; private set;}
        public Team Team {get; private set;}
        public int Score { get; set;}
        public bool IsLocal { get; private set; }

        public static readonly int kStartScore = 2000;

        public Player(string id, string name, Team t, bool isLocal = false)
        { 
            ID = id;
            ScreenName = name;
            Team = t;
            Score = kStartScore;
            IsLocal = isLocal;
        }

    }

    public static class DemoPlayerData
    {
        private static readonly List<string> firstNames = new List<string>() {
            "Alice", "Bob", "Carol", "Don", "Evan", "Frank", "Gayle", "Herb",
            "Inez", "Jim", "Kayla", "Lara", "Mike", "Noel", "Orlando", "Paul",
            "Quentin", "Rachel", "Sam", "Terry", "Umberto", "Vera", "Will", "Xavier",
            "Yasmin", "Zack"
        };

        private static readonly List<string> lastNames = new List<string>() {
            "A.", "B.", "C.", "D.", "E.", "F.", "G.", "H.",
            "I.", "J.", "K.", "L.", "M.", "N.", "O.", "P.",
            "Q.", "R.", "S.", "T.", "U.", "V.", "W.", "X.",
            "Y.", "Z."
        };

        public static Player CreatePlayer(bool isLocal = false) {
            string name = string.Format("{0} {1}",
                firstNames[(int)Random.Range(0,firstNames.Count)],
                lastNames[(int)Random.Range(0,lastNames.Count)] );

            string id = string.Format("{0:X8}", name.GetHashCode()); // Just making up an ID-looking string

            Team team = Team.teamData[(int)Random.Range(0,Team.teamData.Count)];

            return new Player(id, name, team, isLocal);

        }
    }
}
