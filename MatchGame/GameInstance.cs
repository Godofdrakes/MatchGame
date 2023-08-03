﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MatchGame;

public class GameInstance : Nez.Core
{
	public GameInstance()
	{
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
	}

	protected override void Initialize()
	{
		base.Initialize();
		
		// TODO: Add your initialization logic here
	}

	protected override void LoadContent()
	{
		// TODO: use this.Content to load your game content here
	}

	protected override void Update(GameTime gameTime)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
		    Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();

		// TODO: Add your update logic here

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.CornflowerBlue);

		// TODO: Add your drawing code here

		base.Draw(gameTime);
	}
}