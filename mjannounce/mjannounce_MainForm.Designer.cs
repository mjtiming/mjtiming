/*
 * Created by SharpDevelop.
 * User: Murray
 * Date: 3/4/2014
 * Time: 5:14 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace RaceBeam
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.scoresTextBox = new System.Windows.Forms.TextBox();
			this.scoringList = new System.Windows.Forms.CheckedListBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.bestRunRadioButton = new System.Windows.Forms.RadioButton();
			this.day1Plusset2RadioButton = new System.Windows.Forms.RadioButton();
			this.set2RadioButton = new System.Windows.Forms.RadioButton();
			this.set1RadioButton = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.OfficialRunsTextBox = new System.Windows.Forms.TextBox();
			this.webserverButton = new System.Windows.Forms.Button();
			this.Day1TextBox = new System.Windows.Forms.TextBox();
			this.Day2TextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.DataFolderTextBox = new System.Windows.Forms.TextBox();
			this.queryCountBox = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// scoresTextBox
			// 
			this.scoresTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.scoresTextBox.Font = new System.Drawing.Font("Lucida Console", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.scoresTextBox.Location = new System.Drawing.Point(11, 145);
			this.scoresTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.scoresTextBox.Multiline = true;
			this.scoresTextBox.Name = "scoresTextBox";
			this.scoresTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.scoresTextBox.Size = new System.Drawing.Size(760, 321);
			this.scoresTextBox.TabIndex = 0;
			this.scoresTextBox.WordWrap = false;
			// 
			// scoringList
			// 
			this.scoringList.BackColor = System.Drawing.SystemColors.Control;
			this.scoringList.CheckOnClick = true;
			this.scoringList.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.scoringList.FormattingEnabled = true;
			this.scoringList.Items.AddRange(new object[] {
			"Run Times",
			"Raw Times",
			"PAX Times",
			"Class Times",
			"XGroup Times",
			"Cone Counts",
			"Teams"});
			this.scoringList.Location = new System.Drawing.Point(11, 12);
			this.scoringList.Name = "scoringList";
			this.scoringList.Size = new System.Drawing.Size(129, 132);
			this.scoringList.TabIndex = 9;
			this.scoringList.SelectedIndexChanged += new System.EventHandler(this.ScoringListSelectedIndexChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.bestRunRadioButton);
			this.groupBox1.Controls.Add(this.day1Plusset2RadioButton);
			this.groupBox1.Controls.Add(this.set2RadioButton);
			this.groupBox1.Controls.Add(this.set1RadioButton);
			this.groupBox1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox1.Location = new System.Drawing.Point(162, 11);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.groupBox1.Size = new System.Drawing.Size(136, 117);
			this.groupBox1.TabIndex = 27;
			this.groupBox1.TabStop = false;
			// 
			// bestRunRadioButton
			// 
			this.bestRunRadioButton.Location = new System.Drawing.Point(5, 68);
			this.bestRunRadioButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.bestRunRadioButton.Name = "bestRunRadioButton";
			this.bestRunRadioButton.Size = new System.Drawing.Size(127, 20);
			this.bestRunRadioButton.TabIndex = 3;
			this.bestRunRadioButton.TabStop = true;
			this.bestRunRadioButton.Text = "Best single run";
			this.bestRunRadioButton.UseVisualStyleBackColor = true;
			this.bestRunRadioButton.CheckedChanged += new System.EventHandler(this.BestRunRadioButtonCheckedChanged);
			// 
			// day1Plusset2RadioButton
			// 
			this.day1Plusset2RadioButton.Location = new System.Drawing.Point(5, 50);
			this.day1Plusset2RadioButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.day1Plusset2RadioButton.Name = "day1Plusset2RadioButton";
			this.day1Plusset2RadioButton.Size = new System.Drawing.Size(127, 20);
			this.day1Plusset2RadioButton.TabIndex = 2;
			this.day1Plusset2RadioButton.TabStop = true;
			this.day1Plusset2RadioButton.Text = "Set1+Set2";
			this.day1Plusset2RadioButton.UseVisualStyleBackColor = true;
			this.day1Plusset2RadioButton.CheckedChanged += new System.EventHandler(this.Day1Plusset2RadioButtonCheckedChanged);
			// 
			// set2RadioButton
			// 
			this.set2RadioButton.Location = new System.Drawing.Point(5, 32);
			this.set2RadioButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.set2RadioButton.Name = "set2RadioButton";
			this.set2RadioButton.Size = new System.Drawing.Size(127, 20);
			this.set2RadioButton.TabIndex = 1;
			this.set2RadioButton.TabStop = true;
			this.set2RadioButton.Text = "Set2 only";
			this.set2RadioButton.UseVisualStyleBackColor = true;
			this.set2RadioButton.CheckedChanged += new System.EventHandler(this.Set2RadioButtonCheckedChanged);
			// 
			// set1RadioButton
			// 
			this.set1RadioButton.Location = new System.Drawing.Point(4, 16);
			this.set1RadioButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.set1RadioButton.Name = "set1RadioButton";
			this.set1RadioButton.Size = new System.Drawing.Size(127, 20);
			this.set1RadioButton.TabIndex = 0;
			this.set1RadioButton.TabStop = true;
			this.set1RadioButton.Text = "Set1 only";
			this.set1RadioButton.UseVisualStyleBackColor = true;
			this.set1RadioButton.CheckedChanged += new System.EventHandler(this.Set1RadioButtonCheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.OfficialRunsTextBox);
			this.groupBox2.Location = new System.Drawing.Point(321, 10);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.groupBox2.Size = new System.Drawing.Size(118, 70);
			this.groupBox2.TabIndex = 30;
			this.groupBox2.TabStop = false;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(5, 14);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(106, 18);
			this.label3.TabIndex = 28;
			this.label3.Text = "# official runs";
			// 
			// OfficialRunsTextBox
			// 
			this.OfficialRunsTextBox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.OfficialRunsTextBox.Location = new System.Drawing.Point(45, 38);
			this.OfficialRunsTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.OfficialRunsTextBox.Name = "OfficialRunsTextBox";
			this.OfficialRunsTextBox.Size = new System.Drawing.Size(40, 21);
			this.OfficialRunsTextBox.TabIndex = 27;
			this.OfficialRunsTextBox.Text = "99";
			// 
			// webserverButton
			// 
			this.webserverButton.BackColor = System.Drawing.Color.Lime;
			this.webserverButton.ForeColor = System.Drawing.Color.Black;
			this.webserverButton.Location = new System.Drawing.Point(321, 90);
			this.webserverButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.webserverButton.Name = "webserverButton";
			this.webserverButton.Size = new System.Drawing.Size(118, 37);
			this.webserverButton.TabIndex = 31;
			this.webserverButton.Text = "Start web server";
			this.webserverButton.UseVisualStyleBackColor = false;
			this.webserverButton.Click += new System.EventHandler(this.WebserverButtonClick);
			// 
			// Day1TextBox
			// 
			this.Day1TextBox.Location = new System.Drawing.Point(521, 24);
			this.Day1TextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Day1TextBox.Name = "Day1TextBox";
			this.Day1TextBox.Size = new System.Drawing.Size(123, 20);
			this.Day1TextBox.TabIndex = 33;
			// 
			// Day2TextBox
			// 
			this.Day2TextBox.Location = new System.Drawing.Point(521, 50);
			this.Day2TextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Day2TextBox.Name = "Day2TextBox";
			this.Day2TextBox.Size = new System.Drawing.Size(123, 20);
			this.Day2TextBox.TabIndex = 34;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(462, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 19);
			this.label1.TabIndex = 35;
			this.label1.Text = "Day 1:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(462, 54);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(54, 19);
			this.label2.TabIndex = 36;
			this.label2.Text = "Day 2:";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(462, 82);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(113, 27);
			this.label4.TabIndex = 37;
			this.label4.Text = "Event data folder";
			// 
			// DataFolderTextBox
			// 
			this.DataFolderTextBox.Location = new System.Drawing.Point(462, 104);
			this.DataFolderTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.DataFolderTextBox.Name = "DataFolderTextBox";
			this.DataFolderTextBox.Size = new System.Drawing.Size(259, 20);
			this.DataFolderTextBox.TabIndex = 38;
			// 
			// queryCountBox
			// 
			this.queryCountBox.Location = new System.Drawing.Point(679, 45);
			this.queryCountBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.queryCountBox.Name = "queryCountBox";
			this.queryCountBox.Size = new System.Drawing.Size(74, 20);
			this.queryCountBox.TabIndex = 39;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(679, 20);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(74, 19);
			this.label5.TabIndex = 40;
			this.label5.Text = "Queries";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(780, 475);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.queryCountBox);
			this.Controls.Add(this.DataFolderTextBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.Day2TextBox);
			this.Controls.Add(this.Day1TextBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.webserverButton);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.scoringList);
			this.Controls.Add(this.scoresTextBox);
			this.ForeColor = System.Drawing.SystemColors.WindowText;
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "MainForm";
			this.Text = "mjannounce";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.TextBox queryCountBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox DataFolderTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox Day2TextBox;
		private System.Windows.Forms.TextBox Day1TextBox;
		private System.Windows.Forms.Button webserverButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox OfficialRunsTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton set1RadioButton;
		private System.Windows.Forms.RadioButton set2RadioButton;
		private System.Windows.Forms.RadioButton day1Plusset2RadioButton;
		private System.Windows.Forms.RadioButton bestRunRadioButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckedListBox scoringList;
		private System.Windows.Forms.TextBox scoresTextBox;
	}
}
