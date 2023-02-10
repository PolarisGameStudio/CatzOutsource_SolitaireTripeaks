using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public interface IStorageHandGroup
	{
		void DONext();

		void Undo();

		void Over();

		Transform GetTransform();

		UnityEvent Changed();
	}
}
