///////////////////////////////////
//Heist!, © Cristian Baldi - 2022//
///////////////////////////////////

using System.Collections.Generic;

namespace HeistGame
{
    internal class MessageLog
    {
        private List<string> allMessages;

        public MessageLog()
        {
            allMessages = new List<string>();
        }

        public void LogMessage(Message message)
        {
            if (message.HasBeenRead) { return; }
            if (message.Text[0] == string.Empty || message.Text[0] == " ") { return; }

            allMessages.Add($"Level: {message.LevelName}");
            allMessages.Add($"Type: {message.Type}");
            allMessages.Add(" ");
            allMessages.AddRange(message.Text);
            allMessages.Add(" ");
            allMessages.Add("~··~");
            allMessages.Add(" ");
        }
    
        public void ClearLog() => allMessages.Clear();

        public void RestoreLog(string[] log) => allMessages.AddRange(log);

        public string[] GetMessagesLog()
        {
            if (allMessages.Count <= 0)
            {
                return new string[] {"No messages were logged yet", " "};
            }

            return allMessages.ToArray();
        }
    }

    internal class Message
    {
        public MessageType Type { get; private set; }
        public string LevelName{ get; private set;}
        public string[] Text { get; private set; }
        public bool HasBeenRead { get; set; }

        public Message(MessageType type, string levelName, string[] text)
        {
            Type = type;
            LevelName = levelName;
            Text = text;
            HasBeenRead = false;
        }
    }

    internal enum MessageType
    {
        BRIEFING,
        DEBRIFIENG,
        SIGNPOST,
        OBJECTIVE
    }
}
