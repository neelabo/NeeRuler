using System;
using System.IO;
using System.Windows;

namespace NeeLaboratory.Resources
{
    public class AppFileSource : IFileSource
    {
        private readonly Uri _uri;

        public AppFileSource(Uri uri)
        {
            _uri = uri;
        }

        public string Name => _uri.ToString();

        public Stream Open()
        {
            var ms = new MemoryStream();
            Application.GetResourceStream(_uri).Stream.CopyTo(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}
