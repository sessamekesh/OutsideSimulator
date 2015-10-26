using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using OutsideSimulator.Commands.Events;
using OutsideSimulator.Renderable;

namespace OutsideSimulator.Scene
{
    public struct Submenu
    {
        public MenuButton Button { get; private set; }
        public Menu Menu { get; private set; }

        public Submenu(MenuButton btn, Menu mnu) { Button = btn;  Menu = mnu; }
    }

    public struct MenuAction
    {
        public MenuButton Button { get; private set; }
        public Action Action { get; private set; }

        public MenuAction(MenuButton btn, Action actn) { Button = btn;  Action = actn; }
    }

    /// <summary>
    /// Represents all the data required for a menu to function
    ///  1) Menu rendering information
    ///  2) Submenus and buttons to access them
    /// </summary>
    public class Menu : IRenderable, ITextured, MouseDownSubscriber, MouseUpSubscriber, MouseMoveSubscriber, KeyDownSubscriber, KeyUpSubscriber
    {
        #region Properties
        public string MenuFilename { get; private set; }
        public List<Submenu> Submenus { get; set; }
        public List<MenuAction> Actions { get; set; }
        public Menu ActiveMenu { get; protected set; }

        public MenuButton[] MenuButtons
        {
            get
            {
                return Submenus.Select((x) => x.Button)
                    .Concat(Actions.Select((y) => y.Button))
                    .ToArray();
            }
        }

        public Keys? Hotkey;
        #endregion

        #region Logic
        private bool _isHotkeyPressed;
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
            float amt = 0.9f;
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
        { }

        /// <summary>
        /// Notify the proper button that it is being mouseovered
        /// </summary>
        /// <param name="e"></param>
        public void OnMouseMove(MouseEventArgs e)
        { }

        /// <summary>
        /// Invoke a button press, if the mouse was over a button both when pressed and released
        /// </summary>
        /// <param name="e"></param>
        public void OnMouseUp(MouseEventArgs e)
        { }

        public string GetTexturePath()
        {
            return MenuFilename;
        }
        #endregion

        public static readonly string RenderableName = "MenuRenderable";
    }
}
