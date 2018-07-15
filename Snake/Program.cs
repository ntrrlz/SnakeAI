using System.Linq;
using Snake.AI;
using Snake.Game;
using Snake.Renderer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AForge.Neuro;
using AForge.Neuro.Learning;

namespace Snake
{
	class Program
	{
		private static int trainWorldWidth = 20;
		private static int trainWorldHeigh = 20;
		private static int worldWidth = 40;
		private static int worldHeigh = 20;
		private static int foodPoints = 100;
		private static long learningIteration = 10000000000;
		private static int learningPeers = 150;
		private static int learningPeersRandom = 350;
		//private static int numberOfLearningPaths = 5;
		private static int learningPeersToUseForSuper = 20;
		private static long startMutantsAfterNumberOfRuns = 100000000000 /(learningPeers + learningPeersRandom);


		public static SnakeNeuralAI TrainNeuralAI()
		{
			var SuperNetwork = SnakeNeuralAI.CreateNetwork();

			//Continue training AI
			//SuperNetwork = (ActivationNetwork)ActivationNetwork.Load(@"c:\work\Snake\Snake\AI\TrainedNetwork");
			
			var SuperTeacher = new PerceptronLearning(SuperNetwork) { LearningRate = 0.1 };
			int bestScore = 0;

			IList<SnakeNeuralAiTrainer> bestSnakeAI = new List<SnakeNeuralAiTrainer>();

			for (long i = 0; i < learningIteration / (learningPeers + learningPeersRandom); i++)
			{
				bestSnakeAI.Clear();

				for (int j = 0; j < learningPeers; j++)
				{
					SnakeNeuralAI snakeAI;

					//SnakeNeuralAiTrainer parent = trainers[j % numberOfLearningPaths];
					snakeAI = SnakeNeuralAI.Clone(SuperNetwork, true);

					if (snakeAI.random.Next(10000) < 3)
						snakeAI.isMutant = true;

					bestSnakeAI.Add(new SnakeNeuralAiTrainer(snakeAI));

				}

				for (int j = 0; j < learningPeersRandom; j++)
				{
					SnakeNeuralAI snakeAI = SnakeNeuralAI.Clone(SuperNetwork, true);
					if (i < startMutantsAfterNumberOfRuns)
						snakeAI.Network.Randomize();
					else
						snakeAI.isMutant = true;

					bestSnakeAI.Add(new SnakeNeuralAiTrainer(snakeAI));
				}

				Parallel.ForEach(bestSnakeAI, new ParallelOptions(), (trainer) =>
				{
					World world = new World(trainWorldWidth, trainWorldHeigh, foodPoints);
					trainer.Train(world);
				});

				bestSnakeAI = bestSnakeAI.OrderByDescending(x => x.Score).Take(learningPeersToUseForSuper).ToList();

				foreach (var bestOfTheBest in bestSnakeAI)
				{
					if (bestOfTheBest.Score > bestScore * 0.1)
						bestOfTheBest.AddResults(SuperTeacher);
				}

				bestScore = Math.Max(bestSnakeAI.Max(x => x.Score), bestScore);

				Console.CursorVisible = false;
				Console.SetCursorPosition(0, 0);
				Console.WriteLine("Current average score: " + Math.Round(bestSnakeAI.Average(x => x.Score)) + "    ");
				Console.WriteLine("Current best score: " + bestSnakeAI.Max(x => x.Score) + "     ");
				Console.Write("Learning progress: " + Math.Round((100m / (learningIteration / (learningPeers + learningPeersRandom))) * i) + "% (" + i * (learningPeers + learningPeersRandom) + ")");
				if (i > startMutantsAfterNumberOfRuns)
					Console.WriteLine("   MUTANTS!!!");
				else
					Console.WriteLine();

				if (Console.KeyAvailable)
				{
					Console.ReadKey();
					break;
				}
			}
			SuperNetwork.Save(@"c:\temp\Network" + bestScore);
			return SnakeNeuralAI.Clone(SuperNetwork, false);
		}

		static void Main(string[] args)
		{
			//SnakeAI snakeAI = new SnakeFakeAI();
			//SnakeNeuralAI snakeAI = TrainNeuralAI();
			
			SnakeNeuralAI snakeAI = new SnakeNeuralAI()
			{
				Network = ActivationNetwork.Load(@"c:\work\Snake\Snake\AI\TrainedNetwork")
			};


			while (true)
			{
				World world = new World(worldWidth, worldHeigh, foodPoints);

				while (world.Snake.Alive)
				{
					snakeAI.MakeDecision(world);

					ConsoleRenderer.Render(world);
					Thread.Sleep(80);


					if (Console.KeyAvailable)
					{
						Console.ReadKey();
						break;
					}
				}

				ConsoleRenderer.Render(world);
				Thread.Sleep(2000);
			}
		}
	}
}
