using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TotallyFair
{
    internal class Input
    {
        public enum MouseClickState { LEFT, RIGHT };
        private struct GameAction
        {
            public bool Continuous;
            public bool UseKey; //Use Key provided if TRUE, else use MouseClickState
            public Keys Key;
            public MouseClickState ClickState;
            public Action Handler;

            public GameAction(bool Continuous, Keys Key, Action Handler)
            {
                this.Continuous = Continuous;
                this.UseKey = true;
                this.Key = Key;
                this.ClickState = MouseClickState.LEFT;
                this.Handler = Handler;
            }

            public GameAction(bool Continuous, MouseClickState ClickState, Action Handler)
            {
                this.Continuous = Continuous;
                this.UseKey = false;
                this.Key = Keys.A;
                this.ClickState = ClickState;
                this.Handler = Handler;
            }
        }

        private KeyboardState KbState, KbState_Last = Keyboard.GetState();
        private MouseState MState, MState_Last = Mouse.GetState();
        private Queue<GameAction> Actions = new Queue<GameAction>();
        private Dictionary<Keys, List<GameAction>> OnKeyPress = new Dictionary<Keys, List<GameAction>>();
        private Dictionary<MouseClickState, List<GameAction>> OnMouseClick = new Dictionary<MouseClickState, List<GameAction>>();

        public void AddKeyPressHandler(bool Continuous, Action Handler, Keys Key)
        {
            if (!OnKeyPress.ContainsKey(Key))
                OnKeyPress.Add(Key, new List<GameAction>());
            OnKeyPress[Key].Add(new GameAction(Continuous, Key, Handler));
        }

        public void AddMouseHandler(bool Continuous, Action Handler, MouseClickState CurrentMouse)
        {
            if (!OnMouseClick.ContainsKey(CurrentMouse))
                OnMouseClick.Add(CurrentMouse, new List<GameAction>());
            OnMouseClick[CurrentMouse].Add(new GameAction(Continuous, CurrentMouse, Handler));
        }

        public void AddToQueue(Keys Key)
        {
            if (!OnKeyPress.ContainsKey(Key)) return;
            foreach (GameAction MyAction in OnKeyPress[Key]) Actions.Enqueue(MyAction);
        }

        public void Update() //Update function called every game update
        {

            KbState = Keyboard.GetState();
            MState = Mouse.GetState();
            Queue<GameAction> ActionBuffer = new Queue<GameAction>();

            //Execute All Queued Actions
            while (Actions.Count > 0)
            {
                GameAction NewAction = Actions.Peek();
                if (NewAction.Continuous && KbState.IsKeyDown(NewAction.Key))
                {
                    Actions.Dequeue().Handler(); //Execute Action and Dequeue
                    ActionBuffer.Enqueue(NewAction); //Key is continuous type and key is still pressed, add to Buffer to be repeated next cycle.
                }
                else Actions.Dequeue().Handler(); //Execute Action and Dequeue
            }
            //Ensure Continous Actions are Repeated Next Cycle
            Actions = ActionBuffer;

            //Enqueue Actions for new keyboard presses
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
                if (KbState.IsKeyDown(key) && !KbState_Last.IsKeyDown(key) && OnKeyPress.ContainsKey(key)) 
                    foreach (var GA in OnKeyPress[key]) Actions.Enqueue(GA);
            //Enqueue Actions for new Left Mouse Clicks
            if (MState.LeftButton.Equals(ButtonState.Pressed) && OnMouseClick.ContainsKey(MouseClickState.LEFT))
                foreach (var GA in OnMouseClick[MouseClickState.LEFT]) Actions.Enqueue(GA);
            //Enqueue Actions for new Right Mouse Clicks
            if (MState.RightButton.Equals(ButtonState.Pressed) && OnMouseClick.ContainsKey(MouseClickState.RIGHT))
                foreach (var GA in OnMouseClick[MouseClickState.RIGHT]) Actions.Enqueue(GA);

            //Set Keyboard & Mouse state for next cycle
            KbState_Last = KbState;
            MState_Last = MState;
        }
    }
}
