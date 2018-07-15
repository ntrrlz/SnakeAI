using System;
using System.Collections.Generic;
using System.Text;

namespace Snake.Game
{
    public class Food
    {
		public int Points { get; set; }
		public Location Location { get; set; }

		public Food(int points)
		{
			this.Points = points;
		}
	}
}
