using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using NAudio.Wave;

namespace SimplyHost
{
    public class HostController
    {
        private VstPluginContext _plugin;
        private HostCommandStub _host;

        private readonly MainForm _form;
        private WaveOut _audioPlayer;
        private AudioGenerator _audioGenerator;

        public HostController(MainForm form)
        {
            _form = form;
        }

        private VstPluginContext OpenPlugin(string pluginPath)
        {
            try
            {
                _host = new HostCommandStub();

                var ctx = VstPluginContext.Create(pluginPath, _host);

                ctx.Set("PluginPath", pluginPath);
                ctx.Set("HostCmdStub", _host);

                ctx.PluginCommandStub.SetSampleRate(1000 * _host.GetSampleRate());

                ctx.PluginCommandStub.Open();

                return ctx;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return null;
        }

        public void Start()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length >= 2)
            {
                var pluginName = args[1];
                _plugin = OpenPlugin(Directory.GetCurrentDirectory() + "\\" + pluginName);
                if (_plugin != null)
                {
                    _form.Text = string.Format("{0} {1} {2} {3}",
                        _plugin.PluginCommandStub.GetEffectName(),
                        _plugin.PluginCommandStub.GetProductString(),
                        _plugin.PluginCommandStub.GetVendorString(),
                        _plugin.PluginCommandStub.GetVendorVersion());

                    _plugin.PluginCommandStub.MainsChanged(true);

                    Rectangle wndRect;
                    if (_plugin.PluginCommandStub.EditorGetRect(out wndRect))
                    {
                        var panelSize = _form.GetSizeFromClientSize(new Size(wndRect.Width, wndRect.Height));
                        panelSize.Height += _form.FMenu.Height;
                        _form.Size = panelSize;

                        _plugin.PluginCommandStub.EditorOpen(_form.PluginPanel.Handle);
                    }
                    else
                    {
                        _form.PluginPanel.AutoScroll = true;
                        for (int i = 0; i < _plugin.PluginInfo.ParameterCount; ++i)
                        {
                            var editor = new ParameterEditor(i, _plugin.PluginCommandStub);
                            editor.Location = new Point(0, i * editor.Height);
                            _form.PluginPanel.Controls.Add(editor);
                        }
                    }

                    _plugin.PluginCommandStub.MainsChanged(false);

                    _audioGenerator = new AudioGenerator(_host, _plugin);
                    _audioPlayer = new WaveOut();
                    _audioPlayer.DesiredLatency = 150;

                    _audioPlayer.Init(_audioGenerator);
                    _audioPlayer.Play();

	                //Plugin.PluginCommandStub.ProcessEvents(new VstEvent[]
	                //{
		            //    new VstMidiEvent(0, 0, 0, new byte[] {144, (byte)(60 + 9), 127, 0}, 0, 0)
	                //});
                }
            }

            if (_plugin == null)
            {
                MessageBox.Show("No plugin loaded.");
                _form.Close();
            }
        }

        public void Finish()
        {
            if (_plugin != null)
            {
                _audioPlayer.Stop();
                _audioPlayer.Dispose();

                _plugin.PluginCommandStub.EditorClose();
                _plugin.Dispose();
                _plugin = null;
            }
        }

        private readonly HashSet<Keys> _pressedKeys = new HashSet<Keys>(); 

	    public void KeyDown(KeyEventArgs keyEventArgs)
        {
            if (_pressedKeys.Contains(keyEventArgs.KeyCode))
                return;

	        _pressedKeys.Add(keyEventArgs.KeyCode);

            int note = GetKeyNote(keyEventArgs);
			if (note >= 0)
			{
				_plugin.PluginCommandStub.ProcessEvents(new VstEvent[]
				{
					new VstMidiEvent(0, 0, 0, new byte[] {144, (byte)(60 + note), 127, 0}, 0, 0)
				});
			}
		}

		public void KeyUp(KeyEventArgs keyEventArgs)
		{
		    _pressedKeys.Remove(keyEventArgs.KeyCode);

            int note = GetKeyNote(keyEventArgs);
			if (note >= 0)
			{
				_plugin.PluginCommandStub.ProcessEvents(new VstEvent[]
				{
					new VstMidiEvent(0, 0, 0, new byte[] {128, (byte)(60 + note), 127, 0}, 0, 0)
				});
			}
		}

		private static int GetKeyNote(KeyEventArgs keyEventArgs)
		{
			int note = -1;

			switch (keyEventArgs.KeyCode)
			{
				case Keys.Q:
					note = 0; break;
				case Keys.W:
					note = 2; break;
				case Keys.E:
					note = 4; break;
				case Keys.R:
					note = 5; break;
				case Keys.T:
					note = 7; break;
				case Keys.Y:
					note = 9; break;
				case Keys.U:
					note = 11; break;
				case Keys.I:
					note = 12; break;
			}

			return note;
		}

        public void SetBypass(bool bypass)
        {
            _plugin.PluginCommandStub.SetBypass(bypass);
        }
    }
}
