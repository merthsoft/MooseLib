# MooseLib
This is a game library built on top of MonoGame and MonoGame.Extended. 

There's no design philosophy or overarching goal driving its development. 

There's no thought given to backwards compatability and breaking changes.

There's no concept of a scene.

There's a lot I would do differently if I started from scratch now.

I wouldn't really recommend anyone else use it.

## Using It
I guess you don't heed warnings. I like that. The best way to use this would be to copy and paste whatever feature you find useful into your game directly.

If you absolutely want your game to be a MOOSE game, use it like MonoGame with extras. Your game will implement `MooseGame`. You will need a map and renderers. There are helpful default implementations to get started. Eventually, you'll probably want your own renderer.

## MooseLib.Ui
Half-baked UI library built to work with MooseLib, but could probably be added to any MonoGame game. It's got some basic inputs and outputs like textboxes and buttons and such.

## Possible Future
I plan on adding:
- Scenes
- More UI elements
- Better code organization and reuse
- More interfaces at API level

I plan on using a lot of Monogame.Extended features in the service of implementing these things, e.g. scenes.

## Liscensing
All code, unless otherwise stated, is liscenced under the WTF-PL. See LICENCE file. 

Art has largely been licensed for individual use (mine) so you can't just take art from here and use it in your game. If you're going to make money off your game, support artist and purchase their assets, or use freely available assets with appropriate attribution. See the content for authors.

Do what thou wilt is the whole of the law.
