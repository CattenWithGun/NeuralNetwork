namespace BackPropagationHelper
{
    public static class BackPropagation
    {
        public static double[] ErrorWithRespectToOutputs(double[] outputLayer, double[] expectedValues)
        {
            double[] errorWithOutputs = new double[outputLayer.Length];
            for(int i = 0; i < outputLayer.Length; i++)
            {
                errorWithOutputs[i] = outputLayer[i] - expectedValues[i];
            }
            return errorWithOutputs;
        }

        public static double[] LayerOutputsWithRespectToTanh(double[] layerOutputs)
        {
            double[] outputsWithTanh = new double[layerOutputs.Length];
            for(int i = 0; i < outputsWithTanh.Length; i++)
            {
                outputsWithTanh[i] = 1 - Math.Pow(Math.Tanh(layerOutputs[i]), 2);
            }
            return outputsWithTanh;
        }

        public static double[,] InputsOfLayerWithWeights(double[] layer, double[,] layerWeights)
        {
            double[,] inputsOfLayerWithRespectToWeights = new double[layerWeights.GetLength(0), layerWeights.GetLength(1)];
            for(int heightIndex = 0; heightIndex < layerWeights.GetLength(0); heightIndex++)
            {
                for(int widthIndex = 0; widthIndex < layerWeights.GetLength(1); widthIndex++)
                {
                    inputsOfLayerWithRespectToWeights[heightIndex, widthIndex] = layer[heightIndex];
                }
            }
            return inputsOfLayerWithRespectToWeights;
        }

        public static double[] ErrorWithLayer(double[] leftLayer, double[,] leftLayerWeights, double[] rightLayer, double[] errorWithRightLayer, double[] rightLayerWithTanh)
        {
            double[] errorWithLayer = new double[leftLayer.Length];
            for(int leftLayerIndex = 0; leftLayerIndex < leftLayer.Length; leftLayerIndex++)
            {
                double error = 0;
                for(int connectionIndex = 0; connectionIndex < rightLayer.Length; connectionIndex++)
                {
                    error += errorWithRightLayer[connectionIndex] * rightLayerWithTanh[connectionIndex] * leftLayerWeights[connectionIndex, leftLayerIndex];
                }
                errorWithLayer[leftLayerIndex] = error;
            }
            return errorWithLayer;
        }

        //Left layer could be replaced by a GetLength() of leftLayerWeights later, but that is confusing to me right now
        public static double[,] NewLayerWeights(double[] leftLayer, double[,] leftLayerWeights, double[] rightLayer, double learningRate, double[] errorWithRespectToRightLayer, double[] rightLayerWithRespectToTanh, double[,] inputsOfRightLayerWithRespectToWeights)
        {
            double[,] newLeftLayerWeights = leftLayerWeights;
            for(int heightIndex = 0; heightIndex < rightLayer.Length; heightIndex++)
            {
                for(int widthIndex = 0; widthIndex < leftLayer.Length; widthIndex++)
                {
                    newLeftLayerWeights[heightIndex, widthIndex] -= learningRate * errorWithRespectToRightLayer[heightIndex] * rightLayerWithRespectToTanh[heightIndex] * inputsOfRightLayerWithRespectToWeights[heightIndex, widthIndex];
                }
            }
            return newLeftLayerWeights;
        }

        public static double[] NewLayerBiases(double[] leftLayerBiases, double[] errorWithRespectToRightLayer, double[] rightLayerWithRespectToTanh, double learningRate)
        {
            double[] newLeftLayerBiases = leftLayerBiases;
            for(int i = 0; i < newLeftLayerBiases.Length; i++)
            {
                newLeftLayerBiases[i] -= learningRate * errorWithRespectToRightLayer[i] * rightLayerWithRespectToTanh[i];
            }
            return newLeftLayerBiases;
        }
    }
}
