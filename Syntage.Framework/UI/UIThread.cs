using System;
using System.Collections.Generic;
using System.Windows;

namespace Syntage.Framework.UI
{
    public class UIThread
    {
        public static readonly UIThread Instance = new UIThread();

        private UIThread()
        {
        }

		private readonly List<Action> _uiActions = new List<Action>();
		
        public void InvokeUIAction(Action action)
        {
            _uiActions.Add(action);
        }

        public void Update()
        {
#if DEBUG
            try
            {
#endif
                while (_uiActions.Count > 0)
                {
                    _uiActions[0]();
                    _uiActions.RemoveAt(0);
                }
#if DEBUG
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + Environment.NewLine + exception.StackTrace);
                throw;
            }
#endif
        }
    }
}
