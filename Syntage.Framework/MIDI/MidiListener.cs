using System;
using System.Linq;
using Jacobi.Vst.Core;
using Jacobi.Vst.Framework;

namespace Syntage.Framework.MIDI
{
    public class MidiListener : IVstMidiProcessor
    {
        public class NoteEventArgs : EventArgs
        {
            public int Note;
            public int Octava;
            public int Velocity;

            public int NoteAbsolute { get { return Note + Octava * 12; } }
        }
        
        public int ChannelCount
        {
            get { return 16; }
        }

        public event EventHandler<NoteEventArgs> OnNoteOn;
        public event EventHandler<NoteEventArgs> OnNoteOff;

        public void Process(VstEventCollection events)
        {
            foreach (var midiMessage in events
                .Where(x => x.EventType == VstEventTypes.MidiEvent)
                .Select(x => new MidiMessage((VstMidiEvent)x)))
            {
                bool off = false;
                bool on = false;
                
                if (midiMessage.HasStatusByte((byte)EMidiChannelMessage.NoteOff))
                {
                    off = true;
                }
                else if (midiMessage.HasStatusByte((byte)EMidiChannelMessage.NoteOn))
                {
                    if (midiMessage.Data2 == 0) // Velocity
                    {
                        off = true;
                    }
                    else
                    {
                        on = true;
                    }
                }

                if (!on
                    && !off)
                    return;

                var noteEventArgs = new NoteEventArgs
                {
                    Note = midiMessage.Data1 % 12,
                    Octava = midiMessage.Data1 / 12,
                    Velocity = midiMessage.Data2
                };
                
                if (on)
                    OnNoteOn?.Invoke(this, noteEventArgs);
                else if (off)
                    OnNoteOff?.Invoke(this, noteEventArgs);
            }
        }

        public void NotePressedFromUI(NoteEventArgs noteEventArgs)
        {
            OnNoteOn?.Invoke(this, noteEventArgs);
        }

        public void NoteReleasedFromUI(NoteEventArgs noteEventArgs)
        {
            OnNoteOff?.Invoke(this, noteEventArgs);
        }
    }
}
