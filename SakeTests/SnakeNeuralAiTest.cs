using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Snake.AI;
using Snake.Game;

namespace SakeTests
{
	[TestClass]
	public class SnakeNeuralAiTest
	{
		[TestMethod]
		public void ConvertOutputContinueTest()
		{
			Decision result = Snake.AI.SnakeNeuralAI.ConvertOutputToDecision(new double[]
			{
				1,
				0.5,
				0.8
			});

			Assert.AreEqual(Decision.Continue, result);
		}

		[TestMethod]
		public void ConvertOutputLeftTest()
		{
			Decision result = Snake.AI.SnakeNeuralAI.ConvertOutputToDecision(new double[]
			{
				0.9,
				1,
				0.8
			});

			Assert.AreEqual(Decision.TurnLeft, result);
		}

		[TestMethod]
		public void ConvertOutputRightTest()
		{
			Decision result = Snake.AI.SnakeNeuralAI.ConvertOutputToDecision(new double[]
			{
				0.5,
				0.7,
				1
			});

			Assert.AreEqual(Decision.TurnRight, result);
		}

	}
}
