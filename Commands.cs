using Newtonsoft.Json;

internal static class Commands
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

    public static void Train(NeuralNetwork network, byte[] labels, byte[,,] images)
    {
        string learningRateString = Prompts.SamePromptUntilConditionMet("Enter the learning rate: ", Checks.IsDouble, "");
        if(learningRateString == "exit") return;
        double learningRate = Convert.ToDouble(learningRateString);
        network = Training.TrainNetwork(network, learningRate, labels, images);
    }

    public static void Test(NeuralNetwork network, byte[] labels, byte[,,] images)
    {
        string answer = "";
        Random random = new Random();
        while(answer != "exit")
        {
            if(answer == "r")
            {
                byte[,] image = MNISTFileHandler.RandomizedImage();
                byte[] imageBytes = MNISTFileHandler.ImageToByteArray(image);
                byte guess = network.FeedForwardAndGetGuess(network, imageBytes);

                MNISTFileHandler.WriteImage(image);
                Console.WriteLine($"{network.name}'s guess: {guess}");
            }
            else
            {
                int imageIndex = random.Next(0, labels.Length);
                byte[,] image = MNISTFileHandler.GetImage(images, imageIndex);
                byte[] imageBytes = MNISTFileHandler.ImageToByteArray(image);
                byte guess = network.FeedForwardAndGetGuess(network, imageBytes);

                Console.WriteLine($"Expected: {labels[imageIndex]}");
                MNISTFileHandler.WriteImage(image);
                Console.WriteLine($"{network.name}'s guess: {guess}");
            }
            Console.WriteLine("outputLayer:");
            DebugTools.ShowDoubles(network.outputLayer);
            answer = Console.ReadLine() ?? "exit";
        }
    }

    public static void PrintError(NeuralNetwork network, byte[] labels, byte[,,] images)
    {
        //Finds the average error of the network
        double[] errors = new double[labels.Length];
        for(int i = 0; i < labels.Length; i++)
        {
            byte[,] image = MNISTFileHandler.GetImage(images, i);
            byte[] imageBytes = MNISTFileHandler.ImageToByteArray(image);
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
            byte[,] image = MNISTFileHandler.GetImage(images, i);
            byte[] imageBytes = MNISTFileHandler.ImageToByteArray(image);
            if(network.FeedForwardAndGetGuess(network, imageBytes) == labels[i])
            {
                correctGuesses++;
            }
        }
        double percent = correctGuesses / Convert.ToDouble(labels.Length);

        Console.WriteLine($"The percent the network guessed correctly was {percent * 100}%");
        Console.WriteLine($"The error of {network.name} is: {averageError}");
    }

    public static void Store(NeuralNetwork network, string filePathToStoreIn)
    {
        string json = network.ToString();
        File.WriteAllText(filePathToStoreIn, json);
        Console.WriteLine($"Saved {network.name} in {filePathToStoreIn}");
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

            NeuralNetwork network = new NeuralNetwork(name);

            networks.Add(network);
            networkNames.Add(name);
            Console.WriteLine($"\nCreated {network.name}");
        }
    }

    public static void Delete(List<NeuralNetwork> networks, List<string> networkNames, NeuralNetwork network)
    {
        for(int i = 0; i < networks.Count; i++)
        {
            if(networks[i].name == network.name)
            {
                networks.RemoveAt(i);
                networkNames.RemoveAt(i);
            }
        }
        Console.WriteLine($"\nDeleted {network.name}");
    }

    public static void Peek(NeuralNetwork network)
    {
        Console.WriteLine("outputLayer: ");
        DebugTools.ShowDoubles(network.outputLayer);
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
        Console.WriteLine("Peek inside network:              peek");
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
