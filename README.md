# Cybersecurity Awareness Bot - Part 3

## Description
A WPF-based Cybersecurity Awareness Chatbot built in C# with advanced features including
a Task Assistant, Cybersecurity Quiz, NLP Simulation and Activity Log.

## Features
-  Chatbot - Ask about passwords, phishing, privacy, scams and more
-  Task Assistant - Add and manage cybersecurity tasks stored in MySQL database
-  Quiz - Test your cybersecurity knowledge with 11 questions
-  Activity Log - Tracks all bot actions with timestamps 

## Requirements
- Visual Studio 2022
- .NET Framework 4.7.2
- MySQL Server (running locally)
- MySql.Data NuGet package

## Database Setup
1. Open MySQL Workbench
2. Run the following SQL:

CREATE DATABASE IF NOT EXISTS cybersecurity_bot;
USE cybersecurity_bot;

CREATE TABLE IF NOT EXISTS tasks (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    reminder VARCHAR(255),
    reminder_date VARCHAR(100),
    is_completed BOOLEAN DEFAULT FALSE,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

3. In TaskManager.cs, update the connection string with your MySQL password:
Server=localhost;Database=cybersecurity_bot;Uid=root;Pwd=YOUR_PASSWORD;

## How to Run
1. Clone the repository
2. Open CybersecurityBot_Part3.sln in Visual Studio 2022
3. Set up the database as shown above
4. Install MySql.Data via NuGet Package Manager
5. Press F5 to run

## How to Use
- Type messages in the  Chatbot tab to chat about cybersecurity
- Use  Tasks tab to add and manage your cybersecurity tasks
- Use  Quiz tab to test your knowledge
- Use  Activity Log tab to view recent bot actions

## NLP Keywords Supported
- passwords, phishing, privacy, scams, 2FA, VPN, browsing, links
- "add task", "set reminder", "show activity log", "quiz"

## Author
Katlego Itumeleng Mangwedi
Student Number: ST10478067
