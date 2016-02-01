using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist
{
    public class TestControllerFactory
    {
        public static WhistController generate() {
            Player player1 = new Player("Player 1");
            Player player2 = new Player("Player 2");
            Player player3 = new Player("Player 3");
            Player player4 = new Player("Player 4");

            Team team1 = new Team(new Player[] { player1, player3 },"Team 1");
            Team team2 = new Team(new Player[] { player2, player4 },"Team 2");

            return new WhistController(new Player[] { player1, player2, player3, player4 }, new Team[] { team1, team2 }, new StandardDealer(), new SimpleScoreMechanisme(), new StandardReferee(), new DeckCollection());
        }
    }
}
