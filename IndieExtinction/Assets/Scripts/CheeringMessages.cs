using UnityEngine;
namespace Irrelevant.Assets.Scripts
{
    public enum MessageType 
    { 
        Wave,
        Click,
        Loss
    };

    public sealed class CheeringMessages
    {
        public static string GetMessage(MessageType type)
        {
            if (prevMessageType == null || prevMessageType.Value != type)
            {
                Randomize();
                prevMessageType = type;                     
            }

            switch (type)
            {
                case MessageType.Wave:
                    return waveMessages[messageIndex];
                case MessageType.Click:
                    return clickMessages[messageIndex];
                case MessageType.Loss:
                    return lossMessages[messageIndex];
            }

            return string.Empty;
        }

        private static void Randomize()
        {
            messageIndex = Random.Range(0, 3);
        }

        private static MessageType? prevMessageType;

        private static int messageIndex = 0;

        private static readonly string[] waveMessages = new[] {
            "Restructuring in {0} seconds",
            "Ejection of surplus employees in: {0}",
            "Termination party in: {0}",
            "Firing round in {0} seconds" };

        private static readonly string[] clickMessages = new[] {
            "Crush the indie upstarts!",
            "Click studios to destroy them!",
            "Kill them all! Click them to oblivion!",
            "Use your giant hand to show them who WAS boss!"};

        private static readonly string[] lossMessages = new[] {
            "You lost a market share.",
            "They took a piece of YOUR pie!",
            "You suck! Metacritic of 10.",
            "Another slice lost. Likelyhood of bonus - unlikely." };
    }
}
