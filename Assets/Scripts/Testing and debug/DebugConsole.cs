using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    public TextMeshProUGUI consoleText; // Assign the Text component in the inspector
    public int maxMessages = 10; // Limit the number of messages to display
    TMP_InputField inputField;
    private Queue<string> messageQueue = new Queue<string>();

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
    }


    public void AddMessage(string message)
    {
        if (messageQueue.Count >= maxMessages)
        {
            messageQueue.Dequeue(); // Remove the oldest message
        }

        messageQueue.Enqueue(message);
        UpdateConsole();
    }

    private void UpdateConsole()
    {
        inputField.text = string.Join("\n", messageQueue);
    }

    public void ClearConsole()
    {
        messageQueue.Clear();
        UpdateConsole();
    }
}
