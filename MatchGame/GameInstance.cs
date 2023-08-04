using MatchGame.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez.Console;
using Nez.ImGuiTools;
using Debug = System.Diagnostics.Debug;

namespace MatchGame;

public class GameInstance : Nez.Core
{
	private static void EnableImGui(bool? bEnabled)
	{
		var imgui = GetGlobalManager<ImGuiManager>();
		if (bEnabled is true && imgui is null)
		{
			RegisterGlobalManager(new ImGuiManager(new ImGuiOptions()
				.IncludeDefaultFont(true)
				.SetGameWindowTitle(nameof(MatchGame))));
		}
		else
		{
			imgui?.SetEnabled(bEnabled ?? imgui.Enabled);
		}
	}

	[Command("imgui", "")]
	private static void ToggleImgui()
	{
		EnableImGui(default);
	}


	public GameInstance()
		: base(windowTitle: nameof(MatchGame))
	{
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
	}

	protected override void Initialize()
	{
		base.Initialize();

		// TODO: Add your initialization logic here

#if DEBUG
		EnableImGui(true);
#endif

		Scene = new SampleScene();
	}

	protected override void LoadContent()
	{
		// TODO: use this.Content to load your game content here

		Debug.WriteLine(ContentReference.Content.Items.TestItem);
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