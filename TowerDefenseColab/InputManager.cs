using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TowerDefenseColab.GameBusHere;
using TowerDefenseColab.GameBusHere.Messages;

namespace TowerDefenseColab
{
    public class InputManager
    {
        private readonly Dictionary<Keys, bool> _keyStates = new Dictionary<Keys, bool>();
        private readonly GameBus _bus;

        public delegate void OnMouseActionHandler(MouseEventArgs e);

        public event OnMouseActionHandler OnMouseDragged;
        public event OnMouseActionHandler OnMouseReleased;

        private Func<Point> _getMousePointFunction;

        public InputManager(GameBus bus)
        {
            _bus = bus;
        }


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

            _bus.Publish(new KeyReleased(key));
        }

        public bool GetKeyState(Keys key)
        {
            bool downState;
            _keyStates.TryGetValue(key, out downState);
            return downState;
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