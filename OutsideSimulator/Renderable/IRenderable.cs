
namespace OutsideSimulator.Renderable
{
    /// <summary>
    /// Renderable object, which behaves as a vertex factory
    /// </summary>
    public interface IRenderable
    {
        object[] GetVertexList(string EffectName);
        uint[] GetIndexList(string EffectName);

        string GetTexturePath();
    }
}
