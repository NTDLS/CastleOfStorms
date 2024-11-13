using Cos.Library.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Cos.Engine.Sprite._Superclass._Root
{
    /// <summary>
    /// Represents a single item that can be rendered to the screen. All on-screen objects are derived from this class.
    /// </summary>
    public partial class SpriteBase
    {
        #region Post-movement movement vector collision detection.

        /// <summary>
        /// Returns a list of all collisions the sprite made on is current movement vector, in the order in which they would be encountered.
        /// </summary>
        /// <returns></returns>
        public List<SpriteBase> FindReverseCollisionsAlongMovementVector(float epoch)
            => FindReverseCollisionsAlongMovementVector(_engine.Sprites.Visible(), epoch);

        /// <summary>
        /// Returns a list of all collisions the sprite made on is current movement vector, in the order in which they would be encountered.
        /// </summary>
        /// <param name="objectsThatCanBeHit"></param>
        /// <returns></returns>
        public List<SpriteBase> FindReverseCollisionsAlongMovementVector(SpriteBase[] objectsThatCanBeHit, float epoch)
        {
            /// Takes the position of an object after it has been moved and tests each location
            ///     between where it ended up and where it should have come from given its movement vector.

            var collisions = new List<SpriteBase>();

            //Get the starting position of the sprite before it was last moved.
            var hitTestPosition = new CosVector(Location - (MovementVector * epoch));
            var directionVector = MovementVector.Normalize();
            var totalTravelDistance = Math.Abs(Location.DistanceTo(hitTestPosition));

            if (totalTravelDistance > _engine.Display.TotalCanvasDiagonal)
            {
                //This is just a sanity check, if the epoch is super high then the engine is
                //  lagging like mad and the last thing we want is to trace a giant vector path.
                // Keep in mind that we are tracing the individual steps per "frame", so this IS NOT
                //  going to greatly effect collision detection even if the lagging is really bad.
                totalTravelDistance = _engine.Display.TotalCanvasDiagonal;
            }

            //Hit-test each position along the sprite path.
            for (int i = 0; i < totalTravelDistance; i++)
            {
                hitTestPosition += directionVector;
                foreach (var obj in objectsThatCanBeHit)
                {
                    if (obj.IntersectsAABB(hitTestPosition))
                    {
                        collisions.Add(obj);
                    }
                }
            }

            return collisions;
        }

        /// <summary>
        /// Returns the first collision (if any) the sprite made on is current movement vector.
        /// </summary>
        /// <returns></returns>
        public SpriteBase? FindFirstReverseCollisionAlongMovementVectorAABB(float epoch)
            => FindFirstReverseCollisionAlongMovementVectorAABB(_engine.Sprites.Visible(), epoch);

        /// <summary>
        /// Returns the first collision (if any) the sprite made on is current movement vector.
        /// </summary>
        /// <param name="objectsThatCanBeHit"></param>
        /// <returns></returns>
        public SpriteBase? FindFirstReverseCollisionAlongMovementVectorAABB(SpriteBase[] objectsThatCanBeHit, float epoch)
        {
            /// Takes the position of an object after it has been moved and tests each location
            ///     between where it ended up and where it should have come from given its movement vector.

            //Get the starting position of the sprite before it was last moved.
            var hitTestPosition = new CosVector(Location - (MovementVector * epoch));
            var directionVector = MovementVector.Normalize();
            var totalTravelDistance = Math.Abs(Location.DistanceTo(hitTestPosition));

            if (totalTravelDistance > _engine.Display.TotalCanvasDiagonal)
            {
                //This is just a sanity check, if the epoch is super high then the engine is
                //  lagging like mad and the last thing we want is to trace a giant vector path.
                // Keep in mind that we are tracing the individual steps per "frame", so this IS NOT
                //  going to greatly effect collision detection even if the lagging is really bad.
                totalTravelDistance = _engine.Display.TotalCanvasDiagonal;
            }

            //Hit-test each position along the sprite path.
            for (int i = 0; i < totalTravelDistance; i++)
            {
                hitTestPosition += directionVector;
                foreach (var obj in objectsThatCanBeHit)
                {
                    if (obj.IntersectsAABB(hitTestPosition))
                    {
                        return obj;
                    }
                }
            }

            return null;
        }

        #endregion

        #region Pre-movement movement vector collision detection.

        /// <summary>
        /// Returns a list of all collisions the sprite will make on is current movement vector, in the order in which they would be encountered.
        /// </summary>
        /// <returns></returns>
        public List<SpriteBase> FindForwardCollisionsAlongMovementVectorAABB(float epoch)
            => FindForwardCollisionsAlongMovementVectorAABB(_engine.Sprites.Visible(), epoch);

        /// <summary>
        /// Returns a list of all collisions the sprite will make on is current movement vector, in the order in which they would be encountered.
        /// </summary>
        /// <param name="objectsThatCanBeHit"></param>
        /// <returns></returns>
        public List<SpriteBase> FindForwardCollisionsAlongMovementVectorAABB(SpriteBase[] objectsThatCanBeHit, float epoch)
        {
            /// Takes the position of an object before it has been moved and tests each location
            ///     between where it is and where it will end up given its movement vector.

            var collisions = new List<SpriteBase>();

            var hitTestPosition = new CosVector(Location);
            var destinationPoint = new CosVector(Location + (MovementVector * epoch));
            var directionVector = MovementVector.Normalize();
            var totalTravelDistance = Math.Abs(Location.DistanceTo(destinationPoint));

            if (totalTravelDistance > _engine.Display.TotalCanvasDiagonal)
            {
                //This is just a sanity check, if the epoch is super high then the engine is
                //  lagging like mad and the last thing we want is to trace a giant vector path.
                // Keep in mind that we are tracing the individual steps per "frame", so this IS NOT
                //  going to greatly effect collision detection even if the lagging is really bad.
                totalTravelDistance = _engine.Display.TotalCanvasDiagonal;
            }

            //Hit-test each position along the sprite path.
            for (int i = 0; i < totalTravelDistance; i++)
            {
                hitTestPosition += directionVector;
                foreach (var obj in objectsThatCanBeHit)
                {
                    if (obj.IntersectsAABB(hitTestPosition))
                    {
                        collisions.Add(obj);
                    }
                }
            }

            return collisions;
        }

        /// <summary>
        /// Returns the first collision (if any) the sprite will make on is current movement vector.
        /// </summary>
        /// <returns></returns>
        public SpriteBase? FindFirstForwardCollisionAlongMovementVectorAABB(float epoch)
            => FindFirstForwardCollisionAlongMovementVectorAABB(_engine.Sprites.Visible(), epoch);

        /// <summary>
        /// Returns the first collision (if any) the sprite will make on is current movement vector.
        /// </summary>
        /// <param name="objectsThatCanBeHit"></param>
        /// <returns></returns>
        public SpriteBase? FindFirstForwardCollisionAlongMovementVectorAABB(SpriteBase[] objectsThatCanBeHit, float epoch)
        {
            /// Takes the position of an object before it has been moved and tests each location
            ///     between where it is and where it will end up given its movement vector.

            var hitTestPosition = new CosVector(Location);
            var destinationPoint = new CosVector(Location + (MovementVector * epoch));
            var directionVector = MovementVector.Normalize();
            var totalTravelDistance = Math.Abs(Location.DistanceTo(destinationPoint));

            if (totalTravelDistance > _engine.Display.TotalCanvasDiagonal)
            {
                //This is just a sanity check, if the epoch is super high then the engine is
                //  lagging like mad and the last thing we want is to trace a giant vector path.
                // Keep in mind that we are tracing the individual steps per "frame", so this IS NOT
                //  going to greatly effect collision detection even if the lagging is really bad.
                totalTravelDistance = _engine.Display.TotalCanvasDiagonal;
            }

            //Hit-test each position along the sprite path.
            for (int i = 0; i < totalTravelDistance; i++)
            {
                hitTestPosition += directionVector;
                foreach (var obj in objectsThatCanBeHit)
                {
                    if (obj.IntersectsAABB(hitTestPosition))
                    {
                        return obj;
                    }
                }
            }

            return null;
        }

        #endregion

        #region Vector distance collision detection.

        /// <summary>
        /// Returns a list of all collisions the sprite will make over a given distance and optional angle, in the order in which they would be encountered.
        /// </summary>
        /// <param name="distance">Distance to detect collisions.</param>
        /// <param name="angle">Optional angle for detection, if not specified then the sprites forward angle is used.</param>
        /// <returns></returns>
        public List<SpriteBase> FindCollisionsAlongDistanceVectorAABB(float distance, CosVector? angle = null)
            => FindCollisionsAlongDistanceVectorAABB(_engine.Sprites.Visible(), distance, angle);

        /// <summary>
        ///  Returns a list of all collisions the sprite will make over a given distance and optional angle, in the order in which they would be encountered.
        /// </summary>
        /// <param name="objectsThatCanBeHit">List of objects to test for collisions.</param>
        /// <param name="distance">Distance to detect collisions.</param>
        /// <param name="angle">Optional angle for detection, if not specified then the sprites forward angle is used.</param>
        /// <returns></returns>
        public List<SpriteBase> FindCollisionsAlongDistanceVectorAABB(SpriteBase[] objectsThatCanBeHit, float distance, CosVector? angle = null)
        {
            var collisions = new List<SpriteBase>();

            var hitTestPosition = new CosVector(Location);
            var directionVector = angle ?? Orientation;

            //Hit-test each position along the sprite path.
            for (int i = 0; i < distance; i++)
            {
                hitTestPosition += directionVector;
                foreach (var obj in objectsThatCanBeHit)
                {
                    if (obj.IntersectsAABB(hitTestPosition))
                    {
                        collisions.Add(obj);
                    }
                }
            }

            return collisions;
        }

        /// <summary>
        /// Returns a the first object the sprite will collide with over a given distance and optional angle.
        /// </summary>
        /// <param name="distance">Distance to detect collisions.</param>
        /// <param name="angle">Optional angle for detection, if not specified then the sprites forward angle is used.</param>
        /// <returns></returns>
        public SpriteBase? FindFirstCollisionAlongDistanceVectorAABB(float distance, CosVector? angle = null)
            => FindFirstCollisionAlongDistanceVectorAABB(_engine.Sprites.Visible(), distance, angle);

        /// <summary>
        /// Returns a the first object the sprite will collide with over a given distance and optional angle.
        /// </summary>
        /// <param name="objectsThatCanBeHit">List of objects to test for collisions.</param>
        /// <param name="distance">Distance to detect collisions.</param>
        /// <param name="angle">Optional angle for detection, if not specified then the sprites forward angle is used.</param>
        /// <returns></returns>
        public SpriteBase? FindFirstCollisionAlongDistanceVectorAABB(SpriteBase[] objectsThatCanBeHit, float distance, CosVector? angle = null)
        {
            var hitTestPosition = new CosVector(Location);
            var directionVector = angle ?? Orientation;

            //Hit-test each position along the sprite path.
            for (int i = 0; i < distance; i++)
            {
                hitTestPosition += directionVector;
                foreach (var obj in objectsThatCanBeHit)
                {
                    if (obj.IntersectsAABB(hitTestPosition))
                    {
                        return obj;
                    }
                }
            }

            return null;
        }

        #endregion

        #region Intersections.

        /// <summary>
        /// Determines if two axis-aligned bounding boxes (AABB) intersect.
        /// </summary>
        /// <param name="otherObject"></param>
        /// <returns></returns>
        public bool IntersectsAABB(SpriteBase otherObject)
        {
            if (Visible && otherObject.Visible && !IsQueuedForDeletion && !otherObject.IsQueuedForDeletion)
            {
                return Bounds.IntersectsWith(otherObject.Bounds);
            }
            return false;
        }

        public bool IntersectsWithTrajectory(SpriteBase otherObject)
        {
            if (Visible && otherObject.Visible)
            {
                var previousPosition = otherObject.Location;

                for (int i = 0; i < otherObject.Speed; i++)
                {
                    previousPosition.X -= otherObject.Orientation.X;
                    previousPosition.Y -= otherObject.Orientation.Y;

                    if (IntersectsAABB(previousPosition))
                    {
                        return true;

                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if two axis-aligned bounding boxes (AABB) intersect.
        /// </summary>
        /// <returns></returns>
        public bool IntersectsAABB(SpriteBase otherObject, CosVector sizeAdjust)
        {
            if (Visible && otherObject.Visible && !IsQueuedForDeletion && !otherObject.IsQueuedForDeletion)
            {
                var alteredHitBox = new RectangleF(
                    otherObject.Bounds.X - (sizeAdjust.X / 2.0f),
                    otherObject.Bounds.Y - (sizeAdjust.Y / 2.0f),
                    otherObject.Bounds.Width + (sizeAdjust.X / 2.0f),
                    otherObject.Bounds.Height + (sizeAdjust.Y / 2.0f));

                return Bounds.IntersectsWith(alteredHitBox);
            }
            return false;
        }

        /// <summary>
        /// Intersect detection with another object using adjusted "hit box" size.
        /// </summary>
        /// <returns></returns>
        public bool Intersects(SpriteBase with, int variance = 0)
        {
            var alteredHitBox = new RectangleF(
                (with.Bounds.X - variance),
                (with.Bounds.Y - variance),
                with.Size.Width + variance * 2, with.Size.Height + variance * 2);

            return Bounds.IntersectsWith(alteredHitBox);
        }

        /// <summary>
        /// Determines if two axis-aligned bounding boxes (AABB) intersect.
        /// </summary>
        /// <returns></returns>
        public bool IntersectsAABB(CosVector location, CosVector size)
        {
            var alteredHitBox = new RectangleF(
                location.X,
                location.Y,
                size.X,
                size.Y
                );

            return Bounds.IntersectsWith(alteredHitBox);
        }

        /// <summary>
        /// Determines if two axis-aligned bounding boxes (AABB) intersect.
        /// </summary>
        /// <returns></returns>
        public bool RenderLocationIntersectsAABB(CosVector location, CosVector size)
        {
            var alteredHitBox = new RectangleF(
                location.X,
                location.Y,
                size.X,
                size.Y
                );

            return RenderBounds.IntersectsWith(alteredHitBox);
        }

        /// <summary>
        /// Determines if two axis-aligned bounding boxes (AABB) intersect.
        /// </summary>
        /// <returns></returns>
        public bool IntersectsAABB(CosVector location)
        {
            var alteredHitBox = new RectangleF(location.X, location.Y, 1f, 1f);
            return Bounds.IntersectsWith(alteredHitBox);
        }

        #endregion
    }
}
