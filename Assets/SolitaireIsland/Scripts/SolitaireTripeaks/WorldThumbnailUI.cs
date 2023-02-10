using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class WorldThumbnailUI : MonoBehaviour
	{
		public Button PlayButton;

		public Text StarLabel;

		public Image LockImage;

		public int world;

		private void Awake()
		{
			int num = 0;
			num = ((world != -1) ? (UniverseConfig.Get().GetAllLevelInWorld(world) * 3) : (SingletonClass<ExpertLevelConfigGroup>.Get().GetWorldConfig().GetPoints()
				.Count * 3));
				StarLabel.text = $"{PlayData.Get().GetStars(world)}/{num}";
				PlayButton.onClick.AddListener(delegate
				{
					Object.FindObjectOfType<WorldScene>().JumpTo(world);
				});
				UpdateUI();
			}

			public void UpdateUI()
			{
				LockImage.gameObject.SetActive(PlayData.Get().IsLock(world));
			}
		}
	}
