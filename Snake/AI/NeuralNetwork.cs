using System;
using System.Collections.Generic;
using System.IO;

namespace Snake.AI
{
	[Serializable]
	public class NeuralNetwork
	{
		private int[] layers; //layers
		private double[][] neurons; //neuron matix
		private double[][][] weights; //weight matrix
		private Random random = new Random(Guid.NewGuid().GetHashCode());

		public NeuralNetwork()
		{

		}
		/// <summary>
		/// Initilizes and neural network with random weights
		/// </summary>
		/// <param name="layers">layers to the neural network</param>
		public NeuralNetwork(int[] layers)
		{
			//deep copy of layers of this network 
			this.layers = new int[layers.Length];
			for (int i = 0; i < layers.Length; i++)
			{
				this.layers[i] = layers[i];
			}

			//generate matrix
			InitNeurons();
			InitWeights();
		}

		/// <summary>
		/// Deep copy constructor 
		/// </summary>
		/// <param name="copyNetwork">Network to deep copy</param>
		public NeuralNetwork(NeuralNetwork copyNetwork)
		{
			this.layers = new int[copyNetwork.layers.Length];
			for (int i = 0; i < copyNetwork.layers.Length; i++)
			{
				this.layers[i] = copyNetwork.layers[i];
			}

			InitNeurons();
			InitWeights();
			CopyWeights(copyNetwork.weights);
		}

		private void CopyWeights(double[][][] copyWeights)
		{
			for (int i = 0; i < weights.Length; i++)
			{
				for (int j = 0; j < weights[i].Length; j++)
				{
					for (int k = 0; k < weights[i][j].Length; k++)
					{
						weights[i][j][k] = copyWeights[i][j][k];
					}
				}
			}
		}

		/// <summary>
		/// Create neuron matrix
		/// </summary>
		private void InitNeurons()
		{
			//Neuron Initilization
			List<double[]> neuronsList = new List<double[]>();

			for (int i = 0; i < layers.Length; i++) //run through all layers
			{
				neuronsList.Add(new double[layers[i]]); //add layer to neuron list
			}

			neurons = neuronsList.ToArray(); //convert list to array
		}

		/// <summary>
		/// Create weights matrix.
		/// </summary>
		private void InitWeights()
		{

			List<double[][]> weightsList = new List<double[][]>(); //weights list which will later will converted into a weights 3D array

			//itterate over all neurons that have a weight connection
			for (int i = 1; i < layers.Length; i++)
			{
				List<double[]> layerWeightsList = new List<double[]>(); //layer weight list for this current layer (will be converted to 2D array)

				int neuronsInPreviousLayer = layers[i - 1];

				//itterate over all neurons in this current layer
				for (int j = 0; j < neurons[i].Length; j++)
				{
					double[] neuronWeights = new double[neuronsInPreviousLayer]; //neruons weights

					//itterate over all neurons in the previous layer and set the weights randomly between 0.5f and -0.5
					for (int k = 0; k < neuronsInPreviousLayer; k++)
					{
						//give random weights to neuron weights
						neuronWeights[k] = (double)random.NextDouble() - 0.5f;
					}

					layerWeightsList.Add(neuronWeights); //add neuron weights of this current layer to layer weights
				}

				weightsList.Add(layerWeightsList.ToArray()); //add this layers weights converted into 2D array into weights list
			}

			weights = weightsList.ToArray(); //convert to 3D array
		}

		/// <summary>
		/// Feed forward this neural network with a given input array
		/// </summary>
		/// <param name="inputs">Inputs to network</param>
		/// <returns></returns>
		public double[] FeedForward(double[] inputs)
		{
			//Add inputs to the neuron matrix
			for (int i = 0; i < inputs.Length; i++)
			{
				neurons[0][i] = inputs[i];
			}

			//itterate over all neurons and compute feedforward values 
			for (int i = 1; i < layers.Length; i++)
			{
				for (int j = 0; j < neurons[i].Length; j++)
				{
					double value = 0f;

					for (int k = 0; k < neurons[i - 1].Length; k++)
					{
						value += weights[i - 1][j][k] * neurons[i - 1][k]; //sum off all weights connections of this neuron weight their values in previous layer
					}

					neurons[i][j] = (double)Math.Tanh(value); //Hyperbolic tangent activation
				}
			}

			return neurons[neurons.Length - 1]; //return output layer
		}

		/// <summary>
		/// Mutate neural network weights
		/// </summary>
		public void Mutate()
		{
			//Reseed
			random = new Random(Guid.NewGuid().GetHashCode());
			
			for (int i = 0; i < weights.Length; i++)
			{
				for (int j = 0; j < weights[i].Length; j++)
				{
					for (int k = 0; k < weights[i][j].Length; k++)
					{
						double weight = weights[i][j][k];


						//mutate weight value 
						double randomNumber = random.Next(0, 100000);

						if (randomNumber <= 2f)
						{ //if 1
						  //flip sign of weight
							weight *= -1f;
						}
						else if (randomNumber <= 4f)
						{ //if 2
						  //pick random weight between -1 and 1
							weight = (double)random.NextDouble() - 0.5f;
						}
						else if (randomNumber <= 6f)
						{ //if 3
						  //randomly increase by 0% to 100%
							double factor = (double)random.NextDouble() + 1f;
							weight *= factor;
						}
						else if (randomNumber <= 8f)
						{ //if 4
						  //randomly decrease by 0% to 100%
							double factor = (double)random.NextDouble();
							weight *= factor;
						}

						weights[i][j][k] = weight;
					}
				}
			}
		}

		public void Save(string filePath)
		{
			using (Stream stream = File.Open(filePath, FileMode.Create))
			{
				var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				binaryFormatter.Serialize(stream, this);
			}
		}

		public static NeuralNetwork Load(string filePath)
		{
			using (Stream stream = File.Open(filePath, FileMode.Open))
			{
				var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				return (NeuralNetwork)binaryFormatter.Deserialize(stream);
			}
		}
	}
}
