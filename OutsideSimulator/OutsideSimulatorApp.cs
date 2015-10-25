using System;
using System.Collections.Generic;

using SlimDX.Direct3D11;
using System.Windows.Forms;

using OutsideSimulator.D3DCore;
using OutsideSimulator.Scene;
using OutsideSimulator.Scene.Cameras;
using OutsideSimulator.Effects;
using OutsideSimulator.Commands;
using System.Drawing;

namespace OutsideSimulator
{
    public class OutsideSimulatorApp : D3DForm
    {
        #region Members
        protected Camera Camera;
        protected SceneGraph SceneGraph;
        protected Dirtyable<SlimDX.Matrix> ProjMatrix;
        protected CreateNewDefaultScene NewSceneCreator;
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
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }
        #endregion

        #region Game Loop
        protected override void UpdateScene(float dt)
        {
            base.UpdateScene(dt);

            // Update the camera
            Camera.Update(dt);
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
            // Create default scene
            NewSceneCreator = new CreateNewDefaultScene();
            NewSceneCreator.CreateNewScene(out Camera, out SceneGraph);

            RenderEffects = new List<RenderEffect>();
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
