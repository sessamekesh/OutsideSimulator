using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using SlimDX.Direct3D11;
using System.Windows.Forms;

using OutsideSimulator.D3DCore;
using OutsideSimulator.Scene;
using OutsideSimulator.Scene.Cameras;
using OutsideSimulator.Effects;
using OutsideSimulator.Commands;
using OutsideSimulator.Commands.Events;
using OutsideSimulator.Flyweights;
using OutsideSimulator.Scene.UserInteractions;
using System.Drawing;

namespace OutsideSimulator
{
    /// <summary>
    /// Outside Simulator entry point.
    /// Only one is allowed to exist during the lifecycle of this project, and as such,
    ///  it may implement the singleton pattern so that it may be accessed gloablly
    ///  (for things such as subscribing to key presses, etc.)
    /// </summary>
    public class OutsideSimulatorApp : D3DForm
    {
        protected static OutsideSimulatorApp _appInst;

        #region General Members
        protected Camera Camera;
        protected SceneGraph SceneGraph;
        protected Dirtyable<SlimDX.Matrix> ProjMatrix;
        protected CreateNewDefaultScene NewSceneCreator;
        #endregion

        #region Logic / Interactions
        public ObjectPicker ObjectPicker { get; protected set; }
        public ObjectSpawner ObjectSpawner { get; protected set; }

        public CommandStack CommandStack;
        #endregion

        #region Effects
        protected List<RenderEffect> RenderEffects;
        protected RenderEffect ActiveRenderEffect;
        protected Effects.MenuEffect.MenuEffect MenuRenderEffect;
        #endregion

        #region Subscriber Pattern
        protected List<KeyDownSubscriber> KeyDownSubscribers;
        protected List<KeyUpSubscriber> KeyUpSubscribers;
        protected List<MouseDownSubscriber> MouseDownSubscribers;
        protected List<MouseMoveSubscriber> MouseMoveSubscribers;
        protected List<MouseUpSubscriber> MouseUpSubscribers;
        protected List<MouseWheelSubscriber> MouseWheelSubscribers;
        protected List<TimerTickSubscriber> TimerTickSubscribers;

        /// <summary>
        /// Subscribe to all appropriate actions in this form
        /// </summary>
        /// <param name="Subscriber">An implementation of one or more subscriber interfaces which may be broadcasted by this appliation</param>
        public void Subscribe(object Subscriber)
        {
            if (Subscriber is KeyDownSubscriber)
                KeyDownSubscribers.Add(Subscriber as KeyDownSubscriber);
            if (Subscriber is KeyUpSubscriber)
                KeyUpSubscribers.Add(Subscriber as KeyUpSubscriber);
            if (Subscriber is MouseDownSubscriber)
                MouseDownSubscribers.Add(Subscriber as MouseDownSubscriber);
            if (Subscriber is MouseMoveSubscriber)
                MouseMoveSubscribers.Add(Subscriber as MouseMoveSubscriber);
            if (Subscriber is MouseUpSubscriber)
                MouseUpSubscribers.Add(Subscriber as MouseUpSubscriber);
            if (Subscriber is MouseWheelSubscriber)
                MouseWheelSubscribers.Add(Subscriber as MouseWheelSubscriber);
            if (Subscriber is TimerTickSubscriber)
                TimerTickSubscribers.Add(Subscriber as TimerTickSubscriber);
        }

        /// <summary>
        /// Unsubscribe from all broadcasters
        /// </summary>
        /// <param name="Subscriber">An implementation of one or more subscriber interfaces supported</param>
        public void Unsubscribe(object Subscriber)
        {
            if (Subscriber is KeyDownSubscriber)
                KeyDownSubscribers.Remove(Subscriber as KeyDownSubscriber);
            if (Subscriber is KeyUpSubscriber)
                KeyUpSubscribers.Remove(Subscriber as KeyUpSubscriber);
            if (Subscriber is MouseDownSubscriber)
                MouseDownSubscribers.Remove(Subscriber as MouseDownSubscriber);
            if (Subscriber is MouseMoveSubscriber)
                MouseMoveSubscribers.Remove(Subscriber as MouseMoveSubscriber);
            if (Subscriber is MouseUpSubscriber)
                MouseUpSubscribers.Remove(Subscriber as MouseUpSubscriber);
            if (Subscriber is MouseWheelSubscriber)
                MouseWheelSubscribers.Remove(Subscriber as MouseWheelSubscriber);
            if (Subscriber is TimerTickSubscriber)
                TimerTickSubscribers.Remove(Subscriber as TimerTickSubscriber);
        }
        #endregion

        #region Logic
        /// <summary>
        ///  Save the scene to a file
        /// </summary>
        public void SaveScene()
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Outside Simulator Scene (*.oss)|*.oss";
            var dr = saveDialog.ShowDialog();
            if (dr != DialogResult.OK)
            {
                MessageBox.Show("SaveScene aborted", "Outside Simulator");
                return;
            }

            var xe = new XElement("OutsideSimulatorScene",
                new XElement("Version", 1.0f),
                new XElement("SceneGraph", SceneGraph.Serialize(SceneRootNode)),
                new XElement("CommandStack", CommandStack.Serialize())
            );

            var sw = new System.IO.StreamWriter(saveDialog.FileName);

            foreach (string line in xe.ToString().Split('\n'))
            {
                sw.WriteLine(line.Trim('\n').Trim('\r'));
            }

            sw.Close();
        }

        /// <summary>
        ///  Load the scene from a file
        /// </summary>
        public void LoadScene()
        {
            var loadDialog = new OpenFileDialog();
            loadDialog.Filter = "Outside Simulator Scene (*.oss)|*.oss";

            var dr = loadDialog.ShowDialog();
            if (dr != DialogResult.OK)
            {
                MessageBox.Show("LoadScene aborted", "Outside Simulator");
                return;
            }

            var sr = new System.IO.StreamReader(loadDialog.FileName);

            var xe = XElement.Parse(sr.ReadToEnd());

            if (xe.Name != "OutsideSimulatorScene")
            {
                MessageBox.Show("LoadScene failed - file is not a valid OutsideSimulatorScene", "Outside Simulator");
                return;
            }
            
            var sg = SceneGraph.Deserialize(
                (xe.Nodes().First((x) => (x as XElement).Name == "SceneGraph") as XElement).Nodes().First((x) => (x as XElement).Name == "SceneGraph").ToString()
            );
            var cs = CommandStack.Deserialize(
                (xe.Nodes().First((x) => (x as XElement).Name == "CommandStack") as XElement).Nodes().First((x) => (x as XElement).Name == "CommandStack").ToString()
            );

            SceneGraph.Children.Remove("Scene");
            SceneGraph.AttachChild("Scene", sg);
            CommandStack = cs;
        }

        /// <summary>
        /// Creates the default scene
        /// </summary>
        public void CreateNewScene()
        {
            //
            // Create Startup Scene
            //
            SceneGraph = new SceneGraph(SlimDX.Matrix.Identity);
            SceneGraph.AttachChild("Menu", new SceneGraph(SlimDX.Matrix.Identity));
            SceneGraph.Children["Menu"].Renderable = Builders.MenuFactory.BuildMainMenu();

            Unsubscribe(Camera);

            SceneGraph defaultScene;
            NewSceneCreator = new CreateNewDefaultScene();
            NewSceneCreator.CreateNewScene(out Camera, out defaultScene);
            SceneGraph.AttachChild("Scene", defaultScene);
        }
        #endregion

        #region Action Overrides
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (ProjMatrix != null)
            {
                ProjMatrix.DoTheNasty();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Control && e.KeyCode == Keys.S)
            {
                SaveScene();
            }
            else if (e.Control && e.KeyCode == Keys.O)
            {
                LoadScene();
            }
            else
            {
                foreach (var KDS in KeyDownSubscribers)
                {
                    KDS.OnKeyPress(e);
                }
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            foreach (var KUS in KeyUpSubscribers)
            {
                KUS.OnKeyUp(e);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            foreach (var MDS in MouseDownSubscribers)
            {
                MDS.OnMouseDown(e);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            for (int i = 0; i < MouseUpSubscribers.Count; i++)
            {
                MouseUpSubscribers[i].OnMouseUp(e);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            foreach (var MWS in MouseWheelSubscribers)
            {
                MWS.OnMouseWheel(e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            foreach (var MMS in MouseMoveSubscribers)
            {
                MMS.OnMouseMove(e);
            }
        }
        #endregion

        #region Game Loop
        protected override void UpdateScene(float dt)
        {
            base.UpdateScene(dt);

            for (var i = 0; i < TimerTickSubscribers.Count; i++)
            {
                TimerTickSubscribers[i].Update(dt);
            }

            // Update the camera
            // TODO KAM: Make this a subscriber (it should be)
            Camera.Update(dt);
        }

        protected override void DrawScene()
        {
            base.DrawScene();

            ImmediateContext.ClearRenderTargetView(RenderTargetView, new SlimDX.Color4(0.5f, 0.5f, 1.0f));
            ImmediateContext.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);

            // Render the scene...
            ActiveRenderEffect.Render(SceneGraph.Children["Scene"], Camera, ProjMatrix);

            // Render the menus...
            MenuRenderEffect.Render(SceneGraph.Children["Menu"], Camera, ProjMatrix);

            SwapChain.Present(0, SlimDX.DXGI.PresentFlags.None);
        }
        #endregion

        #region Ctor, Init, Destroy
        private OutsideSimulatorApp(string title) : base(title)
        {
            //
            // Dirtyables...
            //
            ProjMatrix = new Dirtyable<SlimDX.Matrix>(() =>
            {
                return SlimDX.Matrix.PerspectiveFovLH(0.25f * (float)Math.PI, AspectRatio, 0.1f, 10000.0f);
            });

            //
            // Subscribers...
            //
            KeyDownSubscribers = new List<KeyDownSubscriber>();
            KeyUpSubscribers = new List<KeyUpSubscriber>();
            MouseDownSubscribers = new List<MouseDownSubscriber>();
            MouseWheelSubscribers = new List<MouseWheelSubscriber>();
            MouseMoveSubscribers = new List<MouseMoveSubscriber>();
            MouseUpSubscribers = new List<MouseUpSubscriber>();
            TimerTickSubscribers = new List<TimerTickSubscriber>();

            CommandStack = new CommandStack();
            Subscribe(CommandStack);

            RenderEffects = new List<RenderEffect>();
        }

        public static OutsideSimulatorApp GetInstance()
        {
            if (_appInst == null)
            {
                _appInst = new OutsideSimulatorApp("Outside Simulator 2015");
            }

            return _appInst;
        }

        /// <summary>
        /// Initialize everything Direct3D needs to run
        /// </summary>
        protected override void InitD3D()
        {
            base.InitD3D();

            // Create default scene
            CreateNewScene();

            // Initialize our picker
            ObjectPicker = new ObjectPicker();
            ObjectSpawner = new ObjectSpawner();
            Subscribe(ObjectSpawner);

            InitEffects();

            // Use the first valid render effect, if exists
            if (RenderEffects.Count == 0)
            {
                throw new Exception("No render effects could be loaded!");
            }

            ActiveRenderEffect = RenderEffects[1];
        }

        /// <summary>
        /// Initialize our effects! Try to create all of them, if successful,
        ///  they will be added to a list of valid effects, which can be used later.
        /// </summary>
        protected void InitEffects()
        {
            //
            // World Rendering
            //
            // Attempt to get our TestEffect...
            try
            {
                RenderEffects.Add(new Effects.TestEffect.TestEffect(Device));
            }
            catch (Effects.EffectBuildException buildException)
            {
                MessageBox.Show("Could not build TestEffect!\n" + buildException.Message, WindowCaption);
            }

            // Attempt to get our BasicEffect...
            try
            {
                RenderEffects.Add(new Effects.BasicEffect.BasicEffect(Device));
            }
            catch (Effects.EffectBuildException buildException)
            {
                MessageBox.Show("Could not build BasicEffect!\n" + buildException.Message, WindowCaption);
            }

            //
            // Specialized Rendering
            //
            // Attempt to get MenuEffect...
            try
            {
                MenuRenderEffect = new Effects.MenuEffect.MenuEffect(Device);
            }
            catch (Effects.EffectBuildException buildException)
            {
                MessageBox.Show("Could not build MenuEffect!\n" + buildException.Message, WindowCaption);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var RenderEffect in RenderEffects)
                {
                    RenderEffect.Dispose();
                }

                TextureManager.GetInstance().Dispose();
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Accessors
        public SceneGraph SceneRootNode
        {
            get
            {
                if (SceneGraph == null) return null;
                else return SceneGraph.Children["Scene"];
            }
        }

        public Camera SceneCamera
        {
            get
            {
                return Camera;
            }
        }

        public SlimDX.Matrix ProjectionMatrix
        {
            get
            {
                return ProjMatrix.Value;
            }
        }
        #endregion
    }
}
