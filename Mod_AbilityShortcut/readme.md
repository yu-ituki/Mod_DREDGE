# Readme
DREDGE のModです。  
各アビリティのキーボードショートカットを追加します。  

# 解説
各アビリティのショートカットキーを設定します。  
発動形式のアビリティはそのまま発動します。  
また、ON/OFFトグル形式のアビリティはキー押下でON/OFFを切り替えます。  
キーアサインはBepInExのコンフィグファイルで変更します。　　
コンフィグファイルは通常、下記パスに存在します。  
<DREDGEインストールフォルダ>/BepInEx/config/yu-ituki.dredge.mod-ability-shortcut.cfg  

# コンフィグの解説
* Key_～  : 各ショーとカットキー設定です。-1で無効化します。  
* IsNoneCooltime ： 各スキルのクールタイムを無視して発動できるようにするチートフラグです。  

## 使い方
1. このプロジェクトをビルドするか、zipを解凍してください。
2. <DREDGEインストールフォルダ>/BepInEx/plugins 以下にフォルダごと配置してください。
  
基本的にはこれだけで動作します。  
BepInExは有志が作成してくれたDREDGEファイル構成用のものがあるので、それを使ってください。   
https://www.nexusmods.com/dredge/mods/18?tab=files  


## 動作環境
* .Net Framework 4.8で動作しています。    
* Visual Studio 2022 と .Net Framework 4.8を入れてもらえればとりあえず動くと思います。  

