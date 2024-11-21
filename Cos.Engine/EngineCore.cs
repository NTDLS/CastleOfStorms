using Cos.Engine.Manager;
using Cos.Engine.Sprite;
using Cos.Engine.Sprite._Superclass._Root;
using Cos.Engine.TickController.PlayerSpriteTickController;
using Cos.Engine.TickController.UnvectoredTickController;
using Cos.Library;
using Cos.Library.Mathematics;
using Cos.Rendering;
using Newtonsoft.Json;
using NTDLS.Helpers;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using static Cos.Library.CosConstants;

namespace Cos.Engine
{
    /// <summary>
    /// The core game engine. Contained the controllers and managers.
    /// </summary>
    public class EngineCore
    {
        #region Backend variables.

        private readonly EngineWorldClock _worldClock;

        #endregion

        #region Public properties.

        public CosEngineInitializationType ExecutionType { get; private set; }

        public bool IsRunning { get; private set; } = false;
        public bool IsInitializing { get; private set; } = false;

        #endregion

        #region Managers. 

        public InputManager Input { get; private set; }
        public DisplayManager Display { get; private set; }
        public SpriteManager Sprites { get; private set; } //Also contains all of the sprite tick controllers.
        public AudioManager Audio { get; private set; }
        public AssetManager Assets { get; private set; }

        #endregion

        #region Tick Controllers.

        public EventTickController Events { get; private set; }
        public PlayerSpriteTickController Player { get; private set; }
        public CosRendering Rendering { get; private set; }
        public CosEngineSettings Settings { get; private set; }

        #endregion

        #region Events.

        public delegate void InitializationEvent(EngineCore engine);
        public event InitializationEvent? OnInitialization;

        public delegate void ShutdownEvent(EngineCore engine);
        public event ShutdownEvent? OnShutdown;

        #endregion

        public EngineCore()
        {
            var drawingSurface = new Control();

            ExecutionType = CosEngineInitializationType.None;

            Settings = LoadSettings();

            Display = new DisplayManager(this, drawingSurface);
            Rendering = new CosRendering(Settings, drawingSurface, Display.CanvasSize);
            Assets = new AssetManager(this);
            Events = new EventTickController(this);
            Sprites = new SpriteManager(this);
            Input = new InputManager(this);

            Audio = new AudioManager(this);
            Player = new PlayerSpriteTickController(this);

            _worldClock = new EngineWorldClock(this);
        }

        /// <summary>
        /// Initializes a new instance of the game engine.
        /// </summary>
        /// <param name="drawingSurface">The window that the game will be rendered to.</param>
        public EngineCore(Control drawingSurface, CosEngineInitializationType executionType)
        {
            ExecutionType = executionType;

            Settings = LoadSettings();

            Display = new DisplayManager(this, drawingSurface);
            Rendering = new CosRendering(Settings, drawingSurface, Display.CanvasSize);
            Assets = new AssetManager(this);
            Events = new EventTickController(this);
            Sprites = new SpriteManager(this);
            Input = new InputManager(this);

            Audio = new AudioManager(this);
            Player = new PlayerSpriteTickController(this);

            _worldClock = new EngineWorldClock(this);
        }

        public static CosEngineSettings LoadSettings()
        {
            var engineSettingsText = AssetManager.GetUserText("Engine.Settings.json");

            if (string.IsNullOrEmpty(engineSettingsText))
            {
                var defaultSettings = new CosEngineSettings();

                int x = 1024;
                int y = 768;

                if (Screen.PrimaryScreen != null)
                {
                    x = (int)(Screen.PrimaryScreen.Bounds.Width * 0.75);
                    y = (int)(Screen.PrimaryScreen.Bounds.Height * 0.75);
                    if (x % 2 != 0) x++;
                    if (y % 2 != 0) y++;
                }

                defaultSettings.Resolution = new Size(x, y);

                engineSettingsText = JsonConvert.SerializeObject(defaultSettings, Formatting.Indented);
                AssetManager.PutUserText("Engine.Settings.json", engineSettingsText);
            }

            return JsonConvert.DeserializeObject<CosEngineSettings>(engineSettingsText).EnsureNotNull();
        }

        #region WorldClock Semaphore.

        /// <summary>
        /// Delegate for executions that do not require a return value.
        /// </summary>
        public delegate void WorldClockSemaphoreDelegate();

        private readonly Semaphore _worldClockSemaphore = new(1, 1);

        /// <summary>
        /// Used to ensure that sprites aren't added/deleted while the scene is being executed or rendered.
        /// </summary>
        public void UseWorldClock(WorldClockSemaphoreDelegate function)
        {
            try
            {
                _worldClockSemaphore.WaitOne();
                function();
            }
            finally
            {
                _worldClockSemaphore.Release();
            }
        }

        /// <summary>
        /// Used to ensure that sprites aren't added/deleted while the scene is being executed or rendered.
        /// </summary>
        public bool TryUseWorldClock(TimeSpan timeout, WorldClockSemaphoreDelegate function)
        {
            if (_worldClockSemaphore.WaitOne(timeout))
            {
                try
                {
                    function();
                    return true;
                }
                finally
                {
                    _worldClockSemaphore.Release();
                }
            }
            return false;
        }

        #endregion

        public static void SaveSettings(CosEngineSettings settings)
        {
            AssetManager.PutUserText("Engine.Settings.json", JsonConvert.SerializeObject(settings, Formatting.Indented));
        }

        public void ResetGame()
        {
            Sprites.TextBlocks.PlayerStatsText.Visible = false;
            Sprites.QueueDeletionOfActionSprites();
        }

        public void StartGame()
        {
            Sprites.QueueDeletionOfActionSprites();
        }

        public void RenderEverything()
        {
            try
            {
                Rendering.RenderTargets.Use((o =>
                {
                    if (o.ScreenRenderTarget != null && o.IntermediateRenderTarget != null)
                    {
                        o.IntermediateRenderTarget.BeginDraw();

                        if (ExecutionType == CosEngineInitializationType.Play)
                        {
                            o.IntermediateRenderTarget.Clear(Rendering.Materials.Colors.Black);
                        }
                        else
                        {
                            o.IntermediateRenderTarget.Clear(Rendering.Materials.Colors.EditorBackground);
                        }

                        Sprites.Render(o.IntermediateRenderTarget);

                        o.IntermediateRenderTarget.EndDraw();

                        o.ScreenRenderTarget.BeginDraw();

                        Rendering.TransferWithZoom(o.IntermediateRenderTarget, o.ScreenRenderTarget, 1.0f);

                        o.ScreenRenderTarget.EndDraw();
                    }
                }));
            }
            catch
            {
            }
        }

        public void StartEngine()
        {
            if (IsRunning)
            {
                throw new Exception("The game engine is already running.");
            }

            IsRunning = true;
            //Sprites.ResetPlayer();
            _worldClock.Start();

            var loadingHeader = Sprites.TextBlocks.Add(Rendering.TextFormats.Loading,
                Rendering.Materials.Brushes.Red, new CosVector(100, 100), true);

            var loadingDetail = Sprites.TextBlocks.Add(Rendering.TextFormats.Loading,
                Rendering.Materials.Brushes.Green, new CosVector(loadingHeader.X, loadingHeader.Y + 50), true);

            IsInitializing = true;

            HydrateCache(loadingHeader, loadingDetail);

            loadingHeader.QueueForDelete();
            loadingDetail.QueueForDelete();

            OnInitialization?.Invoke(this);

            IsInitializing = false;

            if (ExecutionType == CosEngineInitializationType.Play)
            {
                if (Settings.PlayMusic)
                {
                    Audio.BackgroundMusicSound.Play();
                }
            }
        }

        private void HydrateCache(SpriteTextBlock loadingHeader, SpriteTextBlock loadingDetail)
        {
            loadingHeader.SetTextAndCenterX("Hydrating cache...");

            loadingHeader.SetTextAndCenterX("Hydrating reflection cache...");
            CosReflection.BuildReflectionCacheOfType<SpriteBase>();

            if (Settings.PreCacheAllAssets)
            {
                Assets.HydrateCache(loadingHeader, loadingDetail);
                Sprites.HydrateCache(loadingHeader, loadingDetail);
            }
        }

        public void ShutdownEngine()
        {
            if (IsRunning)
            {
                IsRunning = false;

                OnShutdown?.Invoke(this);

                _worldClock.Dispose();
                Sprites.Dispose();
                Rendering.Dispose();
                Assets.Dispose();
            }
        }

        public bool IsPaused() => _worldClock.IsPaused();
        public void TogglePause() => _worldClock.TogglePause();
        public void Pause() => _worldClock.Pause();
        public void Resume() => _worldClock.Resume();
    }
}
