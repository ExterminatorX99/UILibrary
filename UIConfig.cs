using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.Config;

namespace UILibrary
{
	public abstract class UIConfig : ModConfig
	{
		public sealed override ConfigScope Mode => ConfigScope.ClientSide;

		public delegate void SaveDelegate(ModConfig config);

		private static readonly SaveDelegate Save =
			Reflection.Delegate<SaveDelegate>(typeof(ConfigManager), "Save", BindingFlags.Static | BindingFlags.NonPublic);

		// in-game ModConfig saving from mod code is not supported yet in tmodloader, and subject to change, so we need to be extra careful.
		// This code only supports client configs, and doesn't call onchanged. It also doesn't support ReloadRequired or anything else.
		public void SaveConfig() => Save(this);

		[Label("Position of draggable UI elements")]
		[Tooltip("This will automatically save, no need to adjust")]
		public Dictionary<string, Vector2> Positions = new();

		[Label("Sizes of resizeable UI elements")]
		[Tooltip("This will automatically save, no need to adjust")]
		public Dictionary<string, Vector2> Sizes = new();
	}
}
