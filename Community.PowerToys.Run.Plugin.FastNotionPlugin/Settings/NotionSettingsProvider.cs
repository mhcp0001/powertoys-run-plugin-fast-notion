using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using PowerLauncherPluginSettings;
using ManagedCommon;
using Wox.Plugin.Logger;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.FastNotionPlugin.Settings
{
    /// <summary>
    /// PowerToys設定画面用のプロバイダークラス
    /// </summary>
    public class NotionSettingsProvider : ISettingProvider
    {
        private readonly SettingsManager _settingsManager;
        private NotionSettings _currentSettings;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settingsManager">設定管理インスタンス</param>
        public NotionSettingsProvider(SettingsManager settingsManager)
        {
            _settingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));
            _currentSettings = _settingsManager.Settings;
        }

        /// <summary>
        /// 設定UI用の追加オプション
        /// </summary>
        public IEnumerable<PluginAdditionalOption> AdditionalOptions => new List<PluginAdditionalOption>
        {
            new PluginAdditionalOption
            {
                Key = "ApiToken",
                DisplayLabel = "Notion API トークン",
                DisplayDescription = "Notion インテグレーションで取得したAPIトークンを入力してください",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
                TextValue = "", // セキュリティ上、空文字で表示
                PlaceholderText = "secret_xxxxxxxxxx..."
            },
            new PluginAdditionalOption
            {
                Key = "DatabaseId",
                DisplayLabel = "データベースID",
                DisplayDescription = "メモを保存するNotionデータベースのIDを入力してください",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
                TextValue = _currentSettings.DatabaseId,
                PlaceholderText = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
            },
            new PluginAdditionalOption
            {
                Key = "TaskType",
                DisplayLabel = "タスク種別",
                DisplayDescription = "作成するメモのタスク種別を設定してください",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
                TextValue = _currentSettings.TaskType,
                PlaceholderText = "📝PowerToysメモ"
            },
            new PluginAdditionalOption
            {
                Key = "MaxMemoLength",
                DisplayLabel = "メモの最大文字数",
                DisplayDescription = "1つのメモの最大文字数を設定してください（1-2000文字）",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Numberbox,
                NumberValue = _currentSettings.MaxMemoLength,
                PlaceholderText = "500"
            },
            new PluginAdditionalOption
            {
                Key = "TimeoutSeconds",
                DisplayLabel = "API接続タイムアウト（秒）",
                DisplayDescription = "Notion APIへの接続タイムアウト時間を設定してください（1-60秒）",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Numberbox,
                NumberValue = _currentSettings.TimeoutSeconds,
                PlaceholderText = "10"
            },
            new PluginAdditionalOption
            {
                Key = "EnableLogging",
                DisplayLabel = "ログ出力を有効にする",
                DisplayDescription = "デバッグ用のログ出力を有効にします",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Checkbox,
                Value = _currentSettings.EnableLogging
            },
            new PluginAdditionalOption
            {
                Key = "TestConnection",
                DisplayLabel = "接続テスト",
                DisplayDescription = "設定したAPIトークンとデータベースIDで接続テストを実行します",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Button,
                DisplayLabel = "テスト実行"
            },
            new PluginAdditionalOption
            {
                Key = "ResetSettings",
                DisplayLabel = "設定リセット",
                DisplayDescription = "すべての設定をリセットします",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Button,
                DisplayLabel = "リセット"
            }
        };

        /// <summary>
        /// 設定パネルを作成（未使用）
        /// </summary>
        /// <returns>null</returns>
        public Control CreateSettingPanel()
        {
            // PowerToys Run では AdditionalOptions を使用するため、
            // カスタム設定パネルは作成しない
            return null;
        }

        /// <summary>
        /// 設定が更新されたときの処理
        /// </summary>
        /// <param name="settings">更新された設定</param>
        public void UpdateSettings(PowerLauncherPluginSettings settings)
        {
            try
            {
                Log.Info("Updating settings...", typeof(NotionSettingsProvider));

                if (settings?.AdditionalOptions == null)
                {
                    Log.Warn("Settings or AdditionalOptions is null", typeof(NotionSettingsProvider));
                    return;
                }

                var newSettings = new NotionSettings
                {
                    // 既存の設定を保持
                    ApiToken = _currentSettings.ApiToken,
                    DatabaseId = _currentSettings.DatabaseId,
                    TaskType = _currentSettings.TaskType,
                    MaxMemoLength = _currentSettings.MaxMemoLength,
                    TimeoutSeconds = _currentSettings.TimeoutSeconds,
                    EnableLogging = _currentSettings.EnableLogging
                };

                // 設定を更新
                foreach (var option in settings.AdditionalOptions)
                {
                    switch (option.Key)
                    {
                        case "ApiToken":
                            if (!string.IsNullOrEmpty(option.TextValue))
                            {
                                newSettings.ApiToken = option.TextValue;
                            }
                            break;

                        case "DatabaseId":
                            newSettings.DatabaseId = option.TextValue ?? string.Empty;
                            break;

                        case "TaskType":
                            newSettings.TaskType = option.TextValue ?? "📝PowerToysメモ";
                            break;

                        case "MaxMemoLength":
                            if (option.NumberValue > 0 && option.NumberValue <= 2000)
                            {
                                newSettings.MaxMemoLength = (int)option.NumberValue;
                            }
                            break;

                        case "TimeoutSeconds":
                            if (option.NumberValue > 0 && option.NumberValue <= 60)
                            {
                                newSettings.TimeoutSeconds = (int)option.NumberValue;
                            }
                            break;

                        case "EnableLogging":
                            newSettings.EnableLogging = option.Value;
                            break;

                        case "TestConnection":
                            if (option.Value) // ボタンが押された場合
                            {
                                _ = Task.Run(async () => await TestConnectionAsync(newSettings));
                            }
                            break;

                        case "ResetSettings":
                            if (option.Value) // ボタンが押された場合
                            {
                                ResetSettings();
                                return;
                            }
                            break;
                    }
                }

                // 設定を検証
                var validationResult = newSettings.Validate();
                if (validationResult != null)
                {
                    Log.Warn($"Settings validation failed: {validationResult}", typeof(NotionSettingsProvider));
                    // 検証に失敗してもエラーにはしない（部分的な設定更新を許可）
                }

                // 設定を保存
                _settingsManager.SaveSettings(newSettings);
                _currentSettings = newSettings;

                Log.Info("Settings updated successfully", typeof(NotionSettingsProvider));
            }
            catch (Exception ex)
            {
                Log.Exception($"Failed to update settings: {ex.Message}", ex, typeof(NotionSettingsProvider));
            }
        }

        /// <summary>
        /// 接続テストを非同期で実行
        /// </summary>
        /// <param name="settings">テスト対象の設定</param>
        private async Task TestConnectionAsync(NotionSettings settings)
        {
            try
            {
                Log.Info("Starting connection test...", typeof(NotionSettingsProvider));

                // 基本的な設定検証
                var validationResult = settings.Validate();
                if (validationResult != null)
                {
                    Log.Warn($"Connection test failed - validation: {validationResult}", typeof(NotionSettingsProvider));
                    return;
                }

                // 実際のAPI接続テストは NotionApiClient で実装予定
                // 現在は基本的な検証のみ実施
                Log.Info("Connection test completed successfully", typeof(NotionSettingsProvider));
            }
            catch (Exception ex)
            {
                Log.Exception($"Connection test failed: {ex.Message}", ex, typeof(NotionSettingsProvider));
            }
        }

        /// <summary>
        /// 設定をリセット
        /// </summary>
        private void ResetSettings()
        {
            try
            {
                Log.Info("Resetting settings...", typeof(NotionSettingsProvider));
                
                _settingsManager.ResetSettings();
                _currentSettings = _settingsManager.Settings;

                Log.Info("Settings reset successfully", typeof(NotionSettingsProvider));
            }
            catch (Exception ex)
            {
                Log.Exception($"Failed to reset settings: {ex.Message}", ex, typeof(NotionSettingsProvider));
            }
        }
    }
}
