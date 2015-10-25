using System;
using System.Linq;
using OutsideSimulator.Scene;
using OutsideSimulator.Scene.Cameras;
using OutsideSimulator.D3DCore;
using SlimDX.Direct3D11;
using System.Collections.Generic;

namespace OutsideSimulator.Effects.TestEffect
{
    /// <summary>
    /// Test Effect. Only here to test the architecture. Will be removed eventually.
    /// </summary>
    public class TestEffect : RenderEffect
    {
        #region D3D Members
        protected Device Device;
        protected DeviceContext ImmediateContext;
        protected InputLayout InputLayout;
        protected Effect Effect;
        protected EffectTechnique EffectTechnique;
        #endregion

        #region LogicVariables
        public SlimDX.Matrix WorldViewProj;
        #endregion

        #region Shader Variables
        protected EffectMatrixVariable CPO_WorldViewProj;
        #endregion

        #region Buffers
        protected SlimDX.Direct3D11.Buffer VertexBuffer;
        protected SlimDX.Direct3D11.Buffer IndexBuffer;
        #endregion

        /// <summary>
        /// Create our test RenderEffect.
        /// </summary>
        /// <param name="Device"></param>
        public TestEffect(Device Device)
        {
            this.Device = Device;
            ImmediateContext = Device.ImmediateContext;

            // Compile our shader...
            string compileErrors;
            var compiledShader = SlimDX.D3DCompiler.ShaderBytecode.CompileFromFile
            (
                "../../Effects/TestEffect/TestEffect.fx",
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
            EffectTechnique = Effect.GetTechniqueByName("TestTechnique");

            var vertexDesc = new[]
            {
                new InputElement("POSITION", 0, SlimDX.DXGI.Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0),
                new InputElement("COLOR", 0, SlimDX.DXGI.Format.R32G32B32A32_Float, 12, 0, InputClassification.PerVertexData, 0)
            };

            WorldViewProj = SlimDX.Matrix.Identity;
            CPO_WorldViewProj = Effect.GetVariableByName("gWorldViewProj").AsMatrix();
            InputLayout = new InputLayout(Device, EffectTechnique.GetPassByIndex(0).Description.Signature, vertexDesc);

            Util.ReleaseCom(ref compiledShader);
        }

        #region RenderEffect Implementation
        public void Render(SceneGraph SceneGraph, Camera Camera, SlimDX.Matrix ProjMatrix)
        {
            // Set input assembler information...
            ImmediateContext.InputAssembler.InputLayout = InputLayout;

            // TODO KAM: Shouldn't the primitive topology be set by the objects being rendered themselves?
            ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            // Build our vertex and index buffer...
            var verts = GetAllVertices(SceneGraph);
            var indices = GetAllIndices(SceneGraph);

            var vertBufferDesc = new BufferDescription(TestEffectVertex.Stride * verts.Length,
                ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            Util.ReleaseCom(ref VertexBuffer);
            VertexBuffer = new SlimDX.Direct3D11.Buffer(Device, new SlimDX.DataStream(verts, true, false), vertBufferDesc);

            var indexBuferDesc = new BufferDescription(
                sizeof(uint) * indices.Length,
                ResourceUsage.Default,
                BindFlags.IndexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0
            );
            Util.ReleaseCom(ref IndexBuffer);
            IndexBuffer = new SlimDX.Direct3D11.Buffer(Device, new SlimDX.DataStream(indices, false, false), indexBuferDesc);

            // Set our vertex and index buffers
            ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, TestEffectVertex.Stride, 0));
            ImmediateContext.InputAssembler.SetIndexBuffer(IndexBuffer, SlimDX.DXGI.Format.R32_UInt, 0);

            // Go through, render all nodes
            var renderPass = EffectTechnique.GetPassByIndex(0);
            renderPass.Apply(ImmediateContext);
            int a, b;
            RenderNode(SceneGraph, Camera, ProjMatrix, 0, 0, out a, out b);
        }

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

                // TODO KAM: This is horribly inefficient, change this!
                ImmediateContext.DrawIndexed(nIndices, indexOffset, vertexOffset);

                ic += nIndices;
                vc += nVerts;
            }

            foreach (var Child in Node.Children)
            {
                int cic, cvc;
                RenderNode(Child.Value, Camera, ProjMatrix, indexOffset + ic, vertexOffset + vc, out cic, out cvc);
                ic += cic;
                vc += cvc;
            }

            indicesConsumed = ic;
            verticesConsumed = vc;
        }
        
        private TestEffectVertex[] GetAllVertices(SceneGraph Node)
        {
            IEnumerable<TestEffectVertex> returnSet = (Node.Renderable == null) ? new TestEffectVertex[] { } : Node.Renderable.GetVertexList(EffectName()).Cast<TestEffectVertex>();

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

        public string EffectName()
        {
            return EffectsGlobals.TestEffectName;
        }

        public void Dispose()
        {
            Util.ReleaseCom(ref VertexBuffer);
            Util.ReleaseCom(ref IndexBuffer);
            Util.ReleaseCom(ref InputLayout);
            Util.ReleaseCom(ref Effect);
        }
        #endregion
    }
}
