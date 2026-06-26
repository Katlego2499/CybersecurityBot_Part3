using System.Collections.Generic;

namespace CybersecurityBot_Part3
{
    public class QuizQuestion
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public int CorrectIndex { get; set; }
        public string Explanation { get; set; }
    }

    public class QuizEngine
    {
        public List<QuizQuestion> Questions { get; private set; }
        public int CurrentIndex { get; private set; }
        public int Score { get; private set; }

        public QuizEngine()
        {
            CurrentIndex = 0;
            Score = 0;
            Questions = new List<QuizQuestion>
            {
                new QuizQuestion {
                    Question = "What should you do if you receive an email asking for your password?",
                    Options = new List<string> { "A) Reply with your password", "B) Delete the email", "C) Report it as phishing", "D) Ignore it" },
                    CorrectIndex = 2,
                    Explanation = "Reporting phishing emails helps prevent scams and protects others."
                },
                new QuizQuestion {
                    Question = "True or False: You should use the same password for all accounts.",
                    Options = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 1,
                    Explanation = "Reusing passwords puts all your accounts at risk if one is breached."
                },
                new QuizQuestion {
                    Question = "What does 2FA stand for?",
                    Options = new List<string> { "A) Two-Factor Authentication", "B) Two-File Access", "C) Twice Failed Attempt", "D) Two-Form Authorization" },
                    CorrectIndex = 0,
                    Explanation = "2FA adds an extra layer of security beyond just your password."
                },
                new QuizQuestion {
                    Question = "True or False: Public Wi-Fi is safe to use for online banking.",
                    Options = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 1,
                    Explanation = "Public Wi-Fi can be intercepted by hackers. Always use a VPN."
                },
                new QuizQuestion {
                    Question = "Which is the strongest password?",
                    Options = new List<string> { "A) password123", "B) John1990", "C) P@ssw0rd!", "D) Tr!g3r$BlueMoon#9" },
                    CorrectIndex = 3,
                    Explanation = "Long passphrases with symbols and mixed case are the strongest."
                },
                new QuizQuestion {
                    Question = "What is phishing?",
                    Options = new List<string> { "A) A type of fishing sport", "B) Fake emails or sites to steal info", "C) A firewall technique", "D) Encrypted browsing" },
                    CorrectIndex = 1,
                    Explanation = "Phishing tricks users into giving away personal information."
                },
                new QuizQuestion {
                    Question = "True or False: HTTPS websites are always 100% safe.",
                    Options = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 1,
                    Explanation = "HTTPS encrypts traffic but doesn't guarantee the site itself is trustworthy."
                },
                new QuizQuestion {
                    Question = "What should you do before clicking a link in an email?",
                    Options = new List<string> { "A) Click it immediately", "B) Hover over it to check the URL", "C) Forward it to friends", "D) Reply to verify" },
                    CorrectIndex = 1,
                    Explanation = "Hovering reveals the real destination URL before you commit to clicking."
                },
                new QuizQuestion {
                    Question = "True or False: Antivirus software makes you completely safe online.",
                    Options = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 1,
                    Explanation = "Antivirus helps but isn't foolproof. Safe browsing habits are also essential."
                },
                new QuizQuestion {
                    Question = "What is social engineering in cybersecurity?",
                    Options = new List<string> { "A) Building social media apps", "B) Manipulating people to reveal info", "C) Engineering social networks", "D) Online community management" },
                    CorrectIndex = 1,
                    Explanation = "Social engineering exploits human psychology rather than technical vulnerabilities."
                },
                new QuizQuestion {
                    Question = "How often should you update your passwords?",
                    Options = new List<string> { "A) Never", "B) Every 5 years", "C) Regularly or after a suspected breach", "D) Only when you forget them" },
                    CorrectIndex = 2,
                    Explanation = "Regular updates and immediate changes after breaches keep your accounts secure."
                }
            };
        }

        public QuizQuestion GetCurrentQuestion()
            => CurrentIndex < Questions.Count ? Questions[CurrentIndex] : null;

        public (bool isCorrect, string feedback) SubmitAnswer(int answerIndex)
        {
            var q = Questions[CurrentIndex];
            bool correct = answerIndex == q.CorrectIndex;
            if (correct) Score++;
            CurrentIndex++;
            string feedback = correct
                ? $"✅ Correct! {q.Explanation}"
                : $"❌ Wrong! The answer was: {q.Options[q.CorrectIndex]}. {q.Explanation}";
            return (correct, feedback);
        }

        public bool IsFinished => CurrentIndex >= Questions.Count;

        public string GetFinalFeedback()
        {
            double pct = (double)Score / Questions.Count * 100;
            if (pct >= 90) return "🏆 Amazing! You're a cybersecurity pro!";
            if (pct >= 70) return "👍 Good work! Keep learning to stay safe online!";
            if (pct >= 50) return "📚 Not bad! Review some cybersecurity basics to improve.";
            return "⚠️ Keep learning to stay safe online! Cybersecurity is very important!";
        }
    }
}
