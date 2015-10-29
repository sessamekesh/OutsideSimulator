using System.Collections.Generic;
using System.Linq;

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
        protected EffectPass Pass;

        protected SlimDX.Direct3D11.Buffer VertexBuffer;
        protected SlimDX.Direct3D11.Buffer IndexBuffer;
        #endregion

        #region Shader Resources
        protected SlimDX.Matrix WorldViewProj;
        protected EffectMatrixVariable CPO_WorldViewProj;
        protected EffectVectorVariable CPO_SelectionColor;
        protected EffectResourceVariable SRV_BasicTexture;
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
                SlimDX.D3DCompiler.ShaderFlags.Debug | SlimDX.D3DCompiler.ShaderFlags.SkipOptimization,
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
            CPO_SelectionColor = Effect.GetVariableByName("gSelectionColor").AsVector();
            SRV_BasicTexture = Effect.GetVariableByName("gBasicTexture").AsResource();

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
            var verts = GetAllVertices(SceneGraph);
            var indices = GetAllIndices(SceneGraph);

            var vertBufferDesc = new BufferDescription(BasicEffectVertex.Stride * verts.Length,
                ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            Util.ReleaseCom(ref VertexBuffer);
            VertexBuffer = new SlimDX.Direct3D11.Buffer(Device, new DataStream(verts, false, false), vertBufferDesc);

            var indexBufferDesc = new BufferDescription(sizeof(uint) * indices.Length,
                ResourceUsage.Default, BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            Util.ReleaseCom(ref IndexBuffer);
            IndexBuffer = new SlimDX.Direct3D11.Buffer(Device, new DataStream(indices, false, false), indexBufferDesc);

            // Set vertex and index buffers
            ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, BasicEffectVertex.Stride, 0));
            ImmediateContext.InputAssembler.SetIndexBuffer(IndexBuffer, SlimDX.DXGI.Format.R32_UInt, 0);

            // Render all nodes!
            Pass = EffectTechnique.GetPassByIndex(0);
            int a, b;
            RenderNode(SceneGraph, Camera, ProjMatrix, 0, 0, out a, out b);
        }

        public string EffectName()
        {
            return EffectsGlobals.BasicEffectName;
        }

        public void Dispose()
        {
            Util.ReleaseCom(ref VertexBuffer);
            Util.ReleaseCom(ref IndexBuffer);
            Util.ReleaseCom(ref InputLayout);
            Util.ReleaseCom(ref Effect);
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
                SRV_BasicTexture.SetResource(Flyweights.TextureManager.GetInstance().GetResource(Device, Node.Renderable.GetTexturePath()));
                //ImmediateContext.PixelShader.SetShaderResource(Flyweights.TextureManager.GetInstance().GetResource(Device, Node.Renderable.GetTexturePath()), 0);

                int nIndices = Node.Renderable.GetIndexList(EffectName()).Length;
                int nVerts = Node.Renderable.GetVertexList(EffectName()).Length;

                // If selected, set the color:
                if (OutsideSimulatorApp.GetInstance().ObjectPicker.ClickedNode == Node)
                {
                    CPO_SelectionColor.Set(new Color4(0.75f, 0.34f, 0.66f, 1.0f));
                }
                else
                {
                    CPO_SelectionColor.Set(new Color4(0.0f, 0.0f, 0.0f, 0.0f));
                }

                Pass.Apply(ImmediateContext);
                ImmediateContext.DrawIndexed(nIndices, indexOffset, vertexOffset);

                ic += nIndices;
                vc += nVerts;
            }

            foreach (var Child in Node.Children)
            {
                int cic, cvc;
                RenderNode(Child.Value, Camera, ProjMatrix, indexOffset + ic, vertexOffset + vc, out cic, out cvc);
                ic = cic;
                vc = cvc;
            }

            indicesConsumed = ic;
            verticesConsumed = vc;
        }

        private BasicEffectVertex[] GetAllVertices(SceneGraph Node)
        {
            IEnumerable<BasicEffectVertex> returnSet = (Node.Renderable == null) ? new BasicEffectVertex[] { } : Node.Renderable.GetVertexList(EffectName()).Cast<BasicEffectVertex>();

            foreach (var Child in Node.Children)
            {
                returnSet = returnSet.Concat(GetAllVertices(Child.Value));
            }

            return returnSet.ToArray();
        }

        private uint[] GetAllIndices(SceneGraph Node)
        {
            IEnumerable<uint> returnSet = (Node.Renderable == null) ? new uint[] { } : Node.Renderable.GetIndexList(EffectName()).Cast<uint>();

            foreach (var Child in Node.Children)
            {
                returnSet = returnSet.Concat(GetAllIndices(Child.Value));
            }

            return returnSet.ToArray();
        }
        #endregion
    }
}
