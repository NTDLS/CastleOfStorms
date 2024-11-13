namespace Cos.Client
{
    partial class FormStartup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormStartup));
            buttonSettings = new System.Windows.Forms.Button();
            buttonStart = new System.Windows.Forms.Button();
            buttonExit = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // buttonSettings
            // 
            buttonSettings.Location = new System.Drawing.Point(288, 391);
            buttonSettings.Name = "buttonSettings";
            buttonSettings.Size = new System.Drawing.Size(75, 23);
            buttonSettings.TabIndex = 2;
            buttonSettings.Text = "Settings";
            buttonSettings.UseVisualStyleBackColor = true;
            buttonSettings.Click += ButtonSettings_Click;
            // 
            // buttonStart
            // 
            buttonStart.Location = new System.Drawing.Point(411, 391);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new System.Drawing.Size(75, 23);
            buttonStart.TabIndex = 0;
            buttonStart.Text = "Start";
            buttonStart.UseVisualStyleBackColor = true;
            buttonStart.Click += ButtonStart_Click;
            // 
            // buttonExit
            // 
            buttonExit.Location = new System.Drawing.Point(165, 391);
            buttonExit.Name = "buttonExit";
            buttonExit.Size = new System.Drawing.Size(75, 23);
            buttonExit.TabIndex = 1;
            buttonExit.Text = "Exit";
            buttonExit.UseVisualStyleBackColor = true;
            buttonExit.Click += ButtonExit_Click;
            // 
            // FormStartup
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackgroundImage = (System.Drawing.Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new System.Drawing.Size(650, 448);
            ControlBox = false;
            Controls.Add(buttonSettings);
            Controls.Add(buttonStart);
            Controls.Add(buttonExit);
            ForeColor = System.Drawing.SystemColors.ControlText;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "FormStartup";
            Opacity = 0.9D;
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Strikeforce Infinite";
            Load += FormStartup_Load;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button buttonSettings;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonExit;
    }
}