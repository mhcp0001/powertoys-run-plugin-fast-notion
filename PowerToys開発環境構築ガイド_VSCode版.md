# PowerToys Runプラグイン開発環境構築ガイド（VS Code版）

このガイドでは、VS CodeでPowerToys Runプラグインを開発するための環境を構築します。

## 1. 事前準備・確認

### 必要な環境
- Windows 10/11
- 管理者権限でのアクセス
- インターネット接続

### 現在の環境確認
```bash
# PowerShellまたはコマンドプロンプトで実行
python --version  # Pythonがインストールされていることを確認
code --version    # VS Codeがインストールされていることを確認
```

## 2. .NET 8 SDK のインストール

PowerToys プラグインは .NET 8 で開発することを推奨します（最新の安定版）。

### 方法 1: WinGet を使用（推奨）
```bash
# PowerShellで実行
winget install --id Microsoft.DotNet.SDK.8
```

### 方法 2: 公式サイトからダウンロード
1. [.NET 8 ダウンロードページ](https://dotnet.microsoft.com/ja-jp/download/dotnet/8.0) にアクセス
2. 「SDK」セクションから「x64」をクリック
3. ダウンロードした `.exe` ファイルを実行してインストール

### インストール確認
```bash
# インストール後、新しいPowerShellウィンドウで実行
dotnet --version
# 8.0.xxx が表示されることを確認

dotnet --list-sdks
# .NET 8.x が含まれることを確認
```

## 3. VS Code 拡張機能のインストール

VS Code が既にインストールされている前提で、必要な拡張機能をインストールします。

### 必須拡張機能

1. **C# Dev Kit**（統合C#開発環境）
   ```
   拡張機能ID: ms-dotnettools.csdevkit
   ```
   - VS Code の拡張機能タブ（Ctrl+Shift+X）で「C# Dev Kit」を検索
   - 「Install」をクリック
   - 依存関係として「C#」拡張機能も自動インストールされます

2. **自動インストールされる関連拡張機能**
   - C# (ms-dotnettools.csharp)
   - .NET Install Tool (ms-dotnettools.vscode-dotnet-runtime)
   - IntelliCode for C# Dev Kit (ms-dotnettools.vscodeintellicode-csharp) - オプション

### インストール確認
```bash
# VS Codeで Ctrl+Shift+P を押してコマンドパレットを開き
# "> .NET: New Project" と入力して候補が表示されることを確認
```

## 4. PowerToys プラグインテンプレートのインストール

コミュニティが提供するプロジェクトテンプレートを使用して効率的に開発を始めます。

```bash
# PowerShellで実行
dotnet new install Community.PowerToys.Run.Plugin.Templates
```

### テンプレート確認
```bash
dotnet new list | findstr PowerToys
# または
dotnet new list

# 以下が表示されることを確認：
# - powertoys-run-plugin-solution (PowerToys Run Plugin Solution)
# - powertoys-run-plugin (PowerToys Run Plugin)
```

## 5. 開発環境のテスト

実際にプラグインプロジェクトを作成してビルドできることを確認します。

### テストプロジェクトの作成
```bash
# 作業用ディレクトリを作成
mkdir C:\PowerToysPluginDev
cd C:\PowerToysPluginDev

# テンプレートを使用してプロジェクトを作成
dotnet new powertoys-run-plugin-solution -n TestPlugin
cd TestPlugin

# プロジェクト構造を確認
dir
```

### VS Code でプロジェクトを開く
```bash
# VS Code でプロジェクトを開く
code .
```

### ビルドテスト
```bash
# VS Code のターミナル（Ctrl+`）または外部PowerShellで実行
dotnet build
```

成功すると以下のようなメッセージが表示されます：
```
ビルドに成功しました。
    0 個の警告
    0 エラー
```

## 6. PowerToys の動作確認

### PowerToys のインストール確認
```bash
# PowerToysが動作していることを確認
# Alt+Space で PowerToys Run が起動することを確認
```

### プラグイン配置場所の確認
```bash
# PowerShellで実行
$env:LOCALAPPDATA + "\Microsoft\PowerToys\PowerToys Run\Plugins"

# 通常は以下のパスです：
# C:\Users\[ユーザー名]\AppData\Local\Microsoft\PowerToys\PowerToys Run\Plugins
```

## 7. 追加の便利ツール（オプション）

### Git（ソース管理）
```bash
winget install --id Git.Git
```

### PowerShell 7（最新のPowerShell）
```bash
winget install --id Microsoft.PowerShell
```

### Windows Terminal（開発効率向上）
```bash
winget install --id Microsoft.WindowsTerminal
```

## 8. 環境設定の調整

### VS Code の設定調整

1. **設定を開く**: Ctrl+, 
2. **推奨設定**:
   ```json
   {
     "dotnet.preferCSharpExtension": true,
     "csharp.semanticHighlighting.enabled": true,
     "editor.formatOnSave": true,
     "editor.codeActionsOnSave": {
       "source.organizeImports": true
     }
   }
   ```

### PowerShell 実行ポリシーの確認
```bash
# PowerShellで実行（管理者権限）
Get-ExecutionPolicy

# "RemoteSigned" または "Unrestricted" が推奨
# 必要に応じて設定変更
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
```

## 9. Notion API クライアントのテスト

### HTTP クライアントのテスト
```bash
# テスト用の簡単なHTTPリクエスト
# VS Code のターミナルで実行
dotnet add package Newtonsoft.Json
dotnet add package System.Net.Http
```

## 10. 環境構築完了チェックリスト

以下がすべて ✅ になれば環境構築完了です：

- [ ] .NET 8 SDK がインストールされている（`dotnet --version`）
- [ ] VS Code に C# Dev Kit がインストールされている
- [ ] PowerToys プラグインテンプレートがインストールされている（`dotnet new list`）
- [ ] テストプロジェクトが作成・ビルドできる
- [ ] VS Code で C# ファイルのシンタックスハイライトが動作する
- [ ] PowerToys Run が動作する（Alt+Space）
- [ ] プラグイン配置フォルダが存在する

## 11. 次のステップ

環境構築が完了したら、実際にNotionメモプラグインの開発を開始できます：

1. **新しいプラグインプロジェクトの作成**
   ```bash
   dotnet new powertoys-run-plugin-solution -n NotionMemoPlugin
   ```

2. **Notion API クライアントの実装**
3. **PowerToys Run プラグインインターフェースの実装**
4. **テストとデバッグ**

## 12. トラブルシューティング

### よくある問題と解決方法

**問題**: `dotnet` コマンドが認識されない
```bash
# 解決方法: 環境変数PATHの確認とVS Codeの再起動
# PowerShellを再起動してから再試行
```

**問題**: C# Dev Kit のライセンスエラー
```
解決方法: VS Code でコマンドパレット（Ctrl+Shift+P）から
"C# Dev Kit: Open Walkthrough" を実行してライセンス認証を完了
```

**問題**: PowerToys Run プラグインが認識されない
```bash
# 解決方法: PowerToysサービスの再起動
# タスクマネージャーで PowerToys.exe を終了後、再度起動
```

## 13. 開発環境の構成概要

### インストールされるコンポーネント
```
開発環境
├── .NET 8 SDK
│   ├── .NET Runtime 8.0.x
│   ├── ASP.NET Core Runtime 8.0.x
│   └── .NET Desktop Runtime 8.0.x
├── VS Code 拡張機能
│   ├── C# Dev Kit
│   ├── C# 
│   └── .NET Install Tool
└── PowerToys プラグインテンプレート
    ├── powertoys-run-plugin-solution
    └── powertoys-run-plugin
```

### 開発フロー
```
1. テンプレートからプロジェクト作成
   ↓
2. VS Code でコード編集
   ↓
3. dotnet build でビルド
   ↓
4. PowerToys プラグインフォルダに配置
   ↓
5. PowerToys 再起動でテスト
```

## 14. パフォーマンス・軽量化のポイント

### VS Code の動作を軽快に保つ設定
```json
{
  "files.exclude": {
    "**/bin": true,
    "**/obj": true,
    "**/.vs": true
  },
  "search.exclude": {
    "**/node_modules": true,
    "**/bower_components": true,
    "**/bin": true,
    "**/obj": true
  }
}
```

### .NET プロジェクトの最適化
- 不要なパッケージ参照の削除
- Release ビルドの使用
- AOT（Ahead-of-Time）コンパイルの検討（.NET 8+）

---

この環境構築が完了すると、VS Code で本格的な PowerToys プラグイン開発を開始できます！