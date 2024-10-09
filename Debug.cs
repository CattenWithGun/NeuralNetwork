namespace Debugging
{
	public static class DebugTools
	{
		public static void Show2DDoubles(double[,] doubles)
		{
			string row = "";
			for(int height = 0; height < doubles.GetLength(0); height++)
			{
				for(int width = 0; width < doubles.GetLength(1); width++)
				{
					row += Convert.ToString(doubles[height, width]) + ", ";
				}
				Console.WriteLine(row);
				row = "";
			}
		}

		public static void ShowDoubles(double[] doubles)
		{
			string row = "";
			for(int i = 0; i < doubles.Length; i++)
			{
				row += Convert.ToString(doubles[i]) + ", ";
			}
			Console.WriteLine(row);
		}

		public static bool IsDoubleArrayEqual(double[] array1, double[] array2)
		{
			if(array1.Length != array2.Length)
			{
				return false;
			}

			for(int i = 0; i < array1.Length; i++)
			{
				if(array1[i] != array2[i])
				{
					return false;
				}
			}

			return true;
		}

		public static bool Is2DDoubleArrayEqual(double[,] array1, double[,] array2)
		{
			if((array1.GetLength(0) != array2.GetLength(0)) || (array1.GetLength(1) != array2.GetLength(1)))
			{
				return false;
			}

			for(int heightIndex = 0; heightIndex < array1.GetLength(0); heightIndex++)
			{
				for(int widthIndex = 0; widthIndex < array1.GetLength(1); widthIndex++)
				{
					if(array1[heightIndex, widthIndex] != array2[heightIndex, widthIndex])
					{
						return false;
					}
				}
			}

			return true;
		}

		public static bool ContainsNegative(double[] doubles)
		{
			for(int i = 0; i < doubles.Length; i++)
			{
				if(doubles[i] < 0)
				{
					return true;
				}
			}
			return false;
		}
	}
}
