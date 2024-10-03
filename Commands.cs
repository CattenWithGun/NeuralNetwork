using ErrorChecks;
using MNIST;
using NeuralNetworking;
using NetworkTraining;
using Newtonsoft.Json;
using UserPrompts;
using Debugging;

namespace Actions
{
	public static class Commands
	{
		public static void ShowNetworks(List<NeuralNetwork> neuralNetworks)
		{
			Console.WriteLine("\nSaved Neural Networks:\n");
			if(neuralNetworks.Count == 0)
			{
				Console.WriteLine("No saved networks");
				return;
			}
			for(int i = 0; i < neuralNetworks.Count; i++)
			{
				Console.WriteLine(neuralNetworks[i].name);
			}
		}

		public static void Train(NeuralNetwork network, string labelsFilePath, string imagesFilePath)
		{
			//Get the learning rate
			string learningRateString = Prompts.SamePromptUntilConditionMet("Enter the learning rate: ", Checks.IsDouble, "");
			if(learningRateString == "exit") return;
			double learningRate = Convert.ToDouble(learningRateString);

			network = Training.TrainNetwork(network, learningRate, labelsFilePath, imagesFilePath);
		}

		public static void Test(List<NeuralNetwork> networks, List<string> networkNames, string labelsFilePath, string imagesFilePath)
		{
			if(networks.Count == 0)
			{
				Console.WriteLine("There are no networks to test");
				return;
			}

			//Get the name of the network to be tested
			string nameToTest = Prompts.PromptUntilConditionMet("Enter a network to test: ", "Network wasn't in the saved networks list\nEnter again: ", Checks.NetworkListContainsName, "", networkNames);
			if(nameToTest == "exit") return;

			//Find the network in the list
			NeuralNetwork networkToTest = new NeuralNetwork("placeholder");
			for(int i = 0; i < networks.Count; i++)
			{
				if(networks[i].name == nameToTest)
				{
					networkToTest = networks[i];
				}
			}

			string exit = "";
			Random random = new Random();
			byte[] labels = MNISTFileHandler.GetLabels(labelsFilePath);
			byte[,,] images = MNISTFileHandler.GetImages(imagesFilePath);
			int imageIndex = 0;
			while(exit != "exit")
			{
				imageIndex = random.Next(0, labels.Length);
				MNISTFileHandler.WriteImage(imageIndex, images);
				Console.WriteLine($"Real: {labels[imageIndex]}");
				Console.WriteLine($"{networkToTest.name}'s guess: {networkToTest.FeedForwardAndGetGuess(networkToTest, MNISTFileHandler.ImageToByteArray(images, imageIndex))}");
				Console.WriteLine("outputLayer:");
				DebugTools.ShowDoubles(networkToTest.outputLayer);
				exit = Console.ReadLine() ?? "exit";
			}
		}

		public static void PrintError(NeuralNetwork network, string labelsFilePath, string imagesFilePath)
		{
			//Finds the average error of the network
			byte[] labels = MNISTFileHandler.GetLabels(labelsFilePath);
			byte[,,] images = MNISTFileHandler.GetImages(imagesFilePath);
			double[] errors = new double[labels.Length];
			for(int i = 0; i < labels.Length; i++)
			{
				byte[] imageBytes = MNISTFileHandler.ImageToByteArray(images, i);
				errors[i] = network.Error(network, imageBytes, labels[i]);
			}

			double averageError = 0;
			for(int i = 0; i < errors.Length; i++)
			{
				averageError += errors[i];
			}
			averageError /= errors.Length;

			//Finds the percent that the network guesses correctly
			double correctGuesses = 0;
			for(int i = 0; i < labels.Length; i++)
			{
				if(network.FeedForwardAndGetGuess(network, MNISTFileHandler.ImageToByteArray(images, i)) == labels[i])
				{
					correctGuesses++;
				}
			}
			double percent = correctGuesses / Convert.ToDouble(labels.Length);

			Console.WriteLine($"The percent the network guessed correctly was {percent * 100}%");
			Console.WriteLine($"The error of {network.name} is: {averageError}");
		}

		public static void Delete(List<NeuralNetwork> networks, List<string> networkNames)
		{
			if(networks.Count == 0)
			{
				Console.WriteLine("There are no networks to delete");
				return;
			}

			//Get the name of the network to be deleted
			string nameToDelete = Prompts.PromptUntilConditionMet("Enter a network you want to delete: ", "Name wasn't in the saved networks list\nEnter again: ", Checks.NetworkListContainsName, "", networkNames);
			if(nameToDelete == "exit") return;

			//Find the network in the list and delete it
			for(int i = 0; i < networks.Count; i++)
			{
				if(networks[i].name == nameToDelete)
				{
					networks.RemoveAt(i);
					networkNames.RemoveAt(i);
				}
			}
			Console.WriteLine($"\nDeleted {nameToDelete}");
		}

		public static void Make(List<NeuralNetwork> networks, List<string> networkNames)
		{
			if(networks.Count == 20)
			{
				Console.WriteLine("You know what? No. You don't get anymore than 20 networks because no sane human being would ever need that many.");
				return;
			}
			string useSaveDataString = Prompts.SamePromptUntilConditionMet("Do you want to use save data? (y/n): ", Checks.IsYesNo, "");
			if(useSaveDataString == "exit") return;
			bool useSaveData = useSaveDataString == "y";

			if(useSaveData)
			{
				Console.Write("Enter a file path to the network data: ");
				string networkDataFilePath = Console.ReadLine() ?? "exit";
				if(networkDataFilePath == "exit") return;

				//Remove excess quotation marks from a file path, because Windows copies file paths in that way (i use arch btw now)
				if(networkDataFilePath.Length > 1)
				{
					if((networkDataFilePath[0] == '"') && (networkDataFilePath[networkDataFilePath.Length - 1] == '"'))
					{
						networkDataFilePath = networkDataFilePath.Substring(1, networkDataFilePath.Length - 2);
					}
				}

				string networkData;
				try
				{
					networkData = File.ReadAllText(networkDataFilePath);
				}
				catch
				{
					Console.WriteLine($"Error reading file at {networkDataFilePath}");
					return;
				}

				//network is null if there was an error with reading the save data
				NeuralNetwork network = JsonToNetwork(networkData)!;
				if(network == null) { return; }

				if(Checks.NetworkListContainsName(network.name, networkNames))
				{
					Console.WriteLine("There is already a network with that name");
					string name = Prompts.PromptUntilConditionMet("Enter a name for the network: ", "Name length needs to be 1-20 characters and not already used\nEnter again: ", Checks.IsValidNetworkName, "", networkNames);
					if(name == "exit") return;
					network.name = name;
				}

				networkNames.Add(network.name);
				networks.Add(network);
				Console.WriteLine($"{network.name} created from save data");
			}
			else
			{
				string name = Prompts.PromptUntilConditionMet("Enter a name for the network: ", "Name length needs to be 1-20 characters and not already used\nEnter again: ", Checks.IsValidNetworkName, "", networkNames);
				if(name == "exit") return;

				//Creates a randomized network with the name
				NeuralNetwork network = new NeuralNetwork(name);

				networks.Add(network);
				networkNames.Add(name);
				Console.WriteLine($"\nCreated {network.name}");
			}
		}

		public static void Store(List<NeuralNetwork> networks, List<string> networkNames, string filePathToStoreIn)
		{
			if(networks.Count == 0)
			{
				Console.WriteLine("There are no networks to store");
				return;
			}

			string nameToStore = Prompts.PromptUntilConditionMet("Enter a network to store: ", "Network wasn't in the saved networks list\nEnter again: ", Checks.NetworkListContainsName, "", networkNames);
			if(nameToStore == "exit") return;

			for(int i = 0; i < networks.Count; i++)
			{
				if(networks[i].name == nameToStore)
				{
					string json = networks[i].ToString();
					File.WriteAllText(filePathToStoreIn, json);
					Console.WriteLine($"Saved {nameToStore} in {filePathToStoreIn}");
					return;
				}
			}
		}

		public static void ShowCommands()
		{
			Console.WriteLine("Type commands to do stuff:");
			Console.WriteLine("Show list of neural networks:     show");
			Console.WriteLine("Train neural network:             train");
			Console.WriteLine("Test the network:                 test");
			Console.WriteLine("Get the error of the network:     error");
			Console.WriteLine("Store neural network:             store");
			Console.WriteLine("Make new neural network:          make");
			Console.WriteLine("Delete neural network:            delete");
			Console.WriteLine("Exits out of a prompt:            exit");
			Console.WriteLine("Clear the screen:                 clear");
			Console.WriteLine("Show this dialogue again:         help");
		}

		private static NeuralNetwork? JsonToNetwork(string networkJson)
		{
			try
			{
				return JsonConvert.DeserializeObject<NeuralNetwork>(networkJson);
			}
			catch
			{
				Console.WriteLine("Error reading save data");
				return null;
			}
		}
	}
}
