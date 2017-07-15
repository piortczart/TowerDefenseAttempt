using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TowerDefenseColab
{
    public class InputManager
    {
        private readonly Dictionary<Keys, bool> _keyStates = new Dictionary<Keys, bool>();

        public delegate void OnClickHandler(MouseEventArgs e);
        public delegate void OnMouseActionHandler(MouseEventArgs e);

        public delegate void OnKeyReleasedHandler(Keys key);

        public event OnClickHandler OnClick;
        public event OnKeyReleasedHandler OnKeyReleased;
        public event OnMouseActionHandler OnMouseDragged;
        public event OnMouseActionHandler OnMouseReleased;

        private Func<Point> _getMousePointFunction;

        public void SetMousePointFunction(Func<Point> mousePointFunc)
        {
            _getMousePointFunction = mousePointFunc;
        }

        public Point GetMousePosition()
        {
            if (_getMousePointFunction != null)
            {
                return _getMousePointFunction();
            }
            return new Point();
        }

        public void KeyPressed(Keys key)
        {
            _keyStates[key] = true;
        }

        public void KeyReleased(Keys key)
        {
            _keyStates[key] = false;

            OnKeyReleased?.Invoke(key);
        }

        public bool GetKeyState(Keys key)
        {
            bool downState;
            _keyStates.TryGetValue(key, out downState);
            return downState;
        }

        public void MouseClicked(MouseEventArgs e)
        {
            OnClick?.Invoke(e);
        }

        internal void MouseRelease(MouseEventArgs e)
        {
            OnMouseReleased?.Invoke(e);
        }

        internal void MouseDrag(MouseEventArgs e)
        {
            OnMouseDragged?.Invoke(e);
        }
    }
}