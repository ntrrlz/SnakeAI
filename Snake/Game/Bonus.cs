using System;
using System.Collections.Generic;
using System.Text;

namespace Snake.Game
{
	public class Bonus
	{
		public int ActiveTick { get; set; }
		public bool Active { get; set; }
		public Location Location { get; set; }

		public void Update()
		{
			ActiveTick++;

			if (ActiveTick > 500)
				Active = true;
		}
	}
}
