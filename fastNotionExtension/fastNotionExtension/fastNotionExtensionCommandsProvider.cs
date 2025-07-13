using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using CommunityToolkit.Mvvm.Input;
using fastNotionExtension;

namespace fastNotionExtension;

public partial class fastNotionExtensionCommandsProvider : CommandProvider
{
    private const string CommandName = "memo";

    public fastNotionExtensionCommandsProvider()
    {
        DisplayName = "Notion";
        Icon = IconHelpers.FromRelativePath(@"Assets\StoreLogo.png");
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return new[]
        {
            new CommandItem(CommandName, "Add to Notion")
            {
                Command = new CommandWrapper(async (contextObject) =>
                {
                    dynamic context = contextObject; // Use dynamic to avoid compile-time errors

                    var inputText = context.Query.Text.Trim();
                    var commandName = "memo";
                    var noteContent = inputText.StartsWith(commandName + " ", StringComparison.OrdinalIgnoreCase)
                        ? inputText.Substring(commandName.Length).Trim()
                        : string.Empty;

                    if (string.IsNullOrWhiteSpace(noteContent))
                    {
                        context.ShowNotification("Warning", "Please provide content for the Notion memo.");
                        return;
                    }

                    var settings = await NotionSettings.LoadAsync();
                    var apiToken = settings.ApiToken;
                    var databaseId = settings.DatabaseId;

                    if (string.IsNullOrEmpty(apiToken) || string.IsNullOrEmpty(databaseId))
                    {
                        context.ShowNotification("Warning", "Notion API token or Database ID is not set. Please configure them using the 'settings' command.");
                        return;
                    }

                    var notionClient = new NotionApiClient(apiToken, databaseId);

                    try
                    {
                        await notionClient.CreatePageAsync(noteContent);
                        context.ShowNotification("Success", "Notion memo created successfully.");
                    }
                    catch (Exception ex)
                    {
                        context.ShowNotification("Error", $"Failed to create Notion memo: {ex.Message}");
                    }
                }, this.Icon, CommandName),
                Icon = this.Icon,
            },
            new CommandItem("settings", "Configure Notion API Settings")
            {
                Command = new CommandWrapper(async (contextObject) =>
                {
                    dynamic context = contextObject;
                    var inputText = context.Query.Text.Trim();
                    var parts = inputText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length < 3 || parts[0].ToLower() != "settings")
                    {
                        context.ShowNotification("Warning", "Usage: settings <api_token> <database_id>");
                        return;
                    }

                    var apiToken = parts[1];
                    var databaseId = parts[2];

                    var settings = new NotionSettings { ApiToken = apiToken, DatabaseId = databaseId };
                    try
                    {
                        await settings.SaveAsync();
                        context.ShowNotification("Success", "Notion settings saved successfully.");
                    }
                    catch (Exception ex)
                    {
                        context.ShowNotification("Error", $"Failed to save settings: {ex.Message}");
                    }
                }, this.Icon, "settings"),
                Icon = this.Icon,
            }
        };
    }
}