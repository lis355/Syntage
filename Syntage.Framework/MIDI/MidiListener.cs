using System;
using System.Collections.Generic;
using System.Linq;
using CannedBytes.Midi.Message;
using Jacobi.Vst.Core;
using Jacobi.Vst.Framework;

namespace Syntage.Framework.MIDI
{
    public class MidiListener : IVstMidiProcessor
    {
        private readonly HashSet<int> _pressedNotes = new HashSet<int>(); 

        public class NoteEventArgs : EventArgs
        {
            public int Note;
            public int Octava;
            public int Velocity;

            public int NoteAbsolute { get { return Note + Octava * 12; } }

            public NoteEventArgs(int noteAbsolute, int velocity)
            {
                Note = noteAbsolute % 12;
                Octava = noteAbsolute / 12;
                Velocity = velocity;
            }
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
                if (midiMessage.HasStatusByte((byte)MidiChannelCommand.NoteOff))
                {
                    NoteOff(GetNoteEventArgsFromMessage(midiMessage));
                }
                else if (midiMessage.HasStatusByte((byte)MidiChannelCommand.NoteOn))
                {
                    if (midiMessage.Data2 == 0) // Velocity
                    {
                        NoteOff(GetNoteEventArgsFromMessage(midiMessage));
                    }
                    else
                    {
                        NoteOn(GetNoteEventArgsFromMessage(midiMessage));
                    }
                }
                else if (midiMessage.HasStatusByte((byte)MidiChannelCommand.ControlChange))
                {
                    if (midiMessage.Data1 == (byte)MidiControllerType.AllNotesOff
                        || midiMessage.Data1 == (byte)MidiControllerType.AllSoundOff)
                    {
                        var notes = new HashSet<int>(_pressedNotes);
                        foreach (var pressedNote in notes)
                            NoteOff(new NoteEventArgs(pressedNote, 0));
                    }
                }
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

        private static NoteEventArgs GetNoteEventArgsFromMessage(MidiMessage midiMessage)
        {
            return new NoteEventArgs(midiMessage.Data1, midiMessage.Data2);
        }

        private void NoteOn(NoteEventArgs noteEventArgs)
        {
            _pressedNotes.Add(noteEventArgs.NoteAbsolute);
            OnNoteOn?.Invoke(this, noteEventArgs);
        }

        private void NoteOff(NoteEventArgs noteEventArgs)
        {
            _pressedNotes.Remove(noteEventArgs.NoteAbsolute);
            OnNoteOff?.Invoke(this, noteEventArgs);
        }
    }
}
