using ArrayCopying;
using MNIST;
using NeuralNetworking;
using System.Diagnostics;

namespace NetworkTraining
{
	public static class Training
	{
		public static NeuralNetwork TrainNetwork(NeuralNetwork network, double learningRate, string labelsFilePath, string imagesFilePath)
		{
			Console.WriteLine("Training network...");

			byte[] labels = MNISTFileHandler.GetLabels(labelsFilePath);
			byte[,,] images = MNISTFileHandler.GetImages(imagesFilePath);
			int batchSize = 10000;

			Random random = new Random();
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			//Old weights and biases
			double[,] oldHiddenLayer2Weights = ArrayUtils.Clone(network.hiddenLayer2Weights);
			double[,] oldHiddenLayer1Weights = ArrayUtils.Clone(network.hiddenLayer1Weights);
			double[,] oldInputLayerWeights = ArrayUtils.Clone(network.inputLayerWeights);
			double[] oldHiddenLayer2Biases = ArrayUtils.Clone(network.hiddenLayer2Biases);
			double[] oldHiddenLayer1Biases = ArrayUtils.Clone(network.hiddenLayer1Biases);
			double[] oldInputLayerBiases = ArrayUtils.Clone(network.inputLayerBiases);

			//New weights and biases
			double[][,] newHiddenLayer2Weights = new double[batchSize][,];
			double[][,] newHiddenLayer1Weights = new double[batchSize][,];
			double[][,] newInputLayerWeights = new double[batchSize][,];
			double[][] newHiddenLayer2Biases = new double[batchSize][];
			double[][] newHiddenLayer1Biases = new double[batchSize][];
			double[][] newInputLayerBiases = new double[batchSize][];

			for(int batchIndex = 0; batchIndex < batchSize; batchIndex++)
			{
				//Gets the new weights and biases, using the backpropagation function
				int trainingDataIndex;
				//This do while makes sure that the batches always have equal amounts of data of all types
				do
				{
					trainingDataIndex = random.Next(0, labels.Length);
				}
				while(labels[trainingDataIndex] != batchIndex % 10);
				network = network.FeedForward(network, MNISTFileHandler.ImageToByteArray(images, trainingDataIndex));
				network = network.BackPropagate(network, MNISTFileHandler.LabelToExpectedValues(labels[trainingDataIndex]), learningRate);

				//Stores the new weights and biases in the big jagged arrays
				newHiddenLayer2Weights[batchIndex] = ArrayUtils.Clone(network.hiddenLayer2Weights);
				newHiddenLayer1Weights[batchIndex] = ArrayUtils.Clone(network.hiddenLayer1Weights);
				newInputLayerWeights[batchIndex] = ArrayUtils.Clone(network.inputLayerWeights);
				newHiddenLayer2Biases[batchIndex] = ArrayUtils.Clone(network.hiddenLayer2Biases);
				newHiddenLayer1Biases[batchIndex] = ArrayUtils.Clone(network.hiddenLayer1Biases);
				newInputLayerBiases[batchIndex] = ArrayUtils.Clone(network.inputLayerBiases);

				//Reverts the network back to the old weights and biases
				network.hiddenLayer2Weights = ArrayUtils.Clone(oldHiddenLayer2Weights);
				network.hiddenLayer1Weights = ArrayUtils.Clone(oldHiddenLayer1Weights);
				network.inputLayerWeights = ArrayUtils.Clone(oldInputLayerWeights);
				network.hiddenLayer2Biases = ArrayUtils.Clone(oldHiddenLayer2Biases);
				network.hiddenLayer1Biases = ArrayUtils.Clone(oldHiddenLayer1Biases);
				network.inputLayerBiases = ArrayUtils.Clone(oldInputLayerBiases);
			}

			//Notice how these arrays are the same type as the new weights and biases, because the new weights and biases are changes themselves, but they only have the new value instead of the value added to the old value to get the new value
			double[][,] hiddenLayer2WeightChanges = GetWeightChanges(batchSize, oldHiddenLayer2Weights, newHiddenLayer2Weights);
			double[][,] hiddenLayer1WeightChanges = GetWeightChanges(batchSize, oldHiddenLayer1Weights, newHiddenLayer1Weights);
			double[][,] inputLayerWeightChanges = GetWeightChanges(batchSize, oldInputLayerWeights, newInputLayerWeights);
			double[][] hiddenLayer2BiaseChanges = GetBiaseChanges(batchSize, oldHiddenLayer2Biases, newHiddenLayer2Biases);
			double[][] hiddenLayer1BiaseChanges = GetBiaseChanges(batchSize, oldHiddenLayer1Biases, newHiddenLayer1Biases);
			double[][] inputLayerBiaseChanges = GetBiaseChanges(batchSize, oldInputLayerBiases, newInputLayerBiases);

			//Finds the average of all of the changes for each weight and biase.
			double[,] averageHiddenLayer2WeightChanges = AverageWeightsAcrossChanges(network.hiddenLayer2Weights.GetLength(0), network.hiddenLayer2Weights.GetLength(1), batchSize, hiddenLayer2WeightChanges);
			double[,] averageHiddenLayer1WeightChanges = AverageWeightsAcrossChanges(network.hiddenLayer1Weights.GetLength(0), network.hiddenLayer1Weights.GetLength(1), batchSize, hiddenLayer1WeightChanges);
			double[,] averageInputLayerWeightChanges = AverageWeightsAcrossChanges(network.inputLayerWeights.GetLength(0), network.inputLayerWeights.GetLength(1), batchSize, inputLayerWeightChanges);
			double[] averageHiddenLayer2BiaseChanges = AverageBiasesAcrossChanges(network.hiddenLayer2Biases.Length, batchSize, hiddenLayer2BiaseChanges);
			double[] averageHiddenLayer1BiaseChanges = AverageBiasesAcrossChanges(network.hiddenLayer1Biases.Length, batchSize, hiddenLayer1BiaseChanges);
			double[] averageInputLayerBiaseChanges = AverageBiasesAcrossChanges(network.inputLayerBiases.Length, batchSize, inputLayerBiaseChanges);

			//Changes the weights and biases of the network to be the averaged out changes
			network.hiddenLayer2Weights = ArrayUtils.Clone(averageHiddenLayer2WeightChanges);
			network.hiddenLayer1Weights = ArrayUtils.Clone(averageHiddenLayer1WeightChanges);
			network.inputLayerWeights = ArrayUtils.Clone(averageInputLayerWeightChanges);
			network.hiddenLayer2Biases = ArrayUtils.Clone(averageHiddenLayer2BiaseChanges);
			network.hiddenLayer1Biases = ArrayUtils.Clone(averageHiddenLayer1BiaseChanges);
			network.inputLayerBiases = ArrayUtils.Clone(averageInputLayerBiaseChanges);

			stopwatch.Stop();
			Console.WriteLine($"Training finished, took about {stopwatch.ElapsedMilliseconds / 1000} seconds");
			return network;
		}

		private static double[][,] GetWeightChanges(int batchSize, double[,] oldWeights, double[][,] newWeights)
		{
			//Copies newWeights because otherwise the 2D arrays aren't set, and there is a null reference exception
			double[][,] weightChanges = ArrayUtils.Clone(newWeights);
			for(int heightIndex = 0; heightIndex < oldWeights.GetLength(0); heightIndex++)
			{
				for(int widthIndex = 0; widthIndex < oldWeights.GetLength(1); widthIndex++)
				{
					for(int batchIndex = 0; batchIndex < batchSize; batchIndex++)
					{
						weightChanges[batchIndex][heightIndex, widthIndex] = oldWeights[heightIndex, widthIndex] - newWeights[batchIndex][heightIndex, widthIndex];
					}
				}
			}
			return weightChanges;
		}

		private static double[][] GetBiaseChanges(int batchSize, double[] oldBiases, double[][] newBiases)
		{
			//Copies newBiases because otherwise the arrays aren't set, and there is a null reference exception
			double[][] biaseChanges = ArrayUtils.Clone(newBiases);
			for(int i = 0; i < oldBiases.Length; i++)
			{
				for(int batchIndex = 0; batchIndex < batchSize; batchIndex++)
				{
					biaseChanges[batchIndex][i] = oldBiases[i] - newBiases[batchIndex][i];
				}
			}
			return biaseChanges;
		}

		private static double[,] AverageWeightsAcrossChanges(int weightsHeight, int weightsWidth, int batchSize, double[][,] weightChanges)
		{
			double[,] averageWeightChanges = new double[weightsHeight, weightsWidth];
			for(int heightIndex = 0; heightIndex < weightsHeight; heightIndex++)
			{
				for(int widthIndex = 0; widthIndex < weightsWidth; widthIndex++)
				{
					double sum = 0;
					for(int batchIndex = 0; batchIndex < batchSize; batchIndex++)
					{
						sum += weightChanges[batchIndex][heightIndex, widthIndex];
					}
					averageWeightChanges[heightIndex, widthIndex] = sum / batchSize;
				}
			}
			return averageWeightChanges;
		}

		private static double[] AverageBiasesAcrossChanges(int biaseLength, int batchSize, double[][] biaseChanges)
		{
			double[] averageBiaseChanges = new double[biaseLength];
			for(int i = 0; i < biaseLength; i++)
			{
				double sum = 0;
				for(int batchIndex = 0; batchIndex < batchSize; batchIndex++)
				{
					sum += biaseChanges[batchIndex][i];
				}
				averageBiaseChanges[i] = sum / batchSize;
			}
			return averageBiaseChanges;
		}
	}
}
