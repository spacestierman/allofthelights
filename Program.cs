using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllOfTheLights
{
	class Program
	{
		private static DmxProgram _dmxProgram;

		static void Main(string[] args)
		{
			_dmxProgram = new DmxProgram();
			_dmxProgram.OnRefresh += RefreshToRed;

			try
			{
				_dmxProgram.Start();
			}
			catch (Exception e)
			{
				Console.WriteLine("Unable to start the DMX program.");
				return;
			}

			Console.WriteLine("Connected to " + _dmxProgram.ConnectedToSerialNumber);
			Console.WriteLine("Press q to quit.");

			bool shouldBeRunning = true;
			while (shouldBeRunning)
			{
				ConsoleKeyInfo key = Console.ReadKey();
				switch(key.Key)
				{
					case ConsoleKey.Q:
						_dmxProgram.Stop();
						shouldBeRunning = false;
						Console.WriteLine("uiting...");
						break;
					default:
						Console.WriteLine(" is an unrecognized key.");
						break;
				}
			}

			_dmxProgram.Stop();
			Console.WriteLine("Done.");
		}

		private static Color RefreshToRed(DateTime now, TimeSpan timeSinceStart, TimeSpan timeSinceLastRefresh)
		{
			Color color = new Color(1.0, 0.0, 0.0);
			return color;
		}
















		private static Color RefreshWithRedPhasing(DateTime now, TimeSpan timeSinceStart, TimeSpan timeSinceLastRefresh)
		{
			Color color = new Color(
				MathUtils.NormalizeWaveToZeroToOne(Math.Sin(timeSinceStart.TotalSeconds)), // We need a number between 0-1 for this constructor.  Sine waves go from -1 to +1.  So we normalize it.
				0.0,
				0.0
			);
			return color;
		}

		private static Color RefreshWithColorPhasing(DateTime now, TimeSpan timeSinceStart, TimeSpan timeSinceLastRefresh)
		{
			Color color = new Color(
				MathUtils.NormalizeWaveToZeroToOne(Math.Sin(timeSinceStart.TotalSeconds)),
				MathUtils.NormalizeWaveToZeroToOne(Math.Cos(timeSinceStart.TotalSeconds)),
				MathUtils.NormalizeWaveToZeroToOne(Math.Cos(timeSinceStart.TotalSeconds))
			);
			return color;
		}

		private static Color RefreshWithColorPhasingAndVolume(DateTime now, TimeSpan timeSinceStart, TimeSpan timeSinceLastRefresh)
		{
			Color color = new Color(
				MathUtils.NormalizeWaveToZeroToOne(Math.Sin(timeSinceStart.TotalSeconds)),
				MathUtils.NormalizeWaveToZeroToOne(Math.Cos(timeSinceStart.TotalSeconds)),
				MathUtils.NormalizeWaveToZeroToOne(Math.Cos(timeSinceStart.TotalSeconds))
			);
			color.Multiply(0.25 + _dmxProgram.SystemVolume); // By multiplying our values by the system volume (0-1) we can control brightness.
			return color;
		}
	}
}