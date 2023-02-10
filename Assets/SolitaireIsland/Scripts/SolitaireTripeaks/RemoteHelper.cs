using Nightingale.Tasks;
using Nightingale.Utilitys;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	public class RemoteHelper : SingletonBehaviour<RemoteHelper>
	{
		private readonly List<string> files = new List<string>
		{
			string.Empty,
			string.Empty,
			string.Empty
		};

		public void Download()
		{
			foreach (string file in files)
			{
				if (!FileUtility.Exists(GlobalConfig.GetPathByRuntimePlatform(file)))
				{
					TaskHelper.GetDownload().AppendTask(new RemoteAssetTask(NightingaleConfig.Get().StorageBlobAddress, GlobalConfig.GetPathByRuntimePlatform(file)));
				}
			}
		}

		public bool IsReady(string file)
		{
			return FileUtility.Exists(GlobalConfig.GetPathByRuntimePlatform(file));
		}

		public bool ShowRemote(string file)
		{
			if (FileUtility.Exists(GlobalConfig.GetPathByRuntimePlatform(file)))
			{
				TaskHelper.GetLocal().AppendTask(new LocalAssetTask(GlobalConfig.GetPathByRuntimePlatform(file))).RemoveAllListeners()
					.AddListener(delegate(object asset, float p)
					{
						if (asset == null)
						{
						}
					});
				return true;
			}
			return false;
		}
	}
}
