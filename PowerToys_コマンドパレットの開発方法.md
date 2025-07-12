以下は **PowerToys Command Palette** 向け拡張機能（Extension）をゼロから作るための“最短ルート”です。まずはテンプレートで動くものを作り、そこから機能を肉付けしていきましょう。

---

## 1. 開発環境を整える

| 必須ツール                   | 推奨バージョン                        | 補足                                                       |
| ----------------------- | ------------------------------ | -------------------------------------------------------- |
| **Windows 11 / 10**     | 最新（PowerToys v0.82 以降を推奨）      | Command Palette が同梱されています                                |
| **PowerToys**           | Microsoft Store/winget で最新版を取得 | 起動後 **Command Palette** を有効化                             |
| **Visual Studio 2022**  | 17.10 以上                       | *「.NET デスクトップ開発」* ＋ *「ユニバーサル Windows プラットフォーム開発」* ワークロード |
| **.NET 8 SDK**          | PowerToys 本体と同じメジャーを選択         |                                                          |
| **Windows App SDK 1.6** | Template に同梱                   | 自動で参照が入る                                                 |

---

## 2. テンプレートで骨格を作る（最速パス）

1. **Command Palette を呼び出す**
   `Win + Alt + Space` → `"Create a new extension"` と入力・実行。
2. フォームに入力

   * **ExtensionName**：PascalCase で（例: `SampleToolkit`）
   * **Extension Display Name**：ユーザーに見せる名前（日本語可）
   * **Output Path**：プロジェクト出力フォルダー
3. 生成が終わると以下の構成が作成されます（抜粋）:

   ```
   SampleToolkit/
     ├─ SampleToolkit.sln
     └─ SampleToolkit/
         ├─ Package.appxmanifest
         ├─ SampleToolkitPage.cs
         ├─ SampleToolkitCommandsProvider.cs
         └─ Assets/...
   ```

   この時点で **F5 → Deploy** すれば “TODO” コマンドだけの拡張機能が Palette 内に現れます。([Microsoft Learn][1])

---

## 3. コマンドを追加する

### 3-1. 1 行で動く “スクリプト” 系コマンド

`SampleToolkitPage.cs` の `GetItems()` を編集し、例えばドキュメントを開くコマンドを追加:

```csharp
public override IListItem[] GetItems()
{
    var docs = new OpenUrlCommand(
        "https://learn.microsoft.com/windows/powertoys/command-palette/adding-commands");
    return new[]
    {
        new ListItem(docs) { Title = "Command Palette のドキュメントを開く" }
    };
}
```

`OpenUrlCommand` は Toolkit に用意された便利クラスです。([Microsoft Learn][2])

### 3-2. 自作アクション（IInvokableCommand）

```csharp
internal sealed class ShowMessageCommand : InvokableCommand
{
    public override string Name => "メッセージを表示";
    public override IconInfo Icon => new("\uE8A7");   // MDL2 Chat icon

    public override CommandResult Invoke()
    {
        _ = MessageBox(IntPtr.Zero, "Hello Palette!", "Demo", 0);
        return CommandResult.KeepOpen();              // 実行後も Palette を閉じない
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
}
```

同様に `GetItems()` に `new ListItem(new ShowMessageCommand())` を追加すれば OK。

### 3-3. 階層化／UI ページ

* **ListPage**：子コマンドを一覧表示
* **ContentPage**：Markdown やフォーム (Adaptive Cards) を表示

これらも `ICommand` なので、普通のコマンドと同列に配置できます。([Microsoft Learn][2])

---

## 4. デバッグとリロード

| シーン         | コツ                                                                                    |
| ----------- | ------------------------------------------------------------------------------------- |
| **デバッグ**    | Visual Studio を「パッケージプロジェクト」で F5。別プロセスになるためブレークポイントが効かない場合は *Debugger.Launch()* でアタッチ |
| **ホットリロード** | コード変更 → 再 Deploy → Palette 内で `Reload` コマンド実行（テンプレートに用意済み）                            |

---

## 5. 配布・公開

1. **MSIX パッケージ** を生成（`PublishProfiles/win-x64.pubxml`）。
2. `Package.appxmanifest` に自動生成された *AppExtension* 設定を確認

   * `Name="com.microsoft.commandpalette"`
   * CLSID が一貫していること
3. **WinGet 公開**

   * マニフェストにタグ `windows-commandpalette-extension` を追加
   * 依存関係に `Microsoft.WindowsAppRuntime` を記載
4. Microsoft Store でも可（ただし Palette 内検索は WinGet 経由が推奨）。([Microsoft Learn][3], [The Verge][4])

---

## 6. PowerToys Run との違い

| 項目  | Command Palette Extension                                 | PowerToys Run Plugin   |
| --- | --------------------------------------------------------- | ---------------------- |
| API | WinAppSDK + WinRT (`Microsoft.CommandPalette.Extensions`) | .NET Standard クラスライブラリ |
| UI  | ページ遷移・Markdown・フォームなどリッチ                                  | 1 段の検索結果リスト            |
| 配布  | MSIX + AppExtension, WinGet 連携                            | 単一 DLL をフォルダ配置         |
| 目的  | “VS Code 的” コマンドランチャー                                     | 高速ランチャー／検索             |

既存資産が Run プラグインの場合は “再利用 or 共存” を検討してください。([Conduct of Code][5])

---

## 7. よくあるハマりどころ

| 症状                       | 原因と対処                                                                     |
| ------------------------ | ------------------------------------------------------------------------- |
| 拡張が Palette に出ない         | **Deploy** していない／CLSID 不一致／Package バージョン上書き失敗                             |
| アイコンが豆腐                  | セゴエ MDL2 glyph または 24×24 PNG が足りない                                        |
| DEBUG ビルドで動かない           | Unpackaged 実行だと AppExtension が登録されない → パッケージ実行に切り替え                       |
| コマンド実行後に Palette が閉じてしまう | `CommandResult.ClosePalette()` を返していないか確認 (`KeepOpen()` or `Close()` を選択) |

---

## 8. 追加リソース

* **Extensibility overview**（全体像）([Microsoft Learn][6])
* **Creating an extension**（雛形作成ガイド）([Microsoft Learn][1])
* **Adding commands**（コマンド追加チュートリアル）([Microsoft Learn][2])
* **Extension samples**（GitHub に各種サンプル）([GitHub][7])

---

### Key Takeaways

1. **Palette 自身がテンプレート生成機能を持つ**ので、まずは `"Create a new extension"` を走らせる。
2. 生成プロジェクトは **WinAppSDK + MSIX**。`ShowMessageCommand` など小さなサンプルから着手。
3. `ListPage / ContentPage` により **階層 UI やフォーム** を簡単に組み込める。
4. 配布は **WinGet + windows-commandpalette-extension タグ** が公式導線。
5. 開発・デバッグ時は “Deploy + Reload” を忘れずに。

これで土台は完成です。あとは Koki さん固有のワークフローに合わせてコマンドやページを増やしていけば、PowerToys Command Palette を自分専用の万能ランチャーへ育てられます。Happy hacking!

[1]: https://learn.microsoft.com/en-us/windows/powertoys/command-palette/creating-an-extension "Creating an extension | Microsoft Learn"
[2]: https://learn.microsoft.com/en-us/windows/powertoys/command-palette/adding-commands "Adding commands to your extension | Microsoft Learn"
[3]: https://learn.microsoft.com/en-us/windows/powertoys/command-palette/publish-extension?utm_source=chatgpt.com "Command Palette Extension Publishing | Microsoft Learn"
[4]: https://www.theverge.com/news/668719/microsoft-command-palette-powertoy-launcher?utm_source=chatgpt.com "Microsoft's Command Palette is a powerful launcher for apps, search, and more"
[5]: https://conductofcode.io/post/creating-custom-powertoys-run-plugins/?utm_source=chatgpt.com "Creating custom PowerToys Run plugins - Conduct of Code"
[6]: https://learn.microsoft.com/en-us/windows/powertoys/command-palette/extensibility-overview?utm_source=chatgpt.com "Command Palette Extensibility - Learn Microsoft"
[7]: https://github.com/MicrosoftDocs/windows-dev-docs/blob/docs/hub/powertoys/command-palette/samples.md "windows-dev-docs/hub/powertoys/command-palette/samples.md at docs · MicrosoftDocs/windows-dev-docs · GitHub"
