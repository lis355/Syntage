using System;
using System.Collections.Generic;
using Syntage.Framework.MIDI;
using Syntage.Plugin;

namespace Syntage.UI
{
    public interface IKey
    {
        void Press();
        void Release();

        event Action OnPressFromUI;
        event Action OnReleaseFromUI;
    }

    public class UIDispatcher
    {
        public static readonly UIDispatcher Instance = new UIDispatcher();

        private const int KKeyStartNum = 12;
        private Plugin.PluginController _pluginController;
        private readonly Dictionary<int, IKey> _keys = new Dictionary<int, IKey>(); 
         
		private UIDispatcher()
        {
        }

        public void SetPlugin(Plugin.PluginController pluginController)
        {
            _pluginController = pluginController;

            _pluginController.MidiListener.OnNoteOn += MidiListenerOnNoteOn;
            _pluginController.MidiListener.OnNoteOff += MidiListenerOnNoteOff;
        }

        public void UILog(string m)
        {
            if (_pluginController.View.Control != null)
                _pluginController.View.Control.LogLabel.Content = m;
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
            _pluginController.MidiListener.NoteReleasedFromUI(noteEvent);

            //UILog(string.Format("Key released {0}{1} frequency {2:F2} hZ", DSPFunctions.ToNoteName(noteEvent.Note), noteEvent.Octava,
            //	DSPFunctions.GetMidiNoteFrequency(noteEvent.Note, noteEvent.Octava)));
        }
        
        private void KeyOnPressFromUI(int num)
        {
	        var noteEvent = new MidiListener.NoteEventArgs {Note = num % 12, Octava = num / 12, Velocity = 127};
            _pluginController.MidiListener.NotePressedFromUI(noteEvent);

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
