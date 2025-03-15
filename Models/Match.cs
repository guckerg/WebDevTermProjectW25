using EventManager.Models;

namespace EventManager.Models
{
    public class Match
    {
        public int MatchID { get; set; }
        public int EventID { get; set; }
        public Event Event { get; set; }

        public AppUser Player1 { get; set; }
        public AppUser Player2 { get; set; }

        //nullable in case no winner yet
        public AppUser? Winner { get; set; }
    }

}
