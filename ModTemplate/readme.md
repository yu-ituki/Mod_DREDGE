# Readme
DREDGE の Mod開発用の雛形です。  
様々なサポート機能を有しています。  

# 解説
このテンプレートは下記を有しています。
* 基本となる最小限のプロジェクト、ソース群（エントリポイントや雛形など）
* Mod用の簡易的なローカライズ機能（全言語対応用）
* 簡易的にゲームインストールフォルダとDLL参照できる仕組み
* ゲーム本体のBepInExフォルダに成果物を自動コピーする仕組み
* ゲーム中の基本的なライフサイクルの管理
* 動的HarmonyPatch登録の簡易化機能
* BepInExコンフィグ関連の簡易的なサポート機能

## 使い方
1. このフォルダをコピーしてください。
2. このリポジトリに存在するLibsフォルダもコピーしてください。
   1. ライブラリ的なソースコードを別フォルダ（./../Libs）に括りだしています。 
3. 「ModTemplate.sln」を自身のMod名にリネームしてください。
4. ModInfo.cs の「ModTemplate」および「mod-template」を自身のMod名に変更してください。
5. config.bat に自身の環境情報を記載してください
6. 準備完了です。これでビルドできるようになっているはずです。

## 動作環境
* .Net Framework 4.8で動作しています。    
* Visual Studio 2022 と .Net Framework 4.8を入れてもらえればとりあえず動くと思います。  
* このプロジェクトにはUnityは必要ありませんが、プレハブやScriptableObject、AssetBundleなどを生成する場合は必要です。  
  * C#のみでも、とりあえず new GameObject() して AddComponentしたり、 Texture2D.LoadRawTextureData() なりを活用すれば   
    もしかしたらUnityが必要ないかもしれませんが、リソースを新規追加するんだったら有ったほうが良いです。   
* あとはゲーム本体を買ってインストールすれば動作環境が完成です。布教用に10本くらい買ってください。  

## Libsについて
* 各Modで完全に共通使用出来そうなソース群です。 
* このフォルダの1つ上にフォルダが存在しています。 
* config.batを叩くことで、Libsとプロジェクトのsrc/Lib以下にシンボリックリンクが張られ参照されます。 
* Libsを一緒に落としてきて頂き、プロジェクトフォルダと同列のディレクトリに置いて頂ければ、  
  テンプレートをコピペしてもとりあえずビルドできるようになっています。  

## 基本となる最小限のプロジェクト、ソース群
* Mod構築用の最小限のプロジェクトやソースが入っています。
* DLL参照はとりあえず必要そうな物が入っています。必要に応じて自由に追加してください。
* Plugin.cs がエントリポイントです。
* ModConfig.cs にMod用コンフィグ（BepInExコンフィグ形式）を書きます。
* BepInExロガーを使用して、DebugUtil.Log()、LogError() などを用意しました。 Debug.Log() 的に使用できます。
  * BepInExログを閲覧するにはBepInExコンソールを開く必要があります。
  * ゲームインストールフォルダ/BepInEx/config/BepInEx.cfg の [Logging.Console] にて、Enabled = true でゲーム起動時に開きます

## ゲーム中の基本的なライフサイクルの管理
* MyModManager.cs がライブラリ群の初期化やゲーム内の基本的なタイミングのコールバックを管理しています。  
* Plugin.cs で初期化＆コールバック登録をしています。  
* RegisterOnBootAction() でゲーム開始時のコールバックが登録できます。
  * NewGameまたはLoadでゲームがセットアップされ、開始される直前のコールバックです。
  * ほぼすべてのデータが揃った状態のコールバックになります。
  * ゲーム中の様々なデータにアクセスしたい初期化処理はここで書いてください。

## Mod用の簡易的なローカライズ機能（全言語対応用）
* エクセルを使用した簡易的なローカライズ機能です
* エクセルで定義したIDとC#のenumを同期させる仕組みが入っています
  * 最低限エクセルさえあれば誰でも叩けるということで、VBAという化石で適当に作っています
    * エクセルすらない場合はそもそもテーブル自体いじれないので無視します
  * data/resource/tables/mod_texts.xlsm にテーブル定義＆VBAが入っています
* エクセル上のボタンを叩くとsrc/TextID.cs および mod_texts.json が出力されます
* src/Lib/ModTextManager.cs で読み込んでいます。
* ModTextManager.Instance.GetText( eTextID ) で、現在の言語コードを自動識別して文字列を返却します。
* 言語を増やす場合はmod_texts.xlsmおよび、Const.cs のeLanguageを増やして、ModText.cs を適当にいじって増やしてください。
* 文中ユーザーデータ埋め込みに対応しています。文中の[0]～[8]と ModTextmanager.Instance.SetUserData() が対応していて、indexに応じて置き換わります。

## 簡易的にゲームインストールフォルダとDLL参照できる仕組み
* config.batに書かれたインストールフォルダのDLLを参照しに行きます
* 仕組み的にはconfig.batにてシンボリックリンクを貼り、csprojで参照しているだけです
* DLL参照を増やす場合は直に参照を増やしてもいいですし、csprojを直にいじって、適当にコピペして増やしても大丈夫です
* シンボリックリンクから相対パスを辿る場合は多少面倒ですがcsprojを直編集してください。他のDLLの記載をコピペして書き換えればOKです。

## ゲーム本体のPackageフォルダに成果物を自動コピーする仕組み
* config.batに書かれたインストールフォルダに下記をコピーします
  * ビルド後のDLL
  * data/resource フォルダごと全部
* これでビルド -> 起動するだけでゲーム本体で即座に動作確認が可能です
* コピーされたファイル群はそのまま配信データとして使用可能です
* 仕組み的にはシンボリックリンクを辿ってビルド後イベントでxcopyしているだけです

## 動的HarmonyPatch登録の簡易化機能
* ローカルファンクションの登録や、パフォーマンスの観点から特定タイミングでのみ登録しておきたいなど、時には動的にPatchを当てたい時もあると思います
* そんな際になるたけ簡易的にPatch関数を登録できるよう、サポート機能を用意しました。
* 下記のように書くと、登録、登録解除が比較的かんたんに出来ます。
```
// GameのInitにパッチを当てる例.
satic void _OnInitGame() {

}

var info = new ModPatchInfo() {
  m_TargetType = typeof(Game),
  m_Regix = "Init",
  m_Prefix = CommonUtil.ToMethodInfo( _OnInitGame )
};

// 当てる.
MyModManager.Instance.AddPatch(info);

// 外す.
MyModManager.Instance.RemovePatch(info);

```

