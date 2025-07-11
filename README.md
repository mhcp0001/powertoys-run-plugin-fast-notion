# NotionMemo - PowerToys Run Plugin

<div align="center">

![NotionMemo Logo](Community.PowerToys.Run.Plugin.NotionMemo/Images/memo.light.png)

**PowerToys RunからNotionに素早くメモを作成**

[![Version](https://img.shields.io/badge/version-1.0.0-blue.svg)](https://github.com/hlaueriksson/Community.PowerToys.Run.Plugin.FastNotionPlugin/releases)
[![PowerToys](https://img.shields.io/badge/PowerToys-0.87.0+-green.svg)](https://github.com/microsoft/PowerToys)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)

</div>

## 📝 概要

NotionMemoは、PowerToys Runから`Alt + Space`を押して`memo [内容]`と入力するだけで、Notionデータベースに素早くメモを作成できるプラグインです。

### ✨ 主な機能

- 🚀 **高速メモ作成**: `memo [内容]` でワンコマンドメモ作成
- 🔧 **自動設定誘導**: 初回使用時の分かりやすい設定ガイド
- 🛡️ **堅牢なエラーハンドリング**: ネットワークエラーや認証エラーに対応
- 📱 **リアルタイムフィードバック**: メモ作成の成功・失敗を即座に通知
- 🔒 **安全な認証管理**: APIトークンの暗号化保存
- ⚡ **非同期処理**: UIをブロックしないスムーズな操作

## 🎯 使用例

```
Alt + Space → memo 明日の会議でAPI設計について議論する
Alt + Space → memo バグレポート：ログイン時にセッションタイムアウトエラー
Alt + Space → memo アイデア：PowerShellスクリプトでデプロイ自動化
```

## 📦 インストール

### 前提条件

- [PowerToys](https://github.com/microsoft/PowerToys) v0.87.0 以上
- Windows 10/11
- [Notion](https://notion.so) アカウント

### インストール手順

1. **プラグインをダウンロード**
   - [最新リリース](https://github.com/hlaueriksson/Community.PowerToys.Run.Plugin.FastNotionPlugin/releases)からzipファイルをダウンロード

2. **PowerToysを停止**
   ```
   タスクトレイのPowerToysアイコンを右クリック → "Exit"
   ```

3. **プラグインを配置**
   ```
   zipファイルを解凍し、以下のディレクトリにコピー：
   %LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins\NotionMemo\
   ```

4. **PowerToysを再起動**

## 🔧 Notion設定

### 1. Notionインテグレーションの作成

1. [Notion Developers](https://developers.notion.com/)にアクセス
2. "New integration"をクリック
3. インテグレーション名を入力（例：PowerToys NotionMemo）
4. **Internal Integration Token**をコピーして保存 🔑

### 2. データベースの準備

1. Notionで新しいデータベースを作成
2. 以下のプロパティを設定：
   - **タイトル**（Title型）- メモの内容が入る
   - **タスク種別**（Select型）- "📝PowerToysメモ" の選択肢を追加
   - **作成日時**（Created time型）- 自動設定

3. データベースのURLから**Database ID**を取得：
   ```
   https://notion.so/workspace/XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX?v=...
                           ↑ この32文字がDatabase ID
   ```

4. データベースにインテグレーションを招待：
   - データベースページで "Share" → 作成したインテグレーションを選択

### 3. プラグイン設定

1. PowerToys設定を開く：`Alt + Space` → "PowerToys Settings"
2. 左サイドバーから "PowerToys Run" を選択
3. "Plugins" タブで "NotionMemo" を見つける
4. 以下を設定：
   - **API Token**: 手順1で取得したInternal Integration Token
   - **Database ID**: 手順2で取得した32文字のID

5. "Test Connection" をクリックして接続を確認 ✅

## 🚀 使い方

### 基本的な使用方法

1. `Alt + Space` でPowerToys Runを開く
2. `memo` と入力してスペースを入れる
3. メモの内容を入力
4. `Enter` で実行

```
memo 今日学んだこと：ReactのuseCallbackの使い方
```

### コンテキストメニュー

結果を右クリック（または `Ctrl + Shift + Enter`）で追加オプション：
- 📋 **メモ内容をコピー** (`Ctrl + C`)
- ⚙️ **設定を確認** (`Ctrl + S`)

## 🛠️ トラブルシューティング

### よくある問題

#### ❌ "設定が未完了です"
**原因**: APIトークンまたはデータベースIDが未設定
**解決方法**: PowerToys設定画面でNotionMemoプラグインの設定を確認

#### ❌ "Notion APIトークンが無効です"
**原因**: APIトークンが間違っているか、期限切れ
**解決方法**: 
1. Notion Developersで新しいトークンを生成
2. PowerToys設定で新しいトークンを設定

#### ❌ "指定されたデータベースが見つかりません"
**原因**: データベースIDが間違っているか、インテグレーションが招待されていない
**解決方法**:
1. データベースIDを再確認（32文字のハイフンなしUUID）
2. データベースにインテグレーションを招待

#### ❌ "API利用制限に達しました"
**原因**: Notion APIのレート制限
**解決方法**: 数秒待ってから再試行

### ログの確認

詳細なエラー情報は以下で確認できます：
```
%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Logs\
```

## 🔄 今後の予定

### Phase 2 機能（開発予定）
- 🔄 **複数データベース対応**: `memo.work [内容]`、`memo.private [内容]`
- 🏷️ **動的プロパティ指定**: `memo [内容] #タグ @プロジェクト !優先度`
- 📄 **本文への書き込み**: タイトルと本文の分離
- 🎨 **Windows Toast通知**: より見やすい通知表示

### Phase 3 機能（将来的に）
- 🔍 **既存メモの検索・編集**
- 📋 **テンプレート機能**
- 📊 **メモのカテゴリ分類**

## 🤝 貢献

バグ報告や機能提案は[Issues](https://github.com/hlaueriksson/Community.PowerToys.Run.Plugin.FastNotionPlugin/issues)でお願いします。

### 開発者向け

```bash
# リポジトリをクローン
git clone https://github.com/hlaueriksson/Community.PowerToys.Run.Plugin.FastNotionPlugin.git

# ビルド
dotnet build

# テスト実行
dotnet test
```

**技術スタック:**
- .NET 9.0
- System.Text.Json
- PowerToys Run Plugin API
- Notion API v2022-06-28

## 📄 ライセンス

MIT License - 詳細は[LICENSE](LICENSE)を参照

## 🙏 謝辞

- [PowerToys](https://github.com/microsoft/PowerToys) チーム
- [Notion](https://notion.so) API
- PowerToysコミュニティの皆様

---

<div align="center">

**⭐ このプラグインが役に立ったら、スターをお願いします！**

[🐛 バグ報告](https://github.com/hlaueriksson/Community.PowerToys.Run.Plugin.FastNotionPlugin/issues) | [💡 機能提案](https://github.com/hlaueriksson/Community.PowerToys.Run.Plugin.FastNotionPlugin/issues) | [📖 ドキュメント](https://github.com/hlaueriksson/Community.PowerToys.Run.Plugin.FastNotionPlugin/wiki)

</div>