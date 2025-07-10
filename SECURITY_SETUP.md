# 🔐 セキュリティ設定ガイド

## SettingsExample.cs の設定方法

### 1. 実際のAPIトークンとデータベースIDを記述する場所

`SettingsExample.cs` の `TestSettings()` メソッド内で、以下の値を実際の値に置き換えてください：

```csharp
var newSettings = new NotionSettings
{
    // TODO: 実際の値に置き換えてください
    ApiToken = "secret_YOUR_ACTUAL_API_TOKEN_HERE",        // ← 実際のAPIトークンを記述
    DatabaseId = "YOUR_ACTUAL_DATABASE_ID_HERE",          // ← 実際のデータベースIDを記述
    TaskType = "📝PowerToysメモ",
    MaxMemoLength = 500,
    TimeoutSeconds = 10,
    EnableLogging = true
};
```

### 2. APIトークンの取得方法

1. [Notion Developers](https://developers.notion.com/) にアクセス
2. 「New integration」をクリック
3. インテグレーション名を入力（例：「PowerToys Memo Plugin」）
4. 「Submit」をクリック
5. 「Internal Integration Token」をコピー（`secret_` で始まる文字列）

### 3. データベースIDの取得方法

1. Notionでメモを保存したいデータベースを開く
2. URLをコピー
3. URLから32文字のIDを取得
   - 例：`https://www.notion.so/your-db-id?v=view-id`
   - `your-db-id` の部分がデータベースID

### 4. インテグレーションの接続

1. メモを保存したいデータベースページを開く
2. 右上の「...」メニューをクリック
3. 「接続を追加」→ 作成したインテグレーションを選択
4. 「確認」をクリック

### 5. 設定例

```csharp
var newSettings = new NotionSettings
{
    ApiToken = "secret_1234567890abcdef1234567890abcdef12345678",
    DatabaseId = "12345678-1234-1234-1234-123456789012",
    TaskType = "📝PowerToysメモ",
    MaxMemoLength = 500,
    TimeoutSeconds = 10,
    EnableLogging = true
};
```

## ⚠️ セキュリティ注意事項

1. **APIトークンは絶対に公開しない**
   - GitHubなどの公開リポジトリにプッシュしない
   - 他人と共有しない

2. **SettingsExample.cs は .gitignore に追加済み**
   - このファイルはGitで管理されません
   - 安全に実際の値を記述できます

3. **本番環境では**
   - PowerToys Run の設定画面でユーザーが入力
   - コードにAPIトークンを直接記述しない

## 🧪 テスト方法

1. 上記の設定を完了
2. プロジェクトをビルド
3. PowerToys Run でプラグインをテスト
4. ログでエラーがないか確認

## 🔧 トラブルシューティング

### APIトークンエラー
- トークンが正しく入力されているか確認
- インテグレーションが作成されているか確認

### データベースIDエラー
- データベースIDが32文字の正しい形式か確認
- インテグレーションがデータベースに接続されているか確認

### 接続エラー
- インターネット接続を確認
- Notion APIの利用制限に達していないか確認
