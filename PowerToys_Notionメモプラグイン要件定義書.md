# PowerToys Notionメモプラグイン 要件定義書

## 1. プロジェクト概要

### 1.1 プロジェクト名
PowerToys Run用Notionメモプラグイン

### 1.2 目的
Alt+SpaceでPowerToysを起動し、コマンド入力によってNotionの特定データベースに素早くメモを作成できるようにする。日常のタスク管理や情報収集の効率化を図る。

### 1.3 ユーザーストーリー
「開発者として、アイデアや重要な情報をすぐにメモしたいが、Notionを開いてページを作成するのは時間がかかる。PowerToysからワンコマンドでメモを作成したい。」

## 2. 機能要件

### 2.1 基本機能
1. **メモ作成コマンド**
   - コマンド形式：`memo [メモ内容]`
   - 例：`memo 明日の会議でAPI設計について議論する`
   - Notionの指定されたデータベースに新しいページとして追加

2. **レスポンス表示**
   - メモ作成成功時の確認メッセージ：「✓ Notionメモを作成しました」
   - エラー時の適切なエラーメッセージ：「⚠ メモの作成に失敗しました：[理由]」

### 2.2 詳細仕様

#### 2.2.1 入力仕様
- **起動方法**: Alt+Space → `memo [内容]`
- **文字数制限**: 最大500文字（Notion API制限内）
- **特殊文字**: 基本的なテキスト文字をサポート

#### 2.2.2 Notion連携仕様
- **ページタイトル**: 入力されたメモ内容全体をそのまま設定
- **設定するプロパティ**:
  - **タイトル**（Title）: 入力されたメモ内容
  - **タスク種別**（Select）: "📝PowerToysメモ" 固定値
- **自動設定されるプロパティ**:
  - **作成日時**（Created time）: Notion側で自動設定
- **その他のプロパティ**: 未設定のまま（既存の値を保持）

### 2.3 使用例

#### 基本的な使用フロー
1. Alt + Space でPowerToys Run起動
2. `memo 明日の会議でAPI設計について議論する` と入力
3. Enterで実行
4. Notionデータベースに以下のページが作成される：
   - **タイトル**: "明日の会議でAPI設計について議論する"
   - **タスク種別**: "📝PowerToysメモ"
   - **作成日時**: 自動設定（例：2025-07-10 15:30）

#### その他の使用例
- `memo バグレポート：ログイン時にセッションタイムアウトエラー`
- `memo アイデア：PowerShellスクリプトでデプロイ自動化`
- `memo TODO：来週までにドキュメント更新`

## 3. 技術要件

### 3.1 開発環境
- **.NET Framework**: .NET 6.0以上
- **開発言語**: C#
- **開発ツール**: Visual Studio 2022
- **依存関係**: PowerToys Run プラグインAPI

### 3.2 必要なライブラリ・パッケージ
```xml
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
<PackageReference Include="System.Net.Http" Version="4.3.4" />
```

### 3.3 PowerToys プラグイン仕様
- **プラグイン形式**: Class Library (.dll)
- **配置場所**: `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins`
- **実装インターフェース**: 
  - `IPlugin`
  - `IDelayedExecutionPlugin`（オプション）
- **アクティベーションコマンド**: `memo`

### 3.4 ファイル構成
```
Community.PowerToys.Run.Plugin.NotionMemo/
├── Community.PowerToys.Run.Plugin.NotionMemo.csproj
├── Main.cs                    # メインプラグインクラス
├── NotionApiClient.cs         # Notion API通信クラス
├── plugin.json               # プラグインメタデータ
├── Images/
│   ├── memo.dark.png         # ダークテーマアイコン
│   └── memo.light.png        # ライトテーマアイコン
└── Settings/
    └── SettingsPanel.cs      # 設定UI（オプション）
```

## 4. Notion API要件

### 4.1 API設定
- **認証方式**: Bearer Token（インテグレーションシークレット）
- **API バージョン**: 2022-06-28
- **エンドポイント**: `https://api.notion.com/v1/pages`

### 4.2 必要な設定情報
1. **Notionインテグレーションシークレット**
   - ユーザーが設定画面で入力
   - 暗号化して保存

2. **対象データベースID**
   - ユーザーが設定画面で入力
   - 形式：32文字のUUID

### 4.3 APIリクエスト仕様
```json
{
  "parent": {
    "database_id": "データベースID"
  },
  "properties": {
    "タイトル": {
      "title": [
        {
          "text": {
            "content": "入力されたメモ内容"
          }
        }
      ]
    },
    "タスク種別": {
      "select": {
        "name": "📝PowerToysメモ"
      }
    }
  }
}
```

**注意事項**:
- ページ本文（children）は設定せず、タイトルにメモ内容をそのまま設定
- 「作成日時」は Created time 型で Notion が自動設定するため API では指定不要
- 他のプロパティ（Status、プロジェクト等）は未設定のまま

## 5. 非機能要件

### 5.1 パフォーマンス
- **応答時間**: API呼び出し含めて3秒以内
- **メモリ使用量**: 50MB以下

### 5.2 セキュリティ
- **APIトークン**: Windows Credential Storeに暗号化保存
- **入力検証**: XSS対策、SQLインジェクション対策不要（Notion API側で処理）

### 5.3 可用性
- **エラーハンドリング**: ネットワークエラー、API制限エラーに対応
- **オフライン対応**: エラーメッセージ表示のみ（オフライン保存は今回対象外）

## 6. ユーザーインターフェース要件

### 6.1 プラグイン設定画面
PowerToysの設定画面内に以下の設定項目を追加：

1. **Notion設定**
   - インテグレーションシークレット（パスワード形式）
   - データベースID（テキストボックス）
   - 接続テストボタン

2. **その他設定**
   - プラグインの有効/無効
   - グローバル検索への表示の有無

### 6.2 実行時UI
- **入力**: PowerToys Runの標準入力フィールド
- **結果表示**: アイコン + "メモを作成：[内容の一部]" + 説明文
- **実行後**: 成功/失敗メッセージの表示

## 7. エラーハンドリング

### 7.1 想定エラーケース
1. **設定エラー**
   - APIトークン未設定
   - データベースID未設定
   - 無効なAPIトークン

2. **API通信エラー**
   - ネットワーク接続エラー
   - Notion API レート制限
   - 権限不足エラー

3. **入力エラー**
   - 空のメモ内容
   - 文字数制限超過

### 7.2 エラーメッセージ
- **日本語**: メインのエラーメッセージ
- **英語**: 詳細なエラー情報（開発者向け）

## 8. テスト要件

### 8.1 単体テスト
- Notion API通信クラスのテスト
- 入力検証のテスト
- エラーハンドリングのテスト

### 8.2 統合テスト
- PowerToys環境での動作テスト
- 実際のNotionデータベースとの連携テスト

### 8.3 ユーザビリティテスト
- 設定画面の使いやすさ
- メモ作成の操作性

## 9. 配布・インストール要件

### 9.1 配布形式
- GitHub Releasesでzipファイル形式
- 手動インストール（PowerToysプラグインディレクトリにコピー）

### 9.2 必要なファイル
```
NotionMemoPlugin.zip
├── Community.PowerToys.Run.Plugin.NotionMemo.dll
├── Newtonsoft.Json.dll
├── plugin.json
└── Images/
    ├── memo.dark.png
    └── memo.light.png
```

### 9.3 インストール手順
1. PowerToysが停止している状態でzipファイルを解凍
2. `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins`に配置
3. PowerToys再起動
4. 設定画面でNotion認証情報を設定

## 10. 今後の拡張可能性

### 10.1 Phase 2機能候補
- メモのカテゴリ分類
- 複数データベースサポート
- テンプレート機能
- 既存メモの検索・編集

### 10.2 技術的拡張
- オフライン時の一時保存
- リッチテキスト対応
- 添付ファイル対応

## 11. 開発スケジュール（想定）

### Phase 1: 基本機能実装（1-2週間）
- [ ] プロジェクト雛形作成（テンプレート使用）
- [ ] Notion API連携実装（タイトル、タスク種別のみ）
- [ ] PowerToysプラグイン実装（memo コマンド）
- [ ] 基本的なエラーハンドリング（API通信エラー、認証エラー）

### Phase 2: 設定・UI・安定性向上（1週間）
- [ ] 設定画面実装（APIトークン、データベースID）
- [ ] アイコン・UI調整
- [ ] 詳細なエラーハンドリング（入力検証、API制限等）
- [ ] ユーザビリティ改善

### Phase 3: テスト・リリース（3-5日）
- [ ] 単体テスト実装
- [ ] 統合テスト（実際のNotionDBでのテスト）
- [ ] ドキュメント整備（README、インストール手順）
- [ ] リリース準備（zipパッケージ作成）

**総実装期間**: 約2-3週間

## 12. 実装上の注意点

### 12.1 既存プロジェクトからの変更点
- **アクションキーワード**: "fastnotionplugin" → "memo" に変更
- **プラグイン名**: "FastNotionPlugin" → "NotionMemo" に変更
- **機能**: クリップボードコピー → Notion API連携

### 12.2 現在のプロジェクト構成との対応
```
現在のプロジェクト構成:
├── Community.PowerToys.Run.Plugin.FastNotionPlugin/
│   ├── Main.cs (既存 - 大幅修正必要)
│   ├── plugin.json (既存 - 設定変更必要)
│   ├── Images/ (既存 - アイコン変更検討)
│   └── *.csproj (既存 - 依存関係追加必要)

追加予定:
├── NotionApiClient.cs (新規作成)
├── Settings/ (新規作成)
└── Models/ (新規作成 - 設定用データクラス)
```

---

この要件定義書に基づいて、既存のFastNotionPluginプロジェクトを改修していきます。