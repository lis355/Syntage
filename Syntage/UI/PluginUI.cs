using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Syntage.Framework.Midi;
using Syntage.Framework.UI;
using Syntage.Plugin;
using Syntage.Framework.Parameters;

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
        private readonly Dictionary<int, Key> _keys = new Dictionary<int, Key>();

        public PluginController PluginController { get; set; }

        private PluginUI()
        {
            Framework.Tools.Log.Instance.OnLog += Log;
        }

        public override void Open(IntPtr hWnd)
        {
            base.Open(hWnd); 
            
            BindParameters(PluginController.ParametersManager.Parameters); 
            FillLFOParameters(PluginController.ParametersManager.Parameters); 
            
            Control.Oscilloscope.SetOscillogpaph(PluginController.AudioProcessor.Oscillograph);
            
            PluginController.MidiListener.OnNoteOn += MidiListenerOnNoteOn; 
            PluginController.MidiListener.OnNoteOff += MidiListenerOnNoteOff;
        }

        public override void Close()
        {
            base.Close();

            _keys.Clear();
        }

        public override void ProcessIdle()
        {
            base.ProcessIdle();

            UIThread.Instance.InvokeUIAction(() => Control.Oscilloscope.Update());
        }
        
        public void Log(string m)
        {
            UIThread.Instance.InvokeUIAction(() => _instance.Control.LogLabel.Content = m);
        }

        public void RegisterPianoKey(Key key)
        {
            UIThread.Instance.InvokeUIAction(() =>
            {
                int number = key.KeyNumber;
                _keys.Add(number, key);

                key.OnPressFromUI += () => KeyOnPressFromUI(KKeyStartNum + number);
                key.OnReleaseFromUI += () => KeyOnReleaseFromUI(KKeyStartNum + number);
            });
        }

		private void BindParameters(IEnumerable<Parameter> parameters)
		{
			foreach (var parameter in parameters)
			{
				var name = parameter.Name;
				var element = Control.FindName(name);
				var parameterController = element as IUIParameterController;
				if (parameterController != null)
				{
					parameterController.SetParameter(parameter);
					parameterController.UpdateController();

					parameter.OnValueChange += changeType => UIThread.Instance.InvokeUIAction(() => parameterController.UpdateController());
				}
			}
		}

        private void FillLFOParameters(IEnumerable<Parameter> parameters)
        {
            var paramBox = new TextBlock();
            paramBox.Text = "--";
            Control.LFOParamsList.Items.Add(paramBox);
            Control.LFOParamsList.SelectedIndex = 0;

            foreach (var parameter in parameters.Where(parameter => parameter.CanBeAutomated))
            {
                paramBox = new TextBlock();
                paramBox.Text = parameter.Name;
                Control.LFOParamsList.Items.Add(paramBox);
            }

            Control.LFOParameterChanged += x =>
            {
                var parameter = PluginController.ParametersManager.Parameters.FirstOrDefault(y => y.Name == x.Text);
                PluginController.AudioProcessor.LFOModifier.Target = parameter;
            };
        }

        private void KeyOnReleaseFromUI(int num)
        {
            var noteEvent = new MidiListener.NoteEventArgs(num, 127);
            PluginController.MidiListener.NoteReleasedFromUI(noteEvent);

            //UILog(string.Format("Key released {0}{1} frequency {2:F2} hZ", DSPFunctions.ToNoteName(noteEvent.Note), noteEvent.Octava,
            //	DSPFunctions.GetMidiNoteFrequency(noteEvent.Note, noteEvent.Octava)));
        }

        private void KeyOnPressFromUI(int num)
        {
            var noteEvent = new MidiListener.NoteEventArgs(num, 127);
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
