using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BasicPrimitiveBuddy;
using CollisionBuddy;
using GameTimer;
using HadoukInput;

namespace CircleRectTest
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		Circle _circle;
		Rectangle _box;

		GameClock _clock;

		InputState _inputState;
		InputWrapper _inputWrapper;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			_circle = new Circle();
			_box = new Rectangle();

			_clock = new GameClock();
			_inputState = new InputState();
			_inputWrapper = new InputWrapper(PlayerIndex.One, _clock.GetCurrentTime);
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			_box = graphics.GraphicsDevice.Viewport.TitleSafeArea;
			_box.X += 100;
			_box.Width -= 200;
			_box.Y += 100;
			_box.Height -= 200;

			_circle.Pos = new Vector2(graphics.GraphicsDevice.Viewport.TitleSafeArea.Center.X,
				graphics.GraphicsDevice.Viewport.TitleSafeArea.Center.Y);
			_circle.Radius = 80.0f;

			_clock.Start();

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

			//update the timer
			_clock.Update(gameTime);
			
			//update the input
			_inputState.Update();
			_inputWrapper.Update(_inputState, false);

			//move the circle
			float movespeed = 200.0f;
			if (_inputWrapper.Controller.KeystrokeHeld[(int)EKeystroke.Up])
			{
				_circle.Translate(0.0f, -movespeed * _clock.TimeDelta);
			}
			else if (_inputWrapper.Controller.KeystrokeHeld[(int)EKeystroke.Down])
			{
				_circle.Translate(0.0f, movespeed * _clock.TimeDelta);
			}
			else if (_inputWrapper.Controller.KeystrokeHeld[(int)EKeystroke.Forward])
			{
				_circle.Translate(movespeed * _clock.TimeDelta, 0.0f);
			}
			else if (_inputWrapper.Controller.KeystrokeHeld[(int)EKeystroke.Back])
			{
				_circle.Translate(-movespeed * _clock.TimeDelta, 0.0f);
			}

			//put the circle back in the box?
			Vector2 overlap = Vector2.Zero;
			Vector2 collisionPoint = Vector2.Zero;
			if (CollisionCheck.CircleRectCollision(_circle, _box, ref collisionPoint, ref overlap))
			{
				//move the circle by the overlap
				_circle.Translate(overlap);
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin();

			//draw the circle
			BasicPrimitive circlePrim = new BasicPrimitive(graphics.GraphicsDevice);
			circlePrim.Circle(_circle.Pos, _circle.Radius, Color.Red, spriteBatch);

			//darw the rectangle
			BasicPrimitive rectPrim = new BasicPrimitive(graphics.GraphicsDevice);
			rectPrim.AxisAlignedBox(new Vector2(_box.Left, _box.Top),
				new Vector2(_box.Right, _box.Bottom), 
				Color.White,
				spriteBatch);

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
