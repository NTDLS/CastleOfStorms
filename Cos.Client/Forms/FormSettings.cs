using Cos.Client.Hardware;
using Cos.Engine;
using Cos.Rendering;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Cos.Client
{
    public partial class FormSettings : Form
    {
        private const int MAX_RESOLUTIONS = 32;

        private Screen _screen;

        public FormSettings(Screen screen)
        {
            InitializeComponent();
            _screen = screen;
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            trackBarResolution.Scroll += TrackBarResolution_Scroll;

            var settings = EngineCore.LoadSettings();

            checkBoxFineTuneFrameRate.Checked = settings.FineTuneFramerate;
            checkBoxPlayMusic.Checked = settings.PlayMusic;
            checkBoxEnableAntiAliasing.Checked = settings.AntiAliasing;
            checkBoxEnableVerticalSync.Checked = settings.VerticalSync;
            checkBoxAutoZoomWhenMoving.Checked = settings.EnableSpeedScaleFactoring;
            checkBoxHighlightAllSprites.Checked = settings.HighlightAllSprites;
            checkBoxHighlightNaturalBounds.Checked = settings.HighlightNaturalBounds;
            checkBoxEnableSpriteInterrogation.Checked = settings.EnableSpriteInterrogation;
            checkBoxPreCacheAllAssets.Checked = settings.PreCacheAllAssets;
            textBoxTargetFrameRate.Text = $"{settings.TargetFrameRate:n0}";
            textBoxOverdrawScale.Text = $"{settings.OverdrawScale:n0}";
            textBoxInitialFrameStarCount.Text = $"{settings.InitialFrameStarCount:n0}";
            textBoxDeltaFrameTargetStarCount.Text = $"{settings.DeltaFrameTargetStarCount:n0}";

            if (checkBoxEnableVerticalSync.Checked)
            {
                checkBoxFineTuneFrameRate.Checked = false;

                checkBoxFineTuneFrameRate.Enabled = false;
                textBoxTargetFrameRate.Enabled = false;

                textBoxTargetFrameRate.Text = Display.GetControlMonitorRefreshRate(this).ToString();
            }

            trackBarResolution.Minimum = 1;
            trackBarResolution.Maximum = MAX_RESOLUTIONS;

            for (int i = 0; i < MAX_RESOLUTIONS + 1; i++)
            {
                int baseX = _screen.Bounds.Width - (int)((float)_screen.Bounds.Width * (1.0 - ((float)i / (float)MAX_RESOLUTIONS)));
                int baseY = _screen.Bounds.Height - (int)((float)_screen.Bounds.Height * (1.0 - ((float)i / (float)MAX_RESOLUTIONS)));

                if ((baseX % 2) != 0) baseX++;
                if ((baseY % 2) != 0) baseY++;

                if (trackBarResolution.Minimum == 1 && baseX >= 540 && baseY >= 540)
                {
                    trackBarResolution.Minimum = i;
                }

                if (baseX >= settings.Resolution.Width && baseY >= settings.Resolution.Height)
                {
                    trackBarResolution.Value = i;
                    TrackBarResolution_Scroll(this, new EventArgs());
                    break;
                }
            }

            var adapters = CosRenderingUtility.GetGraphicsAdapters();
            foreach (var item in adapters)
            {
                comboBoxGraphicsAdapter.Items.Add(item);
                if (settings.GraphicsAdapterId == item.DeviceId)
                {
                    comboBoxGraphicsAdapter.SelectedItem = item;
                }
            }

            if (comboBoxGraphicsAdapter.SelectedItem == null)
            {
                comboBoxGraphicsAdapter.SelectedItem = adapters.OrderByDescending(o => o.VideoMemoryMb).First();
            }
        }

        private void TrackBarResolution_Scroll(object? sender, EventArgs e)
        {
            int baseX = _screen.Bounds.Width;
            int baseY = _screen.Bounds.Height;

            if (trackBarResolution.Value < MAX_RESOLUTIONS)
            {
                baseX = _screen.Bounds.Width - (int)((float)_screen.Bounds.Width * (1.0 - ((float)trackBarResolution.Value / (float)MAX_RESOLUTIONS)));
                baseY = _screen.Bounds.Height - (int)((float)_screen.Bounds.Height * (1.0 - ((float)trackBarResolution.Value / (float)MAX_RESOLUTIONS)));

                if ((baseX % 2) != 0) baseX++;
                if ((baseY % 2) != 0) baseY++;

                labelResolution.Text = $"{baseX} x {baseY}";
            }
            else
            {
                labelResolution.Text = $"{baseX} x {baseY} [Full Screen]";
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private float GetAndValidate(TextBox textbox, float min, float max, string fieldNameForError)
        {
            if (float.TryParse(textbox.Text, out var value) == false || value < min || value > max)
            {
                throw new Exception($"Can invalid value has been specified for: {fieldNameForError}. Enter a whole or decimal numeric value between {min} and {max}.");
            }
            return value;
        }

        private int GetAndValidate(TextBox textbox, int min, int max, string fieldNameForError)
        {
            if (int.TryParse(textbox.Text, out var value) == false || value < min || value > max)
            {
                throw new Exception($"Can invalid value has been specified for: {fieldNameForError}. Enter a while numeric value between {min} and {max}.");
            }
            return value;
        }
        private void ButtonSave_Click(object sender, EventArgs e)
        {
            try
            {
                var settings = EngineCore.LoadSettings();

                settings.FineTuneFramerate = checkBoxFineTuneFrameRate.Checked;
                settings.PlayMusic = checkBoxPlayMusic.Checked;
                settings.VerticalSync = checkBoxEnableVerticalSync.Checked;
                settings.AntiAliasing = checkBoxEnableAntiAliasing.Checked;
                settings.EnableSpeedScaleFactoring = checkBoxAutoZoomWhenMoving.Checked;
                settings.HighlightAllSprites = checkBoxHighlightAllSprites.Checked;
                settings.HighlightNaturalBounds = checkBoxHighlightNaturalBounds.Checked;
                settings.EnableSpriteInterrogation = checkBoxEnableSpriteInterrogation.Checked;
                settings.PreCacheAllAssets = checkBoxPreCacheAllAssets.Checked;

                settings.TargetFrameRate = GetAndValidate(textBoxTargetFrameRate, 10, settings.WorldTicksPerSecond, "Frame Limiter");
                settings.OverdrawScale = GetAndValidate(textBoxOverdrawScale, 1.0f, 10.0f, "Overdraw scale");
                settings.InitialFrameStarCount = GetAndValidate(textBoxInitialFrameStarCount, 0, 1000, "Initial frame star count");
                settings.DeltaFrameTargetStarCount = GetAndValidate(textBoxDeltaFrameTargetStarCount, 0, 1000, "Delta-frame target star count");

                int baseX = _screen.Bounds.Width - (int)((float)_screen.Bounds.Width * (1.0 - ((float)trackBarResolution.Value / (float)MAX_RESOLUTIONS)));
                int baseY = _screen.Bounds.Height - (int)((float)_screen.Bounds.Height * (1.0 - ((float)trackBarResolution.Value / (float)MAX_RESOLUTIONS)));
                settings.Resolution = new System.Drawing.Size(baseX, baseY);

                settings.FullScreen = (trackBarResolution.Value == MAX_RESOLUTIONS);

                var graphicsAdapter = comboBoxGraphicsAdapter.SelectedItem as CosGraphicsAdapter;
                if (graphicsAdapter == null)
                {
                    throw new Exception("You must select a graphics adapter.");
                }

                settings.GraphicsAdapterId = graphicsAdapter.DeviceId;

                EngineCore.SaveSettings(settings);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Strikeforce Infinite", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void CheckBoxEnableVerticalSync_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxEnableVerticalSync.Checked)
            {
                checkBoxFineTuneFrameRate.Checked = false;
                textBoxTargetFrameRate.Text = Display.GetControlMonitorRefreshRate(this).ToString();
            }

            checkBoxFineTuneFrameRate.Enabled = !checkBoxEnableVerticalSync.Checked;
            textBoxTargetFrameRate.Enabled = !checkBoxEnableVerticalSync.Checked;
        }
    }
}
