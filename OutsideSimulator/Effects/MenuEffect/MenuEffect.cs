using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutsideSimulator.Renderable;
using OutsideSimulator.Scene;
using OutsideSimulator.Scene.Cameras;
using OutsideSimulator.D3DCore;
using OutsideSimulator.Flyweights;
using SlimDX;
using SlimDX.Direct3D11;

namespace OutsideSimulator.Effects.MenuEffect
{
    public class MenuEffect : RenderEffect
    {
        #region D3D Members
        protected Device Device;
        protected DeviceContext ImmediateContext;
        protected InputLayout InputLayout;
        protected Effect Effect;
        protected EffectTechnique Technique;
        #endregion

        #region LogicVariables
        #endregion

        #region Shader Variables
        protected EffectVectorVariable CPO_BlendColor;
        #endregion

        #region Buffers
        protected SlimDX.Direct3D11.Buffer VertexBuffer;
        protected SlimDX.Direct3D11.Buffer IndexBuffer;
        #endregion

        public MenuEffect(Device device)
        {
            Device = device;
            ImmediateContext = Device.ImmediateContext;

            // Compile the shader...
            string compileErrors;
            var compiledShader = SlimDX.D3DCompiler.ShaderBytecode.CompileFromFile
            (
                "../../Effects/MenuEffect/MenuEffect.fx",
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
            Technique = Effect.GetTechniqueByName("MenuTechnique");

            var vertexDesc = new[]
            {
                new InputElement("POSITION", 0, SlimDX.DXGI.Format.R32G32_Float, 0, 0, InputClassification.PerVertexData, 0),
                new InputElement("TEXCOORD", 0, SlimDX.DXGI.Format.R32G32_Float, 8, 0, InputClassification.PerVertexData, 0)
            };

            CPO_BlendColor = Effect.GetVariableByName("gBlendColor").AsVector();

            InputLayout = new InputLayout(Device, Technique.GetPassByIndex(0).Description.Signature, vertexDesc);

            Util.ReleaseCom(ref compiledShader);
        }

        #region RenderEffect
        public void Dispose()
        {
            Util.ReleaseCom(ref VertexBuffer);
            Util.ReleaseCom(ref IndexBuffer);
            Util.ReleaseCom(ref InputLayout);
            Util.ReleaseCom(ref Effect);
        }

        public string EffectName()
        {
            return EffectsGlobals.MenuEffectName;
        }

        public void Render(SceneGraph SceneGraph, Camera Camera, Matrix ProjMatrix)
        {
            // Set input assembler information...
            ImmediateContext.InputAssembler.InputLayout = InputLayout;
            ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            // Since a menu node is a root node (according to the scene graph),
            //  just draw it without traversing or recursing or any of that mess.
            var vertsList = new List<object>();
            var indicesList = new List<uint>();
            var sets = new List<Tuple<int, int>>();
            vertsList.AddRange(SceneGraph.Renderable.GetVertexList(EffectName()));
            indicesList.AddRange(SceneGraph.Renderable.GetIndexList(EffectName()));
            sets.Add(new Tuple<int, int>(vertsList.Count, indicesList.Count));

            // TODO KAM: Remove this hack.
            if (vertsList.Count == 0)
            {
                return;
            }

            // Render buttons individually, both action and submenu buttons
            var menuButtons = (SceneGraph.Renderable as Menu).MenuButtons;
            for (var i = 0; i < menuButtons.Length; i++)
            {
                vertsList.AddRange(menuButtons[i].GetVertexList(EffectName()));
                indicesList.AddRange(menuButtons[i].GetIndexList(EffectName()));
                sets.Add(new Tuple<int, int>(vertsList.Count - sets[sets.Count - 1].Item1, indicesList.Count - sets[sets.Count - 1].Item2));
            }

            var vertexBufferDesc = new BufferDescription(MenuEffectVertex.Stride * vertsList.Count,
                ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            Util.ReleaseCom(ref VertexBuffer); // TODO KAM: Make this dirtyable instead
            VertexBuffer = new SlimDX.Direct3D11.Buffer(Device, new SlimDX.DataStream(vertsList.Cast<MenuEffectVertex>().ToArray(), false, false), vertexBufferDesc);

            var indexBufferDesc = new BufferDescription(
                sizeof(uint) * indicesList.Count,
                ResourceUsage.Default,
                BindFlags.IndexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0);
            Util.ReleaseCom(ref IndexBuffer);
            IndexBuffer = new SlimDX.Direct3D11.Buffer(Device, new SlimDX.DataStream(indicesList.ToArray(), false, false), indexBufferDesc);

            // Set buffers
            ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, MenuEffectVertex.Stride, 0));
            ImmediateContext.InputAssembler.SetIndexBuffer(IndexBuffer, SlimDX.DXGI.Format.R32_UInt, 0);

            var renderPass = Technique.GetPassByIndex(0);
            renderPass.Apply(ImmediateContext);
            CPO_BlendColor.Set(new Vector4(1.0f, 1.0f, 1.0f, 0.8f)); // 80% opacity

            // TODO: Enable blending for a transparent background...
            //
            // Render background...
            //
            ImmediateContext.PixelShader.SetShaderResource(TextureManager.GetInstance().GetResource(Device, (SceneGraph.Renderable as ITextured).GetTexturePath()), 0);
            ImmediateContext.DrawIndexed(sets[0].Item2, 0, 0);

            //
            // Render each button...
            //
            for (var i = 0; i < menuButtons.Length; i++)
            {
                ImmediateContext.PixelShader.SetShaderResource(TextureManager.GetInstance().GetResource(Device, menuButtons[i].GetTexturePath()), 0);
                ImmediateContext.DrawIndexed(6, sets[i + 1].Item2, sets[i + 1].Item1);
            }
        }
        #endregion
    }
}
