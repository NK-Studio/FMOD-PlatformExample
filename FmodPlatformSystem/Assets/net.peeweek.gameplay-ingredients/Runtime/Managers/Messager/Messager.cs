using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients
{
    public static class Messager
    {
        public delegate void Message(GameObject instigator = null);

        private static readonly Dictionary<string, List<Message>> m_RegisteredMessages;

        static Messager() => m_RegisteredMessages = new Dictionary<string, List<Message>>();

        public static void RegisterMessage(string messageName, Message message)
        {
            if (!m_RegisteredMessages.ContainsKey(messageName))
                m_RegisteredMessages.Add(messageName, new List<Message>());

            if (!m_RegisteredMessages[messageName].Contains(message))
                m_RegisteredMessages[messageName].Add(message);
            else
            {
                Debug.LogWarning($"Messager : {messageName} entry already contains reference to message.");
            }
        }

        public static void RemoveMessage(string messageName, Message message)
        {
            var currentEvent = m_RegisteredMessages[messageName];
            if(currentEvent.Contains(message))
                currentEvent.Remove(message);

            if (currentEvent.Count == 0)
                m_RegisteredMessages.Remove(messageName);
        }

        public static void Send(string eventName, GameObject instigator = null)
        {
            if (m_RegisteredMessages.ContainsKey(eventName))
            {
                try
                {
                    // Get a copy of registered messages to iterate on. This prevents issues while deregistering message recievers while iterating.
                    var messages = m_RegisteredMessages[eventName].ToArray();
                    foreach (var message in messages)
                    {
                        message?.Invoke(instigator);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Messager : Caught {e.GetType().Name} while sending Message {eventName}");
                    Debug.LogException(e);
                }
            }
        }
    }
}

