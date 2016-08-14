namespace SimplyHost
{
    partial class ParameterEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.NameLabel = new System.Windows.Forms.Label();
            this.ValueSlider = new System.Windows.Forms.TrackBar();
            this.ValueLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ValueSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(3, 7);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(35, 13);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "label1";
            // 
            // ValueSlider
            // 
            this.ValueSlider.Location = new System.Drawing.Point(55, 3);
            this.ValueSlider.Maximum = 100;
            this.ValueSlider.Name = "ValueSlider";
            this.ValueSlider.Size = new System.Drawing.Size(129, 45);
            this.ValueSlider.TabIndex = 1;
            this.ValueSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // ValueLabel
            // 
            this.ValueLabel.AutoSize = true;
            this.ValueLabel.Location = new System.Drawing.Point(190, 7);
            this.ValueLabel.Name = "ValueLabel";
            this.ValueLabel.Size = new System.Drawing.Size(35, 13);
            this.ValueLabel.TabIndex = 2;
            this.ValueLabel.Text = "label2";
            // 
            // ParameterEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ValueLabel);
            this.Controls.Add(this.ValueSlider);
            this.Controls.Add(this.NameLabel);
            this.Name = "ParameterEditor";
            this.Size = new System.Drawing.Size(279, 30);
            ((System.ComponentModel.ISupportInitialize)(this.ValueSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.TrackBar ValueSlider;
        private System.Windows.Forms.Label ValueLabel;
    }
}
