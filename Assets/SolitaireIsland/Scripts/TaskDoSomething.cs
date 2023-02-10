using System.Collections;
using UnityEngine.Events;

namespace Nightingale.Tasks
{
	public delegate IEnumerator TaskDoSomething(UnityAction<object, float> completed);
}
