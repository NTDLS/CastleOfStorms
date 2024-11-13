namespace Cos.Client
{
    partial class FormSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSettings));
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPageDisplay = new System.Windows.Forms.TabPage();
            labelGraphicsAdapter = new System.Windows.Forms.Label();
            comboBoxGraphicsAdapter = new System.Windows.Forms.ComboBox();
            trackBarResolution = new System.Windows.Forms.TrackBar();
            labelResolutionLabel = new System.Windows.Forms.Label();
            labelResolution = new System.Windows.Forms.Label();
            tabPageDisplayAdvanced = new System.Windows.Forms.TabPage();
            checkBoxEnableAntiAliasing = new System.Windows.Forms.CheckBox();
            checkBoxFineTuneFrameRate = new System.Windows.Forms.CheckBox();
            checkBoxEnableVerticalSync = new System.Windows.Forms.CheckBox();
            labelInitialStarCount = new System.Windows.Forms.Label();
            labelFrameTargetStarCount = new System.Windows.Forms.Label();
            checkBoxAutoZoomWhenMoving = new System.Windows.Forms.CheckBox();
            textBoxDeltaFrameTargetStarCount = new System.Windows.Forms.TextBox();
            textBoxInitialFrameStarCount = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            labelOverdrawScale = new System.Windows.Forms.Label();
            textBoxOverdrawScale = new System.Windows.Forms.TextBox();
            textBoxTargetFrameRate = new System.Windows.Forms.TextBox();
            tabPageDebug = new System.Windows.Forms.TabPage();
            checkBoxEnableSpriteInterrogation = new System.Windows.Forms.CheckBox();
            checkBoxHighlightAllSprites = new System.Windows.Forms.CheckBox();
            checkBoxHighlightNaturalBounds = new System.Windows.Forms.CheckBox();
            tabPageAdvanced = new System.Windows.Forms.TabPage();
            checkBoxPreCacheAllAssets = new System.Windows.Forms.CheckBox();
            tabPageAudio = new System.Windows.Forms.TabPage();
            checkBoxPlayMusic = new System.Windows.Forms.CheckBox();
            buttonCancel = new System.Windows.Forms.Button();
            buttonSave = new System.Windows.Forms.Button();
            tabControl1.SuspendLayout();
            tabPageDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarResolution).BeginInit();
            tabPageDisplayAdvanced.SuspendLayout();
            tabPageDebug.SuspendLayout();
            tabPageAdvanced.SuspendLayout();
            tabPageAudio.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPageDisplay);
            tabControl1.Controls.Add(tabPageDisplayAdvanced);
            tabControl1.Controls.Add(tabPageDebug);
            tabControl1.Controls.Add(tabPageAdvanced);
            tabControl1.Controls.Add(tabPageAudio);
            tabControl1.Location = new System.Drawing.Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(550, 332);
            tabControl1.TabIndex = 17;
            // 
            // tabPageDisplay
            // 
            tabPageDisplay.Controls.Add(labelGraphicsAdapter);
            tabPageDisplay.Controls.Add(comboBoxGraphicsAdapter);
            tabPageDisplay.Controls.Add(trackBarResolution);
            tabPageDisplay.Controls.Add(labelResolutionLabel);
            tabPageDisplay.Controls.Add(labelResolution);
            tabPageDisplay.Location = new System.Drawing.Point(4, 24);
            tabPageDisplay.Name = "tabPageDisplay";
            tabPageDisplay.Padding = new System.Windows.Forms.Padding(3);
            tabPageDisplay.Size = new System.Drawing.Size(542, 304);
            tabPageDisplay.TabIndex = 0;
            tabPageDisplay.Text = "Display";
            tabPageDisplay.UseVisualStyleBackColor = true;
            // 
            // labelGraphicsAdapter
            // 
            labelGraphicsAdapter.AutoSize = true;
            labelGraphicsAdapter.Location = new System.Drawing.Point(15, 17);
            labelGraphicsAdapter.Name = "labelGraphicsAdapter";
            labelGraphicsAdapter.Size = new System.Drawing.Size(98, 15);
            labelGraphicsAdapter.TabIndex = 19;
            labelGraphicsAdapter.Text = "Graphics Adapter";
            // 
            // comboBoxGraphicsAdapter
            // 
            comboBoxGraphicsAdapter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxGraphicsAdapter.FormattingEnabled = true;
            comboBoxGraphicsAdapter.Location = new System.Drawing.Point(15, 35);
            comboBoxGraphicsAdapter.Name = "comboBoxGraphicsAdapter";
            comboBoxGraphicsAdapter.Size = new System.Drawing.Size(511, 23);
            comboBoxGraphicsAdapter.TabIndex = 0;
            // 
            // trackBarResolution
            // 
            trackBarResolution.LargeChange = 1;
            trackBarResolution.Location = new System.Drawing.Point(15, 100);
            trackBarResolution.Name = "trackBarResolution";
            trackBarResolution.Size = new System.Drawing.Size(223, 45);
            trackBarResolution.TabIndex = 1;
            // 
            // labelResolutionLabel
            // 
            labelResolutionLabel.AutoSize = true;
            labelResolutionLabel.Location = new System.Drawing.Point(15, 82);
            labelResolutionLabel.Name = "labelResolutionLabel";
            labelResolutionLabel.Size = new System.Drawing.Size(69, 15);
            labelResolutionLabel.TabIndex = 9;
            labelResolutionLabel.Text = "Resolution: ";
            // 
            // labelResolution
            // 
            labelResolution.AutoSize = true;
            labelResolution.Location = new System.Drawing.Point(90, 82);
            labelResolution.Name = "labelResolution";
            labelResolution.Size = new System.Drawing.Size(61, 15);
            labelResolution.TabIndex = 10;
            labelResolution.Text = "0000x0000";
            // 
            // tabPageDisplayAdvanced
            // 
            tabPageDisplayAdvanced.Controls.Add(checkBoxEnableAntiAliasing);
            tabPageDisplayAdvanced.Controls.Add(checkBoxFineTuneFrameRate);
            tabPageDisplayAdvanced.Controls.Add(checkBoxEnableVerticalSync);
            tabPageDisplayAdvanced.Controls.Add(labelInitialStarCount);
            tabPageDisplayAdvanced.Controls.Add(labelFrameTargetStarCount);
            tabPageDisplayAdvanced.Controls.Add(checkBoxAutoZoomWhenMoving);
            tabPageDisplayAdvanced.Controls.Add(textBoxDeltaFrameTargetStarCount);
            tabPageDisplayAdvanced.Controls.Add(textBoxInitialFrameStarCount);
            tabPageDisplayAdvanced.Controls.Add(label2);
            tabPageDisplayAdvanced.Controls.Add(labelOverdrawScale);
            tabPageDisplayAdvanced.Controls.Add(textBoxOverdrawScale);
            tabPageDisplayAdvanced.Controls.Add(textBoxTargetFrameRate);
            tabPageDisplayAdvanced.Location = new System.Drawing.Point(4, 24);
            tabPageDisplayAdvanced.Name = "tabPageDisplayAdvanced";
            tabPageDisplayAdvanced.Padding = new System.Windows.Forms.Padding(3);
            tabPageDisplayAdvanced.Size = new System.Drawing.Size(542, 304);
            tabPageDisplayAdvanced.TabIndex = 1;
            tabPageDisplayAdvanced.Text = "Display (Advanced)";
            tabPageDisplayAdvanced.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnableAntiAliasing
            // 
            checkBoxEnableAntiAliasing.AutoSize = true;
            checkBoxEnableAntiAliasing.Location = new System.Drawing.Point(17, 138);
            checkBoxEnableAntiAliasing.Name = "checkBoxEnableAntiAliasing";
            checkBoxEnableAntiAliasing.Size = new System.Drawing.Size(136, 19);
            checkBoxEnableAntiAliasing.TabIndex = 5;
            checkBoxEnableAntiAliasing.Text = "Enable Anti-aliasing?";
            checkBoxEnableAntiAliasing.UseVisualStyleBackColor = true;
            // 
            // checkBoxFineTuneFrameRate
            // 
            checkBoxFineTuneFrameRate.AutoSize = true;
            checkBoxFineTuneFrameRate.Location = new System.Drawing.Point(17, 88);
            checkBoxFineTuneFrameRate.Name = "checkBoxFineTuneFrameRate";
            checkBoxFineTuneFrameRate.Size = new System.Drawing.Size(139, 19);
            checkBoxFineTuneFrameRate.TabIndex = 3;
            checkBoxFineTuneFrameRate.Text = "Fine-tune frame rate?";
            checkBoxFineTuneFrameRate.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnableVerticalSync
            // 
            checkBoxEnableVerticalSync.AutoSize = true;
            checkBoxEnableVerticalSync.Location = new System.Drawing.Point(17, 63);
            checkBoxEnableVerticalSync.Name = "checkBoxEnableVerticalSync";
            checkBoxEnableVerticalSync.Size = new System.Drawing.Size(137, 19);
            checkBoxEnableVerticalSync.TabIndex = 2;
            checkBoxEnableVerticalSync.Text = "Enable Vertical-Sync?";
            checkBoxEnableVerticalSync.UseVisualStyleBackColor = true;
            checkBoxEnableVerticalSync.CheckedChanged += CheckBoxEnableVerticalSync_CheckedChanged;
            // 
            // labelInitialStarCount
            // 
            labelInitialStarCount.AutoSize = true;
            labelInitialStarCount.Location = new System.Drawing.Point(230, 68);
            labelInitialStarCount.Name = "labelInitialStarCount";
            labelInitialStarCount.Size = new System.Drawing.Size(100, 15);
            labelInitialStarCount.TabIndex = 33;
            labelInitialStarCount.Text = "Initial frame stars:";
            // 
            // labelFrameTargetStarCount
            // 
            labelFrameTargetStarCount.AutoSize = true;
            labelFrameTargetStarCount.Location = new System.Drawing.Point(229, 120);
            labelFrameTargetStarCount.Name = "labelFrameTargetStarCount";
            labelFrameTargetStarCount.Size = new System.Drawing.Size(129, 15);
            labelFrameTargetStarCount.TabIndex = 32;
            labelFrameTargetStarCount.Text = "Delta-frame star count:";
            // 
            // checkBoxAutoZoomWhenMoving
            // 
            checkBoxAutoZoomWhenMoving.AutoSize = true;
            checkBoxAutoZoomWhenMoving.Location = new System.Drawing.Point(17, 113);
            checkBoxAutoZoomWhenMoving.Name = "checkBoxAutoZoomWhenMoving";
            checkBoxAutoZoomWhenMoving.Size = new System.Drawing.Size(168, 19);
            checkBoxAutoZoomWhenMoving.TabIndex = 4;
            checkBoxAutoZoomWhenMoving.Text = "Auto-zoom when moving?";
            checkBoxAutoZoomWhenMoving.UseVisualStyleBackColor = true;
            // 
            // textBoxDeltaFrameTargetStarCount
            // 
            textBoxDeltaFrameTargetStarCount.Location = new System.Drawing.Point(230, 138);
            textBoxDeltaFrameTargetStarCount.Name = "textBoxDeltaFrameTargetStarCount";
            textBoxDeltaFrameTargetStarCount.Size = new System.Drawing.Size(133, 23);
            textBoxDeltaFrameTargetStarCount.TabIndex = 8;
            // 
            // textBoxInitialFrameStarCount
            // 
            textBoxInitialFrameStarCount.Location = new System.Drawing.Point(230, 86);
            textBoxInitialFrameStarCount.Name = "textBoxInitialFrameStarCount";
            textBoxInitialFrameStarCount.Size = new System.Drawing.Size(133, 23);
            textBoxInitialFrameStarCount.TabIndex = 7;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(17, 16);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(101, 15);
            label2.TabIndex = 27;
            label2.Text = "Target Frame rate:";
            // 
            // labelOverdrawScale
            // 
            labelOverdrawScale.AutoSize = true;
            labelOverdrawScale.Location = new System.Drawing.Point(230, 16);
            labelOverdrawScale.Name = "labelOverdrawScale";
            labelOverdrawScale.Size = new System.Drawing.Size(90, 15);
            labelOverdrawScale.TabIndex = 26;
            labelOverdrawScale.Text = "Overdraw scale:";
            // 
            // textBoxOverdrawScale
            // 
            textBoxOverdrawScale.Location = new System.Drawing.Point(230, 34);
            textBoxOverdrawScale.Name = "textBoxOverdrawScale";
            textBoxOverdrawScale.Size = new System.Drawing.Size(133, 23);
            textBoxOverdrawScale.TabIndex = 6;
            // 
            // textBoxTargetFrameRate
            // 
            textBoxTargetFrameRate.Location = new System.Drawing.Point(17, 34);
            textBoxTargetFrameRate.Name = "textBoxTargetFrameRate";
            textBoxTargetFrameRate.Size = new System.Drawing.Size(133, 23);
            textBoxTargetFrameRate.TabIndex = 1;
            // 
            // tabPageDebug
            // 
            tabPageDebug.Controls.Add(checkBoxEnableSpriteInterrogation);
            tabPageDebug.Controls.Add(checkBoxHighlightAllSprites);
            tabPageDebug.Controls.Add(checkBoxHighlightNaturalBounds);
            tabPageDebug.Location = new System.Drawing.Point(4, 24);
            tabPageDebug.Name = "tabPageDebug";
            tabPageDebug.Size = new System.Drawing.Size(542, 304);
            tabPageDebug.TabIndex = 2;
            tabPageDebug.Text = "Debug";
            tabPageDebug.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnableSpriteInterrogation
            // 
            checkBoxEnableSpriteInterrogation.AutoSize = true;
            checkBoxEnableSpriteInterrogation.Location = new System.Drawing.Point(13, 63);
            checkBoxEnableSpriteInterrogation.Name = "checkBoxEnableSpriteInterrogation";
            checkBoxEnableSpriteInterrogation.Size = new System.Drawing.Size(175, 19);
            checkBoxEnableSpriteInterrogation.TabIndex = 3;
            checkBoxEnableSpriteInterrogation.Text = "Enable sprites interrogation?";
            checkBoxEnableSpriteInterrogation.UseVisualStyleBackColor = true;
            // 
            // checkBoxHighlightAllSprites
            // 
            checkBoxHighlightAllSprites.AutoSize = true;
            checkBoxHighlightAllSprites.Location = new System.Drawing.Point(13, 38);
            checkBoxHighlightAllSprites.Name = "checkBoxHighlightAllSprites";
            checkBoxHighlightAllSprites.Size = new System.Drawing.Size(133, 19);
            checkBoxHighlightAllSprites.TabIndex = 2;
            checkBoxHighlightAllSprites.Text = "Highlight all sprites?";
            checkBoxHighlightAllSprites.UseVisualStyleBackColor = true;
            // 
            // checkBoxHighlightNaturalBounds
            // 
            checkBoxHighlightNaturalBounds.AutoSize = true;
            checkBoxHighlightNaturalBounds.Location = new System.Drawing.Point(13, 13);
            checkBoxHighlightNaturalBounds.Name = "checkBoxHighlightNaturalBounds";
            checkBoxHighlightNaturalBounds.Size = new System.Drawing.Size(164, 19);
            checkBoxHighlightNaturalBounds.TabIndex = 1;
            checkBoxHighlightNaturalBounds.Text = "Highlight natural bounds?";
            checkBoxHighlightNaturalBounds.UseVisualStyleBackColor = true;
            // 
            // tabPageAdvanced
            // 
            tabPageAdvanced.Controls.Add(checkBoxPreCacheAllAssets);
            tabPageAdvanced.Location = new System.Drawing.Point(4, 24);
            tabPageAdvanced.Name = "tabPageAdvanced";
            tabPageAdvanced.Padding = new System.Windows.Forms.Padding(3);
            tabPageAdvanced.Size = new System.Drawing.Size(542, 304);
            tabPageAdvanced.TabIndex = 3;
            tabPageAdvanced.Text = "Advanced";
            tabPageAdvanced.UseVisualStyleBackColor = true;
            // 
            // checkBoxPreCacheAllAssets
            // 
            checkBoxPreCacheAllAssets.AutoSize = true;
            checkBoxPreCacheAllAssets.Location = new System.Drawing.Point(6, 20);
            checkBoxPreCacheAllAssets.Name = "checkBoxPreCacheAllAssets";
            checkBoxPreCacheAllAssets.Size = new System.Drawing.Size(133, 19);
            checkBoxPreCacheAllAssets.TabIndex = 1;
            checkBoxPreCacheAllAssets.Text = "Pre-cache all assets?";
            checkBoxPreCacheAllAssets.UseVisualStyleBackColor = true;
            // 
            // tabPageAudio
            // 
            tabPageAudio.Controls.Add(checkBoxPlayMusic);
            tabPageAudio.Location = new System.Drawing.Point(4, 24);
            tabPageAudio.Name = "tabPageAudio";
            tabPageAudio.Padding = new System.Windows.Forms.Padding(3);
            tabPageAudio.Size = new System.Drawing.Size(542, 304);
            tabPageAudio.TabIndex = 4;
            tabPageAudio.Text = "Audio";
            tabPageAudio.UseVisualStyleBackColor = true;
            // 
            // checkBoxPlayMusic
            // 
            checkBoxPlayMusic.AutoSize = true;
            checkBoxPlayMusic.Location = new System.Drawing.Point(6, 16);
            checkBoxPlayMusic.Name = "checkBoxPlayMusic";
            checkBoxPlayMusic.Size = new System.Drawing.Size(88, 19);
            checkBoxPlayMusic.TabIndex = 0;
            checkBoxPlayMusic.Text = "Play music?";
            checkBoxPlayMusic.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new System.Drawing.Point(402, 350);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(75, 23);
            buttonCancel.TabIndex = 18;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // buttonSave
            // 
            buttonSave.Location = new System.Drawing.Point(483, 350);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new System.Drawing.Size(75, 23);
            buttonSave.TabIndex = 19;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += ButtonSave_Click;
            // 
            // FormSettings
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(585, 385);
            Controls.Add(buttonSave);
            Controls.Add(buttonCancel);
            Controls.Add(tabControl1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormSettings";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Strikeforce Infinite : Settings";
            Load += FormSettings_Load;
            tabControl1.ResumeLayout(false);
            tabPageDisplay.ResumeLayout(false);
            tabPageDisplay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarResolution).EndInit();
            tabPageDisplayAdvanced.ResumeLayout(false);
            tabPageDisplayAdvanced.PerformLayout();
            tabPageDebug.ResumeLayout(false);
            tabPageDebug.PerformLayout();
            tabPageAdvanced.ResumeLayout(false);
            tabPageAdvanced.PerformLayout();
            tabPageAudio.ResumeLayout(false);
            tabPageAudio.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageDisplayAdvanced;
        private System.Windows.Forms.TabPage tabPageDisplay;
        private System.Windows.Forms.CheckBox checkBoxAutoZoomWhenMoving;
        private System.Windows.Forms.TrackBar trackBarResolution;
        private System.Windows.Forms.Label labelResolutionLabel;
        private System.Windows.Forms.Label labelResolution;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelOverdrawScale;
        private System.Windows.Forms.TextBox textBoxOverdrawScale;
        private System.Windows.Forms.TextBox textBoxTargetFrameRate;
        private System.Windows.Forms.Label labelInitialStarCount;
        private System.Windows.Forms.Label labelFrameTargetStarCount;
        private System.Windows.Forms.TextBox textBoxDeltaFrameTargetStarCount;
        private System.Windows.Forms.TextBox textBoxInitialFrameStarCount;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TabPage tabPageDebug;
        private System.Windows.Forms.CheckBox checkBoxEnableSpriteInterrogation;
        private System.Windows.Forms.CheckBox checkBoxHighlightAllSprites;
        private System.Windows.Forms.CheckBox checkBoxHighlightNaturalBounds;
        private System.Windows.Forms.TabPage tabPageAdvanced;
        private System.Windows.Forms.CheckBox checkBoxPreCacheAllAssets;
        private System.Windows.Forms.Label labelGraphicsAdapter;
        private System.Windows.Forms.ComboBox comboBoxGraphicsAdapter;
        private System.Windows.Forms.CheckBox checkBoxEnableAntiAliasing;
        private System.Windows.Forms.TabPage tabPageAudio;
        private System.Windows.Forms.CheckBox checkBoxPlayMusic;
        private System.Windows.Forms.CheckBox checkBoxFineTuneFrameRate;
        private System.Windows.Forms.CheckBox checkBoxEnableVerticalSync;
    }
}