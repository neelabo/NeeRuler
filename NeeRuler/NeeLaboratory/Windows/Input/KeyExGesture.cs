using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NeeLaboratory.Windows.Input
{
    /// <summary>
    /// 拡張キージェスチャ
    /// 単キー対応
    /// </summary>
    [TypeConverter(typeof(KeyExGestureConverter))]
    public class KeyExGesture : InputGesture
    {
        // メインキー
        public Key Key { get; private set; }

        // 修飾キー
        public ModifierKeys ModifierKeys { get; private set; }


        // コンストラクタ
        public KeyExGesture(Key key) : this(key, ModifierKeys.None)
        {
        }

        public KeyExGesture(Key key, ModifierKeys modifierKeys)
        {
            if (!IsDefinedKey(key)) throw new NotSupportedException();
            Key = key;
            ModifierKeys = modifierKeys;
        }

        // 入力判定
        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (inputEventArgs is not KeyEventArgs keyEventArgs) return false;

            // ALTが押されたときはシステムキーを通常キーとする
            Key key = keyEventArgs.Key;
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0)
            {
                key = keyEventArgs.Key == Key.System ? keyEventArgs.SystemKey : keyEventArgs.Key;
            }

            return this.Key == key && this.ModifierKeys == Keyboard.Modifiers;
        }

        private static bool IsDefinedKey(Key key)
        {
            return Key.None <= key && key <= Key.OemClear;
        }
    }

}
