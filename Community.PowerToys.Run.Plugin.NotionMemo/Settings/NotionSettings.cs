using System;
using System.ComponentModel;
using System.Security;
using System.Text.Json.Serialization;
using System.Linq;

namespace Community.PowerToys.Run.Plugin.NotionMemo.Settings
{
    /// <summary>
    /// Notion API設定を保持するクラス
    /// </summary>
    public class NotionSettings : INotifyPropertyChanged
    {
        private string _apiToken = string.Empty;
        private string _databaseId = string.Empty;
        private string _taskType = "📝PowerToysメモ";
        private int _maxMemoLength = 500;
        private int _timeoutSeconds = 10;
        private bool _enableLogging = true;

        /// <summary>
        /// Notion API トークン（暗号化して保存）
        /// </summary>
        [JsonIgnore] // JSONシリアライズ時は除外（セキュリティ上）
        public string ApiToken
        {
            get => _apiToken;
            set
            {
                if (_apiToken != value)
                {
                    _apiToken = value;
                    OnPropertyChanged(nameof(ApiToken));
                    OnPropertyChanged(nameof(IsApiTokenSet));
                }
            }
        }

        /// <summary>
        /// NotionデータベースID
        /// </summary>
        public string DatabaseId
        {
            get => _databaseId;
            set
            {
                if (_databaseId != value)
                {
                    _databaseId = value;
                    OnPropertyChanged(nameof(DatabaseId));
                    OnPropertyChanged(nameof(IsDatabaseIdSet));
                }
            }
        }

        /// <summary>
        /// タスク種別の固定値
        /// </summary>
        public string TaskType
        {
            get => _taskType;
            set
            {
                if (_taskType != value)
                {
                    _taskType = value;
                    OnPropertyChanged(nameof(TaskType));
                }
            }
        }

        /// <summary>
        /// メモの最大文字数
        /// </summary>
        public int MaxMemoLength
        {
            get => _maxMemoLength;
            set
            {
                if (_maxMemoLength != value && value > 0)
                {
                    _maxMemoLength = value;
                    OnPropertyChanged(nameof(MaxMemoLength));
                }
            }
        }

        /// <summary>
        /// API接続タイムアウト（秒）
        /// </summary>
        public int TimeoutSeconds
        {
            get => _timeoutSeconds;
            set
            {
                if (_timeoutSeconds != value && value > 0)
                {
                    _timeoutSeconds = value;
                    OnPropertyChanged(nameof(TimeoutSeconds));
                }
            }
        }

        /// <summary>
        /// ログ出力を有効にするか
        /// </summary>
        public bool EnableLogging
        {
            get => _enableLogging;
            set
            {
                if (_enableLogging != value)
                {
                    _enableLogging = value;
                    OnPropertyChanged(nameof(EnableLogging));
                }
            }
        }

        /// <summary>
        /// APIトークンが設定されているか
        /// </summary>
        [JsonIgnore]
        public bool IsApiTokenSet => !string.IsNullOrEmpty(ApiToken);

        /// <summary>
        /// データベースIDが設定されているか
        /// </summary>
        [JsonIgnore]
        public bool IsDatabaseIdSet => !string.IsNullOrEmpty(DatabaseId) && IsValidDatabaseId(DatabaseId);

        /// <summary>
        /// 設定が完了しているか（APIトークンとデータベースIDが両方設定済み）
        /// </summary>
        [JsonIgnore]
        public bool IsConfigured => IsApiTokenSet && IsDatabaseIdSet;

        /// <summary>
        /// 設定の最終更新日時
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        /// <summary>
        /// PropertyChangedイベント
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public NotionSettings()
        {
        }

        /// <summary>
        /// 設定をリセットする
        /// </summary>
        public void Reset()
        {
            ApiToken = string.Empty;
            DatabaseId = string.Empty;
            TaskType = "📝PowerToysメモ";
            MaxMemoLength = 500;
            TimeoutSeconds = 10;
            EnableLogging = true;
            LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// 設定を検証する
        /// </summary>
        /// <returns>検証結果のメッセージ。nullの場合は正常</returns>
        public string? Validate()
        {
            if (!IsApiTokenSet)
                return "Notion APIトークンが設定されていません";

            if (!IsDatabaseIdSet)
                return "データベースIDが設定されていないか、形式が正しくありません";

            if (string.IsNullOrEmpty(TaskType))
                return "タスク種別が設定されていません";

            if (MaxMemoLength <= 0 || MaxMemoLength > 2000)
                return "メモの最大文字数は1〜2000文字で設定してください";

            if (TimeoutSeconds <= 0 || TimeoutSeconds > 60)
                return "タイムアウトは1〜60秒で設定してください";

            return null; // 正常
        }

        /// <summary>
        /// データベースIDの形式を検証する
        /// </summary>
        /// <param name="databaseId">データベースID</param>
        /// <returns>有効な場合true</returns>
        private static bool IsValidDatabaseId(string databaseId)
        {
            if (string.IsNullOrEmpty(databaseId))
                return false;

            // UUIDの形式チェック（ハイフンあり・なし両方対応）
            if (databaseId.Length == 32)
            {
                // ハイフンなしの場合
                return databaseId.All(c => char.IsLetterOrDigit(c));
            }
            else if (databaseId.Length == 36)
            {
                // ハイフンありの場合
                return Guid.TryParse(databaseId, out _);
            }

            return false;
        }

        /// <summary>
        /// データベースIDを正規化する（ハイフンを除去）
        /// </summary>
        /// <param name="databaseId">データベースID</param>
        /// <returns>正規化されたデータベースID</returns>
        public static string NormalizeDatabaseId(string databaseId)
        {
            if (string.IsNullOrEmpty(databaseId))
                return string.Empty;

            return databaseId.Replace("-", "");
        }

        /// <summary>
        /// PropertyChangedイベントを発生させる
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 設定を文字列として表示（デバッグ用）
        /// </summary>
        /// <returns>設定の概要</returns>
        public override string ToString()
        {
            return $"NotionSettings {{ " +
                   $"ApiToken: {(IsApiTokenSet ? "Set" : "Not Set")}, " +
                   $"DatabaseId: {(IsDatabaseIdSet ? "Set" : "Not Set")}, " +
                   $"TaskType: {TaskType}, " +
                   $"MaxLength: {MaxMemoLength}, " +
                   $"Timeout: {TimeoutSeconds}s, " +
                   $"Logging: {EnableLogging}" +
                   $" }}";
        }
    }
}
