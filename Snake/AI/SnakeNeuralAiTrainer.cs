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
			int noChangeScore = int.MinValue;
			int noChangeCounter = 0;
			while (world.Snake.Alive && 
				(world.Score*2 > limit || limit < 100) &&  //If the snake is running in circles, save cpu cycles and abort run
				noChangeCounter < 255 && //No food picked in 255 cycles? Running in cycles..:(
				limit < 1000000) //Limit runtime
			{
				if (world.Score == noChangeScore)
				{
					noChangeCounter++;
				}
				else
				{
					noChangeScore = world.Score;
					noChangeCounter = 0;
				}
				SnakeNeuralAI.MakeDecision(world);
				limit++;
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
