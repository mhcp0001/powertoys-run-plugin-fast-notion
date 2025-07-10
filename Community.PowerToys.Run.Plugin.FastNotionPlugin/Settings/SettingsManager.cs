using System;
using System.IO;
using System.Text.Json;
using System.Security;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ManagedCommon;
using Wox.Plugin.Logger;
using System.Linq;

namespace Community.PowerToys.Run.Plugin.FastNotionPlugin.Settings
{
    /// <summary>
    /// Notion設定の保存・読み込みを管理するクラス
    /// </summary>
    public class SettingsManager
    {
        private const string SettingsFileName = "NotionSettings.json";
        private const string CredentialTarget = "PowerToys.NotionPlugin.ApiToken";
        private const string PluginName = "FastNotionPlugin";
        
        private readonly string _settingsFilePath;
        private NotionSettings _settings;
        private readonly object _lock = new object();

        /// <summary>
        /// 現在の設定
        /// </summary>
        public NotionSettings Settings
        {
            get
            {
                lock (_lock)
                {
                    return _settings;
                }
            }
        }

        /// <summary>
        /// 設定変更イベント
        /// </summary>
        public event EventHandler<NotionSettings>? SettingsChanged;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="pluginDirectory">プラグインディレクトリ</param>
        public SettingsManager(string? pluginDirectory = null)
        {
            // 設定ファイルのパスを決定
            if (string.IsNullOrEmpty(pluginDirectory))
            {
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var powerToysPluginDir = Path.Combine(localAppData, "Microsoft", "PowerToys", "PowerToys Run", "Settings", "Plugins", PluginName);
                Directory.CreateDirectory(powerToysPluginDir);
                _settingsFilePath = Path.Combine(powerToysPluginDir, SettingsFileName);
            }
            else
            {
                Directory.CreateDirectory(pluginDirectory);
                _settingsFilePath = Path.Combine(pluginDirectory, SettingsFileName);
            }

            // 設定を読み込み
            _settings = LoadSettings();
        }

        /// <summary>
        /// 設定を保存する
        /// </summary>
        /// <param name="settings">保存する設定</param>
        public void SaveSettings(NotionSettings settings)
        {
            try
            {
                lock (_lock)
                {
                    // APIトークンは Windows Credential Store に保存
                    if (!string.IsNullOrEmpty(settings.ApiToken))
                    {
                        SaveCredential(CredentialTarget, settings.ApiToken);
                    }

                    // その他の設定をJSONファイルに保存
                    var settingsToSave = new NotionSettings
                    {
                        DatabaseId = settings.DatabaseId,
                        TaskType = settings.TaskType,
                        MaxMemoLength = settings.MaxMemoLength,
                        TimeoutSeconds = settings.TimeoutSeconds,
                        EnableLogging = settings.EnableLogging,
                        LastUpdated = DateTime.Now
                    };

                    var jsonOptions = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };

                    var jsonString = JsonSerializer.Serialize(settingsToSave, jsonOptions);
                    File.WriteAllText(_settingsFilePath, jsonString);

                    // メモリ内の設定を更新
                    _settings = new NotionSettings
                    {
                        ApiToken = settings.ApiToken,
                        DatabaseId = settings.DatabaseId,
                        TaskType = settings.TaskType,
                        MaxMemoLength = settings.MaxMemoLength,
                        TimeoutSeconds = settings.TimeoutSeconds,
                        EnableLogging = settings.EnableLogging,
                        LastUpdated = settingsToSave.LastUpdated
                    };

                    // イベントを発生
                    SettingsChanged?.Invoke(this, _settings);

                    Log.Info($"Settings saved successfully to {_settingsFilePath}", typeof(SettingsManager));
                }
            }
            catch (Exception ex)
            {
                Log.Exception($"Failed to save settings: {ex.Message}", ex, typeof(SettingsManager));
                throw;
            }
        }

        /// <summary>
        /// 設定を読み込む
        /// </summary>
        /// <returns>読み込んだ設定</returns>
        private NotionSettings LoadSettings()
        {
            try
            {
                var settings = new NotionSettings();

                // JSONファイルから設定を読み込み
                if (File.Exists(_settingsFilePath))
                {
                    var jsonString = File.ReadAllText(_settingsFilePath);
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };

                    var loadedSettings = JsonSerializer.Deserialize<NotionSettings>(jsonString, jsonOptions);
                    if (loadedSettings != null)
                    {
                        settings.DatabaseId = loadedSettings.DatabaseId;
                        settings.TaskType = loadedSettings.TaskType;
                        settings.MaxMemoLength = loadedSettings.MaxMemoLength;
                        settings.TimeoutSeconds = loadedSettings.TimeoutSeconds;
                        settings.EnableLogging = loadedSettings.EnableLogging;
                        settings.LastUpdated = loadedSettings.LastUpdated;
                    }
                }

                // Windows Credential Store からAPIトークンを読み込み
                var apiToken = LoadCredential(CredentialTarget);
                if (!string.IsNullOrEmpty(apiToken))
                {
                    settings.ApiToken = apiToken;
                }

                Log.Info($"Settings loaded successfully from {_settingsFilePath}", typeof(SettingsManager));
                return settings;
            }
            catch (Exception ex)
            {
                Log.Exception($"Failed to load settings: {ex.Message}", ex, typeof(SettingsManager));
                return new NotionSettings(); // デフォルト設定を返す
            }
        }

        /// <summary>
        /// 設定をリセットする
        /// </summary>
        public void ResetSettings()
        {
            try
            {
                lock (_lock)
                {
                    // Credential Store からAPIトークンを削除
                    DeleteCredential(CredentialTarget);

                    // 設定ファイルを削除
                    if (File.Exists(_settingsFilePath))
                    {
                        File.Delete(_settingsFilePath);
                    }

                    // メモリ内の設定をリセット
                    _settings = new NotionSettings();

                    // イベントを発生
                    SettingsChanged?.Invoke(this, _settings);

                    Log.Info("Settings reset successfully", typeof(SettingsManager));
                }
            }
            catch (Exception ex)
            {
                Log.Exception($"Failed to reset settings: {ex.Message}", ex, typeof(SettingsManager));
                throw;
            }
        }

        /// <summary>
        /// 設定が有効かテストする
        /// </summary>
        /// <returns>テスト結果</returns>
        public async Task<(bool Success, string Message)> TestSettingsAsync()
        {
            try
            {
                var validationResult = _settings.Validate();
                if (validationResult != null)
                {
                    return (false, validationResult);
                }

                // 実際のAPI接続テストは NotionApiClient で実装予定
                // ここでは基本的な設定検証のみ
                return (true, "設定は正常です");
            }
            catch (Exception ex)
            {
                Log.Exception($"Settings test failed: {ex.Message}", ex, typeof(SettingsManager));
                return (false, $"設定テストに失敗しました: {ex.Message}");
            }
        }

        #region Windows Credential Store操作

        /// <summary>
        /// Windows Credential Store に資格情報を保存
        /// </summary>
        /// <param name="target">ターゲット名</param>
        /// <param name="credential">資格情報</param>
        private static void SaveCredential(string target, string credential)
        {
            try
            {
                // Windows Credential Manager を使用してAPIトークンを安全に保存
                // 実装には System.Security.Cryptography.ProtectedData または
                // Windows Credential Manager API を使用
                
                // 簡易実装: 環境変数として保存（本番では適切な暗号化実装が必要）
                // TODO: 適切な暗号化実装に置き換える
                var protectedData = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(credential));
                Environment.SetEnvironmentVariable($"POWERTOYS_NOTION_{target}", protectedData, EnvironmentVariableTarget.User);
            }
            catch (Exception ex)
            {
                Log.Exception($"Failed to save credential: {ex.Message}", ex, typeof(SettingsManager));
                throw;
            }
        }

        /// <summary>
        /// Windows Credential Store から資格情報を読み込み
        /// </summary>
        /// <param name="target">ターゲット名</param>
        /// <returns>資格情報</returns>
        private static string? LoadCredential(string target)
        {
            try
            {
                // 簡易実装: 環境変数から読み込み
                // TODO: 適切な暗号化実装に置き換える
                var protectedData = Environment.GetEnvironmentVariable($"POWERTOYS_NOTION_{target}", EnvironmentVariableTarget.User);
                if (string.IsNullOrEmpty(protectedData))
                    return null;

                var credentialBytes = Convert.FromBase64String(protectedData);
                return System.Text.Encoding.UTF8.GetString(credentialBytes);
            }
            catch (Exception ex)
            {
                Log.Exception($"Failed to load credential: {ex.Message}", ex, typeof(SettingsManager));
                return null;
            }
        }

        /// <summary>
        /// Windows Credential Store から資格情報を削除
        /// </summary>
        /// <param name="target">ターゲット名</param>
        private static void DeleteCredential(string target)
        {
            try
            {
                // 簡易実装: 環境変数を削除
                // TODO: 適切な暗号化実装に置き換える
                Environment.SetEnvironmentVariable($"POWERTOYS_NOTION_{target}", null, EnvironmentVariableTarget.User);
            }
            catch (Exception ex)
            {
                Log.Exception($"Failed to delete credential: {ex.Message}", ex, typeof(SettingsManager));
                throw;
            }
        }

        #endregion
    }
}
