using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TotallyFair.Managers
{
    public class InputManager
    {
        public enum MouseClickState { LEFT, RIGHT };
        private struct GameAction
        {
            public bool Continuous;
            public bool UseKey; //Use Key provided if TRUE, else use MouseClickState
            public Keys Key;
            public MouseClickState ClickState;
            public Action Handler;

            public GameAction(bool continuous, Keys key, Action handler)
            {
                Continuous = continuous;
                UseKey = true;
                Key = key;
                ClickState = MouseClickState.LEFT;
                Handler = handler;
            }

            public GameAction(bool continuous, MouseClickState clickState, Action handler)
            {
                Continuous = continuous;
                UseKey = false;
                Key = Keys.A;
                ClickState = clickState;
                Handler = handler;
            }
        }

        private KeyboardState _KbState, _KbState_Last = Keyboard.GetState();
        private MouseState _MState, _MState_Last = Mouse.GetState();
        private Queue<GameAction> _Actions = new Queue<GameAction>();
        private Dictionary<Keys, List<GameAction>> _OnKeyPress = new Dictionary<Keys, List<GameAction>>();
        private Dictionary<MouseClickState, List<GameAction>> _OnMouseClick = new Dictionary<MouseClickState, List<GameAction>>();

        public void AddKeyPressHandler(bool continuous, Action handler, Keys key)
        {
            if (!_OnKeyPress.ContainsKey(key))
                _OnKeyPress.Add(key, new List<GameAction>());
            _OnKeyPress[key].Add(new GameAction(continuous, key, handler));
        }

        public void AddMouseHandler(bool continuous, Action handler, MouseClickState currentMouse)
        {
            if (!_OnMouseClick.ContainsKey(currentMouse))
                _OnMouseClick.Add(currentMouse, new List<GameAction>());
            _OnMouseClick[currentMouse].Add(new GameAction(continuous, currentMouse, handler));
        }

        public void AddToQueue(Keys key)
        {
            if (!_OnKeyPress.ContainsKey(key)) return;
            foreach (GameAction MyAction in _OnKeyPress[key]) _Actions.Enqueue(MyAction);
        }

        public void Update() //Update function called every game update
        {

            _KbState = Keyboard.GetState();
            _MState = Mouse.GetState();
            Queue<GameAction> ActionBuffer = new Queue<GameAction>();

            //Execute All Queued Actions
            while (_Actions.Count > 0)
            {
                GameAction newAction = _Actions.Peek();
                if (newAction.Continuous && _KbState.IsKeyDown(newAction.Key))
                {
                    _Actions.Dequeue().Handler(); //Execute Action and Dequeue
                    ActionBuffer.Enqueue(newAction); //Key is continuous type and key is still pressed, add to Buffer to be repeated next cycle.
                }
                else _Actions.Dequeue().Handler(); //Execute Action and Dequeue
            }
            //Ensure Continous Actions are Repeated Next Cycle
            _Actions = ActionBuffer;

            //Enqueue Actions for new keyboard presses
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
                if (_KbState.IsKeyDown(key) && !_KbState_Last.IsKeyDown(key) && _OnKeyPress.ContainsKey(key))
                    foreach (var GA in _OnKeyPress[key]) _Actions.Enqueue(GA);
            //Enqueue Actions for new Left Mouse Clicks
            if (_MState.LeftButton.Equals(ButtonState.Pressed) && _OnMouseClick.ContainsKey(MouseClickState.LEFT))
                foreach (var GA in _OnMouseClick[MouseClickState.LEFT]) _Actions.Enqueue(GA);
            //Enqueue Actions for new Right Mouse Clicks
            if (_MState.RightButton.Equals(ButtonState.Pressed) && _OnMouseClick.ContainsKey(MouseClickState.RIGHT))
                foreach (var GA in _OnMouseClick[MouseClickState.RIGHT]) _Actions.Enqueue(GA);

            //Set Keyboard & Mouse state for next cycle
            _KbState_Last = _KbState;
            _MState_Last = _MState;
        }
    }
}
