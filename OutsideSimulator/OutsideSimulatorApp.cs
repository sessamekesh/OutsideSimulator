using System;
using System.Collections.Generic;

using SlimDX.Direct3D11;
using System.Windows.Forms;

using OutsideSimulator.D3DCore;
using OutsideSimulator.Scene;
using OutsideSimulator.Scene.Cameras;
using OutsideSimulator.Effects;
using System.Drawing;

namespace OutsideSimulator
{
    public class OutsideSimulatorApp : D3DForm
    {
        #region Members
        protected Camera Camera;
        protected SceneGraph SceneGraph;
        protected Dirtyable<SlimDX.Matrix> ProjMatrix;
        #endregion

        #region REMOVE US PLZ
        private float _radius;
        private float _theta;
        private float _phi;
        private Point _lastMousePos;
        #endregion

        #region Effects
        protected List<RenderEffect> RenderEffects;
        protected RenderEffect ActiveRenderEffect;
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

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Capture = true;
            _lastMousePos = e.Location;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Capture = false;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            _radius -= e.Delta / 120.0f;

            // TODO KAM: Implement utility math functions like Clamp
            _radius = MathF.Clamp(_radius, 2.0f, 20.0f);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button == MouseButtons.Left)
            {
                var dx = MathF.ToRadians(0.25f * (e.X - _lastMousePos.X));
                var dy = MathF.ToRadians(0.25f * (e.Y - _lastMousePos.Y));

                _theta += dx;
                _phi += dy;

                _phi = MathF.Clamp(_phi, 0.1f, MathF.PI - 0.1f);
            }

            _lastMousePos = e.Location;
        }
        #endregion

        #region Game Loop
        protected override void UpdateScene(float dt)
        {
            base.UpdateScene(dt);

            // Update the camera
            Camera.Update(dt);

            // TODO KAM: This is shit, but do it anyways.
            (Camera as TestCamera).SetPosition(0.0f, 0.0f, _radius);

            // TODO KAM: Update the rest of our scene
            SceneGraph.Rotation = SlimDX.Quaternion.RotationYawPitchRoll(-_theta, _phi, 0.0f);
        }

        protected override void DrawScene()
        {
            base.DrawScene();

            ImmediateContext.ClearRenderTargetView(RenderTargetView, new SlimDX.Color4(0.5f, 0.5f, 1.0f));
            ImmediateContext.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);

            ActiveRenderEffect.Render(SceneGraph, Camera, ProjMatrix);

            SwapChain.Present(0, SlimDX.DXGI.PresentFlags.None);
        }
        #endregion

        #region Ctor, Init, Destroy
        public OutsideSimulatorApp(string title) : base(title)
        {
            //
            // Dirtyables...
            //
            ProjMatrix = new Dirtyable<SlimDX.Matrix>(() =>
            {
                return SlimDX.Matrix.PerspectiveFovLH(0.25f * (float)Math.PI, AspectRatio, 0.1f, 10000.0f);
            });

            //
            // Other things...
            //
            RenderEffects = new List<RenderEffect>();
            Camera = new TestCamera(new SlimDX.Vector3(0.0f, 0.0f, -50.0f), new SlimDX.Vector3(0.0f, 0.0f, 0.0f), new SlimDX.Vector3(0.0f, 1.0f, 0.0f));
            SceneGraph = new SceneGraph(SlimDX.Matrix.Identity);
            SceneGraph.Renderable = new Renderable.TestRenderable();

            _phi = 0.0f;
            _theta = 0.0f;
            _radius = 5.0f;
        }

        /// <summary>
        /// Initialize everything Direct3D needs to run
        /// </summary>
        protected override void InitD3D()
        {
            base.InitD3D();

            InitEffects();

            // Use the first valid render effect, if exists
            if (RenderEffects.Count == 0)
            {
                throw new Exception("No render effects could be loaded!");
            }

            ActiveRenderEffect = RenderEffects[0];
        }

        /// <summary>
        /// Initialize our effects! Try to create all of them, if successful,
        ///  they will be added to a list of valid effects, which can be used later.
        /// </summary>
        protected void InitEffects()
        {
            // Attempt to get our TestEffect...
            try
            {
                RenderEffects.Add(new Effects.TestEffect.TestEffect(Device));
            }
            catch (Effects.EffectBuildException buildException)
            {
                MessageBox.Show("Could not build TestEffect!\n" + buildException.Message, WindowCaption);
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
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}
