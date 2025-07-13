// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using Windows.Foundation;

namespace fastNotionExtension;

public partial class CommandWrapper : Microsoft.CommandPalette.Extensions.ICommand, INotifyPropChanged
{
    private readonly AsyncRelayCommand<object> _relayCommand;
    private readonly IIconInfo _icon;
    private readonly string _name;
    private readonly string _id;

    public CommandWrapper(Func<object?, Task> execute, IIconInfo icon, string name)
    {
        _relayCommand = new AsyncRelayCommand<object?>(execute);
        _icon = icon;
        _name = name;
        _id = Guid.NewGuid().ToString();
    }

    public string Id => _id;

    public string Name => _name;

    public IIconInfo Icon => _icon;

    public event TypedEventHandler<object, IPropChangedEventArgs>? PropChanged;

    public bool CanExecute(object? parameter)
    {
        return _relayCommand.CanExecute(parameter);
    }

    public void Execute(object? parameter)
    {
        _relayCommand.Execute(parameter);
    }

    public event EventHandler? CanExecuteChanged
    {
        add { _relayCommand.CanExecuteChanged += value; }
        remove { _relayCommand.CanExecuteChanged -= value; }
    }

    protected void OnPropChanged([CallerMemberName] string? propertyName = null)
    {
        PropChanged?.Invoke(this, new PropChangedEventArgs(propertyName!));
    }
}
