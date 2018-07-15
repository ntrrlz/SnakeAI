using Snake.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snake.AI
{
	public abstract class SnakeAI
	{
		public Random random = new Random(Guid.NewGuid().GetHashCode());
		

		public void ReSeed()
		{
			random = new Random(Guid.NewGuid().GetHashCode());
		}

		public void ApplyDecision(World world, Decision decision)
		{
			switch(decision)
			{
				case Decision.Continue:
					break;
				case Decision.TurnLeft:
					//world.Score-=10;
					world.Snake.TurnLeft();
					break;
				case Decision.TurnRight:
					//world.Score-=10;
					world.Snake.TurnRight();
					break;
				default:
					throw new Exception("Unknown decision");
			}
		}

		public bool DecisionResultsInDead(World world, Decision decision)
		{
			switch (decision)
			{
				case Decision.Continue:
					return !world.IsClearAhead();
				case Decision.TurnLeft:
					return !world.IsClearLeft();
				case Decision.TurnRight:
					return !world.IsClearLeft();
				default:
					throw new Exception("Unknown decision");
			}
		}

		public abstract void MakeDecision(World world);

		public Decision MakeRandomDecision()
		{
			Array values = Enum.GetValues(typeof(Decision));
			
			return (Decision)values.GetValue(random.Next(values.Length));
		}
	}
}
