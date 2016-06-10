using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllOfTheLights
{
	class MathUtils
	{
		public static double Clamp(double value, double min, double max)
		{
			return Clamper<double>.Clamp(value, min, max);
		}

		public static double Clamp(int value, int min, int max)
		{
			return Clamper<int>.Clamp(value, min, max);
		}

		public static double ClampToZeroToOne(double value)
		{
			return Clamper<double>.Clamp(value, 0, 255);
		}

		public static int ClampToZeroTo255(int value)
		{
			return Clamper<int>.Clamp(value, 0, 255);
		}

		public static int Floor(double value)
		{
			return Convert.ToInt16(Math.Floor(value));
		}

		public static double NormalizeWaveToZeroToOne(double value)
		{
			ThrowIfOutside<double>.ThrowIfOutsideRange(value, -1.0, 1.0);
			return (value + 1.0) / 2.0;
		}

		public static void ThrowErrorIfOutside(double value, double minInclusive, double maxInclusive)
		{
			ThrowIfOutside<double>.ThrowIfOutsideRange(value, minInclusive, maxInclusive);
		}

		public static void ThrowErrorIfOutside(int value, int minInclusive, int maxInclusive)
		{
			ThrowIfOutside<int>.ThrowIfOutsideRange(value, minInclusive, maxInclusive);
		}
	}

	class ThrowIfOutside<T> where T : IComparable<T>
	{
		public static T ThrowIfOutsideRange(T value, T minInclusive, T maxInclusive)
		{
			if (value.CompareTo(minInclusive) <= -1)
			{
				throw new ArgumentException("value must be greater than or equal to " + minInclusive);
			}

			if (value.CompareTo(maxInclusive) >= 1)
			{
				throw new ArgumentException("value must be less than or equal to " + maxInclusive);
			}

			return value;
		}
	}

	class Clamper<T> where T :IComparable<T>
	{
		public static T Clamp(T value, T min, T max)
		{
			if (value.CompareTo(min) <= -1)
			{
				value = min;
			}

			if (value.CompareTo(max) >= 1)
			{
				value = max;
			}

			return value;
		}
	}
}