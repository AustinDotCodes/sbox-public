namespace Editor;

/// <summary>
/// Runs anytime ViewportTools is re/built: adds play, pause, eject buttons to the toolbar and hooks up their events.
/// </summary>
partial class ViewportTools
{
	EditorToolButton PlayButton { get; set; }
	EditorToolButton PauseButton { get; set; }
	EditorToolButton EjectButton { get; set; }

	private void BuildPlayToolbar( Layout toolbar )
	{
		PlayButton = AddButton( toolbar, "Play", "play_arrow", PlayStop );
		PauseButton = AddButton( toolbar, "Pause", "pause", Pause );
		EjectButton = AddButton( toolbar, "Eject", "eject", Eject );

		UpdateState();
	}

	/// <summary>
	/// When the state of game changes, e.g we're playing, stopping, ejecting (nope), pausing, this gets called.
	/// Note: scene.pause/resume doesn't call ViewportTools.Rebuild(), so we need to hook into those events to update the toolbar state.
	/// </summary>
	[Event( "scene.pause" )]
	[Event( "scene.resume" )]
	private void UpdateState()
	{
		// Prefabs nada
		if ( sceneViewWidget.Session.IsPrefabSession )
		{
			PlayButton.Enabled = false;
			PauseButton.Enabled = false;
			EjectButton.Enabled = false;
			return;
		}

		if ( Game.IsPlaying )
		{
			PlayButton.ToolTip = "Stop";
			PlayButton.GetIcon = () => "stop";
			PlayButton.Color = Theme.Red;
		}
		else
		{
			PlayButton.ToolTip = "Play";
			PlayButton.GetIcon = () => "play_arrow";
			PlayButton.Color = Theme.Green;
		}

		// We can only pause whilst we're gaming
		PauseButton.Enabled = Game.IsPlaying;
		PauseButton.Color = Game.IsPaused ? Theme.Blue : Theme.TextLight;

		EjectButton.Enabled = Game.IsPlaying;
		bool isEjected = sceneViewWidget.CurrentView == SceneViewWidget.ViewMode.GameEjected;
		EjectButton.GetIcon = () => isEjected ? "sports_esports" : "eject";
		EjectButton.ToolTip = isEjected ? "Return to Game" : "Eject";
		EjectButton.Color = isEjected ? Theme.Green : Theme.TextLight;
	}

	[Shortcut( "editor.toggle-play", "F5" )]
	private void PlayStop()
	{
		if ( !Game.IsPlaying )
		{
			EditorScene.Play( sceneViewWidget.Session );
		}
		else
		{
			EditorScene.Stop();
		}
	}

	[Shortcut( "editor.toggle-pause", "F7" )]
	private void Pause()
	{
		EditorScene.TogglePause();
	}

	[Shortcut( "editor.eject", "F8", ShortcutType.Window )]
	public void Eject()
	{
		sceneViewWidget.ToggleEject();
	}
}
