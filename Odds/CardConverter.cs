using System;
using System.Collections.Generic;
using System.Linq;
using Bot.Data;

namespace Bot.Odds
{
    /// <summary>
    ///     Card for converting one or multiple MyCard-objects into a string that holds the commonly used card description.
    /// </summary>
    internal class CardConverter
    {
        public static string ConvertCards(List<Data.Card> cards)
        {
            var name = cards.Aggregate("", (current, card) => current + (ConvertCard(card) + " "));

            return name.Substring(0, name.Length - 1); //Leaving out the additional space
        }


        private static string ConvertCard(Data.Card card)
        {
            var name = "";

            var value = "";

            switch (card.Value)
            {
                case Value.Two:
                    value = "2";
                    break;
                case Value.Three:
                    value = "3";
                    break;
                case Value.Four:
                    value = "4";
                    break;
                case Value.Five:
                    value = "5";
                    break;
                case Value.Six:
                    value = "6";
                    break;
                case Value.Seven:
                    value = "7";
                    break;
                case Value.Eight:
                    value = "8";
                    break;
                case Value.Nine:
                    value = "9";
                    break;
                case Value.Ten:
                    value = "T";
                    break;
                case Value.Joker:
                    value = "J";
                    break;
                case Value.Queen:
                    value = "Q";
                    break;
                case Value.King:
                    value = "K";
                    break;
                case Value.Ace:
                    value = "A";
                    break;
                case Value.Error:
                    break;
                default:
                    throw new Exception("Error Karte");
            }

            var suit = "";

            switch (card.Suit)
            {
                case Suit.Diamods:
                    suit = "d";
                    break;
                case Suit.Hearts:
                    suit = "h";
                    break;
                case Suit.Spades:
                    suit = "s";
                    break;
                case Suit.Clubs:
                    suit = "c";
                    break;
                case Suit.Error:
                    break;
                default:
                    throw new Exception("Error Karte");
            }

            name = value + suit;

            return name;
        }
    }
}