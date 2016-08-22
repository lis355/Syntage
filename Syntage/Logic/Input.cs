using System;
using System.Collections.Generic;
using Syntage.Framework.MIDI;

namespace Syntage.Logic
{
    public class Input : AudioProcessorPart
    {
        private readonly List<MidiListener.NoteEventArgs> _pressedNotes = new List<MidiListener.NoteEventArgs>();

        public IEnumerable<MidiListener.NoteEventArgs> PressedNotes
        {
            get { return _pressedNotes; }
        }

        public int PressedNotesCount
        {
            get { return _pressedNotes.Count; }
        }

        public event Action OnPressedNotesChanged;

        public Input(AudioProcessor audioProcessor) :
            base(audioProcessor)
        {
            audioProcessor.PluginController.MidiListener.OnNoteOn += MidiListenerOnNoteOn;
            audioProcessor.PluginController.MidiListener.OnNoteOff += MidiListenerOnNoteOff;
        }

        private void MidiListenerOnNoteOn(object sender, MidiListener.NoteEventArgs e)
        {
            if (_pressedNotes.Find(x => x.NoteAbsolute == e.NoteAbsolute) != null)
                throw new ArgumentException();

            _pressedNotes.Add(e);

            OnPressedNotesChanged?.Invoke();
        }

        private void MidiListenerOnNoteOff(object sender, MidiListener.NoteEventArgs e)
        {
            _pressedNotes.RemoveAll(x => x.NoteAbsolute == e.NoteAbsolute);

            OnPressedNotesChanged?.Invoke();
        }
    }
}
