# MooseLib
This is a game library built on top of MonoGame and MonoGame.Extended. 

There's no design philosophy or overarching goal driving its development. 

There's no thought given to backwards compatability and breaking changes.

There's no concept of a scene.

There's a lot I would do differently if I started from scratch now.

I wouldn't really recommend anyone else use it.

## Using It
Use it like MonoGame with extras. Your game will implement `MooseGame`. You will need a map and renderers. There are helpful default implementations to get started. Eventually, you'll probably want your own renderer.

## MooseLib.Ui
Half-baked UI library built to work with MooseLib, but could probably be added to any MonoGame game. It's got some basic inputs and outputs like textboxes and buttons and such.

## Possible Future
I plan on adding:
- Scenes
- More UI elements
- Better code organization and reuse
- More interfaces at API level
