using System;
using System.Collections.Generic;
using System.Linq;
using TriPeaks.ProtoData.Message;

namespace SolitaireTripeaks
{
	[Serializable]
	public class SystemMessageData
	{
		public List<MessageData> Messages;

		public long lastTicks;

		public void SaveMessages(List<Message> messages, long ticks)
		{
			lastTicks = ticks;
			if (Messages == null)
			{
				Messages = new List<MessageData>();
			}
			Message[] array = messages.ToArray();
			foreach (Message message in array)
			{
				MessageData messageData = Messages.Find((MessageData e) => e.MessageId == message.MessageId);
				if (messageData != null)
				{
					messages.Remove(message);
					Messages.Remove(messageData);
					Messages.Add(new MessageData
					{
						PartitionKey = message.PartitionKey,
						RowKey = message.RowKey,
						MessageId = message.MessageId,
						SenderId = message.SenderId,
						SenderName = message.SenderName,
						SendTime = message.SendTime,
						ReceiverId = message.ReceiverId,
						ReceiverName = message.ReceiverName,
						Content = message.Content,
						Tag = message.Tag,
						Read = messageData.Read,
						ReadTime = message.ReadTime,
						ExpiredTime = message.ExpiredTime
					});
				}
			}
			Messages.AddRange(from e in messages
				select new MessageData
				{
					PartitionKey = e.PartitionKey,
					RowKey = e.RowKey,
					MessageId = e.MessageId,
					SenderId = e.SenderId,
					SenderName = e.SenderName,
					SendTime = e.SendTime,
					ReceiverId = e.ReceiverId,
					ReceiverName = e.ReceiverName,
					Content = e.Content,
					Tag = e.Tag,
					Read = e.Read,
					ReadTime = e.ReadTime,
					ExpiredTime = e.ExpiredTime
				});
			Messages.RemoveAll((MessageData e) => e.ExpiredTime > 0 && new DateTime(e.ExpiredTime).Subtract(DateTime.UtcNow).TotalSeconds < 0.0);
		}

		public List<MessageData> GetMessages()
		{
			if (Messages == null)
			{
				Messages = new List<MessageData>();
			}
			Messages.RemoveAll((MessageData e) => e.ExpiredTime > 0 && new DateTime(e.ExpiredTime).Subtract(DateTime.UtcNow).TotalSeconds < 0.0);
			return Messages;
		}

		public int GetUnReadMessages()
		{
			return GetMessages().Count((MessageData e) => e.IsUnread());
		}

		public static SystemMessageData GetReceive()
		{
			if (AuxiliaryData.Get().__ReceiveMessage == null)
			{
				AuxiliaryData.Get().__ReceiveMessage = new SystemMessageData();
			}
			return AuxiliaryData.Get().__ReceiveMessage;
		}

		public static SystemMessageData GetSend()
		{
			if (AuxiliaryData.Get().__SendMessages == null)
			{
				AuxiliaryData.Get().__SendMessages = new SystemMessageData();
			}
			return AuxiliaryData.Get().__SendMessages;
		}
	}
}
