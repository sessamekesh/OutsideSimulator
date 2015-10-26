using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OutsideSimulator.Commands;

namespace OutsideSimulator.Renderable
{
    public class MenuButton : IRenderable, ITextured
    {
        #region Properties
        public string TextureLocation { get; private set; }
        public bool IsMouseOver { get; set; }
        public bool IsMouseDown { get; set; }
        #endregion

        public MenuButton(string TextureLocation)
        {
            this.TextureLocation = TextureLocation;
            IsMouseDown = false;
            IsMouseDown = false;
        }

        #region IRenderable
        public uint[] GetIndexList(string EffectName)
        {
            throw new NotImplementedException();
        }

        public object[] GetVertexList(string EffectName)
        {
            throw new NotImplementedException();
        }

        public string GetTexturePath()
        {
            return TextureLocation;
        }
        #endregion
    }
}
