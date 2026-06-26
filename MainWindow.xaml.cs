using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace CybersecurityBot_Part3
{
    public partial class MainWindow : Window
    {
        private ChatbotEngine chatbot;
        private TaskManager taskManager;
        private QuizEngine quizEngine;
        private ActivityLog activityLog;
        private string userName;

        private Dictionary<TextBox, string> placeholders = new Dictionary<TextBox, string>();

        public MainWindow()
        {
            InitializeComponent();
            chatbot = new ChatbotEngine();
            taskManager = new TaskManager();
            activityLog = new ActivityLog();

            placeholders[TaskTitle] = "Task title (e.g. Enable 2FA)";
            placeholders[TaskDesc] = "Task description";
            placeholders[TaskReminder] = "Reminder (e.g. Remind me in 3 days) — optional";
            placeholders[TaskDate] = "Date/timeframe (e.g. 2025-12-01) — optional";

            GetUserName();
            RefreshTaskList();
        }

        private void GetUserName()
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter your name:", "Cybersecurity Awareness Bot", "", -1, -1);
            while (string.IsNullOrWhiteSpace(input))
                input = Microsoft.VisualBasic.Interaction.InputBox(
                    "Name cannot be empty. Enter your name:", "Cybersecurity Awareness Bot", "", -1, -1);
            userName = input;
            chatbot.SetUserName(userName);
            AddMessage("Bot", $"Hello, {userName}! Welcome to the Cybersecurity Awareness Bot!", "#E94560");
            AddMessage("Bot", "Use the tabs above: 💬 Chat | 📋 Tasks | 🎮 Quiz | 📜 Log", "#E94560");
            AddMessage("Bot", "Ask me about passwords, phishing, browsing, scams or privacy!", "#E94560");
            activityLog.AddEntry($"User '{userName}' started the bot.");
        }

        private void AddMessage(string sender, string message, string color)
        {
            var block = new TextBlock { TextWrapping = TextWrapping.Wrap, Margin = new Thickness(5) };
            block.Inlines.Add(new Run(sender + ": ") { Foreground = Brushes.LightBlue, FontWeight = FontWeights.Bold });
            block.Inlines.Add(new Run(message) { Foreground = (Brush)new BrushConverter().ConvertFromString(color) });
            ChatDisplay.Items.Add(block);
            if (ChatDisplay.Items.Count > 0)
                ChatDisplay.ScrollIntoView(ChatDisplay.Items[ChatDisplay.Items.Count - 1]);
        }

        // ─── CHATBOT ───────────────────────────────────────────────
        private void ProcessUserInput()
        {
            string msg = UserInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(msg)) return;

            AddMessage(userName, msg, "#FFFFFF");
            UserInput.Clear();

            string lower = msg.ToLower();

            // NLP keyword detection
            if (lower.Contains("add task") || lower.Contains("new task") || lower.Contains("create task"))
            {
                AddMessage("Bot", "Go to the 📋 Tasks tab to add a task!", "#E94560");
                activityLog.AddEntry("NLP: User requested task addition via chat.");
                return;
            }
            if (lower.Contains("remind me") || lower.Contains("set reminder") || lower.Contains("set a reminder"))
            {
                AddMessage("Bot", "Head to the 📋 Tasks tab to set a reminder for a task!", "#E94560");
                activityLog.AddEntry("NLP: User requested reminder via chat.");
                return;
            }
            if (lower.Contains("quiz") || lower.Contains("test me") || lower.Contains("play game"))
            {
                AddMessage("Bot", "Switch to the 🎮 Quiz tab to start the cybersecurity quiz!", "#E94560");
                activityLog.AddEntry("NLP: User requested quiz via chat.");
                return;
            }
            if (lower.Contains("show activity") || lower.Contains("what have you done") || lower.Contains("activity log"))
            {
                var entries = activityLog.GetRecentEntries(10);
                AddMessage("Bot", "Here's a summary of recent actions:", "#E94560");
                for (int i = 0; i < entries.Count; i++)
                    AddMessage("Bot", $"{i + 1}. {entries[i]}", "#4CAF50");
                activityLog.AddEntry("User viewed activity log via chat.");
                return;
            }
            if (lower.Contains("view tasks") || lower.Contains("show tasks") || lower.Contains("my tasks"))
            {
                AddMessage("Bot", "Switch to the 📋 Tasks tab to view and manage your tasks!", "#E94560");
                return;
            }

            string response = chatbot.GetResponse(msg);
            AddMessage("Bot", response, "#E94560");
            activityLog.AddEntry($"Chat: '{msg.Substring(0, Math.Min(40, msg.Length))}'");

            if (msg.ToLower() == "exit") Application.Current.Shutdown();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e) => ProcessUserInput();
        private void UserInput_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) ProcessUserInput(); }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ChatDisplay.Items.Clear();
            AddMessage("Bot", "Chat cleared. Ask me about cybersecurity topics!", "#E94560");
        }

        // ─── TASKS ─────────────────────────────────────────────────
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = GetFieldValue(TaskTitle, placeholders[TaskTitle]);
            string desc = GetFieldValue(TaskDesc, placeholders[TaskDesc]);
            string reminder = GetFieldValue(TaskReminder, placeholders[TaskReminder]);
            string date = GetFieldValue(TaskDate, placeholders[TaskDate]);

            if (string.IsNullOrWhiteSpace(title))
            {
                TaskStatusMsg.Text = "⚠️ Please enter a task title.";
                TaskStatusMsg.Foreground = new SolidColorBrush(Colors.OrangeRed);
                return;
            }

            bool added = taskManager.AddTask(title, desc, reminder, date);
            if (added)
            {
                string reminderInfo = string.IsNullOrWhiteSpace(reminder) ? "No reminder set." : $"Reminder: {reminder}";
                TaskStatusMsg.Text = $"✅ Task added: '{title}'. {reminderInfo}";
                TaskStatusMsg.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                activityLog.AddEntry($"Task added: '{title}'" + (string.IsNullOrWhiteSpace(reminder) ? "" : $" | Reminder: {reminder}"));
                RefreshTaskList();
            }
            else
            {
                TaskStatusMsg.Text = "❌ Could not save task. Check DB connection.";
                TaskStatusMsg.Foreground = new SolidColorBrush(Colors.OrangeRed);
            }
        }

        private string GetFieldValue(TextBox box, string placeholder)
            => box.Text == placeholder ? "" : box.Text.Trim();

        private void RefreshTaskList()
        {
            TaskListBox.Items.Clear();
            var tasks = taskManager.GetAllTasks();
            foreach (var t in tasks)
            {
                string status = t.IsCompleted ? "[✅ Done]" : "[⏳ Pending]";
                string reminder = string.IsNullOrWhiteSpace(t.Reminder) ? "" : $" | 🔔 {t.Reminder}";
                string date = string.IsNullOrWhiteSpace(t.ReminderDate) ? "" : $" ({t.ReminderDate})";
                var item = new ListBoxItem
                {
                    Content = $"#{t.Id} {status} {t.Title}{reminder}{date}",
                    Tag = t.Id,
                    Foreground = t.IsCompleted ? Brushes.Gray : Brushes.White,
                    Background = new SolidColorBrush(Color.FromRgb(22, 33, 62))
                };
                TaskListBox.Items.Add(item);
            }
        }

        private void MarkComplete_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem is ListBoxItem item && item.Tag is int id)
            {
                taskManager.MarkCompleted(id);
                activityLog.AddEntry($"Task #{id} marked as completed.");
                TaskStatusMsg.Text = $"✅ Task #{id} marked as completed.";
                TaskStatusMsg.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                RefreshTaskList();
            }
            else { TaskStatusMsg.Text = "⚠️ Select a task first."; }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem is ListBoxItem item && item.Tag is int id)
            {
                taskManager.DeleteTask(id);
                activityLog.AddEntry($"Task #{id} deleted.");
                TaskStatusMsg.Text = $"🗑️ Task #{id} deleted.";
                TaskStatusMsg.Foreground = new SolidColorBrush(Colors.OrangeRed);
                RefreshTaskList();
            }
            else { TaskStatusMsg.Text = "⚠️ Select a task first."; }
        }

        private void RefreshTasks_Click(object sender, RoutedEventArgs e) => RefreshTaskList();

        private void Placeholder_GotFocus(object sender, RoutedEventArgs e)
        {
            var box = sender as TextBox;
            if (placeholders.ContainsKey(box) && box.Text == placeholders[box])
            {
                box.Text = "";
                box.Foreground = Brushes.White;
            }
        }

        private void Placeholder_LostFocus(object sender, RoutedEventArgs e)
        {
            var box = sender as TextBox;
            if (placeholders.ContainsKey(box) && string.IsNullOrWhiteSpace(box.Text))
            {
                box.Text = placeholders[box];
                box.Foreground = Brushes.Gray;
            }
        }

        // ─── QUIZ ──────────────────────────────────────────────────
        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            quizEngine = new QuizEngine();
            activityLog.AddEntry("Quiz started.");
            QuizFeedback.Text = "";
            StartQuizBtn.Content = "🔄 Restart";
            ShowNextQuestion();
        }

        private void ShowNextQuestion()
        {
            QuizOptions.Children.Clear();
            if (quizEngine == null || quizEngine.IsFinished)
            {
                QuizQuestion.Text = $"Quiz Complete! Score: {quizEngine?.Score}/{quizEngine?.Questions.Count}";
                QuizFeedback.Text = quizEngine?.GetFinalFeedback() ?? "";
                QuizFeedback.Foreground = Brushes.LightGreen;
                QuizScore.Text = "";
                activityLog.AddEntry($"Quiz completed. Score: {quizEngine?.Score}/{quizEngine?.Questions.Count}");
                return;
            }

            var q = quizEngine.GetCurrentQuestion();
            QuizQuestion.Text = $"Q{quizEngine.CurrentIndex + 1}/{quizEngine.Questions.Count}: {q.Question}";
            QuizScore.Text = $"Score: {quizEngine.Score}";
            QuizFeedback.Text = "";

            for (int i = 0; i < q.Options.Count; i++)
            {
                int idx = i;
                var btn = new Button
                {
                    Content = q.Options[i],
                    Margin = new Thickness(0, 4, 0, 0),
                    Height = 36,
                    Background = new SolidColorBrush(Color.FromRgb(15, 52, 96)),
                    Foreground = Brushes.White,
                    FontSize = 12,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Padding = new Thickness(8, 0, 0, 0)
                };
                btn.Click += (s, ev) =>
                {
                    var (correct, feedback) = quizEngine.SubmitAnswer(idx);
                    QuizFeedback.Text = feedback;
                    QuizFeedback.Foreground = correct ? Brushes.LightGreen : Brushes.OrangeRed;
                    activityLog.AddEntry($"Quiz answer — {(correct ? "Correct ✅" : "Wrong ❌")}");
                    var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(1.5) };
                    timer.Tick += (ts, te) => { timer.Stop(); ShowNextQuestion(); };
                    timer.Start();
                };
                QuizOptions.Children.Add(btn);
            }
        }

        // ─── ACTIVITY LOG ──────────────────────────────────────────
        private void RefreshLog_Click(object sender, RoutedEventArgs e) => LoadLog(10);
        private void ShowAllLog_Click(object sender, RoutedEventArgs e) => LoadLog(int.MaxValue);

        private void LoadLog(int count)
        {
            LogListBox.Items.Clear();
            var entries = count == int.MaxValue ? activityLog.GetAllEntries() : activityLog.GetRecentEntries(count);
            if (entries.Count == 0) { LogListBox.Items.Add("No activity yet."); return; }
            foreach (var entry in entries)
                LogListBox.Items.Add(entry);
        }
    }
}