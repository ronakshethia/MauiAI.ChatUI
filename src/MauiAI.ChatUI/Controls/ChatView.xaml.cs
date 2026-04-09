using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using MauiAI.ChatUI.Models;
using MauiAI.ChatUI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MauiAI.ChatUI.Controls;

public partial class ChatView : ContentView
{
    public static readonly BindableProperty MessagesProperty = BindableProperty.Create(
        nameof(Messages),
        typeof(ObservableCollection<ChatMessage>),
        typeof(ChatView),
        defaultValueCreator: static _ => new ObservableCollection<ChatMessage>(),
        propertyChanged: static (bindable, oldValue, newValue) =>
            ((ChatView)bindable).OnMessagesChanged(
                oldValue as ObservableCollection<ChatMessage>,
                newValue as ObservableCollection<ChatMessage>));

    public static readonly BindableProperty InputTextProperty = BindableProperty.Create(
        nameof(InputText),
        typeof(string),
        typeof(ChatView),
        string.Empty,
        propertyChanged: static (bindable, _, _) => ((ChatView)bindable).UpdateCanSend());

    public static readonly BindableProperty IsSendingProperty = BindableProperty.Create(
        nameof(IsSending),
        typeof(bool),
        typeof(ChatView),
        false,
        propertyChanged: static (bindable, _, _) => ((ChatView)bindable).UpdateCanSend());

    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(
        nameof(PlaceholderText),
        typeof(string),
        typeof(ChatView),
        "Send a message");

    public static readonly BindableProperty SendButtonTextProperty = BindableProperty.Create(
        nameof(SendButtonText),
        typeof(string),
        typeof(ChatView),
        "Send");

    public static readonly BindableProperty EmptyStateTitleProperty = BindableProperty.Create(
        nameof(EmptyStateTitle),
        typeof(string),
        typeof(ChatView),
        "Start the conversation");

    public static readonly BindableProperty EmptyStateSubtitleProperty = BindableProperty.Create(
        nameof(EmptyStateSubtitle),
        typeof(string),
        typeof(ChatView),
        "Install the package, register AddChatUI().AddOpenAI(...), and this view is ready to stream responses.");

    private ChatService? _chatService;

    public ChatView()
    {
        InitializeComponent();
        OnMessagesChanged(null, Messages);
        UpdateCanSend();
    }

    public ObservableCollection<ChatMessage> Messages
    {
        get => (ObservableCollection<ChatMessage>)GetValue(MessagesProperty);
        set => SetValue(MessagesProperty, value);
    }

    public string InputText
    {
        get => (string)GetValue(InputTextProperty);
        set => SetValue(InputTextProperty, value);
    }

    public bool IsSending
    {
        get => (bool)GetValue(IsSendingProperty);
        set => SetValue(IsSendingProperty, value);
    }

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public string SendButtonText
    {
        get => (string)GetValue(SendButtonTextProperty);
        set => SetValue(SendButtonTextProperty, value);
    }

    public string EmptyStateTitle
    {
        get => (string)GetValue(EmptyStateTitleProperty);
        set => SetValue(EmptyStateTitleProperty, value);
    }

    public string EmptyStateSubtitle
    {
        get => (string)GetValue(EmptyStateSubtitleProperty);
        set => SetValue(EmptyStateSubtitleProperty, value);
    }

    public bool CanSend => !IsSending && !string.IsNullOrWhiteSpace(InputText);

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        _chatService = Handler?.MauiContext?.Services.GetService<ChatService>();
        UpdateCanSend();
    }

    private async void OnSendClicked(object? sender, EventArgs e)
    {
        if (!CanSend)
        {
            return;
        }

        var service = _chatService;
        if (service is null)
        {
            Messages.Add(new ChatMessage
            {
                Content = "Chat services are not registered. Call builder.Services.AddChatUI().AddOpenAI(\"YOUR_API_KEY\").",
                IsUser = false
            });

            return;
        }

        var input = InputText.Trim();
        InputText = string.Empty;
        IsSending = true;

        try
        {
            await service.SendAsync(Messages, input);
        }
        finally
        {
            IsSending = false;
        }
    }

    private void OnMessagesChanged(ObservableCollection<ChatMessage>? oldMessages, ObservableCollection<ChatMessage>? newMessages)
    {
        if (oldMessages is not null)
        {
            oldMessages.CollectionChanged -= OnCollectionChanged;
            UnsubscribeFromItems(oldMessages);
        }

        if (newMessages is not null)
        {
            newMessages.CollectionChanged += OnCollectionChanged;
            SubscribeToItems(newMessages);
        }
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (var item in e.OldItems.OfType<ChatMessage>())
            {
                item.PropertyChanged -= OnMessagePropertyChanged;
            }
        }

        if (e.NewItems is not null)
        {
            foreach (var item in e.NewItems.OfType<ChatMessage>())
            {
                item.PropertyChanged += OnMessagePropertyChanged;
            }
        }

        ScrollToBottom();
    }

    private void OnMessagePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ChatMessage.Content))
        {
            ScrollToBottom();
        }
    }

    private void SubscribeToItems(IEnumerable<ChatMessage> messages)
    {
        foreach (var message in messages)
        {
            message.PropertyChanged += OnMessagePropertyChanged;
        }
    }

    private void UnsubscribeFromItems(IEnumerable<ChatMessage> messages)
    {
        foreach (var message in messages)
        {
            message.PropertyChanged -= OnMessagePropertyChanged;
        }
    }

    private void ScrollToBottom()
    {
        if (Messages.Count == 0)
        {
            return;
        }

        Dispatcher.Dispatch(() =>
        {
            var lastMessage = Messages[^1];
            MessagesView.ScrollTo(lastMessage, position: ScrollToPosition.End, animate: true);
        });
    }

    private void UpdateCanSend()
    {
        OnPropertyChanged(nameof(CanSend));
    }
}
