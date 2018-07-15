using AForge.Neuro.Learning;
using Snake.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.AI
{
	public class SnakeNeuralAiTrainer
	{
		private static int MoveTowardsScore = 10;
		private static int MoveAwayScore = -15;

		public SnakeNeuralAI SnakeNeuralAI { get; set; }
		public int Score { get; set; }

		public SnakeNeuralAiTrainer(SnakeNeuralAI snakeNeuralAI)
		{
			this.SnakeNeuralAI = snakeNeuralAI;
			this.Score = 0;
		}

		public void Train(World world)
		{
			int limit = 0;
			while (world.Snake.Alive && world.Score > -30 && limit < 25000)
			{
				double startDistance = world.Snake.Location.Distance(world.Food.Location);
				
				SnakeNeuralAI.MakeDecision(world);
				limit++;

				double endDistance = world.Snake.Location.Distance(world.Food.Location);

				if (startDistance > endDistance)
					world.Score += MoveTowardsScore;
				else
					world.Score += MoveAwayScore;
			}

			this.Score = world.Score;// (this.Score + world.Score + Math.Max(this.Score, world.Score) * 10) / 12;
		}

		public void AddResults(ISupervisedLearning superTeacher)
		{
			foreach (var run in SnakeNeuralAI.Runs)
			{
				//superTeacher.RunEpoch(new double[][] { run.Item1 }, new double[][] { run.Item2 });
				superTeacher.Run(run.Item1, run.Item2);
			}
		}
	}
}
