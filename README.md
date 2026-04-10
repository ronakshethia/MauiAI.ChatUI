# 🚀 Ronak.MauiAI.ChatUI

### 🔥 Add ChatGPT-like UI to your .NET MAUI app in minutes

> **Ronak.MauiAI.ChatUI** is a lightweight, plug-and-play AI chat UI for .NET MAUI with **streaming responses**, **basic markdown support**, and **OpenAI integration**.

---

## ✨ Why Ronak.MauiAI.ChatUI?

Building AI chat in MAUI today requires:

* manual UI creation
* handling streaming responses
* integrating OpenAI APIs
* managing conversation state

👉 This package simplifies everything into a **drop-in solution**.

---

## ⚡ V1 Features (Focused & Minimal)

* ✅ ChatGPT-style UI (ready-to-use)
* ✅ Real-time streaming responses (`IAsyncEnumerable`)
* ✅ Basic markdown rendering (**bold, code blocks**)
* ✅ In-memory conversation context
* ✅ Plug-and-play setup (2 lines of code)
* ✅ Lightweight and fast

---

## 🚫 What’s NOT included (V1)

To keep the package simple and fast:

* ❌ Chat persistence (database)
* ❌ Azure OpenAI
* ❌ Voice input/output
* ❌ Attachments / media
* ❌ Advanced theming
* ❌ Offline support

---

## 📦 Installation

```bash
dotnet add package Ronak.MauiAI.ChatUI
```

---

## ⚙️ Quick Start (2 Steps)

### 1️⃣ Configure in `MauiProgram.cs`

```csharp
builder.Services.AddChatUI()
                .AddOpenAI("YOUR_OPENAI_API_KEY");
```

---

### 2️⃣ Add ChatView to your page

```xml
<ai:ChatView />
```

---

## ▶️ Result

* Type a message
* Press send
* AI responds **in real-time (streaming)**

---

## 🧠 How It Works

* Messages stored in memory
* Full conversation sent to AI (context-aware)
* Response streamed token-by-token
* UI updates incrementally

---

## ⚠️ Important: Billing & 429 Error

### 🚨 You may see this error:

> **HTTP 429 – Too Many Requests / Quota Exceeded**

### 🔍 Why this happens

* OpenAI **requires billing** for chat/completions API
* Free trial credits (if any) may be expired
* You can still fetch models, but **chat calls fail**

---

### ✅ Fix

1. Go to: https://platform.openai.com/usage
2. Check if you have available credits
3. Add billing: https://platform.openai.com/settings/organization/billing

---

### 💡 Recommended Model

Use:

```csharp
gpt-4o-mini
```

* 💰 Very low cost
* ⚡ Fast
* ✅ Ideal for testing and demos

---

### 🧪 Quick Debug

If you can fetch models but chat fails:

👉 It is **almost always a billing issue**

---

## 🔌 Extensibility (Advanced)

You can plug your own AI provider:

```csharp
public class CustomProvider : IChatProvider
{
    public async IAsyncEnumerable<string> StreamAsync(List<ChatMessage> messages)
    {
        yield return "Custom response";
    }
}
```

---

## 📱 Sample App

A working sample is included:

👉 `MauiAI.ChatUI.Sample`

---

## 📌 Use Cases

* AI assistants
* Customer support chat
* Banking apps
* Internal copilots
* Developer tools

---

## 🧪 Keywords (SEO)

maui ai chat, maui chatgpt ui, openai maui, ai chat control maui, streaming chat maui, dotnet maui ai ui

---

## 🚀 Roadmap (Future)

* Azure OpenAI support
* Chat persistence
* Advanced markdown
* Theming & customization
* Multi-provider support

---

## 🤝 Contributing

Contributions welcome!
Feel free to open issues or PRs.

---

## 📄 License

MIT License

---

## 💥 Final Thought

> Stop building chat UI from scratch.
> Start shipping AI features faster.

---
