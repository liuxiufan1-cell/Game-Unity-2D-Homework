# Game-Unity-2D-Homework

[简体中文](README.md) | **日本語** | [English](README.en.md)

Unity で制作した 2D プラットフォームゲームの課題プロジェクトです。実行可能なゲーム、開発を続けるための素材、ローカルの Unity Editor で開いて編集するためのプロジェクトファイルを保持しています。

## 開発環境

| 項目 | バージョン・パス |
|---|---|
| Unity Editor | `6000.3.13f1` |
| レンダリング | URP `17.3.0`、2D Renderer |
| 入力 | Input System `1.19.0` |
| メインシーン | [2DGameCoursework.unity](Assets/Game/Scenes/2DGameCoursework.unity) |

## ローカルで開く

1. リポジトリをクローンするか、GitHub の **Code → Download ZIP** からダウンロードして展開します。

   ```sh
   git clone https://github.com/liuxiufan1-cell/Game-Unity-2D-Homework.git
   ```

2. Unity Hub で Unity `6000.3.13f1` をインストールし、ライセンスを有効にします。
3. **Add project from disk / ディスクから追加** で、`Assets`、`Packages`、`ProjectSettings` がそろっているリポジトリのルートフォルダーを追加します。
4. 初回のパッケージ解決、アセットのインポート、スクリプトのコンパイルが完了するまで待ちます。
5. `Assets/Game/Scenes/2DGameCoursework.unity` を開き、**Play** を押して Game ウィンドウにフォーカスを合わせます。

ゲームを書き出す場合は **File → Build Profiles** を使います。対象プラットフォームのモジュールをインストールし、メインシーンがビルド対象に含まれていることを確認してください。

## ゲーム内容と操作

- 左右移動、二段ジャンプ、追従カメラ。
- 歩行型・ジャンプ型の敵、踏みつけ判定、死亡後のリスポーン。
- コインの収集と加点、ゴール判定、最終スコアの表示。

| 操作 | キーボード | ゲームパッドの設定 |
|---|---|---|
| 左右移動 | `A / D` または `← / →` | 左スティック |
| ジャンプ・二段ジャンプ | `Space` | 下側のボタン（Xbox の A など） |

ゲームパッドの記載は入力設定に基づくもので、実機では未検証です。現在のプレイヤースクリプトが使う Action は `Move` と `Jump` です。入力アセットに残る他の既定 Action は、対応するゲーム機能の実装を意味しません。コイン取得時のスコアは Console に出力され、ゴール到達時には最終スコアが表示されます。

## プロジェクト構成

| パス | 内容 |
|---|---|
| `Assets/Game` | ゲーム用スクリプト、メインシーン、プレハブ、アニメーション |
| `Assets/Input` | Input Actions と生成された C# ラッパー |
| `Assets/CourseSamples` | 開発用に残した地形、アニメーション、素材プレハブ。旧サンプルコードは削除済み |
| `Assets/ThirdParty` | 背景、キャラクター、敵、コインなどの外部画像素材 |
| `Assets/Settings` | URP、2D Renderer、レンダリング設定 |
| `Packages`、`ProjectSettings` | Unity の依存関係とプロジェクト設定 |
| `Docs/Changelog` | 更新履歴 |
| `Docs/Reviews` | レビュー記録と検証範囲 |

開発用素材は保持しています。復元した素材プレハブでは、旧サンプルコードに依存するコンポーネントやアニメーションイベントを除去しています。利用時は必要なゲームスクリプトを設定してください。すべての素材プレハブが、単体で動く完成済みのキャラクターやマネージャーというわけではありません。

アセットの移動や改名は Unity の Project ウィンドウで行い、対応する `.meta` を保持してください。`Library` や `Temp` などの生成フォルダーはコミット不要です。

## 検証と記録

直近のゲーム検証は `ed937e1` を対象としています。Unity のインポートとコンパイル、14 個のプレハブおよびメインシーンのスクリプト・参照チェック、基本的なゲーム進行のスモークテストに合格しました。Linux のヘッドレス環境での検証であり、Windows Editor の GUI、描画結果、実際の入力機器、書き出したゲームは未検証です。クラウドのログには、以前から存在する Unity の検索インデックス例外が残っています。

- [更新履歴（中国語）](Docs/Changelog/CHANGELOG.md)
- [2026-10-24 レビュー記録（中国語）](Docs/Reviews/2026-10-24.md)
- [外部素材の著作権・利用条件](THIRD_PARTY_NOTICES.txt)
