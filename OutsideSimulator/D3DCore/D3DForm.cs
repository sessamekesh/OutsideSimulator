using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;

using System.Windows.Forms;

/// <summary>
/// Largely taken from the D3DApp class in Frank Luna's book,
///  "3D Game Programming with DirectX11", and code samples adapted
///  from C++ to C# found at http://www.richardssoftware.net/p/directx-11-tutorials.html
/// </summary>

namespace OutsideSimulator.D3DCore
{
    /// <summary>
    /// Main entry point for a Direct3D application
    /// </summary>
    public class D3DForm : RenderForm
    {
        #region Logical Members
        protected GameTimer Timer;
        protected string WindowCaption;

        private int _frameCount;
        private float _timeElapsed;
        private bool _disposed;
        #endregion

        #region D3D Members
        protected SlimDX.Direct3D11.Device Device;
        protected SlimDX.DXGI.SwapChain SwapChain;
        protected DeviceContext ImmediateContext;
        protected Viewport Viewport;
        protected RenderTargetView RenderTargetView;
        protected Texture2D DepthBuffer;
        protected DepthStencilView DepthStencilView;
        protected DepthStencilState DepthStencilState;
        #endregion

        protected float AspectRatio
        {
            get
            {
                return (float)ClientSize.Width / ClientSize.Height;
            }
        }

        protected bool AppPaused;

        public D3DForm(string title) : base(title)
        {
            WindowCaption = title;
            Timer = new GameTimer();
            _frameCount = 0;
            _timeElapsed = 0.0f;
            AppPaused = false;

            Activated += OnActivated;
            Resize += D3DForm_Resize;
        }

        public virtual void D3DForm_Resize(object sender, System.EventArgs e)
        {

        }

        public virtual void OnActivated(object sender, System.EventArgs e)
        { }

        protected void CalculateFrameRateStats()
        {
            _frameCount++;
            if (Timer.TotalTime - _timeElapsed >= 1.0f)
            {
                var fps = (float)_frameCount;
                var mspf = 1000.0f / fps;
                var s = string.Format("{0}\tFPS: {1}\tFrame Time: {2} (ms)", WindowCaption, fps, mspf);
                Text = s;
                _frameCount = 0;
                _timeElapsed += 1.0f;
            }
        }

        protected virtual void UpdateScene(float dt)
        { }

        protected virtual void DrawScene()
        { }

        protected override void OnResize(System.EventArgs e)
        {
            if (SwapChain != null)
            {
                // Do things here.
                Util.ReleaseCom(ref RenderTargetView);
                Util.ReleaseCom(ref DepthStencilView);
                Util.ReleaseCom(ref DepthBuffer);

                // Resize the render target
                SwapChain.ResizeBuffers(1, ClientSize.Width, ClientSize.Height, Format.R8G8B8A8_UNorm, SwapChainFlags.None);
                using (var resource = SlimDX.Direct3D11.Resource.FromSwapChain<Texture2D>(SwapChain, 0))
                {
                    RenderTargetView = new RenderTargetView(Device, resource);
                }

                // Create the depth/stencil buffer and view
                // TODO KAM: On resize the window, do you need to resize the depth/stencil view?
                var depthBufferDesc = new Texture2DDescription
                {
                    ArraySize = 1,
                    BindFlags = BindFlags.DepthStencil,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Format = Format.D32_Float,
                    Height = ClientSize.Height,
                    Width = ClientSize.Width,
                    MipLevels = 1,
                    OptionFlags = ResourceOptionFlags.None,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default
                };

                DepthBuffer = new Texture2D(Device, depthBufferDesc);

                var depthStencilViewDesc = new DepthStencilViewDescription
                {
                    ArraySize = 0,
                    Format = Format.D32_Float,
                    Dimension = DepthStencilViewDimension.Texture2D,
                    MipSlice = 0,
                    Flags = 0,
                    FirstArraySlice = 0
                };

                DepthStencilView = new DepthStencilView(Device, DepthBuffer, depthStencilViewDesc);

                ImmediateContext.OutputMerger.SetTargets(RenderTargetView);

                // Set up viewport, render target (back buffer on swap chain), and render target view
                // TODO KAM: Do the viewports actually need adjusting here? Probably?
                Viewport = new Viewport(0.0f, 0.0f, ClientSize.Width, ClientSize.Height, 0.0f, 1.0f);
                ImmediateContext.Rasterizer.SetViewports(Viewport);

                base.OnResize(e);
            }
        }

        /// <summary>
        /// Initialize everything D3D in here
        ///  Leave pretty empty for now.
        /// </summary>
        protected virtual void InitD3D()
        {
            // Set up rendering mode description (width, height, frame rate, color format)
            var modeDescription = new ModeDescription(0, 0, new SlimDX.Rational(60, 1), Format.R8G8B8A8_UNorm);

            // Set up swap chain description
            var swapChainDesc = new SwapChainDescription
            {
                BufferCount = 2,
                Usage = Usage.RenderTargetOutput,
                OutputHandle = Handle,
                IsWindowed = true,
                ModeDescription = modeDescription,
                SampleDescription = new SampleDescription(1, 0),
                Flags = SwapChainFlags.AllowModeSwitch,
                SwapEffect = SwapEffect.Discard
            };

            // Create our device and swap chain
            SlimDX.Direct3D11.Device.CreateWithSwapChain
            (
                DriverType.Hardware,
                DeviceCreationFlags.None,
                swapChainDesc,
                out Device,
                out SwapChain
            );

            // Obtain our device context (Immediate context)
            ImmediateContext = Device.ImmediateContext;

            // Prevent DXGI handling of alt+enter...
            using (var factory = SwapChain.GetParent<Factory>())
            {
                factory.SetWindowAssociation(Handle, WindowAssociationFlags.IgnoreAltEnter);
            }

            // Instead, handle the command ourselves
            KeyDown += (sender, e) =>
            {
                if (e.Alt && e.KeyCode == Keys.Enter)
                {
                    SwapChain.IsFullScreen = !SwapChain.IsFullScreen;
                }
            };

            // Add resize handling...
            UserResized += (sender, e) =>
            {
                OnResize(e);
            };

            // Set up viewport, render target (back buffer on swap chain), and render target view
            Viewport = new Viewport(0.0f, 0.0f, ClientSize.Width, ClientSize.Height, 0.0f, 1.0f);
            ImmediateContext.Rasterizer.SetViewports(Viewport);
            using (var resource = SlimDX.Direct3D11.Resource.FromSwapChain<Texture2D>(SwapChain, 0))
            {
                RenderTargetView = new RenderTargetView(Device, resource);
            }
            ImmediateContext.OutputMerger.SetTargets(RenderTargetView);

            // Create the depth/stencil buffer and view
            // TODO KAM: On resize the window, do you need to resize the depth/stencil view?
            var depthBufferDesc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.D32_Float,
                Height = ClientSize.Height,
                Width = ClientSize.Width,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            };

            DepthBuffer = new Texture2D(Device, depthBufferDesc);

            var depthStencilViewDesc = new DepthStencilViewDescription
            {
                ArraySize = 0,
                Format = Format.D32_Float,
                Dimension = DepthStencilViewDimension.Texture2D,
                MipSlice = 0,
                Flags = 0,
                FirstArraySlice = 0
            };

            DepthStencilView = new DepthStencilView(Device, DepthBuffer, depthStencilViewDesc);

            // Set up the depth/stencil state description
            var dsStateDesc = new DepthStencilStateDescription
            {
                IsDepthEnabled = true,
                IsStencilEnabled = false,
                DepthWriteMask = DepthWriteMask.All,
                DepthComparison = Comparison.Less
            };

            DepthStencilState = DepthStencilState.FromDescription(Device, dsStateDesc);

            // Set depth/stencil state for the output merger
            ImmediateContext.OutputMerger.DepthStencilState = DepthStencilState;
        }

        /// <summary>
        /// Begin our D3D applicati
        /// </summary>
        public virtual void Begin()
        {
            AppPaused = false;

            InitD3D();

            MessagePump.Run(this, () =>
            {
                Application.DoEvents();

                Timer.Tick();

                if (!AppPaused)
                {
                    CalculateFrameRateStats();
                    UpdateScene(1.0f / 60.0f);
                    DrawScene();
                }
            });

            Dispose();
        }

        /// <summary>
        /// Properly dispose all of our COM objects after exiting. Invoked automatically by window.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Release all COM here
                    Util.ReleaseCom(ref DepthStencilState);
                    Util.ReleaseCom(ref DepthStencilView);
                    Util.ReleaseCom(ref DepthBuffer);
                    Util.ReleaseCom(ref RenderTargetView);

                    if (SwapChain.IsFullScreen)
                    {
                        SwapChain.SetFullScreenState(false, null);
                    }
                    Util.ReleaseCom(ref SwapChain);
                    Util.ReleaseCom(ref Device);
                }
            }

            _disposed = true;

            base.Dispose(disposing);
        }
    }
}
