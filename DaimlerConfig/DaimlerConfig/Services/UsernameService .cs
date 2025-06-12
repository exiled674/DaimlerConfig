using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Storage;
using System.Text.Json;

public class UsernameService : INotifyPropertyChanged
{
    private const string UsernameKey = "Username";
    private string _username;

    public event PropertyChangedEventHandler? PropertyChanged;

    public UsernameService()
    {
        // Load username from Preferences or fall back to Windows user
        _username = Preferences.Default.Get(UsernameKey, string.Empty);

        if (string.IsNullOrWhiteSpace(_username))
        {
            _username = DaimlerConfig.MauiProgram.Username;
            Preferences.Default.Set(UsernameKey, _username);
        }
    }

    public string Username
    {
        get => _username;
        private set
        {
            if (_username != value)
            {
                _username = value;
                OnPropertyChanged();
            }
        }
    }

    public void UpdateUsername(string newUsername)
    {
        if (!string.IsNullOrWhiteSpace(newUsername) && _username != newUsername)
        {
            Preferences.Default.Set(UsernameKey, newUsername);
            Username = newUsername;
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


    public void ResetToWindowsUsername()
    {
        var windowsUsername = DaimlerConfig.MauiProgram.Username;
        Preferences.Default.Set(UsernameKey, windowsUsername);
        Username = windowsUsername;
    }


    public void SaveList(List<string> items)
    {
        var json = JsonSerializer.Serialize(items);
        Preferences.Default.Set("star", json);
    }

    public List<string> LoadList()
    {
        var json = Preferences.Default.Get("star", "[]");
        return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
    }


}
