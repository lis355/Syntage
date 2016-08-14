using System;
using System.Collections.Generic;
using Syntage.Framework.MIDI;
using Syntage.Framework.UI;
using Syntage.Plugin;

namespace Syntage.UI
{
    public class PluginUI : PluginWpfUI<View>
    {
        private static PluginUI _instance;
        public static PluginUI Instance
        {
            get { return _instance ?? (_instance = new PluginUI()); }
        }

        private const int KKeyStartNum = 12;
        private readonly Dictionary<int, IKey> _keys = new Dictionary<int, IKey>();

        public PluginController PluginController { get; set; }

        private PluginUI()
        {
            Framework.Tools.Log.Instance.OnLog += Log;
        }

        public override void Open(IntPtr hWnd)
        {
            base.Open(hWnd);
            
            BindParameters(PluginController.ParametersManager.Parameters);
            
            Control.Oscilloscope.SetOscillogpaph(PluginController.AudioProcessor.Oscillograph);

            PluginController.MidiListener.OnNoteOn += MidiListenerOnNoteOn;
            PluginController.MidiListener.OnNoteOff += MidiListenerOnNoteOff;
        }

        public override void ProcessIdle()
        {
            base.ProcessIdle();

            Control.Oscilloscope.Update();
        }
        
        public void Log(string m)
        {
            UIThread.Instance.InvokeUIAction(() => _instance.Control.LogLabel.Content = m);
        }

        public void RegisterNextPianoKey(IKey key)
        {
            int num = _keys.Count;
            _keys.Add(num, key);

            key.OnPressFromUI += () => KeyOnPressFromUI(num + KKeyStartNum);
            key.OnReleaseFromUI += () => KeyOnReleaseFromUI(num + KKeyStartNum);
        }

        private void KeyOnReleaseFromUI(int num)
        {
            var noteEvent = new MidiListener.NoteEventArgs { Note = num % 12, Octava = num / 12, Velocity = 127 };
            PluginController.MidiListener.NoteReleasedFromUI(noteEvent);

            //UILog(string.Format("Key released {0}{1} frequency {2:F2} hZ", DSPFunctions.ToNoteName(noteEvent.Note), noteEvent.Octava,
            //	DSPFunctions.GetMidiNoteFrequency(noteEvent.Note, noteEvent.Octava)));
        }

        private void KeyOnPressFromUI(int num)
        {
            var noteEvent = new MidiListener.NoteEventArgs { Note = num % 12, Octava = num / 12, Velocity = 127 };
            PluginController.MidiListener.NotePressedFromUI(noteEvent);

            //UILog(string.Format("Key pressed {0}{1} frequency {2:F2} hZ", DSPFunctions.ToNoteName(noteEvent.Note), noteEvent.Octava,
            //	DSPFunctions.GetMidiNoteFrequency(noteEvent.Note, noteEvent.Octava)));
        }

        private void MidiListenerOnNoteOff(object sender, MidiListener.NoteEventArgs e)
        {
            UIThread.Instance.InvokeUIAction(() => _keys[e.NoteAbsolute - KKeyStartNum].Release());
        }

        private void MidiListenerOnNoteOn(object sender, MidiListener.NoteEventArgs e)
        {
            UIThread.Instance.InvokeUIAction(() => _keys[e.NoteAbsolute - KKeyStartNum].Press());
        }
    }
}
