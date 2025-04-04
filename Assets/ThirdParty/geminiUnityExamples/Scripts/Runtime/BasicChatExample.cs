using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoogleApis.GenerativeLanguage;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleApis.Example
{
    /// <summary>
    /// Basic Chat Example
    /// </summary>
    public sealed class BasicChatExample : MonoBehaviour
    {
        [Header("UI references")] [SerializeField]
        private TMP_InputField inputField;

        [SerializeField] private TextMeshProUGUI messageLabel;

        [SerializeField] private Button sendButton;

        [Header("Options")] [SerializeField] [TextArea(1, 10)]
        private string systemInstruction = string.Empty;

        [SerializeField] private bool showAvailableModels = false;

        [SerializeField] private bool useStream = false;


        private GenerativeModel model;
        private readonly List<Content> messages = new();
        private static readonly StringBuilder sb = new();

        private async void Start()
        {
            using var settings = GoogleApiSettings.Get();
            var client = new GenerativeAIClient(settings);

            // List all available models
            if (showAvailableModels)
            {
                var models = await client.ListModelsAsync(destroyCancellationToken);
                Debug.Log($"Available models: {models}");
            }

            model = client.GetModel(Models.Gemini_1_5_Pro);

            // Setup UIs
            sendButton?.onClick.AddListener(async () => await SendRequest());
            if (inputField != null)
            {
                inputField.onSubmit.AddListener(async _ => await SendRequest());

                // for Debug
                inputField.text = "Write a story about a cat and a dog.";
            }
        }

        private async Task SendRequest()
        {
            var input = inputField.text;
            await SendRequest(input, systemInstruction, true);
            inputField.text = string.Empty;
        }
        const string censoredAscii=
            "--------------------------------\n" +
            "-xxxxxxxxxxxxxxxxxxxxxxx-\n" +
            "-xxxxxxxxxxxxxxxxxxxxxxx-\n" +
            "-xxxxxxxxxxxxxxxxxxxxxxx-\n" +
            "-xxxxxxxxxxxxxxxxxxxxxxx-\n" +
            "-xxxxxxCENSOREDxxxxx-\n" +
            "-xxxxxxxxxxxxxxxxxxxxxxx-\n" +
            "-xxxxxxxxxxxxxxxxxxxxxxx-\n" +
            "-xxxxxxxxxxxxxxxxxxxxxxx-\n" +
            "-xxxxxxxxxxxxxxxxxxxxxxx-\n" +
            "--------------------------------";
        public async Task<string> SendRequest(string input, string systemInstruction, bool refreshView = false)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            Content content = new Content(Role.User, input);
            messages.Add(content);
            if (refreshView)
                RefreshView();

            GenerateContentRequest request = messages;

            // Set System prompt if exists
            if (!string.IsNullOrWhiteSpace(systemInstruction))
            {
                request.systemInstruction = new Content(new Content.Part[] { systemInstruction });
            }

            if (useStream)
            {
                await model.StreamGenerateContentAsync(request, destroyCancellationToken, (response) =>
                {
                    if (response.candidates.Length == 0)
                    {
                        return;
                    }

                    // Merge to last message if the role is the same
                    Content streamContent = response.candidates[0].content;
                    bool mergeToLast = messages.Count > 0
                                       && messages[^1].role == streamContent.role;
                    if (mergeToLast)
                    {
                        messages[^1] = MergeContent(messages[^1], streamContent);
                    }
                    else
                    {
                        messages.Add(streamContent);
                    }

                    if (refreshView)
                        RefreshView();
                });
            }
            else
            {
                GenerateContentResponse response;
                try
                {
                    response = await model.GenerateContentAsync(request, destroyCancellationToken);
                }
                catch (System.Exception e)
                {
                    response = new GenerateContentResponse(
                        new Candidate[1]
                        {
                            new Candidate(new Content(Role.Model,
                                new Content.Part()
                                    { text = censoredAscii }))
                        }, null);
                    Debug.LogError("Full exception to find censor reasons : " + e.Message);
                }

                if (response.candidates.Length > 0)
                {
                    var modelContent = response.candidates[0].content;
                    messages.Add(modelContent);
                    if (refreshView)
                        RefreshView();
                    else
                    {
                        try
                        {
                            return string.Join(",", modelContent.parts.Select(x => x?.ToString()));
                        }
                        catch (NullReferenceException e)
                        {
                            
                            return new Content.Part()
                                    { text = censoredAscii }
                                .ToString();
                        }
                    }
                }
            }

            return null;
        }

        private void RefreshView()
        {
            sb.Clear();
            foreach (var message in messages)
            {
                sb.AppendTMPRichText(message);
            }

            messageLabel.SetText(sb);
        }

        private static Content MergeContent(Content a, Content b)
        {
            if (a.role != b.role)
            {
                return null;
            }

            sb.Clear();
            var parts = new List<Content.Part>();
            foreach (var part in a.parts)
            {
                if (string.IsNullOrWhiteSpace(part.text))
                {
                    parts.Add(part);
                }
                else
                {
                    sb.Append(part.text);
                }
            }

            foreach (var part in b.parts)
            {
                if (string.IsNullOrWhiteSpace(part.text))
                {
                    parts.Add(part);
                }
                else
                {
                    sb.Append(part.text);
                }
            }

            parts.Insert(0, sb.ToString());
            return new Content(a.role.Value, parts.ToArray());
        }
    }
}