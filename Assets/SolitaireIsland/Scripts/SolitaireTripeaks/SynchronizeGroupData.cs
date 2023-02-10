using Nightingale.Utilitys;
using System;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	[Serializable]
	public class SynchronizeGroupData : SingletonData<SynchronizeGroupData>
	{
		public List<SynchronizeData> synchronizeDatas;

		public List<string> UploadUsers;

		public void PutData(string userId, long ticks)
		{
			if (synchronizeDatas == null)
			{
				synchronizeDatas = new List<SynchronizeData>();
			}
			SynchronizeData synchronizeData = synchronizeDatas.Find((SynchronizeData e) => e.UserId == userId);
			if (synchronizeData == null)
			{
				synchronizeData = new SynchronizeData();
				synchronizeDatas.Add(synchronizeData);
				synchronizeData.UserId = userId;
			}
			synchronizeData.Ticks = ticks;
			FlushData();
		}

		public void PutUpload(string userId)
		{
			if (UploadUsers == null)
			{
				UploadUsers = new List<string>();
			}
			if (!UploadUsers.Contains(userId))
			{
				UploadUsers.Add(userId);
				FlushData();
			}
		}

		public bool HasTicks(string userId, long ticks)
		{
			if (synchronizeDatas == null)
			{
				return false;
			}
			return synchronizeDatas.Find((SynchronizeData e) => e.UserId == userId && e.Ticks == ticks) != null;
		}

		public bool GetLocalDeviceId(string userId)
		{
			if (UploadUsers == null)
			{
				return false;
			}
			return UploadUsers.Contains(userId);
		}
	}
}
