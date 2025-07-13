// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace fastNotionExtension;

public class NotionSettings
{
    public string ApiToken { get; set; } = string.Empty;
    public string DatabaseId { get; set; } = string.Empty;

    private static string SettingsFilePath
    {
        get
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var settingsDir = Path.Combine(appData, "Microsoft", "PowerToys", "fastNotionExtension");
            Directory.CreateDirectory(settingsDir);
            return Path.Combine(settingsDir, "settings.json");
        }
    }

    public static async Task<NotionSettings> LoadAsync()
    {
        if (!File.Exists(SettingsFilePath))
        {
            return new NotionSettings();
        }

        var json = await File.ReadAllTextAsync(SettingsFilePath);
        return JsonSerializer.Deserialize<NotionSettings>(json) ?? new NotionSettings();
    }

    public async Task SaveAsync()
    {
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(SettingsFilePath, json);
    }
}
