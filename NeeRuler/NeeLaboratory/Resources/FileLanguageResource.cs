using NeeLaboratory.Linq;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace NeeLaboratory.Resources
{
    public class FileLanguageResource : LanguageResource
    {
        private readonly string _path;
        private bool _initialized;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="path">言語リソースのフォルダパス</param>
        public FileLanguageResource(string path)
        {
            _path = path;
        }


        /// <summary>
        /// リソースが存在するカルチャリストを作成する
        /// </summary>
        public void Initialize()
        {
            if (string.IsNullOrEmpty(_path)) throw new InvalidOperationException();

            if (_initialized) return;
            _initialized = true;

            // TODO: ファイルシステムにアクセスしているので async が望ましい
            var cultures = Directory.GetFiles(_path, "*" + _ext)
                .Select(e => GetCultureInfoFromFileName(e))
                .WhereNotNull()
                .OrderBy(e => e.NativeName);

            SetCultures(cultures);
        }

        /// <summary>
        /// リソースファイル名前からカルチャを得る
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static CultureInfo? GetCultureInfoFromFileName(string fileName)
        {
            try
            {
                var name = Path.GetFileNameWithoutExtension(fileName);
                return CultureInfo.GetCultureInfo(name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// カルチャからファイルソースを作成する
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override IFileSource CreateFileSource(CultureInfo culture)
        {
            return new FileSource(Path.Combine(_path, CreateResTextFileName(culture)));
        }
    }
}
