# TickControllers

> The tick controllers have two purposes, they act as factories for specific types of sprites
	and they contain the "world clock logic" for a specific type of object via a call to the
	ExecuteWorldClockTick() function.

> For instance: the [MunitionSpriteTickController] class is used to create/delete munitions
	and the ExecuteWorldClockTick(...) function moves the munitions by the appropriate vector
	as well as tests for munition collisions.

> These are called by Cos.Engine.EngineWorldClock.ExecuteWorldClockTick().


## Factory Functions:
> We typically create a few types of "factory" functions in these controllers.

### Add(...)
These functions create a new sprite and insert it into the global sprite collection.

### Insert(sprite)
These functions insert a pre-created sprite and insert it into the global sprite collection.

### Create(...)
These functions create a sprite but DO NOT insert it into the global sprite collection.
