**まとめ⁠—キーポイントだけ先取り**
PowerToys Run は “Spotlight 風の高速ランチャー” を目指して 2020 年に登場した機能で、アプリ／ファイル／フォルダー検索＋プラグインを中心に据えた単機能型。これに対し 2025 年春に追加された **Command Palette (CmdPal)** は、VS Code のコマンドパレットを Windows 全体へ拡張する “万能コマンド実行ハブ” を志向し、検索機能はもちろんシステム／開発コマンド・ウィンドウ切替え・Web検索・マルチステップ対話 UI まで包含する。Microsoft は「将来的に Run を置き換える後継」だと明言しており、アーキテクチャ・UX・拡張モデル・パフォーマンスすべてが再設計されている。以下で両者を徹底比較する。

---

## 1. 起源と位置づけ

| 年               | モジュール           | 目的                                          | 公式ステータス                                             |
| --------------- | --------------- | ------------------------------------------- | --------------------------------------------------- |
| 2020 – 2024     | PowerToys Run   | Alt+Space で呼び出す高速ランチャー。検索＋簡易プラグイン中心。        | 現行機能（保守フェーズ）([Microsoft Learn][1], [GitHub][2])     |
| 2025 04 (v0.90) | Command Palette | Win+Alt+Space で呼び出す汎用コマンドパレット。Run の後継として設計。 | 「Run の後継を想定」公式表明([Microsoft Learn][3], [Neowin][4]) |

---

## 2. コアコンセプトの差異

| 項目        | PowerToys Run                                              | Command Palette                                                                                  |
| --------- | ---------------------------------------------------------- | ------------------------------------------------------------------------------------------------ |
| 基本的な役割    | 検索＆起動ランチャー                                                 | **検索＋コマンド実行＋ワークフロー HUB**                                                                         |
| デフォルト呼出キー | `Alt + Space`([Microsoft Learn][1])                        | `Win + Alt + Space` (変更可)([Microsoft Learn][3])                                                  |
| 主な機能カテゴリ  | アプリ/ファイル検索、計算、プラグイン                                        | 上記＋システム/開発コマンド、ウィンドウ切替え、ブックマーク、フォーム入力、タグ、Markdown など([Microsoft Learn][5])                       |
| 拡張モデル     | .NET 製プラグイン（IPlugin）— Run と同一プロセスで実行([Conduct of Code][6]) | **Extension SDK**（独立 DLL）— AOT 対応・複数ページ UI・Adaptive Cards 表示可([Microsoft Learn][7], [GitHub][8]) |
| 位置づけ      | 維持は続くが新機能は限定的                                              | Run を将来置換予定の主力モジュール([The Verge][9])                                                              |

---

## 3. UI / UX の違い

### PowerToys Run

* 画面中央に浮かぶ 1 行テキストバーと結果リストのみ。
* 選択中アイテムのプレビューや追加情報は基本なし。
* フラットで軽量な体験を優先。([Microsoft Learn][1])

### Command Palette

* VS Code 風の 2 カラム (リスト＋詳細) レイアウト。
* Command prefix (`>`, `#`, 等) でモード切替え、Backspace で階層を遡る *ページ* 体系。([Microsoft Learn][5])
* コンテキストメニュー・タグ・Markdown・フォーム UI を拡張で埋め込める。([GitHub][8])
* Window Walker が統合され Alt+Tab 代替にも。([The Verge][9])

**アナロジー**:
Run が “Spotlight (macOS) に似たシンプル検索バー” なら、Command Palette は “VS Code の ⌘ + Shift + P を OS 全体に持ち出したもの”。

---

## 4. 機能比較ハイライト

| 機能               | Run                                                       | CmdPal                                                             |
| ---------------- | --------------------------------------------------------- | ------------------------------------------------------------------ |
| ファイル/アプリ/フォルダー検索 | ✔️ (Windows Search & Folder プラグイン)([Microsoft Learn][10]) | ✔️ (Fallback 検索強化)([GitHub][8])                                    |
| システムコマンド (電源/設定) | 一部プラグイン                                                   | ✔️ ネイティブ ＋ 拡張で追加可能([Microsoft Learn][5])                           |
| ウィンドウ切替え         | Window Walker プラグイン (<)                                   | 統合済み；結果はタグ整理される([The Verge][9])                                    |
| 計算・GUID生成など      | Value Generator ほか複数プラグイン                                 | 計算＋日付フォーマットなどを標準実装 (Run からコード移植)([GitHub][8])                      |
| Web検索／URL 起動     | プラグイン依存                                                   | 標準の WebSearch + URL 直接入力に対応([GitHub][8])                           |
| ブックマーク           | なし（サードパーティ）                                               | あり (Bookmark provider)([Microsoft Learn][5])                       |
| 拡張配布             | NuGet/GitHub リポジトリ共有                                      | **Extension Gallery + Wizard** (新規作成ウィザード付き)([Microsoft Learn][7]) |

---

## 5. アーキテクチャとパフォーマンス

* **Run**: 単一プロセス上で JIT 実行。プラグインは同プロセスで動くため、重いプラグインが全体に影響しやすい。
* **CmdPal**: **Ahead-of-Time (AOT) コンパイル準備済み**で、拡張 DLL を個別ロード。クラッシュの局所化と起動速度を両立。([GitHub][8], [Neowin][11])
* ビルド 0.92 系で AOT 対応完了・UI 描画最適化が行われ、入力遅延が Run 比で 15–25 % 改善という報告も。([Neowin][12], [Neowin][11])

---

## 6. 今後のロードマップと選択指針

* Microsoft は **「Run は当面残すが主開発は CmdPal に移行」** と明言。([Microsoft Learn][3], [The Verge][9])
* コミュニティプラグインが Run 側に多数あるため、**既存プラグインを活用したい／軽量さ最優先 → Run**。
* **開発者向けコマンド・マルチステップ UI 拡張・将来性 → CmdPal**。
* 両者は同時有効にできるが、ショートカットが重ならないよう調整を推奨。([GitHub][13])

---

## 7. メリット・デメリット整理

| 観点      | PowerToys Run (現行)      | Command Palette (新)          |
| ------- | ----------------------- | ---------------------------- |
| 起動速度    | 非常に高速 (軽量バー)            | 0.90 以降の最適化で同等レベルまで改善        |
| 拡張資産    | プラグイン数が豊富 (GitHub 上に数百) | SDK 公開直後でこれから増加              |
| UI 情報量  | 最小限＝視線移動少ない             | 詳細ペイン・タグで情報豊富                |
| 機能カバレッジ | 検索中心、コマンドは限定            | コマンド・ワークフロー・検索が一体化           |
| 将来性     | 保守フェーズ                  | 主開発トラック・AOT 化・Windows 統合の可能性 |

---

## 8. まとめ ― 選択のヒント

1. \*\*「検索ランチャーだけ欲しい」\*\*なら現状 Run で十分。軽快さと既存プラグインに優位性。
2. **「VS Code 的コマンド集中管理を OS に持ち込みたい」**、もしくは自作拡張で操作フローを組みたいなら CmdPal がベター。
3. Microsoft は CmdPal に開発リソースを集中しており、**新機能やパフォーマンス改良は CmdPal 側が先行**する見込み。
4. 両モジュールは共存可能なので、**まずは CmdPal をショートカットを変えて試用し、自分のワークフローに合うか検証**するのが現実的な移行パス。

---

### Key Takeaways

* Command Palette は Run の後継として設計され、検索機能に加え多彩なコマンド実行・拡張モデルを搭載。
* 拡張 SDK と AOT 対応で **パフォーマンスと安定性を両立しつつ、将来は Windows 標準機能化も狙う**。
* Run は軽量さと豊富な既存プラグインが魅力だが、長期的には CmdPal への移行を視野に。

---

**参考文献**
(各文中に脚注形式で表示)

---

[1]: https://learn.microsoft.com/en-us/windows/powertoys/run?utm_source=chatgpt.com "PowerToys Run utility for Windows | Microsoft Learn"
[2]: https://github.com/hlaueriksson/awesome-powertoys-run-plugins?utm_source=chatgpt.com "hlaueriksson/awesome-powertoys-run-plugins - GitHub"
[3]: https://learn.microsoft.com/en-us/windows/powertoys/command-palette/overview?utm_source=chatgpt.com "PowerToys Command Palette utility for Windows - Learn Microsoft"
[4]: https://www.neowin.net/news/powertoys-090-is-out-with-a-new-launcher-redesigned-color-picker-and-more/?utm_source=chatgpt.com "PowerToys 0.90 is out with a new launcher, improved Color Picker ..."
[5]: https://learn.microsoft.com/en-us/windows/powertoys/command-palette/overview "PowerToys Command Palette utility for Windows | Microsoft Learn"
[6]: https://conductofcode.io/post/creating-custom-powertoys-run-plugins/?utm_source=chatgpt.com "Creating custom PowerToys Run plugins - Conduct of Code"
[7]: https://learn.microsoft.com/en-us/windows/powertoys/command-palette/publish-extension?utm_source=chatgpt.com "Command Palette Extension Publishing | Microsoft Learn"
[8]: https://github.com/microsoft/powertoys/releases/ "Releases · microsoft/PowerToys · GitHub"
[9]: https://www.theverge.com/news/668719/microsoft-command-palette-powertoy-launcher "Microsoft’s Command Palette is a powerful launcher for apps, search, and more | The Verge"
[10]: https://learn.microsoft.com/en-us/windows/powertoys/run "PowerToys Run utility for Windows | Microsoft Learn"
[11]: https://www.neowin.net/news/powertoys-092-brings-performance-boost-speed-test-windows-11-24h2-default-browser-fix/?utm_source=chatgpt.com "PowerToys 0.92 brings performance boost, speed test, Windows 11 ..."
[12]: https://www.neowin.net/news/powertoys-0921-is-out-with-fixes-for-command-palette-and-more/?utm_source=chatgpt.com "PowerToys 0.92.1 is out with fixes for Command Palette and more"
[13]: https://github.com/microsoft/PowerToys/issues/38400?utm_source=chatgpt.com "Open Command Palette with Win+Space shortcut #38400 - GitHub"
