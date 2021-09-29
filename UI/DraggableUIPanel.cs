using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace UILibrary.UI
{
	public class DraggableUIPanel : UIPanel
	{
		public static Asset<Texture2D> DragTexture { get; internal set; } = null!;
		
		public string UniqueName { get; }

		public Vector2 Offset { get; private set; }
		public bool Draggable { get; set; }
		public bool Dragging { get; private set; }

		public bool ResizeableX { get; set; }
		public bool ResizeableY { get; set; }
		public bool Resizeable => ResizeableX || ResizeableY;
		public bool Resizeing { get; private set; }

		public UIConfig? Config { get; }

		//private int minX, minY, maxX, maxY;
		public List<UIElement> AdditionalDragTargets { get; init; } = new();

		// TODO, move panel back in if offscreen? prevent drag off screen?
		public DraggableUIPanel(string uniqueName, bool draggable = true, bool resizeableX = false, bool resizeableY = false, UIConfig? config = null)
		{
			UniqueName = uniqueName;
			Draggable = draggable;
			ResizeableX = resizeableX;
			ResizeableY = resizeableY;
			Config = config;

			if (Config is not null)
			{
				Vector2 pos = Config.Positions[UniqueName];
				Left.Pixels = pos.X;
				Top.Pixels = pos.Y;
				Vector2 size = Config.Sizes[UniqueName];
				Width.Pixels = size.X;
				Height.Pixels = size.Y;
			}
		}

		public void AddDragTarget(UIElement element)
		{
			AdditionalDragTargets.Add(element);
		}

		//public void SetMinMaxWidth(int min, int max)
		//{
		//	this.minX = min;
		//	this.maxX = max;
		//}

		//public void SetMinMaxHeight(int min, int max)
		//{
		//	this.minY = min;
		//	this.maxY = max;
		//}

		public override void MouseDown(UIMouseEvent evt)
		{
			DragStart(evt);
			base.MouseDown(evt);
		}

		public override void MouseUp(UIMouseEvent evt)
		{
			DragEnd(evt);
			base.MouseUp(evt);
		}

		private void DragStart(UIMouseEvent evt)
		{
			if (evt.Target != this && !AdditionalDragTargets.Contains(evt.Target))
				return;

			CalculatedStyle innerDimensions = GetInnerDimensions();
			Rectangle dragArea = new((int)(innerDimensions.X + innerDimensions.Width - 12), (int)(innerDimensions.Y + innerDimensions.Height - 12), 12 + 6, 12 + 6);
			if (Resizeable && dragArea.Contains(evt.MousePosition.ToPoint()))
			{
				Offset = new Vector2(evt.MousePosition.X - innerDimensions.X - innerDimensions.Width - 6, evt.MousePosition.Y - innerDimensions.Y - innerDimensions.Height - 6);
				Resizeing = true;
			}
			else if (Draggable)
			{
				Offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
				Dragging = true;
			}
		}

		private void DragEnd(UIMouseEvent evt)
		{
			if (evt.Target == this || AdditionalDragTargets.Contains(evt.Target))
			{
				Dragging = false;
				Resizeing = false;
			}

			if (Config is not null)
			{
				CalculatedStyle dimensions = GetOuterDimensions(); // Drag can go negative, need clamped by Min and Max values
				Config.Sizes[UniqueName] = new Vector2(dimensions.Width, dimensions.Height);
				Config.Positions[UniqueName] = new Vector2(Left.Pixels, Top.Pixels);
				Config.SaveConfig();
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetOuterDimensions();
			if (ContainsPoint(Main.MouseScreen))
			{
				Main.LocalPlayer.mouseInterface = true;
				Main.LocalPlayer.cursorItemIconEnabled = false;
				Main.ItemIconCacheUpdate(0);
			}

			if (Dragging)
			{
				Left.Set(Main.MouseScreen.X - Offset.X, 0f);
				Top.Set(Main.MouseScreen.Y - Offset.Y, 0f);
				Recalculate();
			}
			else
			{
				if (Parent != null && !dimensions.ToRectangle().Intersects(Parent.GetDimensions().ToRectangle()))
				{
					var parentSpace = Parent.GetDimensions().ToRectangle();
					Left.Pixels = Utils.Clamp(Left.Pixels, Width.Pixels - parentSpace.Right, 0); // TODO: Adjust automatically for Left.Percent (measure from left or right edge)
					Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
					Recalculate();
				}
			}

			if (Resizeing)
			{
				if (ResizeableX)
				{
					//Width.Pixels = Utils.Clamp(Main.MouseScreen.X - dimensions.X - offset.X, minX, maxX);
					Width.Pixels = Main.MouseScreen.X - dimensions.X - Offset.X;
				}
				if (ResizeableY)
				{
					//Height.Pixels = Utils.Clamp(Main.MouseScreen.Y - dimensions.Y - offset.Y, minY, maxY);
					Height.Pixels = Main.MouseScreen.Y - dimensions.Y - Offset.Y;
				}
				Recalculate();
			}

			base.DrawSelf(spriteBatch);
			if (Resizeable)
			{
				DrawDragAnchor(spriteBatch, DragTexture.Value, this.BorderColor);
			}
		}

		private void DrawDragAnchor(SpriteBatch spriteBatch, Texture2D texture, Color color)
		{
			CalculatedStyle dimensions = GetDimensions();

			//	Rectangle hitbox = GetInnerDimensions().ToRectangle();
			//	Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.LightBlue * 0.6f);

			Point point = new((int)(dimensions.X + dimensions.Width - 12), (int)(dimensions.Y + dimensions.Height - 12));
			spriteBatch.Draw(texture, new Rectangle(point.X - 2, point.Y - 2, 12 - 2, 12 - 2), new Rectangle(12 + 4, 12 + 4, 12, 12), color);
			spriteBatch.Draw(texture, new Rectangle(point.X - 4, point.Y - 4, 12 - 4, 12 - 4), new Rectangle(12 + 4, 12 + 4, 12, 12), color);
			spriteBatch.Draw(texture, new Rectangle(point.X - 6, point.Y - 6, 12 - 6, 12 - 6), new Rectangle(12 + 4, 12 + 4, 12, 12), color);
		}
	}
}