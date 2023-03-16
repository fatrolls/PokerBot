using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot
{
    public class Positions
    {
        public static Point _moneyCheckPoint = new Point(474, 528);
        public static Size _moneySize = new Size(103, 23);

        public static Size _potSize = new Size(150, 24);

        public static Point _relativeMoneyPos = new Point(952, 1073);
        public static Point _relativeMoneyPosRight = new Point(850, 1073);
        public static Point _relativePotPos = new Point(425, 227);

        //Cards
        public static Size _cropSize = new Size(60, 86);
        public static Point _relativeCheckCountCardPos = new Point(10, 50);
        public static Point _relativeCheckerCardPos = new Point(13, 38);
        public static Point _relativePlayerCardPos = new Point(437, 445);
        public static Point _relativeTableCardPos = new Point(342, 251);

        public const int PlayerCardMargin = 63; //check
        public const int TableCardMargin = 63;

        //Seats
        public static List<Point> _relativePlayerPoints = new List<Point>
        {
            new Point(440, 467), //my player
            new Point(812, 343),
            new Point(781, 133),
            new Point(439, 66),
            new Point(98, 132),
            new Point(68, 343),
        };

        //My turn
        public static Point _relativeMyCallPos = new Point(494, 637);
        public static Size _MyCallSize = new Size(159, 67);
        // ToDo: Make detection dynamic, not hardcoded
        public static Point _relativeMyTurnPos = new Point(661, 640);
        public static Size _MyTurnSize = new Size(157, 66);

               
        public static Point _relativeCallPos = new Point(661, 640);
        public static Size _callSize = new Size(159, 64);

        public static Point _relativeRaisePos = new Point(827, 640);
        public static Size _RaiseSize = new Size(159, 64);
    }
}
