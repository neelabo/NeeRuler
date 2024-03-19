using System;
using System.Globalization;

namespace NeeLaboratory.Resources
{
    public class AppLanguageResource : LanguageResource
    {
        private readonly string _path;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="path">言語リソースのリソースフォルダパス</param>
        public AppLanguageResource(string path)
        {
            _path = path.TrimEnd('/') + "/";
        }


        /// <summary>
        /// カルチャからファイルソースを作成する
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override IFileSource CreateFileSource(CultureInfo culture)
        {
            var uri = new Uri(_path + CreateResTextFileName(culture), UriKind.Relative);
            return new AppFileSource(uri);
        }
    }
}
