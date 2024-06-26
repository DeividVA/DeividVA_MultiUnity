/* 
  Package Name: Smart Console
  Version: 2.3.0
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-06-14
  Script Name: TimeCommands.cs

  Description:
  This script implements time related commands.
*/

using UnityEngine;

namespace ED.SC.Extra
{
	[CommandGroup("Time")]
	public static class TimeCommands
	{
		[Command("get_time_scale", "Gets the scale at which time passes")]
		public static void GetTimeScale()
		{
			SmartConsole.Log($"Time scale is {Time.timeScale}.");
		}

		[Command("set_time_scale", "Sets the scale at which time passes")]
		public static void SetTimeScale(float newTimeScale)
		{
			Time.timeScale = newTimeScale;
			SmartConsole.Log($"Updated time scale to {Time.timeScale}.");
		}
	}
}
