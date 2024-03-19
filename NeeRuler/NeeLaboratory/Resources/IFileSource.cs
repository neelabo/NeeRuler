using System.IO;

namespace NeeLaboratory.Resources
{
    public interface IFileSource
    {
        string Name { get; }
        Stream Open();
    }
}
