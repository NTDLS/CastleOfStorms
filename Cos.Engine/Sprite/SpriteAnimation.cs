using Cos.Engine.Sprite.SupportingClasses.Metadata;
using Cos.Library.Mathematics;
using NTDLS.Helpers;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Drawing;
using static Cos.Library.CosConstants;

namespace Cos.Engine.Sprite
{
    public class SpriteAnimation : SpriteMinimalBitmap
    {
        private SharpDX.Direct2D1.Bitmap? _sheetImage;
        private int _frameCount;
        private int _currentFrame = 0;
        private int _currentRow = 0;
        private int _currentColumn = 0;
        private int _rows;
        private int _columns;
        private int _frameDelayMilliseconds;
        private DateTime _lastFrameChange = DateTime.Now.AddSeconds(-60);

        public CosAnimationPlayMode PlayMode { get; private set; }

        public SpriteAnimation(EngineCore engine, string spriteSheetFileName)
            : base(engine)
        {
            Location = new CosVector();

            var metadata = _engine.Assets.GetMetaData<SpriteAnimationMetadata>(spriteSheetFileName);

            Speed = metadata.Speed;

            SetImage(spriteSheetFileName);
            FramesPerSecond = metadata.FramesPerSecond;
            SetSize(new Size(metadata.FrameWidth, metadata.FrameHeight));

            PlayMode = metadata.PlayMode;

            AdvanceImage();
        }

        public float FramesPerSecond
        {
            set => _frameDelayMilliseconds = (int)((1.0f / value) * 1000.0f);
            get => (int)(1.0f / (_frameDelayMilliseconds / 1000.0f));
        }

        /// <summary>
        /// We want to get the entire animation sheet and reserve the base.image for the individual slices set by AdvanceImage().
        /// </summary>
        /// <param name="imagePath"></param>
        public new void SetImage(string imagePath)
        {
            _sheetImage = _engine.Assets.GetBitmap(imagePath);
        }

        public new void SetSize(Size frameSize)
        {
            base.SetSize(frameSize);

            _sheetImage.EnsureNotNull();

            _rows = (int)(_sheetImage.Size.Height / frameSize.Height);
            _columns = (int)(_sheetImage.Size.Width / frameSize.Width);
            _frameCount = _rows * _columns;
        }

        public void Play()
        {
            _currentFrame = 0;
            _currentRow = 0;
            _currentColumn = 0;
            _lastFrameChange = DateTime.Now.AddSeconds(-60);
            Visible = true;
        }

        public override void Render(RenderTarget renderTarget)
        {
            var sourceRect = new RawRectangleF(
                _currentColumn * Size.Width,
                _currentRow * Size.Height,
                _currentColumn * Size.Width + Size.Width,
                _currentRow * Size.Height + Size.Height);

            _engine.Rendering.DrawBitmap(
                renderTarget,
                _sheetImage ?? throw new NullReferenceException(),
                RenderLocation.X - Size.Width / 2.0f,
                RenderLocation.Y - Size.Height / 2.0f,
                Orientation.RadiansSigned,
                sourceRect,
                new Size2F(Size.Width, Size.Height)
            );
        }

        public void AdvanceImage()
        {
            if ((DateTime.Now - _lastFrameChange).TotalMilliseconds > _frameDelayMilliseconds)
            {
                _lastFrameChange = DateTime.Now;

                if (++_currentColumn == _columns)
                {
                    _currentColumn = 0;
                    _currentRow++;
                }

                _currentFrame++;

                if (_currentFrame == _frameCount)
                {
                    switch (PlayMode)
                    {
                        case CosAnimationPlayMode.DeleteAfterPlay:
                            //Delete the animation sprite.
                            QueueForDelete();
                            break;
                        case CosAnimationPlayMode.Infinite:
                            //Reset the frame, but retain the _lastFrameChange.
                            _currentFrame = 0;
                            _currentColumn = 0;
                            _currentRow = 0;
                            break;
                        case CosAnimationPlayMode.Single:
                            //Nothing to do unless the player calls Play() again.
                            break;
                    }

                    return;
                }
            }
        }
    }
}
