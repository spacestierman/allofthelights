using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllOfTheLights
{
	class DmxProgram
	{
		private static uint _currentVolume;

		public delegate Color RefreshHandler(DateTime now, TimeSpan timeSinceStart, TimeSpan timeSinceLastRefresh);
		public event RefreshHandler OnRefresh;

		private Dmx512Controller _controller;
		private Thread _updateThread;
		private bool _shouldBeRunning;

		private string _connectedSerialNumber;
		MMDevice _output;

		public float SystemVolume { get; private set; }

		public DmxProgram()
		{
			_controller = new Dmx512Controller();
			_connectedSerialNumber = null;
			_shouldBeRunning = false;
			
		}

		public void Start()
		{
			if (_controller.IsConnected)
			{
				_controller.Disconnect();
			}

			IEnumerable<string> serialNumbers = _controller.GetAvailableSerialNumbers();
			if (serialNumbers.Count() <= 0)
			{
				throw new Exception("No DMX devices found.");
			}
			
			_shouldBeRunning = true;
			_connectedSerialNumber = serialNumbers.First();
			_controller.OnDeviceConnected += OnControllerConnected;
			_controller.Connect(_connectedSerialNumber);

			MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
			MMDeviceCollection collection = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
			_output = collection.FirstOrDefault();
		}

		public bool IsRunning
		{
			get { return _controller.IsConnected; }
		}

		public string ConnectedToSerialNumber
		{
			get { return _connectedSerialNumber; }
		}

		public void Stop()
		{
			if (_controller.IsConnected)
			{
				_controller.Disconnect();
			}

			_connectedSerialNumber = null;
		}

		private void OnControllerConnected()
		{
			_updateThread = new Thread(new ThreadStart(UpdateThread));
			_updateThread.Start();
		}

		private void UpdateThread()
		{
			DateTime startTimestamp = DateTime.Now;
			DateTime lastRefreshTimestamp = DateTime.Now;
			while (_shouldBeRunning)
			{
				DateTime now = DateTime.Now;
				TimeSpan timeSinceStart = now.Subtract(startTimestamp);
				TimeSpan timeSinceLastRefresh = now.Subtract(lastRefreshTimestamp);

				if (_output != null)
				{
					SystemVolume = _output.AudioMeterInformation.MasterPeakValue;
				}
				
				if (OnRefresh != null)
				{
					Color newColor = OnRefresh(now, timeSinceStart, timeSinceLastRefresh);
					if (_controller.IsConnected)
					{
						_controller.SetColorAtAddress(1, newColor.Red, newColor.Green, newColor.Blue);
					}
				}

				lastRefreshTimestamp = now;

				Thread.Sleep(1);
			}
		}
	}

	class Color
	{
		private int _red;
		private int _green;
		private int _blue;

		public Color(int red, int green, int blue)
		{
			Red = red;
			Green = green;
			Blue = blue;
		}

		public Color(float red, float green, float blue)
		{
			Red = Convert.ToInt16(Math.Floor(red * 255.0));
			Green = Convert.ToInt16(Math.Floor(green * 255.0));
			Blue = Convert.ToInt16(Math.Floor(blue * 255.0));
		}

		public Color(double red, double green, double blue)
		{
			Red = Convert.ToInt16(Math.Floor(red * 255.0));
			Green = Convert.ToInt16(Math.Floor(green * 255.0));
			Blue = Convert.ToInt16(Math.Floor(blue * 255.0));
		}

		public int Red
		{
			get
			{
				return _red;
			}

			set
			{
				ThrowErrorIfInvalidValue(value);
				_red = value;
			}
		}

		public int Green
		{
			get
			{
				return _green;
			}

			set
			{
				ThrowErrorIfInvalidValue(value);
				_green = value;
			}
		}

		public int Blue
		{
			get
			{
				return _blue;
			}

			set
			{
				ThrowErrorIfInvalidValue(value);
				_blue = value;
			}
		}

		public void add(int value)
		{
			MathUtils.ThrowErrorIfOutside(value, 0, 255);

			Red = MathUtils.ClampToZeroTo255(Red + value);
			Green = MathUtils.ClampToZeroTo255(Green + value);
			Blue = MathUtils.ClampToZeroTo255(Blue + value);
		}

		public void add(double value)
		{
			MathUtils.ThrowErrorIfOutside(value, 0.0, 1.0);

			int add = MathUtils.Floor(value / 255.0);
			Red = MathUtils.ClampToZeroTo255(Red + add);
			Green = MathUtils.ClampToZeroTo255(Green + add);
			Blue = MathUtils.ClampToZeroTo255(Blue + add);
		}

		public void Multiply(double scaleBy)
		{
			Red = MathUtils.ClampToZeroTo255(MathUtils.Floor(Red * scaleBy));
			Green = MathUtils.ClampToZeroTo255(MathUtils.Floor(Green * scaleBy));
			Blue = MathUtils.ClampToZeroTo255(MathUtils.Floor(Blue * scaleBy));
		}

		private void ThrowErrorIfInvalidValue(int value)
		{
			if (value < 0 || value > 255)
			{
				throw new ArgumentException("Value must be between 0 and 255!");
			}
		}
	}
}
