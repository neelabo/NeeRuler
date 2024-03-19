using System;
using System.Diagnostics;
using System.Globalization;

namespace NeeLaboratory.Resources
{
    /// <summary>
    /// テキストリソース管理
    /// </summary>
    public class TextResourceManager
    {
        private readonly LanguageResource _languageResource;
        private TextResourceSet _resource = new();
        private TextResourceSet _instantResource = new();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directory">言語フォルダ</param>
        public TextResourceManager(LanguageResource languageResource)
        {
            _languageResource = languageResource;
        }


        /// <summary>
        /// 言語リソース
        /// </summary>
        public LanguageResource LanguageResource => _languageResource ?? throw new InvalidOperationException();

        /// <summary>
        /// テキストリソース
        /// </summary>
        public TextResourceSet Resource => ValidResourceSet();


        /// <summary>
        /// テキスト取得 インデクサ
        /// </summary>
        /// <param name="name">リソース名</param>
        /// <returns>対応するテキスト。存在しない場合 null</returns>
        public string? this[string name]
        {
            get => GetString(name);
        }

        /// <summary>
        /// テキスト取得
        /// </summary>
        /// <param name="name">リソース名</param>
        /// <returns>対応するテキスト。存在しない場合 null</returns>
        public string? GetString(string name)
        {
            return ValidResourceSet().GetString(name);
        }

        /// <summary>
        /// 一時的なカルチャ指定を伴うテキスト取得
        /// </summary>
        /// <param name="name">リソース名</param>
        /// <param name="culture">カルチャ</param>
        /// <returns></returns>
        public string? GetString(string name, CultureInfo culture)
        {
            return InstantResourceSet(culture).GetString(name);
        }

        /// <summary>
        /// 言語リソース読み込み
        /// </summary>
        /// <param name="culture">カルチャ</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Load(CultureInfo culture)
        {
            _resource = LoadCore(_languageResource.ValidateCultureInfo(culture));
        }

        private TextResourceSet LoadCore(CultureInfo culture)
        {
            //_languageResource.Load();
            return new TextResourceFactory(_languageResource).Load(culture);
        }

        private TextResourceSet ValidResourceSet()
        {
            if (_resource.IsValid)
            {
                return _resource;
            }
            _resource = LoadCore(_languageResource.ValidateCultureInfo(CultureInfo.CurrentCulture));
            return _resource;
        }

        private TextResourceSet InstantResourceSet(CultureInfo culture)
        {
            var validCulture = _languageResource.ValidateCultureInfo(culture);
            if (_resource.Culture.Equals(validCulture))
            {
                return _resource;
            }
            if (_instantResource.Culture.Equals(validCulture))
            {
                return _instantResource;
            }
            _instantResource = LoadCore(validCulture);
            return _instantResource;
        }

    }
}
