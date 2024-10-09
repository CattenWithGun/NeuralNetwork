using ArrayCopying;
using BackPropagationHelper;
using Debugging;
using Newtonsoft.Json;

namespace NeuralNetworking
{
    public class NeuralNetwork
    {
        public string name;

        //Layers
        public double[] inputLayer;
        public double[] hiddenLayer1;
        public double[] hiddenLayer2;
        public double[] outputLayer;

        //Weights
        public double[,] inputLayerWeights;
        public double[,] hiddenLayer1Weights;
        public double[,] hiddenLayer2Weights;

        //Biases
        public double[] inputLayerBiases;
        public double[] hiddenLayer1Biases;
        public double[] hiddenLayer2Biases;

        //Creates neural network with saved data
        [JsonConstructor]
        public NeuralNetwork(double[] argumentInputLayer, double[] argumentHiddenLayer1, double[] argumentHiddenLayer2, double[] argumentOutputLayer, double[,] argumentInputLayerWeights, double[,] argumentHiddenLayer1Weights, double[,] argumentHiddenLayer2Weights, double[] argumentInputLayerBiases, double[] argumentHiddenLayer1Biases, double[] argumentHiddenLayer2Biases, string argumentName)
        {
            inputLayer = argumentInputLayer;
            hiddenLayer1 = argumentHiddenLayer1;
            hiddenLayer2 = argumentHiddenLayer2;
            outputLayer = argumentOutputLayer;
            inputLayerWeights = argumentInputLayerWeights;
            hiddenLayer1Weights = argumentHiddenLayer1Weights;
            hiddenLayer2Weights = argumentHiddenLayer2Weights;
            inputLayerBiases = argumentInputLayerBiases;
            hiddenLayer1Biases = argumentHiddenLayer1Biases;
            hiddenLayer2Biases = argumentHiddenLayer2Biases;
            name = argumentName;
        }

        //If no neuron information about the network is given, generate random weights and biases
        public NeuralNetwork(string argumentName)
        {
            inputLayer = new double[784];
            hiddenLayer1 = new double[16];
            hiddenLayer2 = new double[16];
            outputLayer = new double[10];

            Random random = new Random();

            // RANDOM WEIGHTS ARE DISABLED FOR NOW, CHANGE LATEEEEEEEEEEEEEEEEEEERRRRRRRRRRRRRRRRRRR
            inputLayerWeights = Random2DDoubleArray(784, 16, random);
            hiddenLayer1Weights = Random2DDoubleArray(16, 16, random);
            hiddenLayer2Weights = Random2DDoubleArray(16, 10, random);

            inputLayerBiases = RandomDoubleArray(16, random);
            hiddenLayer1Biases = RandomDoubleArray(16, random);
            hiddenLayer2Biases = RandomDoubleArray(10, random);


            /*hiddenLayer2Weights = new double[10, 16]
			{
				{ 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7 },
				{ 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7 },
				{ 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7 },
				{ 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7 },
				{ 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7 },
				{ 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7 },
				{ 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7 },
				{ 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7 },
				{ 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7 },
				{ 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7 }
			};
			hiddenLayer2Biases = new double[10] { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 0.1 };*/


            name = argumentName;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        private static double[] RandomDoubleArray(int length, Random random)
        {
            double[] randomDoubleArray = new double[length];
            for(int i = 0; i < length; i++)
            {
                //Goofy numbers are making a random double between -1 and 1
                randomDoubleArray[i] = random.NextDouble() * ((1) - (-1)) + (-1);
            }
            return randomDoubleArray;
        }

        private static double[,] Random2DDoubleArray(int width, int height, Random random)
        {
            double[,] random2DDoubleArray = new double[height, width];
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    //Goofy numbers are making a random double between -1 and 1
                    random2DDoubleArray[y, x] = random.NextDouble() * ((1) - (-1)) + (-1);
                }
            }
            return random2DDoubleArray;
        }

        public NeuralNetwork FeedForward(NeuralNetwork network, byte[] inputLayerBytes)
        {
            double[] inputLayer = NormalizeInputs(inputLayerBytes);
            double[] hiddenLayer1 = FeedNextLayer(network.inputLayer, network.hiddenLayer1.Length, network.inputLayerWeights, network.inputLayerBiases);
            double[] hiddenLayer2 = FeedNextLayer(network.hiddenLayer1, network.hiddenLayer2.Length, network.hiddenLayer1Weights, network.hiddenLayer1Biases);
            double[] outputLayer = FeedNextLayer(network.hiddenLayer2, network.outputLayer.Length, network.hiddenLayer2Weights, network.hiddenLayer2Biases);
            return new NeuralNetwork
            (
                inputLayer,
                hiddenLayer1,
                hiddenLayer2,
                outputLayer,
                network.inputLayerWeights,
                network.hiddenLayer1Weights,
                network.hiddenLayer2Weights,
                network.inputLayerBiases,
                network.hiddenLayer1Biases,
                network.hiddenLayer2Biases,
                network.name
            );
        }

        private static double[] FeedNextLayer(double[] leftLayer, int rightLayerLength, double[,] weights, double[] biases)
        {
            double[] newRightLayer = new double[rightLayerLength];
            for(int neuronIndex = 0; neuronIndex < rightLayerLength; neuronIndex++)
            {
                double sum = 0;
                for(int weightIndex = 0; weightIndex < leftLayer.Length; weightIndex++)
                {
                    sum += weights[neuronIndex, weightIndex] * leftLayer[weightIndex];
                }
                sum += biases[neuronIndex];
                sum = Math.Tanh(sum);
                newRightLayer[neuronIndex] = sum;
            }
            return newRightLayer;
        }

        //Turns the inputLayerBytes into doubles and normalizes them
        private static double[] NormalizeInputs(byte[] inputLayerBytes)
        {
            double[] inputLayer = new double[inputLayerBytes.Length];
            for(int i = 0; i < inputLayer.Length; i++)
            {
                inputLayer[i] = Convert.ToDouble(inputLayerBytes[i]) / 255;
            }
            return inputLayer;
        }

        //Returns what the network thinks the image is, instead of the outputLayer
        public byte FeedForwardAndGetGuess(NeuralNetwork network, byte[] inputLayerBytes)
        {
            double[] outputLayer = network.FeedForward(network, inputLayerBytes).outputLayer;
            return Convert.ToByte(outputLayer.ToList().IndexOf(outputLayer.Max()));
        }

        //Calculates the SSE (Sum of Squared Errors)
        //Multiplying by 0.5 for derivational purposes? Might change later because its not hard to do the math.
        public double Error(NeuralNetwork network, byte[] inputLayerBytes, byte expectedValueByte)
        {
            NeuralNetwork networkToGetErrorOf = network.FeedForward(network, inputLayerBytes);
            double[] errors = new double[networkToGetErrorOf.outputLayer.Length];
            double error = 0;
            for(int i = 0; i < errors.Length; i++)
            {
                int expectedValue = Convert.ToInt32(i == Convert.ToInt32(expectedValueByte));
                errors[i] = 0.5 * Math.Pow(expectedValue - outputLayer[i], 2);
                error += errors[i];
            }
            return error;
        }

        public NeuralNetwork BackPropagate(NeuralNetwork network, double[] expectedValues, double learningRate)
        {
            //Backpropagates the connections between hiddenLayer2 and the outputLayer
            double[] errorWithOutputs = BackPropagation.ErrorWithRespectToOutputs(network.outputLayer, expectedValues);
            double[] outputsWithTanh = BackPropagation.LayerOutputsWithRespectToTanh(network.outputLayer);
            double[,] inputsOfOutputsWithWeights = BackPropagation.InputsOfLayerWithWeights(network.hiddenLayer2, network.hiddenLayer2Weights);
            double[,] newHiddenLayer2Weights = BackPropagation.NewLayerWeights(network.hiddenLayer2, network.hiddenLayer2Weights, network.outputLayer, learningRate, errorWithOutputs, outputsWithTanh, inputsOfOutputsWithWeights);
            double[] newHiddenLayer2Biases = BackPropagation.NewLayerBiases(network.hiddenLayer2Biases, errorWithOutputs, outputsWithTanh, learningRate);

            //Backpropagates the connections between hiddenLayer1 and hiddenLayer2
            double[] errorWithHiddenLayer2 = BackPropagation.ErrorWithLayer(network.hiddenLayer2, network.hiddenLayer2Weights, network.outputLayer, errorWithOutputs, outputsWithTanh);
            double[] hiddenLayer2WithTanh = BackPropagation.LayerOutputsWithRespectToTanh(network.hiddenLayer2);
            double[,] inputsOfHiddenLayer2WithWeights = BackPropagation.InputsOfLayerWithWeights(network.hiddenLayer1, network.hiddenLayer1Weights);
            double[,] newHiddenLayer1Weights = BackPropagation.NewLayerWeights(network.hiddenLayer1, network.hiddenLayer1Weights, network.hiddenLayer2, learningRate, errorWithHiddenLayer2, hiddenLayer2WithTanh, inputsOfHiddenLayer2WithWeights);
            double[] newHiddenLayer1Biases = BackPropagation.NewLayerBiases(network.hiddenLayer1Biases, errorWithHiddenLayer2, hiddenLayer2WithTanh, learningRate);

            //Backpropagates the connections between the inputLayer and hiddenLayer1
            double[] errorWithHiddenLayer1 = BackPropagation.ErrorWithLayer(network.hiddenLayer1, network.hiddenLayer1Weights, network.hiddenLayer2, errorWithHiddenLayer2, hiddenLayer2WithTanh);
            double[] hiddenLayer1WithTanh = BackPropagation.LayerOutputsWithRespectToTanh(network.hiddenLayer1);
            double[,] inputsOfHiddenLayer1WithWeights = BackPropagation.InputsOfLayerWithWeights(network.inputLayer, network.inputLayerWeights);
            double[,] newInputLayerWeights = BackPropagation.NewLayerWeights(network.inputLayer, network.inputLayerWeights, network.hiddenLayer1, learningRate, errorWithHiddenLayer1, hiddenLayer1WithTanh, inputsOfHiddenLayer1WithWeights);
            double[] newInputLayerBiases = BackPropagation.NewLayerBiases(network.inputLayerBiases, errorWithHiddenLayer1, hiddenLayer1WithTanh, learningRate);

            //Returns the network with all it's new values
            return new NeuralNetwork
            (
                ArrayUtils.Clone(network.inputLayer),
                ArrayUtils.Clone(network.hiddenLayer1),
                ArrayUtils.Clone(network.hiddenLayer2),
                ArrayUtils.Clone(network.outputLayer),
                newInputLayerWeights,
                newHiddenLayer1Weights,
                newHiddenLayer2Weights,
                newInputLayerBiases,
                newHiddenLayer1Biases,
                newHiddenLayer2Biases,
                network.name
            );
        }
    }
}
