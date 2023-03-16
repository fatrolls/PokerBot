using System;

namespace Bot.Data
{
    public enum Suit
    {
        Hearts,
        Diamods,
        Clubs,
        Spades,
        Error
    }

    public enum Value
    {
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Joker,
        Queen,
        King,
        Ace,
        Error
    }

    internal class Card : IComparable
    {
        // 0 - 12 -> Ace, 2, ..., 10, Joker, Queen, King
        // 0 - 3 -> Hearts Diamods Clubs Spades

        public string shortCard = "Error";

        //You can intizialize the card with the enumerators...
        public Card(Suit suit, Value value)
        {
            Suit = suit;
            Value = value;
        }


        //Or a string according to the recource files
        public Card(string name)
        {
            try
            {
                if (name.Length != 2)
                    throw new ArgumentException();

                shortCard = name;

                var c = name[1];
                int i;

                switch (c)
                {
                    case 'H':
                        i = 0;
                        break;
                    case 'D':
                        i = 1;
                        break;
                    case 'C':
                        i = 2;
                        break;
                    case 'S':
                        i = 3;
                        break;
                    default:
                        throw new ArgumentException();
                }

                Suit = (Suit) i;

                c = name[0];

                switch (c)
                {
                    case '2': //0
                    case '3': //1
                    case '4': //2
                    case '5': //3 
                    case '6': //4
                    case '7': //5
                    case '8': //6
                    case '9': //7
                        i = (int)char.GetNumericValue(c) - 2;
                        break;
                    case 'T':
                        i = 8;
                        break;
                    case 'J':
                        i = 9;
                        break;
                    case 'Q':
                        i = 10;
                        break;
                    case 'K':
                        i = 11;
                        break;
                    case 'A':
                        i = 12;
                        break;
                    default:
                        throw new ArgumentException();
                }

                Value = (Value) i;
            }
            catch (ArgumentException)
            {
                Value = Value.Error;
                Suit = Suit.Error;
            }
        }

        public Value Value { get; }

        public Suit Suit { get; }

        public int CompareTo(object obj)
        {
            var compareCard = (Card) obj;

            if (compareCard.Value > Value)
                return -1;
            if (compareCard.Value < Value)
                return 1;
            return 0;
        }


        public override string ToString()
        {
            return shortCard;
        }
    }
}