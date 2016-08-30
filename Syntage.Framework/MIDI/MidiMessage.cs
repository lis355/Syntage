using Jacobi.Vst.Core;

namespace Syntage.Framework.MIDI
{
    /*
    Voice Message           Status Byte      Data Byte1          Data Byte2
    -------------           -----------   -----------------   -----------------
    Note off                      8x      Key number          Note Off velocity
    Note on                       9x      Key number          Note on velocity
    Polyphonic Key Pressure       Ax      Key number          Amount of pressure
    Control Change                Bx      Controller number   Controller value
    Program Change                Cx      Program number      None
    Channel Pressure              Dx      Pressure value      None            
    Pitch Bend                    Ex      MSB                 LSB
    */

    public enum EMidiChannelMessage
    {
        NoteOff =                   0x80, 
        NoteOn =                    0x90,  
        PolyphonicKeyPressure =     0xA0,    
        ControlChange =             0xB0, 
        ProgramChange =             0xC0, 
        ChannelPressure =           0xD0,
        PitchBend =                 0xE0
    }

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

        public bool HasStatusByte(EMidiChannelMessage status)
        {
            return HasStatusByte((byte)status);
        }
    }
}
