using ManagedCommon;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using Wox.Plugin;
using Wox.Plugin.Logger;
using Community.PowerToys.Run.Plugin.NotionMemo.Settings;

namespace Community.PowerToys.Run.Plugin.NotionMemo
{
    /// <summary>
    /// Main class of this plugin that implement all used interfaces.
    /// </summary>
    public class Main : IPlugin, IContextMenu, IDisposable
    {
        /// <summary>
        /// ID of the plugin.
        /// </summary>
        public static string PluginID => "81CE9B3F5B6D4C4F818FFC072C55BC8F";

        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public string Name => "NotionMemo";

        /// <summary>
        /// Description of the plugin.
        /// </summary>
        public string Description => "PowerToys Run用Notionメモプラグイン";

        private PluginInitContext Context { get; set; }
        private string IconPath { get; set; }
        private bool Disposed { get; set; }
        private SettingsManager SettingsManager { get; set; }
        private NotionApiClient NotionApiClient { get; set; }

        /// <summary>
        /// Return a filtered list, based on the given query.
        /// </summary>
        /// <param name="query">The query to filter the list.</param>
        /// <returns>A filtered list, can be empty when nothing was found.</returns>
        public List<Result> Query(Query query)
        {
            var search = query.Search?.Trim();
            
            if (string.IsNullOrWhiteSpace(search))
            {
                return new List<Result>();
            }

            // 設定が未完了の場合は設定誘導を表示
            if (!SettingsManager.Settings.IsConfigured)
            {
                return new List<Result>
                {
                    new Result
                    {
                        QueryTextDisplay = search,
                        IcoPath = IconPath,
                        Title = "⚙️ Notion設定が必要です",
                        SubTitle = "APIトークンとデータベースIDを設定してください",
                        ToolTipData = new ToolTipData("設定が必要", "PowerToys設定画面でNotionの認証情報を設定してください"),
                        Action = _ =>
                        {
                            // PowerToys設定画面を開く（可能であれば）
                            ShowNotification("PowerToys設定画面でNotionMemoプラグインの設定を行ってください");
                            return true;
                        },
                        ContextData = "settings",
                    }
                };
            }

            // メモ作成の候補を表示
            var memoContent = search;
            var truncatedContent = memoContent.Length > 50 ? memoContent.Substring(0, 50) + "..." : memoContent;
            
            return new List<Result>
            {
                new Result
                {
                    QueryTextDisplay = search,
                    IcoPath = IconPath,
                    Title = $"📝 Notionメモを作成: {truncatedContent}",
                    SubTitle = $"'{memoContent}'をNotionデータベースに保存します",
                    ToolTipData = new ToolTipData("Notionメモ作成", $"内容: {memoContent}\n文字数: {memoContent.Length}"),
                    Action = _ =>
                    {
                        // バックグラウンドでメモ作成を実行
                        Task.Run(async () =>
                        {
                            try
                            {
                                await CreateMemoAsync(memoContent);
                            }
                            catch (Exception ex)
                            {
                                Log.Exception("Failed to create memo in background", ex, GetType());
                            }
                        });
                        return true;
                    },
                    ContextData = memoContent,
                }
            };
        }

        /// <summary>
        /// Initialize the plugin with the given <see cref="PluginInitContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="PluginInitContext"/> for this plugin.</param>
        public void Init(PluginInitContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(Context.API.GetCurrentTheme());
            
            // 設定管理とAPIクライアントを初期化
            SettingsManager = new SettingsManager(Context.CurrentPluginMetadata.PluginDirectory);
            NotionApiClient = new NotionApiClient();
        }

        /// <summary>
        /// Return a list context menu entries for a given <see cref="Result"/> (shown at the right side of the result).
        /// </summary>
        /// <param name="selectedResult">The <see cref="Result"/> for the list with context menu entries.</param>
        /// <returns>A list context menu entries.</returns>
        public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
        {
            if (selectedResult.ContextData is string contextData)
            {
                if (contextData == "settings")
                {
                    // 設定画面用のコンテキストメニュー
                    return new List<ContextMenuResult>
                    {
                        new ContextMenuResult
                        {
                            PluginName = Name,
                            Title = "PowerToys設定を開く",
                            FontFamily = "Segoe MDL2 Assets",
                            Glyph = "\xE713", // Settings
                            AcceleratorKey = Key.S,
                            AcceleratorModifiers = ModifierKeys.Control,
                            Action = _ =>
                            {
                                ShowNotification("PowerToys設定画面を開いてNotionMemoプラグインを設定してください");
                                return true;
                            },
                        }
                    };
                }
                else
                {
                    // メモ内容用のコンテキストメニュー
                    return new List<ContextMenuResult>
                    {
                        new ContextMenuResult
                        {
                            PluginName = Name,
                            Title = "メモ内容をコピー (Ctrl+C)",
                            FontFamily = "Segoe MDL2 Assets",
                            Glyph = "\xE8C8", // Copy
                            AcceleratorKey = Key.C,
                            AcceleratorModifiers = ModifierKeys.Control,
                            Action = _ =>
                            {
                                Clipboard.SetDataObject(contextData);
                                ShowNotification($"メモ内容をクリップボードにコピーしました: {contextData.Substring(0, Math.Min(contextData.Length, 30))}...");
                                return true;
                            },
                        },
                        new ContextMenuResult
                        {
                            PluginName = Name,
                            Title = "設定を確認",
                            FontFamily = "Segoe MDL2 Assets",
                            Glyph = "\xE713", // Settings
                            AcceleratorKey = Key.S,
                            AcceleratorModifiers = ModifierKeys.Control,
                            Action = _ =>
                            {
                                var settings = SettingsManager.Settings;
                                var status = settings.IsConfigured ? "✅ 設定完了" : "⚠️ 設定未完了";
                                ShowNotification($"{status} - APIトークン: {(settings.IsApiTokenSet ? "設定済み" : "未設定")}, データベース: {(settings.IsDatabaseIdSet ? "設定済み" : "未設定")}");
                                return true;
                            },
                        }
                    };
                }
            }

            return new List<ContextMenuResult>();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Wrapper method for <see cref="Dispose()"/> that dispose additional objects and events form the plugin itself.
        /// </summary>
        /// <param name="disposing">Indicate that the plugin is disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed || !disposing)
            {
                return;
            }

            if (Context?.API != null)
            {
                Context.API.ThemeChanged -= OnThemeChanged;
            }

            // NotionApiClient のリソースを解放
            NotionApiClient?.Dispose();

            Disposed = true;
        }

        private void UpdateIconPath(Theme theme) => IconPath = theme == Theme.Light || theme == Theme.HighContrastWhite ? "Images/memo.light.png" : "Images/memo.dark.png";

        private void OnThemeChanged(Theme currentTheme, Theme newTheme) => UpdateIconPath(newTheme);

        /// <summary>
        /// Notion にメモを作成する非同期処理
        /// </summary>
        /// <param name="memoContent">メモの内容</param>
        private async Task CreateMemoAsync(string memoContent)
        {
            try
            {
                var settings = SettingsManager.Settings;
                
                // 設定検証
                if (!settings.IsConfigured)
                {
                    ShowNotification("⚠️ 設定が未完了です。PowerToys設定画面でNotionの認証情報を設定してください。");
                    return;
                }

                // 文字数制限チェック
                if (memoContent.Length > settings.MaxMemoLength)
                {
                    ShowNotification($"⚠️ メモが長すぎます。最大{settings.MaxMemoLength}文字以内で入力してください。");
                    return;
                }

                // Notion API でメモを作成
                var response = await NotionApiClient.CreatePageAsync(
                    settings.ApiToken,
                    settings.DatabaseId,
                    memoContent,
                    settings.TaskType);

                if (response != null && !string.IsNullOrEmpty(response.Id))
                {
                    // 成功時のフィードバック
                    ShowNotification($"✅ Notionメモを作成しました: {memoContent.Substring(0, Math.Min(memoContent.Length, 30))}...");
                    
                    // URLをクリップボードにコピー（オプション）
                    if (!string.IsNullOrEmpty(response.Url))
                    {
                        Clipboard.SetDataObject(response.Url);
                    }
                }
                else
                {
                    ShowNotification("⚠️ メモの作成に失敗しました。");
                }
            }
            catch (NotionApiException ex)
            {
                // Notion API固有のエラー
                var userMessage = ex.GetUserFriendlyMessage();
                ShowNotification(userMessage);
                
                // ログ出力
                if (SettingsManager.Settings.EnableLogging)
                {
                    Log.Exception($"NotionApiException in CreateMemoAsync", ex, GetType());
                }
            }
            catch (Exception ex)
            {
                // 予期しないエラー
                ShowNotification("❌ メモの作成中に予期しないエラーが発生しました。");
                Log.Exception($"Unexpected error in CreateMemoAsync", ex, GetType());
            }
        }

        /// <summary>
        /// 通知を表示する（Windows通知またはクリップボード経由）
        /// </summary>
        /// <param name="message">通知メッセージ</param>
        private void ShowNotification(string message)
        {
            try
            {
                // PowerToys Run には直接的な通知機能がないため、
                // クリップボードに結果メッセージをコピーして代用
                var timestamp = DateTime.Now.ToString("HH:mm:ss");
                var notificationMessage = $"[{timestamp}] {message}";
                
                // 元のクリップボード内容を保存
                var originalClipboard = "";
                try
                {
                    originalClipboard = Clipboard.GetText();
                }
                catch { }

                // 通知メッセージをクリップボードに一時的に設定
                Clipboard.SetDataObject(notificationMessage);
                
                // 簡易的な通知実装（実際のプロダクションでは Windows Toast Notification を使用）
                // TODO: Windows.UI.Notifications を使用した適切な通知実装に置き換える
                
                // ログにも記録
                Log.Info($"NotionMemo Notification: {message}", GetType());
            }
            catch (Exception ex)
            {
                // 通知表示でエラーが発生した場合はログのみ
                Log.Exception($"Failed to show notification: {message}", ex, GetType());
            }
        }
    }
}
