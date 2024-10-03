using Actions;
using ErrorChecks;
using NeuralNetworking;
using UserPrompts;

public class Program
{
	public static void Main()
	{
		//Creates a list to store networks and their names
		List<NeuralNetwork> networks = new List<NeuralNetwork>();
		List<string> networkNames = new List<string>();

		//Stores file paths, will change later
		string labelsFilePath = "/home/catten/Desktop/MNIST_Train_Database/train-labels.idx1-ubyte";
		string imagesFilePath = "/home/catten/Desktop/MNIST_Train_Database/train-images.idx3-ubyte";
		string filePathToStoreIn = "/home/catten/Desktop/network.txt";

		NeuralNetwork? nullableNetwork;
		NeuralNetwork network;

		Console.Clear();
		Commands.ShowCommands();
		while (true)
		{
			string command = Prompts.PromptUntilConditionMet("\nEnter: ", "\nNot a command, enter again: ", Checks.IsOption, "");
			//Finds the command and executes it
			switch (command)
			{
				case "show":
					Commands.ShowNetworks(networks);
					break;
				case "train":
					nullableNetwork = GetNetwork(networks, networkNames, "Enter a network to train: ", "Network wasn't in the saved networks list\nEnter again: ");
					if (nullableNetwork == null) continue;
					network = nullableNetwork;
					Commands.Train(network, labelsFilePath, imagesFilePath);
					break;
				case "test":
					Commands.Test(networks, networkNames, labelsFilePath, imagesFilePath);
					break;
				case "error":
					nullableNetwork = GetNetwork(networks, networkNames, "Enter a network to get the error of: ", "Network wasn't in the saved networks list\nEnter again: ");
					if (nullableNetwork == null) continue;
					network = nullableNetwork;
					Commands.PrintError(network, labelsFilePath, imagesFilePath);
					break;
				case "store":
					Commands.Store(networks, networkNames, filePathToStoreIn);
					break;
				case "make":
					Commands.Make(networks, networkNames);
					break;
				case "delete":
					Commands.Delete(networks, networkNames);
					break;
				case "clear":
					Console.Clear();
					break;
				case "help":
					Commands.ShowCommands();
					break;
			}
		}
	}

	public static NeuralNetwork? GetNetwork(List<NeuralNetwork> networks, List<string> networkNames, string startingMessage, string problemMessage)
	{
		if (networks.Count == 0)
		{
			Console.WriteLine("Network list empty");
			return null;
		}

		//Get the name of the network
		string name = Prompts.PromptUntilConditionMet(startingMessage, problemMessage, Checks.NetworkListContainsName, "", networkNames);
		if (name == "exit") return null;

		//Find the network in the list and return it
		for (int i = 0; i < networks.Count; i++)
		{
			if (networks[i].name == name)
			{
				return networks[i];
			}
		}

		return null;
	}
}
