namespace ArrayCopying
{
	public static class ArrayUtils
	{
		//These functions are for deep copying instead of shallow copying
		public static double[] Clone(double[] array)
		{
			return array.Clone() as double[] ?? throw new NullReferenceException();
		}

		public static double[,] Clone(double[,] array)
		{
			return array.Clone() as double[,] ?? throw new NullReferenceException();
		}

		public static double[][] Clone(double[][] array)
		{
			return array.Clone() as double[][] ?? throw new NullReferenceException();
		}

		public static double[][,] Clone(double[][,] array)
		{
			return array.Clone() as double[][,] ?? throw new NullReferenceException();
		}
	}
}
