namespace XamlToResw
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.edDst = new System.Windows.Forms.TextBox();
            this.btnExtract = new System.Windows.Forms.Button();
            this.rbPromptAll = new System.Windows.Forms.RadioButton();
            this.rbReplaceAll = new System.Windows.Forms.RadioButton();
            this.rbSkipAll = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // edDst
            // 
            this.edDst.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edDst.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.edDst.Location = new System.Drawing.Point(13, 12);
            this.edDst.Multiline = true;
            this.edDst.Name = "edDst";
            this.edDst.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.edDst.Size = new System.Drawing.Size(586, 380);
            this.edDst.TabIndex = 0;
            this.edDst.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edDst_KeyDown);
            // 
            // btnExtract
            // 
            this.btnExtract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExtract.Location = new System.Drawing.Point(524, 405);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(75, 23);
            this.btnExtract.TabIndex = 2;
            this.btnExtract.Text = "Extract";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // rbPromptAll
            // 
            this.rbPromptAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbPromptAll.AutoSize = true;
            this.rbPromptAll.Checked = true;
            this.rbPromptAll.Location = new System.Drawing.Point(12, 408);
            this.rbPromptAll.Name = "rbPromptAll";
            this.rbPromptAll.Size = new System.Drawing.Size(83, 16);
            this.rbPromptAll.TabIndex = 3;
            this.rbPromptAll.TabStop = true;
            this.rbPromptAll.Text = "Prompt All";
            this.rbPromptAll.UseVisualStyleBackColor = true;
            // 
            // rbReplaceAll
            // 
            this.rbReplaceAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbReplaceAll.AutoSize = true;
            this.rbReplaceAll.Location = new System.Drawing.Point(104, 408);
            this.rbReplaceAll.Name = "rbReplaceAll";
            this.rbReplaceAll.Size = new System.Drawing.Size(131, 16);
            this.rbReplaceAll.TabIndex = 4;
            this.rbReplaceAll.Text = "Replace Exists All";
            this.rbReplaceAll.UseVisualStyleBackColor = true;
            // 
            // rbSkipAll
            // 
            this.rbSkipAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbSkipAll.AutoSize = true;
            this.rbSkipAll.Location = new System.Drawing.Point(244, 408);
            this.rbSkipAll.Name = "rbSkipAll";
            this.rbSkipAll.Size = new System.Drawing.Size(113, 16);
            this.rbSkipAll.TabIndex = 5;
            this.rbSkipAll.Text = "Skip Exists All";
            this.rbSkipAll.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 436);
            this.Controls.Add(this.rbSkipAll);
            this.Controls.Add(this.rbReplaceAll);
            this.Controls.Add(this.rbPromptAll);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.edDst);
            this.Name = "MainForm";
            this.Text = "Strings from XAML to Resw";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox edDst;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.RadioButton rbPromptAll;
        private System.Windows.Forms.RadioButton rbReplaceAll;
        private System.Windows.Forms.RadioButton rbSkipAll;
    }
}

