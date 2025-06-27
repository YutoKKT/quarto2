# Quarto Unity プロジェクト

Quartoボードゲームの Unity 実装です。

## ゲーム概要

Quartoは2人対戦の戦略ボードゲームです。16個の異なる特徴を持つピースを使い、4つのピースで「クアルト」（4つ揃い）を作ることを目指します。

### Quartoのルール
- **特殊なターン制**: 相手に置かせるピースを選ぶ（通常のゲームと逆）
- **勝利条件**: 4つのピースが1つ以上の共通属性を持つライン（縦・横・斜め）
- **ピース属性**: 4つの2値属性（穴の有無、高さ、形、色）の組み合わせ

[![Image from Gyazo](https://i.gyazo.com/90da4125899236999c22bebcc5d18150.gif)](https://gyazo.com/90da4125899236999c22bebcc5d18150)

## 操作方法

### カメラ操作

#### キーボード操作
- **W/A/S/D** - カメラの前後左右移動
- **十字キー（矢印キー）** - カメラの前後左右移動（WASDと同じ動作）
- **Q/E** - カメラの上下移動
- **スペースキー** - カメラ操作のオン/オフ切り替え
- **P** - カメラ回転を初期位置にリセット

#### マウス操作
- **左クリック + ドラッグ** - カメラの回転
- **右クリック + ドラッグ** - カメラの位置移動

### ゲーム操作
- **マウスクリック** - ピースの選択・配置を行います

## プロジェクト構造

### アーキテクチャの特徴
- **クリーンアーキテクチャ**: Backend/Frontendが完全分離
- **State Pattern**: GamePhaseの継承で各フェーズを管理
- **Presenter/Executerパターン**: UI更新とUnityオブジェクト操作を分離

### ディレクトリ構成
- `Assets/Scripts/Backend/` - ゲームロジック関連
  - `domain_layer/` - ドメインモデル（Board, Piece）
  - `application_layer/` - ユースケース（SelectPiece, PutPiece）
  - `Adoper/` - ゲームフェーズ管理
  - `Player/` - プレイヤー・AI実装
- `Assets/Scripts/Frontend/` - UI・表示関連
  - `Presenter/` - UI更新処理
  - `Executer/` - Unityオブジェクト操作
  - `UIScripts/` - UI部品（CameraMover.cs等）

## AI・プレイヤー設定

### プレイヤータイプ
- **Human**: 人間プレイヤー
- **CPU**: AI プレイヤー

### AI アルゴリズム
- **defo**: 基本アルゴリズム（順次選択・配置）
- **Youkan**: 高度なアルゴリズム（防御戦略・勝利最優先）

### 柔軟な設定
- SelectとPutで別々にCPU/Human設定可能
- 例：人間が選んでCPUが置く、など非対称な設定も可能

## 開発者向け情報

### ゲーム実装の技術的特徴

#### ビット演算による勝利判定
- `isQuarto()`メソッドでビット演算を使った効率的な判定
- PieceIdの値自体に属性情報がエンコード（H/F, T/S, C/S, B/W）
- 下位4ビットで4つの属性を表現

#### UI更新フロー
1. Command作成 → ViewController.execute()
2. GameController → 現在のPhase実行
3. Result → Executer（Unityオブジェクト操作）
4. Information → Presenter（UI更新）

#### 開発時の注意点
- `PutPice`はタイポですが、複数箇所で使用されています
- カメラ操作の感度は `CameraMover.cs` の設定から調整可能
- 十字キー（矢印キー）でのカメラ操作が利用可能です

### AI拡張方法
新しいAIアルゴリズムの追加：
1. `SelectPieceAlgorithm` または `PutPieceAlgorithm` を継承
2. `SelectPlayerPhase` のコンストラクタで新アルゴリズムを登録
3. UIのドロップダウンに自動的に表示される

## 開発環境

- Unity 6000.0.32f1
- Visual Studio 2022

## ビルド方法

1. Unityでプロジェクトを開く
2. File > Build Settings を選択
3. 対象プラットフォームを選択してビルド
