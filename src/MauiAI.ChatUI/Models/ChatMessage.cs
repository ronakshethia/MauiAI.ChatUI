using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiAI.ChatUI.Models;

public class ChatMessage : INotifyPropertyChanged
{
    private string _content = string.Empty;
    private bool _isUser;

    public string Content
    {
        get => _content;
        set => SetProperty(ref _content, value);
    }

    public bool IsUser
    {
        get => _isUser;
        set => SetProperty(ref _isUser, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
        {
            return false;
        }

        backingStore = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
