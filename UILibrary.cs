using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using UILibrary.UI;

namespace UILibrary
{
	public class UILibrary : Mod
	{
		public override void Load()
		{
			Asset<Texture2D> dragTexture = ModContent.Request<Texture2D>("Terraria/Images/UI/PanelBorder");
			DraggableUIPanel.DragTexture = dragTexture;
			DraggableUIElement.DragTexture = dragTexture;
		}

		public override void Unload()
		{
			DraggableUIPanel.DragTexture = null!;
			DraggableUIElement.DragTexture = null!;
		}
	}
}
