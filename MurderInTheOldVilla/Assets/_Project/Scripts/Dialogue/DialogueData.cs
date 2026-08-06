using System;
using UnityEngine;

namespace MurderVilla.Dialogue
{
    /// <summary>
    /// A single question-answer pair in a formal Q&amp;A sequence.
    /// Used for the chat-bubble style conversation.
    /// </summary>
    [Serializable]
    public class QAPair
    {
        [TextArea(1, 3)] public string question;
        [TextArea(2, 5)] public string answer;
    }

    /// <summary>
    /// A dialogue branch: the player's question option and the NPC's response.
    /// If <c>qaSequence</c> is non-empty, the branch displays a multi-round
    /// Q&amp;A chat-bubble sequence. Otherwise it shows a single response.
    /// </summary>
    [Serializable]
    public class DialogueBranch
    {
        [TextArea(1, 3)] public string questionText;
        [TextArea(2, 5)] public string responseText;
        public QAPair[] qaSequence;
    }

    /// <summary>
    /// Dialogue UI state machine.
    /// </summary>
    public enum DialogueState
    {
        Hidden,
        Idle,
        QuestionSelect,
        ShowingResponse,
        QASequence,
        Ended,
        Cooldown
    }
}
