using System;
using System.Threading;
using Bot.Control;
using Bot.Detection;
using Bot.Odds;

namespace Bot
{
    internal class Player
    {
        private readonly BotController _control;
        private readonly Eye _eye;
        private readonly int _id;


        public Player(IntPtr pokerHandle, int id)
        {
            _eye = new Eye(pokerHandle);
            _control = new BotController(pokerHandle);
            _id = id;
        }

        public void Play()
        {
            while (true)
            {
                while (!_eye.MyTurn())
                    Thread.Sleep(2000);

                Console.WriteLine("My Money: " + _eye.GetMoney() + " (un-used in bot strange).");
                Console.WriteLine("Call At: " + _eye.GetMinimumCall() + " RaiseMinimal: " + _eye.GetMinimalRaise());

                var playerCards = _eye.GetPlayerCards();
                var tableCards = _eye.GetTableCards();

                string TableCardsString = "";
                foreach(Bot.Data.Card card in tableCards)
                    TableCardsString += card.ToString() + " ";
                string PlayerCardsString = "";
                foreach (Bot.Data.Card card in playerCards)
                    PlayerCardsString += card.ToString() + " ";
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine("Table Cards: " + TableCardsString);
                Console.WriteLine("Player Cards: " + PlayerCardsString);

                var hand = new MyHand(playerCards, tableCards);
                Console.WriteLine($"Player Hand: {hand.BestCombination}");


                var playerCount = _eye.GetPlayerCount();

                double minimumCallAmount = _eye.GetMinimumCall();
                double potAmount = _eye.GetPotAmount();


                if (!_eye.OnlyCallOrFold() && minimumCallAmount == 0)
                {
                    _control.Check();
                    Console.WriteLine("Checking");
                }
                else
                {
                    var potOdds = minimumCallAmount / (minimumCallAmount + potAmount) * 100;

                    double oddsOffset;

                    if (tableCards.Count == 0)
                        oddsOffset = (110 - minimumCallAmount) / 100;
                    else
                        oddsOffset = 0;


                    var winOdds = OddsCalculator.CalculateOdds(playerCards, tableCards, playerCount);


                    if (oddsOffset > 0)
                    {
                        winOdds *= 1 + oddsOffset;
                        Console.WriteLine($"Increasing Win Odds by {oddsOffset * 100:0,##}%");
                    }


                    Console.WriteLine($"Player {_id}: Pot Odds: {potOdds} | Odds of winning {winOdds}");

                    if (winOdds < potOdds)
                    {
                        Console.WriteLine("Folding");
                        _control.Fold();
                    }
                    else
                    {
                        if (_eye.OnlyCallOrFold())
                        {
                            Console.WriteLine("Raising");
                            _control.CallAll();
                        }
                        else
                        {
                            Console.WriteLine("Calling");
                            _control.Check();
                        }
                    }
                }

                Thread.Sleep(3000);
            }
        }
    }
}