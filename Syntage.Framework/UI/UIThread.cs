using System;
using System.Collections.Generic;

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
            while (_uiActions.Count > 0)
            {
                _uiActions[0]();
                _uiActions.RemoveAt(0);
            }
        }
    }
}
