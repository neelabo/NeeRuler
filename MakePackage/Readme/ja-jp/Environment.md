## 操作方法

マウスでドラッグして読みたいテキスト行に合わせます。  

|操作|説明|
|----|----
|左ボタン|次の行に進む
|右ボタン|コンテキスト メニュー
|中ボタン|マウス追従モードの切り替え
|マウス ホイール|テキスト ライン上部の余白を増減する

ショートカット キーについてはプロファイル フォルダーの `KeyConfig.json` を確認してください。

## 自動ストライドについて

パネルを移動させるときにテキスト行を自動的に検出します。

テキスト行検出の仕組みは、ピクセル ラインをスキャンしてその明度の増減が多いものをテキストとみなすというもので、検出方法としては簡易的なものです。  
仕組み上、パネルが不透明だと機能しなくなります。
また、文字以外に絵などが含まれていると正常に判定できなくなります。


## 設定について

設定は JSON ファイルを直接編集して行います。  
コンテキストメニューの「プロファイル フォルダーを開く」に設定ファイルがあります。通常、`Profiles` フォルダーです。

|ファイル名|説明|
|--|-----
|Settings.json  |設定
|KeyConfig.json |ショートカットキー設定
|Profile-*.json |プロファイル

### Settings.json

設定ファイルです。  
アプリ起動時に読み込まれ、終了時に状態をこのファイルに保存します。

|グループ|名前|型|説明|
|-|--|-|-----
||StorageLocationX|数値|記憶位置の X 座標
||StorageLocationY|数値|記憶位置の Y 座標
||Stride|数値|基本の移動量 (pixel)
||AdjustStride|数値|調整用の移動量 (pixel)
||IsAutoStride|真偽|自動ストライドを有効にする
|AutoStride|MinTextHeight|数値|自動ストライド用パラメータ。最小のテキスト行幅 (pixel)。ルビ等を認識させないために使用します
|AutoStride|TextLineComplexityThreshold|数値|自動ストライド用パラメータ。テキスト行とみなされる行の複雑さを示す値 (0.0-1.0)
|Layout|IsVertical|真偽|パネルを縦方向に配置する
|Layout|IsFlatPanel|真偽|背景色をテキスト ラインの色と同じにしてフラットなパネルにする
|Layout|Width|数値|パネルの横幅 (pixel)。横方向に配置したときの値です
|Layout|Height|数値|パネルの縦幅 (pixel)。横方向に配置したときの値です
|Layout|TextLineHeight|数値|テキスト ラインの縦幅 (pixel)
|Layout|TextLineTopMargin|数値|テキスト ライン上部の余白 (pixel)
|Layout|TextLineBottomMargin|数値|テキスト ライン下部の余白 (pixel)
|Layout|BaseLine|数値|ベースラインの位置 (pixel)。パネルの上端からベースラインまでの長さです
|Layout|BaseLineHeight|数値|ベースラインの縦幅 (pixel)
|Layout|BackgroundColor|色|背景色
|Layout|TextLineColor|色|テキスト ラインの色
|Layout|BaseLineColor|色|ベースラインの色
|Layout|InactiveWindowOpacity|数値|ウィンドウが非アクティブ時の不透明度 (0.0-1.0)
|Layout|IsFollowMouse|真偽|マウス追従モードにする

### KeyConfig.json

コマンドのショートカット キーを定義するファイルです。  
使用できるキー名は [.NET の Key 列挙型](https://learn.microsoft.com/dotnet/api/system.windows.input.key) を参照してください。
`Alt` `Shift` `Ctrl` で修飾できます。

|コマンド|既定|説明|
|--|--|-----
|StoreLocation|`Shift+Q`|位置を記憶
|RestoreLocation|`Q`|位置を復元
|ToggleIsVertical|`V`|縦方向と横方向の切り替え
|ToggleIsAutoStride|`E`|自動ストライドの切り替え
|ToggleIsFlatPanel|`F`|フラット パネルの切り替え
|ToggleIsFollowMouse||マウス追従モードの切り替え
|MoveUp|`W` `Up`|上に移動
|MoveDown|`S` `Down`|下に移動
|MoveLeft|`A` `Left`|左に移動
|MoveRight|`D` `Right`|右に移動
|AdjustUp|`Shift+W` `Shift+Up`|上に調整
|AdjustDown|`Shift+S` `Shift+Down`|下に調整
|AdjustLeft|`Shift+A` `Shift+Left`|左に調整
|AdjustRight|`Shift+D` `Shift+Right`|右に調整
|Exit||アプリを終了
|OpenProfilesFolder||プロファイル フォルダーを開く
|Profile-1|`1`|`Profile-1.json` を適用する
|Profile-2|`2`|`Profile-2.json` を適用する
|Profile-3|`3`|`Profile-3.json` を適用する
|Profile-*||`Profile-*.json` を適用する。`*` はファイル名に依存します

### Profile-*.json

設定の一部を切り替えるための定義ファイルです。
プロファイルは複数保持することができ、`*` は任意の文字列です。  
フォーマットは `Settings.json` と同じです。
省略されたプロパティは現状のまま変更されません。

