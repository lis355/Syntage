using Syntage.Framework.MIDI;

namespace Syntage.Logic
{
    public class Input : AudioProcessorPart
    {
        private MidiListener.NoteEventArgs _lastNote;

        // Текущая нажатая нота. Синтезатор монофонический.
        public int? Note { get; private set; }

        public Input(AudioProcessor audioProcessor) :
            base(audioProcessor)
        {
            audioProcessor.PluginController.MidiListener.OnNoteOn += MidiListenerOnNoteOn;
            audioProcessor.PluginController.MidiListener.OnNoteOff += MidiListenerOnNoteOff;
        }

        private void MidiListenerOnNoteOn(object sender, MidiListener.NoteEventArgs e)
        {
            _lastNote = e;

            Note = e.NoteAbsolute;

            Processor.OscillatorA.StartNote(e.NoteAbsolute);
        }

        private void MidiListenerOnNoteOff(object sender, MidiListener.NoteEventArgs e)
        {
            if (_lastNote.Note == e.Note
                && _lastNote.Octava == e.Octava)
            {
                Note = null;
            }

            Processor.OscillatorA.FinishNote(e.NoteAbsolute);
        }
    }
}
