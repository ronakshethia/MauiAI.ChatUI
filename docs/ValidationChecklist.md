# Validation Checklist

Use this checklist after building the library or consuming the NuGet package.

## Setup

1. Run the sample app and confirm the shell opens without startup exceptions.
2. Configure a valid OpenAI API key through `OPENAI_API_KEY` or `samples/MauiAI.ChatUI.Sample/MauiProgram.cs`.
3. Confirm the first-run empty state explains how to start chatting.

## UI Behavior

1. Verify the chat view renders with left-aligned assistant bubbles and right-aligned user bubbles.
2. Send a short message and confirm the collection auto-scrolls to the latest response.
3. Send multiple prompts and confirm older messages remain visible and the context is preserved.
4. Confirm the send button disables while a response is streaming.

## Streaming

1. Ask for a long answer and confirm the assistant bubble updates progressively instead of waiting for the full response.
2. Confirm the UI remains responsive while streaming is active.
3. Disconnect the network or use an invalid API key and confirm the error appears inside the assistant bubble without crashing the app.

## Markdown

1. Ask the model to return `**bold**` text and verify it renders bold.
2. Ask the model to return inline code such as `` `Console.WriteLine()` `` and verify it renders with code styling.
3. Ask the model to return a fenced code block and verify it renders in a distinct code container.

## Packaging

1. Run `dotnet pack src/MauiAI.ChatUI/MauiAI.ChatUI.csproj -c Release`.
2. Confirm the generated `.nupkg` metadata uses `PackageId` = `MauiAI.ConversationUI`.
3. Confirm the package description, tags, and README are present in the package metadata.
4. Install the package into a fresh MAUI app and verify setup takes less than 10 minutes.

## Prompt

Use this verification prompt when manually testing:

```text
Test MauiAI.ChatUI as if you are a new MAUI developer.
1. Install the package and register builder.Services.AddChatUI().AddOpenAI("API_KEY").
2. Add <ai:ChatView /> to a ContentPage.
3. Send a prompt that produces a long answer and verify tokens stream in gradually.
4. Send a follow-up question and verify prior messages influence the reply.
5. Ask for bold text, inline code, and a fenced code block and verify rendering.
6. Confirm the page stays responsive and no freezes occur during streaming.
7. Confirm the sample project runs and demonstrates the intended developer experience.
```
