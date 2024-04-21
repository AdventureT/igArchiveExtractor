namespace IGAE_GUI.IGZ
{
	partial class IGZ_GeneralForm
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
			this.components = new System.ComponentModel.Container();
			this.treeItems = new System.Windows.Forms.TreeView();
			this.pbTexturePreview = new System.Windows.Forms.PictureBox();
			this.cbFilterImages = new System.Windows.Forms.CheckBox();
			this.btnSaveAllObjects = new System.Windows.Forms.Button();
			this.btnTextureExtract = new System.Windows.Forms.Button();
			this.btnTextureReplace = new System.Windows.Forms.Button();
            this.btnTextureReplaceDDS = new System.Windows.Forms.Button();
            this.btnSaveIGZ = new System.Windows.Forms.Button();
			this.SuspendLayout();
			//
			// treeItems
			//
			this.treeItems.Location = new System.Drawing.Point(12, 12);
			this.treeItems.Name = "treeItems";
			this.treeItems.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericMonospace.Name, 8);
			this.treeItems.Size = new System.Drawing.Size(476, 476);
			this.treeItems.TabIndex = 1;
			this.treeItems.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.SelectionChange);
			//
			// pbTexturePreview
			//
			this.pbTexturePreview.Location = new System.Drawing.Point(494, 253);
			this.pbTexturePreview.Name = "pbTexturePreview";
			this.pbTexturePreview.Anchor = System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom;
			this.pbTexturePreview.Size = new System.Drawing.Size(200, 200);
			this.pbTexturePreview.TabIndex = 0;
			this.pbTexturePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pbTexturePreview.BackColor = System.Drawing.Color.Black;
			this.pbTexturePreview.ClientSize = new System.Drawing.Size(200, 200);
			//
			// cbFilterImages
			//
			this.cbFilterImages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cbFilterImages.Location = new System.Drawing.Point(500, 12);
			this.cbFilterImages.Name = "cbFilterImages";
			this.cbFilterImages.Checked = true;
			this.cbFilterImages.Text = "Only igImage2s?";
			this.cbFilterImages.Size = new System.Drawing.Size(100, 23);
			this.cbFilterImages.TabIndex = 10;
			this.cbFilterImages.CheckedChanged += new System.EventHandler(ChangeFilter);
			// 
			// btnSaveAllObjects
			// 
			this.btnSaveAllObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSaveAllObjects.Location = new System.Drawing.Point(500, 38);
			this.btnTextureReplace.Name = "btnSaveAllObjects";
			this.btnSaveAllObjects.Size = new System.Drawing.Size(75, 23);
			this.btnSaveAllObjects.TabIndex = 2;
			this.btnSaveAllObjects.Text = "Save All";
			this.btnSaveAllObjects.UseVisualStyleBackColor = true;
			this.btnSaveAllObjects.Click += new System.EventHandler(this.ExportAllObjects);
			// 
			// btnTextureExtract
			// 
			this.btnTextureExtract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnTextureExtract.Location = new System.Drawing.Point(526, 465);
			this.btnTextureExtract.Name = "btnTextureExtract";
			this.btnTextureExtract.Size = new System.Drawing.Size(75, 23);
			this.btnTextureExtract.TabIndex = 3;
			this.btnTextureExtract.Text = "Extract";
			this.btnTextureExtract.UseVisualStyleBackColor = true;
			this.btnTextureExtract.Click += new System.EventHandler(this.ExportTexture);
			// 
			// btnTextureReplace
			// 
			this.btnTextureReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnTextureReplace.Location = new System.Drawing.Point(613, 465);
			this.btnTextureReplace.Name = "btnTextureReplace";
			this.btnTextureReplace.Size = new System.Drawing.Size(75, 23);
			this.btnTextureReplace.TabIndex = 4;
			this.btnTextureReplace.Text = "Replace";
			this.btnTextureReplace.UseVisualStyleBackColor = true;
			this.btnTextureReplace.Click += new System.EventHandler(this.ImportTexture);
            // 
            // btnTextureReplaceDDS
            // 
            this.btnTextureReplaceDDS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTextureReplaceDDS.Location = new System.Drawing.Point(613, 38);
            this.btnTextureReplaceDDS.Name = "btnTextureReplaceDDS";
            this.btnTextureReplaceDDS.Size = new System.Drawing.Size(75, 23);
            this.btnTextureReplaceDDS.TabIndex = 4;
            this.btnTextureReplaceDDS.Text = "Replace Whole DDS";
            this.btnTextureReplaceDDS.UseVisualStyleBackColor = true;
            this.btnTextureReplaceDDS.Click += new System.EventHandler(this.ImportDDSTexture);
            // 
            // btnSaveIGZ
            // 
            this.btnSaveIGZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSaveIGZ.Location = new System.Drawing.Point(613, 12);
			this.btnSaveIGZ.Name = "btnSaveIGZ";
			this.btnSaveIGZ.Size = new System.Drawing.Size(75, 23);
			this.btnSaveIGZ.TabIndex = 2;
			this.btnSaveIGZ.Text = "Save IGZ";
			this.btnSaveIGZ.UseVisualStyleBackColor = true;
			this.btnSaveIGZ.Click += new System.EventHandler(this.Save);
			// 
			// IGZ_GeneralForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(700, 500);
			this.Controls.Add(this.treeItems);
			this.Controls.Add(this.pbTexturePreview);
			this.Controls.Add(this.cbFilterImages);
			this.Controls.Add(this.btnSaveAllObjects);
			this.Controls.Add(this.btnTextureExtract);
			this.Controls.Add(this.btnTextureReplace);
            this.Controls.Add(this.btnTextureReplaceDDS);
            this.Controls.Add(this.btnSaveIGZ);
			this.Name = "Form_igzGeneral";
			this.Text = "IGZ Viewer";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Closing);
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion

		public System.Windows.Forms.TreeView treeItems;
		public System.Windows.Forms.PictureBox pbTexturePreview;
		public System.Windows.Forms.CheckBox cbFilterImages;
		public System.Windows.Forms.Button btnSaveAllObjects;
		public System.Windows.Forms.Button btnTextureExtract;
		public System.Windows.Forms.Button btnTextureReplace;
        public System.Windows.Forms.Button btnTextureReplaceDDS;
        public System.Windows.Forms.Button btnSaveIGZ;
	}
}