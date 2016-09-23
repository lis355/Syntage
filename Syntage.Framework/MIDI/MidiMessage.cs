using Jacobi.Vst.Core;

namespace Syntage.Framework.MIDI
{
    public class MidiMessage
    {
        private readonly VstMidiEvent _vstMidiEvent;

        public MidiMessage(VstMidiEvent vstMidiEvent)
        {
            _vstMidiEvent = vstMidiEvent;
        }

        public byte StatusByte { get { return _vstMidiEvent.Data[0]; } }
        public byte Data1 { get { return _vstMidiEvent.Data[1]; } }
        public byte Data2 { get { return _vstMidiEvent.Data[2]; } }

        public bool HasStatusByte(byte status)
        {
            return (StatusByte & 0xF0) == status;
        }
    }
}
