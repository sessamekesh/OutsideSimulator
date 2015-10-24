using SlimDX;
using SlimDX.Direct3D11;

using OutsideSimulator.Scene;
using OutsideSimulator.Scene.Cameras;
using System;

namespace OutsideSimulator.Effects
{
    /// <summary>
    /// Renders a scene graph to a device and context
    /// </summary>
    public interface RenderEffect : IDisposable
    {
        void Render(SceneGraph SceneGraph, Camera Camera, Matrix ProjMatrix);

        string EffectName();
    }
}
