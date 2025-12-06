namespace ShowJson
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtJsonInput = new TextBox();
            btnLoadJson = new Button();
            SuspendLayout();
            // 
            // txtJsonInput
            // 
            txtJsonInput.Location = new Point(12, 12);
            txtJsonInput.Multiline = true;
            txtJsonInput.Name = "txtJsonInput";
            txtJsonInput.Size = new Size(475, 53);
            txtJsonInput.TabIndex = 0;
            // 
            // btnLoadJson
            // 
            btnLoadJson.Location = new Point(560, 12);
            btnLoadJson.Name = "btnLoadJson";
            btnLoadJson.Size = new Size(108, 53);
            btnLoadJson.TabIndex = 1;
            btnLoadJson.Text = "Load JSON";
            btnLoadJson.UseVisualStyleBackColor = true;
            btnLoadJson.Click += btnLoadJson_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1184, 561);
            Controls.Add(btnLoadJson);
            Controls.Add(txtJsonInput);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtJsonInput;
        private Button btnLoadJson;
    }
}
