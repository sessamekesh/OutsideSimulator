﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using OutsideSimulator.Commands.Events;
using OutsideSimulator.Renderable;

using SlimDX;

namespace OutsideSimulator.Scene
{
    public struct Submenu
    {
        private MenuButton _button;
        public MenuButton Button { get { return _button; } private set { _button = value; } }
        private Menu _menu;
        public Menu Menu { get { return _menu; } private set { _menu = value; } }

        public Submenu(MenuButton btn, Menu mnu) { _button = btn;  _menu = mnu; }
    }

    public struct MenuAction
    {
        private MenuButton _btn;
        public MenuButton Button { get { return _btn; } private set { _btn = value; } }
        private Action _act;
        public Action Action { get { return _act; } private set { _act = value; } }

        public MenuAction(MenuButton btn, Action actn) { _btn = btn;  _act= actn; }
    }

    /// <summary>
    /// Represents all the data required for a menu to function
    ///  1) Menu rendering information
    ///  2) Submenus and buttons to access them
    /// </summary>
    public class Menu : IRenderable, MouseDownSubscriber, MouseUpSubscriber, MouseMoveSubscriber, KeyDownSubscriber, KeyUpSubscriber
    {
        #region Properties
        public string MenuFilename { get; private set; }
        public List<Submenu> Submenus { get; protected set; }
        public List<MenuAction> Actions { get; protected set; }
        public Menu ActiveMenu { get; protected set; }

        public MenuButton[] MenuButtons
        {
            get
            {
                return (ActiveMenu ?? this).Submenus.Select((x) => x.Button)
                    .Concat((ActiveMenu ?? this).Actions.Select((y) => y.Button))
                    .ToArray();
            }
        }

        public Keys? Hotkey;
        #endregion

        #region Logic
        private bool _isHotkeyPressed;
        #endregion

        #region Constants
        private static readonly float amt = 0.72f;
        private static readonly float _mbStartY = 0.52f;
        private static readonly float _mbStartX = -0.6f;
        private static readonly float _mbStopX = 0.6f;
        private static readonly float _mbSpacingBetween = -0.19f;
        private static readonly float _mbHeight = -0.164f;
        #endregion

        /// <summary>
        /// Create a new menu instance, including subscribing to keypresses and all.
        /// </summary>
        /// <param name="Title">The title of this menu</param>
        /// <param name="Hotkey">The hotkey which opens/closes this menu (null for no key)</param>
        public Menu(string MenuFilename, Keys? Hotkey)
        {
            // Set up member properties
            ActiveMenu = null; // Null if no menu active - otherwise, the currently active menu or submenu
            this.MenuFilename = MenuFilename;
            Submenus = new List<Submenu>();
            Actions = new List<MenuAction>();
            this.Hotkey = Hotkey;

            // Set up logic properties
            _isHotkeyPressed = false;

            // Subscribe to our main application...
            OutsideSimulatorApp.GetInstance().Subscribe(this);
        }

        public void ClearMenu()
        {
            ActiveMenu = null;
        }

        public void AddSubmenu(string MenuButtonTexturePath, Menu Submenu)
        {
            Submenus.Add(new Submenu(
                new MenuButton(MenuButtonTexturePath, new SlimDX.Vector2(
                    _mbStartX,
                    _mbStartY + (Actions.Count + Submenus.Count) * _mbSpacingBetween
                ), new SlimDX.Vector2(
                    _mbStopX,
                    _mbStartY + (Actions.Count + Submenus.Count) * _mbSpacingBetween + _mbHeight
                )),
                Submenu
            ));
        }

        public void AddAction(string MenuButtonTexturePath, Action Action)
        {
            Actions.Add(new MenuAction(
                new MenuButton(MenuButtonTexturePath, new SlimDX.Vector2(
                    _mbStartX,
                    _mbStartY + (Actions.Count + Submenus.Count) * _mbSpacingBetween
                ), new SlimDX.Vector2(
                    _mbStopX,
                    _mbStartY + (Actions.Count + Submenus.Count) * _mbSpacingBetween + _mbHeight
                )),
                Action
            ));
        }

        #region IRenderable
        public uint[] GetIndexList(string EffectName)
        {
            switch (EffectName)
            {
                case Effects.EffectsGlobals.MenuEffectName:
                    if (ActiveMenu != null)
                    {
                        return new uint[] {
                            0, 2, 1,
                            3, 2, 0
                        };
                    }
                    else
                    {
                        return new uint[] { };
                    }
                default:
                    throw new Renderable.CannotResolveIndicesException(EffectName, RenderableName);
            }
        }

        public object[] GetVertexList(string EffectName)
        {
            switch (EffectName)
            {
                case Effects.EffectsGlobals.MenuEffectName:
                    if (ActiveMenu != null)
                    {
                        return (new Effects.MenuEffect.MenuEffectVertex[] {
                            new Effects.MenuEffect.MenuEffectVertex() { ScreenSpacePos = new SlimDX.Vector2(-amt, -amt), Texcoord = new SlimDX.Vector2(0.0f, 1.0f) },
                            new Effects.MenuEffect.MenuEffectVertex() { ScreenSpacePos = new SlimDX.Vector2(amt, -amt), Texcoord = new SlimDX.Vector2(1.0f, 1.0f) },
                            new Effects.MenuEffect.MenuEffectVertex() { ScreenSpacePos = new SlimDX.Vector2(amt, amt), Texcoord = new SlimDX.Vector2(1.0f, 0.0f) },
                            new Effects.MenuEffect.MenuEffectVertex() { ScreenSpacePos = new SlimDX.Vector2(-amt, amt), Texcoord = new SlimDX.Vector2(0.0f, 0.0f) }
                        }) .Cast<object>().ToArray();
                    }
                    else
                    {
                        return new object[] { };
                    }
                default:
                    throw new Renderable.CannotResolveVerticesException(EffectName, RenderableName);
            }
        }
        #endregion

        #region Listener
        /// <summary>
        /// Listen for hotkey press (e.g., ESC for main menu)
        /// </summary>
        /// <param name="e"></param>
        public void OnKeyPress(KeyEventArgs e)
        {
            // If the key is our hotkey, the hotkey isn't currently being pressed, and there
            //  is currently no active menu open, then open this as the active menu.
            if (e.KeyCode == Hotkey && !_isHotkeyPressed)
            {
                if (ActiveMenu == null)
                {
                    ActiveMenu = this;
                }
                else
                {
                    ActiveMenu = null;
                }
                _isHotkeyPressed = true;
            }
        }

        /// <summary>
        /// Restore state of keypress for hotkey
        /// </summary>
        /// <param name="e"></param>
        public void OnKeyUp(KeyEventArgs e)
        {
            // Notify that our hotkey is no longer being pressed
            if (e.KeyCode == Hotkey && _isHotkeyPressed)
            {
                _isHotkeyPressed = false;
            }
        }

        /// <summary>
        /// Prepare to invoke a button press, if the mouse was over a button when pressed
        /// </summary>
        /// <param name="e"></param>
        public void OnMouseDown(MouseEventArgs e)
        {
            if (ActiveMenu == null) return;

            if (e.Button == MouseButtons.Left)
            {
                Vector2 viewSpacePixel = new Vector2(
                    (2.0f * e.X / OutsideSimulatorApp.GetInstance().Width - 1.0f),
                    (-2.0f * e.Y / OutsideSimulatorApp.GetInstance().Height + 1.0f));
                foreach (var Button in MenuButtons)
                {
                    if (Button.UpperLeft.X < viewSpacePixel.X && Button.LowerRight.X > viewSpacePixel.X
                        && Button.LowerRight.Y < viewSpacePixel.Y && Button.UpperLeft.Y > viewSpacePixel.Y)
                    {
                        Button.IsMouseDown = true;
                    }
                    else
                    {
                        Button.IsMouseDown = false;
                    }
                }
            }
        }

        /// <summary>
        /// Notify the proper button that it is being mouseovered
        /// </summary>
        /// <param name="e"></param>
        public void OnMouseMove(MouseEventArgs e)
        {
            if (ActiveMenu == null) return;

            Vector2 viewSpacePixel = new Vector2(
                    (2.0f * e.X / OutsideSimulatorApp.GetInstance().Width - 1.0f),
                    (-2.0f * e.Y / OutsideSimulatorApp.GetInstance().Height + 1.0f));
            foreach (var Button in MenuButtons)
            {
                if (Button.UpperLeft.X < viewSpacePixel.X && Button.LowerRight.X > viewSpacePixel.X
                    && Button.LowerRight.Y < viewSpacePixel.Y && Button.UpperLeft.Y > viewSpacePixel.Y)
                {
                    Button.IsMouseOver = true;
                }
                else
                {
                    Button.IsMouseOver = false;
                }
            }
        }

        /// <summary>
        /// Invoke a button press, if the mouse was over a button both when pressed and released
        /// </summary>
        /// <param name="e"></param>
        public void OnMouseUp(MouseEventArgs e)
        {
            if (ActiveMenu == null) return;

            if (e.Button == MouseButtons.Left)
            {
                Vector2 viewSpacePixel = new Vector2(
                    (2.0f * e.X / OutsideSimulatorApp.GetInstance().Width - 1.0f),
                    (-2.0f * e.Y / OutsideSimulatorApp.GetInstance().Height + 1.0f));
                foreach (var Button in MenuButtons)
                {
                    if (Button.UpperLeft.X < viewSpacePixel.X && Button.LowerRight.X > viewSpacePixel.X
                        && Button.LowerRight.Y < viewSpacePixel.Y && Button.UpperLeft.Y > viewSpacePixel.Y
                        && Button.IsMouseDown)
                    {
                        // Invoke action!
                        if ((ActiveMenu??this).Submenus.Count((x) => x.Button == Button) > 0)
                        {
                            ActiveMenu = (ActiveMenu ?? this).Submenus.First((x) => x.Button == Button).Menu;
                        }
                        else if ((ActiveMenu ?? this).Actions.Count((x) => x.Button == Button) > 0)
                        {
                            (ActiveMenu ?? this).Actions.First((x) => x.Button == Button).Action.Invoke();
                            ActiveMenu = null;
                        }
                        else
                        {
                            throw new Exception("Um... Action not recognized?");
                        }
                    }
                    Button.IsMouseDown = false;
                }
            }
        }

        public string GetTexturePath()
        {
            return (ActiveMenu ?? this).MenuFilename;
        }
        #endregion

        public static readonly string RenderableName = "MenuRenderable";
    }
}
