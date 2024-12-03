using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sikanoppa
{
    public class GameData
    {
        public int ID { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public bool IsPlayer1Robot { get; set; }
        public bool IsPlayer2Robot { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public bool IsItPlayersTurn1 { get; set; }
        public int RoundScore { get; set; }

        public GameData(int id, string player1Name, string player2Name)
        {
            ID = id;
            Player1Name = player1Name;
            Player2Name = player2Name;
            IsPlayer1Robot = false;
            IsPlayer2Robot = false;
            Player1Score = 0;  // Use the provided score (or default to 0 if not provided)
            Player2Score = 0;  // Use the provided score (or default to 0 if not provided)
            IsItPlayersTurn1 = true;
        }


        public void UpdateIsItPlayersTurn()
        {
            IsItPlayersTurn1 = !IsItPlayersTurn1;
        }
    }
}
