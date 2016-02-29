using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
    public class SimulateGameBidAI : BaseBidAI
    {

        public SimulateGameBidAI(Player player, GameManager game) : base(player, game)
        {

        }



        protected override int GetHandStrength(Suits trump)
        {
            Player[] CopyPlayers = new Player[]
            {
                new Player("copy0", 0),
                new Player("copy1", 1),
                new Player("copy2", 2),
                new Player("copy3", 3)
            };

            Round simRound;
            do
            {
                simRound = new Round(CopyPlayers);
            } while (simRound.GameCase != Case.UNDECIDED);

            for (int i = 0; i < 4; i++)
            {
                CopyPlayers[i].hand.Cards.Clear();
                foreach (var card in game.Players[i].hand.Cards)
                    CopyPlayers[i].hand.AddCard(card);
            }

            while (simRound.InBiddingPhase)
            {
                if (simRound.CurrentPlayer == CopyPlayers.Where(cp => cp.Number == player.Number).Single())
                {
                    if (simRound.BiddingGetPossibleActions().Contains(Action.ABONDANCE))
                        simRound.BiddingDoAction(Action.ABONDANCE);
                    else
                    {
                        switch (trump)
                        {
                            case Suits.HEARTS:
                                {
                                    simRound.BiddingDoAction(Action.HEARTS);
                                    break;
                                }
                            case Suits.DIAMONDS:
                                {
                                    simRound.BiddingDoAction(Action.DIAMONDS);
                                    break;
                                }
                            case Suits.SPADES:
                                {
                                    simRound.BiddingDoAction(Action.SPADES);
                                    break;
                                }
                            case Suits.CLUBS:
                                {
                                    simRound.BiddingDoAction(Action.CLUBS);
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                    simRound.BiddingDoAction(Action.ASK);
                }
                else
                    simRound.BiddingDoAction(Action.PASS);
            }

            simRound.EndBiddingRound();
            while (simRound.InTrickPhase)
            {
                while (simRound.TrickInProgress)
                    simRound.PlayCard(SimpleGameAI.GetMove(simRound));
                simRound.EndTrick();
            }

            return CopyPlayers.Where(cp => cp.Number == player.Number).Single().Tricks;
        }
    }
}
