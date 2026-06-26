using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityBot_Part3
{
    public class ChatbotEngine
    {
        private string[] keywords = {
            "how are you", "purpose", "what can i ask you about",
            "password", "phishing", "browsing", "link"
        };

        private string[] responses = {
            "I am functioning well and ready to help you stay safe online!",
            "My purpose is to educate users about cybersecurity.",
            "You can ask about passwords, phishing, safe browsing, scams and privacy.",
            "Use strong passwords with letters, numbers, and symbols.",
            "Be careful of fake emails asking for personal information.",
            "Only visit trusted websites and avoid suspicious downloads.",
            "Do not click suspicious links. Always verify URLs."
        };

        private Dictionary<string, List<string>> keywordResponses;
        private Dictionary<string, string> userMemory;
        private Dictionary<string, string[]> sentimentPatterns;
        private string currentTopic;
        private string userName;
        private Random random;

        public ChatbotEngine()
        {
            userMemory = new Dictionary<string, string>();
            random = new Random();
            currentTopic = "";
            InitializeFeatures();
        }

        private void InitializeFeatures()
        {
            keywordResponses = new Dictionary<string, List<string>>()
            {
                { "password", new List<string> {
                    "Use strong passwords with letters, numbers, and symbols.",
                    "Never reuse passwords across different accounts.",
                    "Consider using a password manager.",
                    "Enable Two-Factor Authentication when possible."
                }},
                { "scam", new List<string> {
                    "Be wary of unsolicited calls or emails asking for personal information.",
                    "Scammers often create urgency to make you act quickly.",
                    "Never click links in suspicious messages.",
                    "Verify independently through official channels."
                }},
                { "privacy", new List<string> {
                    "Review your privacy settings on social media regularly.",
                    "Use a VPN when connecting to public Wi-Fi.",
                    "Be mindful of what personal information you share online.",
                    "Check app permissions on your phone regularly."
                }},
                { "phishing", new List<string> {
                    "Check email sender addresses carefully.",
                    "Hover over links before clicking to see the destination.",
                    "Legitimate companies never ask for passwords via email.",
                    "Look for spelling errors and generic greetings in emails."
                }},
                { "2fa", new List<string> {
                    "Two-Factor Authentication adds an extra layer of security.",
                    "Enable 2FA on all your important accounts.",
                    "Use an authenticator app instead of SMS for better security."
                }},
                { "vpn", new List<string> {
                    "A VPN encrypts your internet traffic.",
                    "Always use a VPN on public Wi-Fi networks.",
                    "Choose a reputable VPN provider with a no-logs policy."
                }}
            };

            sentimentPatterns = new Dictionary<string, string[]>()
            {
                { "worried",    new string[] { "worried", "scared", "nervous", "anxious", "concerned" } },
                { "frustrated", new string[] { "frustrated", "annoyed", "angry", "upset" } },
                { "curious",    new string[] { "curious", "interested", "want to learn" } },
                { "confused",   new string[] { "confused", "don't understand", "unclear" } }
            };
        }

        public void SetUserName(string name)
        {
            userName = name;
            userMemory["name"] = name;
        }

        public string GetResponse(string userInput)
        {
            string lower = userInput.ToLower();

            if (lower == "exit") return "Goodbye! Stay safe online!";

            if (lower.Contains("hello") || lower.Contains("hi") || lower.Contains("hey"))
                return $"Hello, {userName}! How can I help you stay safe online today?";

            if (lower.Contains("my name") || lower.Contains("remember me"))
                return RecallUserInfo();

            string sentiment = DetectSentiment(lower);

            // Check original keywords
            for (int i = 0; i < keywords.Length; i++)
            {
                if (lower.Contains(keywords[i]))
                {
                    currentTopic = keywords[i];
                    return AddSentimentPrefix(sentiment, responses[i]);
                }
            }

            // Check enhanced keywords
            foreach (var keyword in keywordResponses)
            {
                if (lower.Contains(keyword.Key))
                {
                    currentTopic = keyword.Key;
                    string response = keyword.Value[random.Next(keyword.Value.Count)];
                    return AddSentimentPrefix(sentiment, response);
                }
            }

            if (lower.Contains("tell me more") || lower.Contains("another tip"))
                return GetFollowUpResponse();

            return GetDefaultResponse();
        }

        private string DetectSentiment(string input)
        {
            foreach (var s in sentimentPatterns)
                if (s.Value.Any(p => input.Contains(p))) return s.Key;
            return "neutral";
        }

        private string AddSentimentPrefix(string sentiment, string response)
        {
            switch (sentiment)
            {
                case "worried": return $"I understand your concern. {response}";
                case "frustrated": return $"I hear your frustration. Let me help: {response}";
                case "curious": return $"Great question! {response}";
                case "confused": return $"Let me clarify: {response}";
                default: return response;
            }
        }

        private string GetFollowUpResponse()
        {
            if (!string.IsNullOrEmpty(currentTopic) && keywordResponses.ContainsKey(currentTopic))
            {
                var list = keywordResponses[currentTopic];
                return $"Here's another tip: {list[random.Next(list.Count)]}";
            }
            return "What topic would you like more tips on? Try passwords, phishing, privacy or scams!";
        }

        private string RecallUserInfo()
        {
            if (userMemory.ContainsKey("name"))
                return $"Of course! Your name is {userMemory["name"]}. How can I help you today?";
            return "I'd love to get to know you better! What's your name?";
        }

        private string GetDefaultResponse()
        {
            string[] defaults = {
                "I'm not sure about that. Try asking about passwords, phishing, privacy or scams!",
                "Could you rephrase that? I'm here to help with cybersecurity topics.",
                "Try asking me about passwords, phishing, scams, 2FA or privacy for helpful tips.",
                "I didn't quite understand that. Ask me about cybersecurity topics!"
            };
            return defaults[random.Next(defaults.Length)];
        }
    }
}