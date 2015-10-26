using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SlimDX.Direct3D11;

namespace OutsideSimulator.Flyweights
{
    public class TextureManager : IDisposable
    {
        #region Properties
        protected Dictionary<String, ShaderResourceView> ResourceViews;
        #endregion

        #region Singleton Pattern
        private static TextureManager _inst;

        public static TextureManager GetInstance()
        {
            if (_inst == null)
            {
                _inst = new TextureManager();
            }

            return _inst;
        }
        #endregion

        private TextureManager()
        {
            ResourceViews = new Dictionary<string, ShaderResourceView>();
        }

        public ShaderResourceView GetResource(Device Device, string path)
        {
            if (!ResourceViews.ContainsKey(path))
            {
                ResourceViews.Add(path, ShaderResourceView.FromFile(Device, path));
            }

            return ResourceViews[path];
        }

        public void Dispose()
        {
            var resourceViews = ResourceViews.ToArray();
            ResourceViews.Clear();

            for (var i = 0; i < resourceViews.Length; i++)
            {
                ShaderResourceView rvtr = resourceViews[i].Value;
                OutsideSimulator.D3DCore.Util.ReleaseCom(ref rvtr);
            }
        }
    }
}
