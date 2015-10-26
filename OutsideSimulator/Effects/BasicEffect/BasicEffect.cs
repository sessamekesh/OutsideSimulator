using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OutsideSimulator.Scene;
using OutsideSimulator.Scene.Cameras;
using OutsideSimulator.D3DCore;

using SlimDX.Direct3D11;
using SlimDX;

namespace OutsideSimulator.Effects.BasicEffect
{
    /// <summary>
    /// Basic effect. Does not support any lighting - everything is handled with textures
    ///  in the most basic fashion.
    /// </summary>
    public class BasicEffect : RenderEffect
    {
        #region D3D Members
        protected Device Device;
        protected DeviceContext ImmediateContext;
        protected InputLayout InputLayout;
        protected Effect Effect;
        protected EffectTechnique EffectTechnique;

        protected SlimDX.Direct3D11.Buffer VertexBuffer;
        protected SlimDX.Direct3D11.Buffer IndexBuffer;
        #endregion

        #region Shader Resources
        protected SlimDX.Matrix WorldViewProj;
        protected EffectMatrixVariable CPO_WorldViewProj;
        #endregion

        /// <summary>
        /// Create our BasicEffect
        /// </summary>
        /// <param name="Device">The device which will be used to perform all the rendering</param>
        public BasicEffect(Device Device)
        {
            this.Device = Device;
            ImmediateContext = Device.ImmediateContext;

            //
            // Compile shader
            //
            string compileErrors;
            var compiledShader = SlimDX.D3DCompiler.ShaderBytecode.CompileFromFile
            (
                "../../Effects/BasicEffect/BasicEffect.fx",
                null,
                "fx_5_0",
                SlimDX.D3DCompiler.ShaderFlags.None,
                SlimDX.D3DCompiler.EffectFlags.None,
                null,
                null,
                out compileErrors
            );

            if (compileErrors != null && compileErrors != "")
            {
                throw new EffectBuildException(compileErrors);
            }

            Effect = new Effect(Device, compiledShader);
            EffectTechnique = Effect.GetTechniqueByName("BasicTechnique");

            //
            // Setup input description
            //
            var vertexDesc = new[]
            {
                new InputElement("POSITION", 0, SlimDX.DXGI.Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0),
                new InputElement("TEXCOORD", 0, SlimDX.DXGI.Format.R32G32_Float, 12, 0, InputClassification.PerVertexData, 0)
            };

            WorldViewProj = SlimDX.Matrix.Identity;
            CPO_WorldViewProj = Effect.GetVariableByName("gWorldViewProj").AsMatrix();
            InputLayout = new InputLayout(Device, EffectTechnique.GetPassByIndex(0).Description.Signature, vertexDesc);

            Util.ReleaseCom(ref compiledShader);
        }

        #region RenderEffect implementation
        public void Render(SceneGraph SceneGraph, Camera Camera, Matrix ProjMatrix)
        {
            // Set input assembler information
            ImmediateContext.InputAssembler.InputLayout = InputLayout;
            ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            // Build vertex and index buffer
            // TODO KAM: If nothing has changed, don't re-build the buffers maybe? But how to tell...
            
        }

        public string EffectName()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Helper Methods
        private void RenderNode(SceneGraph Node, Camera Camera, SlimDX.Matrix ProjMatrix, int indexOffset, int vertexOffset, out int indicesConsumed, out int verticesConsumed)
        {
            var ic = indexOffset;
            var vc = vertexOffset;

            if (Node.Renderable != null)
            {
                WorldViewProj = Node.WorldTransform * Camera.GetViewMatrix() * ProjMatrix;
                CPO_WorldViewProj.SetMatrix(WorldViewProj);

                int nIndices = Node.Renderable.GetIndexList(EffectName()).Length;
                int nVerts = Node.Renderable.GetVertexList(EffectName()).Length;

                ImmediateContext.DrawIndexed(nIndices, indexOffset, vertexOffset);

                ic += nIndices;
                vc += nVerts;
            }
        }
        #endregion
    }
}
