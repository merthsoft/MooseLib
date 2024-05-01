﻿// This class comes from Nuclex: https://github.com/brucificus/NuclecticFramework/blob/master/src/Nuclectic.Graphics.TriD/Camera.cs
#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2009 Nuclex Development Labs

This library is free software; you can redistribute it and/or
modify it under the terms of the IBM Common Public License as
published by the IBM Corporation; either version 1.0 of the
License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
IBM Common Public License for more details.

You should have received a copy of the IBM Common Public
License along with this library
*/
#endregion

namespace Merthsoft.Moose.MooseEngine;


/// <summary>Stores the location and orientation of a camera in a 3D scene</summary>
/// <remarks>
///   <para>
///     The view matrix contains the camera's inverted location and orientation.
///     Whereas a normal matrix stores the position and orientation of an object
///     and converts coordinates in the object's coordinate frame into world
///     coordinates, the camera's view matrix is inverted and thus converts
///     world coordinates into coordinates of the camera's local coordinate frame.
///   </para>
///   <para>
///     The projection matrix converts coordinates in the camera's coordinate
///     frame (calculated by transforming world coordiantes through the camera's
///     view matrix) into screen coordinates. Thus, it 'projects' 3D coordinates
///     onto a flat plane, usually the screen.
///   </para>
/// </remarks>
public class Camera3D
{

    /// <summary>Speed at which the camera moves through the scene</summary>
    /// <remarks>
    ///   This value is in world units per second and depends on the scale of
    ///   the scene - but since games universally adopt world units to whatever
    ///   unit is most fitting to the game, this default should provide adequate
    ///   movement speed in all cases.
    /// </remarks>
    private const float MovementSpeed = 100.0f;

    /// <summary>Initializes a new camera with the provided matrices</summary>
    /// <param name="view">view matrix defining the position of the camera</param>
    /// <param name="projection">
    ///   Projection matrix controlling the type of projection that is
    ///   performed to convert the scene to 2D coordinates.
    /// </param>
    public Camera3D(Matrix view, Matrix projection)
    {
        this.view = view;
        this.projection = projection;
    }

    /// <summary>Turns the camera so it is facing the point provided</summary>
    /// <param name="lookAtPosition">Position the camera should be pointing to</param>
    public void LookAt(Vector3 lookAtPosition)
    {
        var inverseview = Matrix.Invert(view);

        // Use a local variable because we can't take a reference to the value returned
        // by a property get. The ref variant is still faster than copying entire
        // matrices around.
        var cameraPosition = inverseview.Translation;

        // CreateLookAt() already constructs an inverted matrix,
        // so we don't need to invert it ourselves
        Matrix.CreateLookAt(
          ref cameraPosition, ref lookAtPosition, ref up, out view
        );
    }

    /// <summary>Moves the camera to the specified location</summary>
    /// <param name="position">Location the camera will be moved to</param>
    public void MoveTo(Vector3 position)
    {
        Matrix.Invert(ref view, out view);
        view.Translation = position;
        Matrix.Invert(ref view, out view);
    }

    /// <summary>The camera's current position</summary>
    public Vector3 Position
    {
        get { return Matrix.Invert(view).Translation; }
        set
        {
            MoveTo(value);
        }
    }

    /// <summary>The camera's forward vector</summary>
    public Vector3 Forward
    {
        get { return Matrix.Invert(view).Forward; }
    }

    /// <summary>The camera's right vector</summary>
    public Vector3 Right
    {
        get { return Matrix.Invert(view).Right; }
    }

    /// <summary>The camera's up vector</summary>
    public Vector3 Up
    {
        get { return Matrix.Invert(view).Up; }
    }

    public Matrix View { get => view; }
    public Matrix Projection { get => projection; }

    ///// <summary>
    /////   Debugging aid that allows the camera to be moved around by the keyboard
    /////   or the game pad
    ///// </summary>
    ///// <param name="gameTime">Game time to use for scaling the movements</param>
    ///// <remarks>
    /////   <para>
    /////     This is only intended as a debugging aid and should not be used for the actual
    /////     player controls. As long as you don't rebuild the camera matrix each frame
    /////     (which is not a good idea in most cases anyway) this will allow you to control
    /////     the camera in the style of the old "Descent" game series.
    /////   </para>
    /////   <para>
    /////     To enable the camera controls, simply call this method from your main loop!
    /////   </para>
    ///// </remarks>
    public void HandleControls(GameTime gameTime)
    {
        HandleControls(gameTime, Keyboard.GetState(), GamePad.GetState(PlayerIndex.One));
    }

    /// <summary>
    ///   Debugging aid that allows the camera to be moved around by the keyboard
    ///   or the game pad
    /// </summary>
    /// <param name="gameTime">Game time to use for scaling the movements</param>
    /// <param name="keyboardState">Current state of the keyboard</param>
    /// <param name="gamepadState">Current state of the gamepad</param>
    /// <remarks>
    ///   <para>
    ///     This is only intended as a debugging aid and should not be used for the actual
    ///     player controls. As long as you don't rebuild the camera matrix each frame
    ///     (which is not a good idea anyway) this will allow you to control the camera
    ///     in the style of the old "Descent" game series.
    ///   </para>
    ///   <para>
    ///     To enable the camera controls, simply call this method from your main loop!
    ///   </para>
    /// </remarks>
    public void HandleControls(
      GameTime gameTime, KeyboardState keyboardState, GamePadState gamepadState
    )
    {
        var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        handleKeyboardControls(keyboardState, delta);
        handleGamePadControls(gamepadState, delta);
    }

    /// <summary>Processes any keyboard input for the debugging aid</summary>
    /// <param name="keyboardState">Current state of the keyboard</param>
    /// <param name="delta">
    ///   Scales the strength of input, should be based on the time passed since
    ///   the last frame was drawn
    /// </param>
    private void handleKeyboardControls(KeyboardState keyboardState, float delta)
    {

        // Rotational controls
        if (keyboardState[Keys.NumPad4] == KeyState.Down)
            view *= Matrix.CreateRotationY(-delta);
        if (keyboardState[Keys.NumPad6] == KeyState.Down)
            view *= Matrix.CreateRotationY(+delta);
        if (keyboardState[Keys.NumPad8] == KeyState.Down)
            view *= Matrix.CreateRotationX(+delta);
        if (keyboardState[Keys.NumPad2] == KeyState.Down)
            view *= Matrix.CreateRotationX(-delta);
        if (keyboardState[Keys.NumPad7] == KeyState.Down)
            view *= Matrix.CreateRotationZ(-delta);
        if (keyboardState[Keys.NumPad9] == KeyState.Down)
            view *= Matrix.CreateRotationZ(+delta);

        // Positional controls
        delta *= MovementSpeed;
        if (keyboardState[Keys.A] == KeyState.Down)
            view.Translation += Vector3.Right * delta;
        if (keyboardState[Keys.D] == KeyState.Down)
            view.Translation -= Vector3.Right * delta;
        if (keyboardState[Keys.W] == KeyState.Down)
            view.Translation -= Vector3.Forward * delta;
        if (keyboardState[Keys.S] == KeyState.Down)
            view.Translation += Vector3.Forward * delta;
        if (keyboardState[Keys.R] == KeyState.Down)
            view.Translation -= Vector3.Up * delta;
        if (keyboardState[Keys.F] == KeyState.Down)
            view.Translation += Vector3.Up * delta;

    }

    /// <summary>Processes any gamepad input for the debugging aid</summary>
    /// <param name="gamepadState">Current state of the gamepad</param>
    /// <param name="delta">
    ///   Scales the strength of input, should be based on the time passed since
    ///   the last frame was drawn
    /// </param>
    private void handleGamePadControls(GamePadState gamepadState, float delta)
    {
        // Buttons
        if (gamepadState.DPad.Left == ButtonState.Pressed)
            view *= Matrix.CreateRotationY(-delta);
        if (gamepadState.DPad.Right == ButtonState.Pressed)
            view *= Matrix.CreateRotationY(+delta);
        if (gamepadState.DPad.Up == ButtonState.Pressed)
            view *= Matrix.CreateRotationX(+delta);
        if (gamepadState.DPad.Down == ButtonState.Pressed)
            view *= Matrix.CreateRotationX(-delta);
        if (gamepadState.Buttons.LeftShoulder == ButtonState.Pressed)
            view *= Matrix.CreateRotationZ(-delta);
        if (gamepadState.Buttons.RightShoulder == ButtonState.Pressed)
            view *= Matrix.CreateRotationZ(+delta);

        // Analog
        view *= Matrix.CreateRotationY(gamepadState.ThumbSticks.Right.X * delta);
        view *= Matrix.CreateRotationX(gamepadState.ThumbSticks.Right.Y * delta);

        delta *= MovementSpeed;
        view.Translation += Vector3.Down * gamepadState.Triggers.Right * delta;
        view.Translation += Vector3.Up * gamepadState.Triggers.Left * delta;

        view.Translation += Vector3.Left * gamepadState.ThumbSticks.Left.X * delta;
        view.Translation += Vector3.Backward * gamepadState.ThumbSticks.Left.Y * delta;


    }

    /// <summary>Returns a default orthographic camera</summary>
    /// <remarks>
    ///   Mainly intended as an aid in unit testing and for some quick verifications
    ///   of algorithms requiring a camera
    /// </remarks>
    public static Camera3D CreateDefaultOrthographic()
    {
        return new Camera3D(
          Matrix.CreateLookAt(Vector3.Zero, Vector3.Forward, Vector3.Up),
          Matrix.CreateOrthographic(1280.0f, 1024.0f, 0.1f, 1000.0f)
        );
    }

    /// <summary>view matrix defining the camera's position within the scene</summary>
    private Matrix view;
    /// <summary>
    ///   Controls the projection of 3D coordinates to the render target surface
    /// </summary>
    /// <remarks>
    ///   The term 'projection' comes from the fact that this matrix is projecting
    ///   3D coordinates onto a flat surface, normally either the screen or some
    ///   render target texture. Typical projection matrices perform either an
    ///   orthogonal projection (CAD-like) or perspective projections (things get
    ///   smaller the farther away they are).
    /// </remarks>
    private Matrix projection;
    /// <summary>
    ///   Default world up vector for the camera, copied to a variable here because the
    ///   Matrix.CreateLookAt() method needs a reference to a Vector3.
    /// </summary>
    private static Vector3 up = Vector3.Up;

}

// namespace Nuclex.Graphics