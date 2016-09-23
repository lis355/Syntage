using System.Collections.Generic;
using System.Threading;
using Syntage.Framework.Audio;
using Syntage.Framework.Parameters;

namespace Syntage.Logic
{
	public class Oscillograph : SyntageAudioProcessorComponentWithParameters<AudioProcessor>, IProcessor
	{
		public class Buffer
		{
			private int _size;
			private readonly List<double> _values = new List<double>();

			public IEnumerable<double> Values
			{
				get { return _values; }
			}

			public int Size
			{
				set
				{
					if (value == _size)
						return;

					_size = value;

					UpdateSize();
				}
			}

			public void AddValue(double f)
			{
				_values.Add(f);

				UpdateSize();
			}

			private void UpdateSize()
			{
				while (_values.Count > _size)
					_values.RemoveAt(0);
			}
		}

		private readonly Mutex _buffersMutex = new Mutex();

		public readonly Buffer LeftBuffer = new Buffer();
		public readonly Buffer RightBuffer = new Buffer();

		public void BlockBuffers()
		{
			_buffersMutex.WaitOne();
		}

		public void ReleaseBuffers()
		{
			_buffersMutex.ReleaseMutex();
		}

		public bool IsDirty { get; set; }

		public int WindowSize
		{
			set
			{
				LeftBuffer.Size = value;
				RightBuffer.Size = value;
			}
		}

		public EnumParameter<EPowerStatus> Power { get; private set; }
		public IntegerParameter Step { get; private set; }

		public Oscillograph(AudioProcessor audioProcessor) : base(audioProcessor)
		{
		}

		public override IEnumerable<Parameter> CreateParameters(string parameterPrefix)
		{
			Power = new EnumParameter<EPowerStatus>(parameterPrefix + "Pwr", "Power", "", false);
			Step = new IntegerParameter(parameterPrefix + "Stp", "Oscillograph Step", "Step", 1, 10, 1, false);
			
			return new List<Parameter> {Power, Step};
		}

		public void Process(IAudioStream stream)
		{
			if (Power.Value == EPowerStatus.Off)
				return;

			BlockBuffers();

			var leftChannel = stream.Channels[0];
			var rigthChannel = stream.Channels[1];
			var lenght = stream.Length;

			for (int i = 0; i < lenght; i += Step.Value)
			{
				LeftBuffer.AddValue(leftChannel.Samples[i]);
				RightBuffer.AddValue(rigthChannel.Samples[i]);
			}

			IsDirty = true;

			ReleaseBuffers();
		}
	}
}
