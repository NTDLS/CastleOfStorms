using Cos.Library.Mathematics;

namespace Cos.Library
{
    public static class CosConstants
    {
        public static string FriendlyName = "Castle of Storms";

        public static class NeighborOffsets
        {
            public static CosVector TopLeft = new CosVector(-32, -32); //top-left
            public static CosVector Top = new CosVector(0, -32); //top
            public static CosVector TopRight = new CosVector(+32, -32); //top-right
            public static CosVector Right = new CosVector(+32, 0); //right
            public static CosVector BottomRight = new CosVector(+32, +32); //bottom-right
            public static CosVector Bottom = new CosVector(0, +32); //bottom
            public static CosVector BottomLeft = new CosVector(-32, +32); //bottom-left
            public static CosVector Left = new CosVector(-32, 0); //left
        }

        public enum TilePackTileType
        {
            TopLeft,
            Top,
            TopRight,
            Right,
            BottomRight,
            Bottom,
            BottomLeft,
            Left,
            Center
        }

        public enum CosEngineInitializationType
        {
            None,
            Play,
            Edit
        }

        public enum CosParticleCleanupMode
        {
            None,
            FadeToBlack,
            DistanceOffScreen
        }

        public enum CosParticleShape
        {
            FilledEllipse,
            HollowEllipse,
            HollowRectangle,
            FilledRectangle,
            Triangle
        }

        public enum CosParticleColorType
        {
            Solid,
            Gradient
        }

        public enum CosParticleVectorType
        {
            /// <summary>
            /// The sprite will travel in the direction determined by it's MovementVector.
            /// </summary>
            Default,
            /// <summary>
            /// The sprite will travel in the direction in which is is oriented.
            /// </summary>
            FollowOrientation
        }

        public enum CosLevelState
        {
            NotYetStarted,
            Started,
            Ended
        }

        public enum CosSituationState
        {
            NotYetStarted,
            Started,
            Ended
        }

        public enum CosCardinalDirection
        {
            None,
            North,
            East,
            South,
            West
        }

        public enum CosAnimationPlayMode
        {
            /// <summary>
            /// The animation will be played once and can be replayed by calling Play().
            /// </summary>
            Single,
            /// <summary>
            /// The animation will be played once then will be deleted.
            /// </summary>
            DeleteAfterPlay,
            /// <summary>
            /// The animation will loop until manually deleted or hidden.
            /// </summary>
            Infinite
        };

        public enum CosDamageType
        {
            Unspecified,
            Shield,
            Hull
        }

        public enum CosFiredFromType
        {
            Unspecified,
            Player,
            Enemy
        }

        public enum CosPlayerKey
        {
            SwitchWeaponLeft,
            SwitchWeaponRight,
            StrafeRight,
            StrafeLeft,
            SpeedBoost,
            Forward,
            Reverse,
            PrimaryFire,
            SecondaryFire,
            RotateCounterClockwise,
            RotateClockwise,
            Escape,
            Left,
            Right,
            Up,
            Down,
            Enter
        }
    }
}
