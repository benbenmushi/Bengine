using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using MyDebugAPI;

namespace Bengine.Test
{
	public class TestDebug : MonoBehaviour
	{
		public MyDebug debug = new MyDebug(3, "[Test]: ");

		private void Start()
		{
			for (int i = 0; i < 5; i++)
			{
				debug.Log(i, "logLevel=" + i);
			}
		}
	}
}
