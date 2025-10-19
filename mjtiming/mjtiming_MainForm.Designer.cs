/*
 * Created by SharpDevelop.
 * User: murray
 * Date: 8/19/2008
 * Time: 7:11 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 using System;
using System.Windows.Forms;
using timingGrid;

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
        	System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
        	this.configurationTabPage = new System.Windows.Forms.TabPage();
        	this.configurationDataGridView = new System.Windows.Forms.DataGridView();
        	this.registrationTabPage = new System.Windows.Forms.TabPage();
        	this.concat_checkbox = new System.Windows.Forms.CheckBox();
        	this.DeleteDriversButton = new System.Windows.Forms.Button();
        	this.FindButton = new System.Windows.Forms.Button();
        	this.findText = new System.Windows.Forms.TextBox();
        	this.MSRImportButton = new System.Windows.Forms.Button();
        	this.addDriverButton = new System.Windows.Forms.Button();
        	this.clearNotesButton = new System.Windows.Forms.Button();
        	this.unregButton = new System.Windows.Forms.Button();
        	this.drivers = new System.Windows.Forms.DataGridView();
        	this.COMPortsTabPage = new System.Windows.Forms.TabPage();
        	this.COMPortsTextBox = new System.Windows.Forms.RichTextBox();
        	this.HeatsTabPage = new System.Windows.Forms.TabPage();
        	this.HeatsTextBox = new System.Windows.Forms.RichTextBox();
        	this.logTabPage = new System.Windows.Forms.TabPage();
        	this.msgTextBox = new System.Windows.Forms.RichTextBox();
        	this.scoringTabPage = new System.Windows.Forms.TabPage();
        	this.groupBox2 = new System.Windows.Forms.GroupBox();
        	this.label3 = new System.Windows.Forms.Label();
        	this.OfficialRunsTextBox = new System.Windows.Forms.TextBox();
        	this.groupBox1 = new System.Windows.Forms.GroupBox();
        	this.bestRunRadioButton = new System.Windows.Forms.RadioButton();
        	this.set1PlusSet2RadioButton = new System.Windows.Forms.RadioButton();
        	this.set2RadioButton = new System.Windows.Forms.RadioButton();
        	this.set1RadioButton = new System.Windows.Forms.RadioButton();
        	this.scoreModifiers = new System.Windows.Forms.CheckedListBox();
        	this.titleTextBox = new System.Windows.Forms.TextBox();
        	this.Day2TextBox = new System.Windows.Forms.TextBox();
        	this.Day1TextBox = new System.Windows.Forms.TextBox();
        	this.scoresTextBox = new System.Windows.Forms.RichTextBox();
        	this.label4 = new System.Windows.Forms.Label();
        	this.ScoreButton = new System.Windows.Forms.Button();
        	this.label2 = new System.Windows.Forms.Label();
        	this.label1 = new System.Windows.Forms.Label();
        	this.scoringList = new System.Windows.Forms.CheckedListBox();
        	this.timingTabPage = new System.Windows.Forms.TabPage();
        	this.SetComboBox = new System.Windows.Forms.ComboBox();
        	this.stopAlign = new System.Windows.Forms.Label();
        	this.startAlign = new System.Windows.Forms.Label();
        	this.stopRadio = new System.Windows.Forms.Label();
        	this.startRadio = new System.Windows.Forms.Label();
            this.timergrid = new TimingGrid();
            this.TimerPortButton = new System.Windows.Forms.Button();
        	this.triggerStopButton = new System.Windows.Forms.Button();
        	this.stopResetButton = new System.Windows.Forms.Button();
        	this.triggerStartButton = new System.Windows.Forms.Button();
        	this.startResetButton = new System.Windows.Forms.Button();
        	this.tabControl1 = new System.Windows.Forms.TabControl();
        	this.classesTabPage = new System.Windows.Forms.TabPage();
        	this.addClassButton = new System.Windows.Forms.Button();
        	this.classDataGridView = new System.Windows.Forms.DataGridView();
        	this.configurationTabPage.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.configurationDataGridView)).BeginInit();
        	this.registrationTabPage.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.drivers)).BeginInit();
        	this.COMPortsTabPage.SuspendLayout();
        	this.HeatsTabPage.SuspendLayout();
        	this.logTabPage.SuspendLayout();
        	this.scoringTabPage.SuspendLayout();
        	this.groupBox2.SuspendLayout();
        	this.groupBox1.SuspendLayout();
        	this.timingTabPage.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.timergrid)).BeginInit();
        	this.tabControl1.SuspendLayout();
        	this.classesTabPage.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.classDataGridView)).BeginInit();
        	this.SuspendLayout();
        	// 
        	// configurationTabPage
        	// 
        	this.configurationTabPage.Controls.Add(this.configurationDataGridView);
        	this.configurationTabPage.Location = new System.Drawing.Point(4, 22);
        	this.configurationTabPage.Margin = new System.Windows.Forms.Padding(2);
        	this.configurationTabPage.Name = "configurationTabPage";
        	this.configurationTabPage.Size = new System.Drawing.Size(724, 408);
        	this.configurationTabPage.TabIndex = 6;
        	this.configurationTabPage.Text = "Configuration";
        	this.configurationTabPage.UseVisualStyleBackColor = true;
        	// 
        	// configurationDataGridView
        	// 
        	this.configurationDataGridView.AllowUserToAddRows = false;
        	this.configurationDataGridView.AllowUserToDeleteRows = false;
        	this.configurationDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
        	this.configurationDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        	this.configurationDataGridView.Location = new System.Drawing.Point(2, 24);
        	this.configurationDataGridView.Margin = new System.Windows.Forms.Padding(2);
        	this.configurationDataGridView.Name = "configurationDataGridView";
        	this.configurationDataGridView.RowTemplate.Height = 24;
        	this.configurationDataGridView.Size = new System.Drawing.Size(722, 383);
        	this.configurationDataGridView.TabIndex = 0;
        	// 
        	// registrationTabPage
        	// 
        	this.registrationTabPage.Controls.Add(this.concat_checkbox);
        	this.registrationTabPage.Controls.Add(this.DeleteDriversButton);
        	this.registrationTabPage.Controls.Add(this.FindButton);
        	this.registrationTabPage.Controls.Add(this.findText);
        	this.registrationTabPage.Controls.Add(this.MSRImportButton);
        	this.registrationTabPage.Controls.Add(this.addDriverButton);
        	this.registrationTabPage.Controls.Add(this.clearNotesButton);
        	this.registrationTabPage.Controls.Add(this.unregButton);
        	this.registrationTabPage.Controls.Add(this.drivers);
        	this.registrationTabPage.Location = new System.Drawing.Point(4, 22);
        	this.registrationTabPage.Margin = new System.Windows.Forms.Padding(2);
        	this.registrationTabPage.Name = "registrationTabPage";
        	this.registrationTabPage.Size = new System.Drawing.Size(724, 408);
        	this.registrationTabPage.TabIndex = 5;
        	this.registrationTabPage.Text = "Registration";
        	this.registrationTabPage.UseVisualStyleBackColor = true;
        	// 
        	// concat_checkbox
        	// 
        	this.concat_checkbox.Location = new System.Drawing.Point(421, 3);
        	this.concat_checkbox.Name = "concat_checkbox";
        	this.concat_checkbox.Size = new System.Drawing.Size(130, 46);
        	this.concat_checkbox.TabIndex = 11;
        	this.concat_checkbox.Text = "Concat car# + class on import";
        	this.concat_checkbox.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
        	this.concat_checkbox.UseVisualStyleBackColor = true;
        	// 
        	// DeleteDriversButton
        	// 
        	this.DeleteDriversButton.Location = new System.Drawing.Point(557, 2);
        	this.DeleteDriversButton.Name = "DeleteDriversButton";
        	this.DeleteDriversButton.Size = new System.Drawing.Size(161, 29);
        	this.DeleteDriversButton.TabIndex = 10;
        	this.DeleteDriversButton.Text = "Delete all drivers";
        	this.DeleteDriversButton.UseVisualStyleBackColor = true;
        	this.DeleteDriversButton.Click += new System.EventHandler(this.DeleteDriversButtonClick);
        	// 
        	// FindButton
        	// 
        	this.FindButton.AllowDrop = true;
        	this.FindButton.Location = new System.Drawing.Point(436, 54);
        	this.FindButton.Margin = new System.Windows.Forms.Padding(2);
        	this.FindButton.Name = "FindButton";
        	this.FindButton.Size = new System.Drawing.Size(88, 19);
        	this.FindButton.TabIndex = 9;
        	this.FindButton.Text = "Find next";
        	this.FindButton.UseMnemonic = false;
        	this.FindButton.UseVisualStyleBackColor = true;
        	this.FindButton.Click += new System.EventHandler(this.FindButtonClick);
        	// 
        	// findText
        	// 
        	this.findText.Location = new System.Drawing.Point(218, 55);
        	this.findText.Margin = new System.Windows.Forms.Padding(2);
        	this.findText.Name = "findText";
        	this.findText.Size = new System.Drawing.Size(214, 20);
        	this.findText.TabIndex = 8;
        	// 
        	// MSRImportButton
        	// 
        	this.MSRImportButton.Location = new System.Drawing.Point(557, 43);
        	this.MSRImportButton.Margin = new System.Windows.Forms.Padding(2);
        	this.MSRImportButton.Name = "MSRImportButton";
        	this.MSRImportButton.Size = new System.Drawing.Size(161, 36);
        	this.MSRImportButton.TabIndex = 7;
        	this.MSRImportButton.Text = "Import from CSV file";
        	this.MSRImportButton.UseVisualStyleBackColor = true;
        	this.MSRImportButton.Click += new System.EventHandler(this.MSRImportButtonClick);
        	// 
        	// addDriverButton
        	// 
        	this.addDriverButton.Location = new System.Drawing.Point(2, 43);
        	this.addDriverButton.Margin = new System.Windows.Forms.Padding(2);
        	this.addDriverButton.Name = "addDriverButton";
        	this.addDriverButton.Size = new System.Drawing.Size(164, 39);
        	this.addDriverButton.TabIndex = 3;
        	this.addDriverButton.Text = "Add driver";
        	this.addDriverButton.UseVisualStyleBackColor = true;
        	this.addDriverButton.Click += new System.EventHandler(this.AddDriverButtonClick);
        	// 
        	// clearNotesButton
        	// 
        	this.clearNotesButton.Location = new System.Drawing.Point(171, 2);
        	this.clearNotesButton.Margin = new System.Windows.Forms.Padding(2);
        	this.clearNotesButton.Name = "clearNotesButton";
        	this.clearNotesButton.Size = new System.Drawing.Size(138, 36);
        	this.clearNotesButton.TabIndex = 2;
        	this.clearNotesButton.Text = "Clear Notes";
        	this.clearNotesButton.UseVisualStyleBackColor = true;
        	this.clearNotesButton.Click += new System.EventHandler(this.ClearNotesButtonClick);
        	// 
        	// unregButton
        	// 
        	this.unregButton.Location = new System.Drawing.Point(0, 2);
        	this.unregButton.Margin = new System.Windows.Forms.Padding(2);
        	this.unregButton.Name = "unregButton";
        	this.unregButton.Size = new System.Drawing.Size(167, 36);
        	this.unregButton.TabIndex = 1;
        	this.unregButton.Text = "Unregister all drivers";
        	this.unregButton.UseVisualStyleBackColor = true;
        	this.unregButton.Click += new System.EventHandler(this.UnregButtonClick);
        	// 
        	// drivers
        	// 
        	this.drivers.AllowUserToAddRows = false;
        	this.drivers.AllowUserToOrderColumns = true;
        	this.drivers.AllowUserToResizeRows = false;
        	this.drivers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
        	this.drivers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        	this.drivers.Location = new System.Drawing.Point(6, 84);
        	this.drivers.Margin = new System.Windows.Forms.Padding(2);
        	this.drivers.Name = "drivers";
        	this.drivers.RowTemplate.Height = 24;
        	this.drivers.Size = new System.Drawing.Size(724, 326);
        	this.drivers.TabIndex = 0;
        	// 
        	// COMPortsTabPage
        	// 
        	this.COMPortsTabPage.Controls.Add(this.COMPortsTextBox);
        	this.COMPortsTabPage.Location = new System.Drawing.Point(4, 22);
        	this.COMPortsTabPage.Margin = new System.Windows.Forms.Padding(2);
        	this.COMPortsTabPage.Name = "COMPortsTabPage";
        	this.COMPortsTabPage.Size = new System.Drawing.Size(724, 408);
        	this.COMPortsTabPage.TabIndex = 4;
        	this.COMPortsTabPage.Text = "COM Ports";
        	this.COMPortsTabPage.UseVisualStyleBackColor = true;
        	// 
        	// COMPortsTextBox
        	// 
        	this.COMPortsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
        	this.COMPortsTextBox.Location = new System.Drawing.Point(0, 2);
        	this.COMPortsTextBox.Margin = new System.Windows.Forms.Padding(2);
        	this.COMPortsTextBox.Name = "COMPortsTextBox";
        	this.COMPortsTextBox.Size = new System.Drawing.Size(728, 411);
        	this.COMPortsTextBox.TabIndex = 0;
        	this.COMPortsTextBox.Text = "";
        	// 
        	// HeatsTabPage
        	// 
        	this.HeatsTabPage.Controls.Add(this.HeatsTextBox);
        	this.HeatsTabPage.Location = new System.Drawing.Point(4, 22);
        	this.HeatsTabPage.Margin = new System.Windows.Forms.Padding(2);
        	this.HeatsTabPage.Name = "HeatsTabPage";
        	this.HeatsTabPage.Size = new System.Drawing.Size(724, 408);
        	this.HeatsTabPage.TabIndex = 3;
        	this.HeatsTabPage.Text = "Run Heats";
        	this.HeatsTabPage.UseVisualStyleBackColor = true;
        	// 
        	// HeatsTextBox
        	// 
        	this.HeatsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
        	this.HeatsTextBox.Font = new System.Drawing.Font("Lucida Console", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.HeatsTextBox.Location = new System.Drawing.Point(0, 0);
        	this.HeatsTextBox.Margin = new System.Windows.Forms.Padding(2);
        	this.HeatsTextBox.Name = "HeatsTextBox";
        	this.HeatsTextBox.Size = new System.Drawing.Size(725, 411);
        	this.HeatsTextBox.TabIndex = 0;
        	this.HeatsTextBox.Text = "";
        	// 
        	// logTabPage
        	// 
        	this.logTabPage.Controls.Add(this.msgTextBox);
        	this.logTabPage.Location = new System.Drawing.Point(4, 22);
        	this.logTabPage.Margin = new System.Windows.Forms.Padding(2);
        	this.logTabPage.Name = "logTabPage";
        	this.logTabPage.Padding = new System.Windows.Forms.Padding(2);
        	this.logTabPage.Size = new System.Drawing.Size(724, 408);
        	this.logTabPage.TabIndex = 1;
        	this.logTabPage.Text = "Log messages";
        	this.logTabPage.UseVisualStyleBackColor = true;
        	// 
        	// msgTextBox
        	// 
        	this.msgTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
        	this.msgTextBox.CausesValidation = false;
        	this.msgTextBox.Location = new System.Drawing.Point(5, 6);
        	this.msgTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        	this.msgTextBox.Name = "msgTextBox";
        	this.msgTextBox.Size = new System.Drawing.Size(716, 396);
        	this.msgTextBox.TabIndex = 1;
        	this.msgTextBox.Text = "";
        	// 
        	// scoringTabPage
        	// 
        	this.scoringTabPage.Controls.Add(this.groupBox2);
        	this.scoringTabPage.Controls.Add(this.groupBox1);
        	this.scoringTabPage.Controls.Add(this.scoreModifiers);
        	this.scoringTabPage.Controls.Add(this.titleTextBox);
        	this.scoringTabPage.Controls.Add(this.Day2TextBox);
        	this.scoringTabPage.Controls.Add(this.Day1TextBox);
        	this.scoringTabPage.Controls.Add(this.scoresTextBox);
        	this.scoringTabPage.Controls.Add(this.label4);
        	this.scoringTabPage.Controls.Add(this.ScoreButton);
        	this.scoringTabPage.Controls.Add(this.label2);
        	this.scoringTabPage.Controls.Add(this.label1);
        	this.scoringTabPage.Controls.Add(this.scoringList);
        	this.scoringTabPage.Location = new System.Drawing.Point(4, 22);
        	this.scoringTabPage.Margin = new System.Windows.Forms.Padding(2);
        	this.scoringTabPage.Name = "scoringTabPage";
        	this.scoringTabPage.Size = new System.Drawing.Size(724, 408);
        	this.scoringTabPage.TabIndex = 2;
        	this.scoringTabPage.Text = "Scoring";
        	this.scoringTabPage.UseVisualStyleBackColor = true;
        	// 
        	// groupBox2
        	// 
        	this.groupBox2.Controls.Add(this.label3);
        	this.groupBox2.Controls.Add(this.OfficialRunsTextBox);
        	this.groupBox2.Location = new System.Drawing.Point(284, 98);
        	this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
        	this.groupBox2.Name = "groupBox2";
        	this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
        	this.groupBox2.Size = new System.Drawing.Size(166, 43);
        	this.groupBox2.TabIndex = 29;
        	this.groupBox2.TabStop = false;
        	// 
        	// label3
        	// 
        	this.label3.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.label3.Location = new System.Drawing.Point(5, 15);
        	this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
        	this.label3.Name = "label3";
        	this.label3.Size = new System.Drawing.Size(98, 18);
        	this.label3.TabIndex = 28;
        	this.label3.Text = "# official runs:";
        	// 
        	// OfficialRunsTextBox
        	// 
        	this.OfficialRunsTextBox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.OfficialRunsTextBox.Location = new System.Drawing.Point(109, 13);
        	this.OfficialRunsTextBox.Margin = new System.Windows.Forms.Padding(2);
        	this.OfficialRunsTextBox.Name = "OfficialRunsTextBox";
        	this.OfficialRunsTextBox.Size = new System.Drawing.Size(40, 21);
        	this.OfficialRunsTextBox.TabIndex = 27;
        	this.OfficialRunsTextBox.Text = "99";
        	// 
        	// groupBox1
        	// 
        	this.groupBox1.Controls.Add(this.bestRunRadioButton);
        	this.groupBox1.Controls.Add(this.set1PlusSet2RadioButton);
        	this.groupBox1.Controls.Add(this.set2RadioButton);
        	this.groupBox1.Controls.Add(this.set1RadioButton);
        	this.groupBox1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.groupBox1.Location = new System.Drawing.Point(142, 48);
        	this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
        	this.groupBox1.Name = "groupBox1";
        	this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
        	this.groupBox1.Size = new System.Drawing.Size(136, 93);
        	this.groupBox1.TabIndex = 26;
        	this.groupBox1.TabStop = false;
        	// 
        	// bestRunRadioButton
        	// 
        	this.bestRunRadioButton.Location = new System.Drawing.Point(5, 68);
        	this.bestRunRadioButton.Margin = new System.Windows.Forms.Padding(2);
        	this.bestRunRadioButton.Name = "bestRunRadioButton";
        	this.bestRunRadioButton.Size = new System.Drawing.Size(127, 20);
        	this.bestRunRadioButton.TabIndex = 3;
        	this.bestRunRadioButton.TabStop = true;
        	this.bestRunRadioButton.Text = "Best single run";
        	this.bestRunRadioButton.UseVisualStyleBackColor = true;
        	this.bestRunRadioButton.CheckedChanged += new System.EventHandler(this.BestRunRadioButtonCheckedChanged);
        	// 
        	// set1PlusSet2RadioButton
        	// 
        	this.set1PlusSet2RadioButton.Location = new System.Drawing.Point(5, 50);
        	this.set1PlusSet2RadioButton.Margin = new System.Windows.Forms.Padding(2);
        	this.set1PlusSet2RadioButton.Name = "set1PlusSet2RadioButton";
        	this.set1PlusSet2RadioButton.Size = new System.Drawing.Size(127, 20);
        	this.set1PlusSet2RadioButton.TabIndex = 2;
        	this.set1PlusSet2RadioButton.TabStop = true;
        	this.set1PlusSet2RadioButton.Text = "Set1+Set2";
        	this.set1PlusSet2RadioButton.UseVisualStyleBackColor = true;
        	this.set1PlusSet2RadioButton.CheckedChanged += new System.EventHandler(this.Day1PlusDay2RadioButtonCheckedChanged);
        	// 
        	// set2RadioButton
        	// 
        	this.set2RadioButton.Location = new System.Drawing.Point(5, 32);
        	this.set2RadioButton.Margin = new System.Windows.Forms.Padding(2);
        	this.set2RadioButton.Name = "set2RadioButton";
        	this.set2RadioButton.Size = new System.Drawing.Size(127, 20);
        	this.set2RadioButton.TabIndex = 1;
        	this.set2RadioButton.TabStop = true;
        	this.set2RadioButton.Text = "Set2 only";
        	this.set2RadioButton.UseVisualStyleBackColor = true;
        	this.set2RadioButton.CheckedChanged += new System.EventHandler(this.Day2RadioButtonCheckedChanged);
        	// 
        	// set1RadioButton
        	// 
        	this.set1RadioButton.Location = new System.Drawing.Point(4, 16);
        	this.set1RadioButton.Margin = new System.Windows.Forms.Padding(2);
        	this.set1RadioButton.Name = "set1RadioButton";
        	this.set1RadioButton.Size = new System.Drawing.Size(127, 20);
        	this.set1RadioButton.TabIndex = 0;
        	this.set1RadioButton.TabStop = true;
        	this.set1RadioButton.Text = "Set1 only";
        	this.set1RadioButton.UseVisualStyleBackColor = true;
        	this.set1RadioButton.CheckedChanged += new System.EventHandler(this.Set1RadioButtonCheckedChanged);
        	// 
        	// scoreModifiers
        	// 
        	this.scoreModifiers.BackColor = System.Drawing.SystemColors.Control;
        	this.scoreModifiers.CheckOnClick = true;
        	this.scoreModifiers.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.scoreModifiers.FormattingEnabled = true;
        	this.scoreModifiers.Items.AddRange(new object[] {
			"Rookie Times"});
        	this.scoreModifiers.Location = new System.Drawing.Point(142, 7);
        	this.scoreModifiers.Margin = new System.Windows.Forms.Padding(2);
        	this.scoreModifiers.Name = "scoreModifiers";
        	this.scoreModifiers.Size = new System.Drawing.Size(137, 36);
        	this.scoreModifiers.TabIndex = 20;
        	this.scoreModifiers.SelectedIndexChanged += new System.EventHandler(this.ScoreModifiersSelectedIndexChanged);
        	// 
        	// titleTextBox
        	// 
        	this.titleTextBox.Location = new System.Drawing.Point(373, 58);
        	this.titleTextBox.Margin = new System.Windows.Forms.Padding(2);
        	this.titleTextBox.Name = "titleTextBox";
        	this.titleTextBox.Size = new System.Drawing.Size(324, 20);
        	this.titleTextBox.TabIndex = 18;
        	// 
        	// Day2TextBox
        	// 
        	this.Day2TextBox.Location = new System.Drawing.Point(373, 33);
        	this.Day2TextBox.Margin = new System.Windows.Forms.Padding(2);
        	this.Day2TextBox.Name = "Day2TextBox";
        	this.Day2TextBox.Size = new System.Drawing.Size(86, 20);
        	this.Day2TextBox.TabIndex = 13;
        	// 
        	// Day1TextBox
        	// 
        	this.Day1TextBox.Location = new System.Drawing.Point(373, 8);
        	this.Day1TextBox.Margin = new System.Windows.Forms.Padding(2);
        	this.Day1TextBox.Name = "Day1TextBox";
        	this.Day1TextBox.Size = new System.Drawing.Size(86, 20);
        	this.Day1TextBox.TabIndex = 12;
        	// 
        	// scoresTextBox
        	// 
        	this.scoresTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
        	this.scoresTextBox.Font = new System.Drawing.Font("Lucida Console", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.scoresTextBox.Location = new System.Drawing.Point(6, 146);
        	this.scoresTextBox.Margin = new System.Windows.Forms.Padding(2);
        	this.scoresTextBox.Name = "scoresTextBox";
        	this.scoresTextBox.Size = new System.Drawing.Size(716, 265);
        	this.scoresTextBox.TabIndex = 0;
        	this.scoresTextBox.Text = "";
        	this.scoresTextBox.WordWrap = false;
        	// 
        	// label4
        	// 
        	this.label4.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.label4.Location = new System.Drawing.Point(322, 57);
        	this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
        	this.label4.Name = "label4";
        	this.label4.Size = new System.Drawing.Size(46, 19);
        	this.label4.TabIndex = 17;
        	this.label4.Text = "Title:";
        	// 
        	// ScoreButton
        	// 
        	this.ScoreButton.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.ScoreButton.Location = new System.Drawing.Point(472, 80);
        	this.ScoreButton.Margin = new System.Windows.Forms.Padding(2);
        	this.ScoreButton.Name = "ScoreButton";
        	this.ScoreButton.Size = new System.Drawing.Size(206, 22);
        	this.ScoreButton.TabIndex = 16;
        	this.ScoreButton.Text = "Create score files";
        	this.ScoreButton.UseVisualStyleBackColor = true;
        	this.ScoreButton.Click += new System.EventHandler(this.ScoreButtonClick);
        	// 
        	// label2
        	// 
        	this.label2.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.label2.Location = new System.Drawing.Point(308, 33);
        	this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(59, 19);
        	this.label2.TabIndex = 15;
        	this.label2.Text = "Day2:";
        	// 
        	// label1
        	// 
        	this.label1.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.label1.Location = new System.Drawing.Point(308, 7);
        	this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(59, 24);
        	this.label1.TabIndex = 14;
        	this.label1.Text = "Day1:";
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
			"Cone Counts",
			"Teams"});
        	this.scoringList.Location = new System.Drawing.Point(8, 7);
        	this.scoringList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        	this.scoringList.Name = "scoringList";
        	this.scoringList.Size = new System.Drawing.Size(129, 132);
        	this.scoringList.TabIndex = 8;
        	this.scoringList.SelectedIndexChanged += new System.EventHandler(this.ScoringListSelectedIndexChanged);
        	// 
        	// timingTabPage
        	// 
        	this.timingTabPage.Controls.Add(this.SetComboBox);
        	this.timingTabPage.Controls.Add(this.stopAlign);
        	this.timingTabPage.Controls.Add(this.startAlign);
        	this.timingTabPage.Controls.Add(this.stopRadio);
        	this.timingTabPage.Controls.Add(this.startRadio);
        	this.timingTabPage.Controls.Add(this.timergrid);
        	this.timingTabPage.Controls.Add(this.TimerPortButton);
        	this.timingTabPage.Controls.Add(this.triggerStopButton);
        	this.timingTabPage.Controls.Add(this.stopResetButton);
        	this.timingTabPage.Controls.Add(this.triggerStartButton);
        	this.timingTabPage.Controls.Add(this.startResetButton);
        	this.timingTabPage.Location = new System.Drawing.Point(4, 22);
        	this.timingTabPage.Margin = new System.Windows.Forms.Padding(2);
        	this.timingTabPage.Name = "timingTabPage";
        	this.timingTabPage.Padding = new System.Windows.Forms.Padding(2);
        	this.timingTabPage.Size = new System.Drawing.Size(724, 408);
        	this.timingTabPage.TabIndex = 0;
        	this.timingTabPage.Text = "Timing";
        	this.timingTabPage.UseVisualStyleBackColor = true;
        	// 
        	// SetComboBox
        	// 
        	this.SetComboBox.BackColor = System.Drawing.SystemColors.Window;
        	this.SetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        	this.SetComboBox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.SetComboBox.ForeColor = System.Drawing.SystemColors.WindowText;
        	this.SetComboBox.FormattingEnabled = true;
        	this.SetComboBox.Items.AddRange(new object[] {
			"Set 1",
			"Set 2",
			"Fun runs"});
        	this.SetComboBox.Location = new System.Drawing.Point(325, 36);
        	this.SetComboBox.Name = "SetComboBox";
        	this.SetComboBox.Size = new System.Drawing.Size(76, 23);
        	this.SetComboBox.TabIndex = 36;
        	this.SetComboBox.SelectedIndexChanged += new System.EventHandler(this.SetComboBox_SelectedIndexChanged);
        	// 
        	// stopAlign
        	// 
        	this.stopAlign.BackColor = System.Drawing.Color.White;
        	this.stopAlign.ForeColor = System.Drawing.Color.White;
        	this.stopAlign.Location = new System.Drawing.Point(77, 27);
        	this.stopAlign.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
        	this.stopAlign.Name = "stopAlign";
        	this.stopAlign.Size = new System.Drawing.Size(64, 19);
        	this.stopAlign.TabIndex = 34;
        	this.stopAlign.Text = "Stop align";
        	// 
        	// startAlign
        	// 
        	this.startAlign.BackColor = System.Drawing.Color.White;
        	this.startAlign.ForeColor = System.Drawing.Color.White;
        	this.startAlign.Location = new System.Drawing.Point(6, 27);
        	this.startAlign.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
        	this.startAlign.Name = "startAlign";
        	this.startAlign.Size = new System.Drawing.Size(66, 19);
        	this.startAlign.TabIndex = 33;
        	this.startAlign.Text = "Start align";
        	// 
        	// stopRadio
        	// 
        	this.stopRadio.BackColor = System.Drawing.Color.White;
        	this.stopRadio.ForeColor = System.Drawing.Color.White;
        	this.stopRadio.Location = new System.Drawing.Point(77, 8);
        	this.stopRadio.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
        	this.stopRadio.Name = "stopRadio";
        	this.stopRadio.Size = new System.Drawing.Size(64, 19);
        	this.stopRadio.TabIndex = 30;
        	this.stopRadio.Text = "Stop radio";
        	// 
        	// startRadio
        	// 
        	this.startRadio.BackColor = System.Drawing.Color.White;
        	this.startRadio.ForeColor = System.Drawing.Color.White;
        	this.startRadio.Location = new System.Drawing.Point(6, 8);
        	this.startRadio.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
        	this.startRadio.Name = "startRadio";
        	this.startRadio.Size = new System.Drawing.Size(66, 19);
        	this.startRadio.TabIndex = 29;
        	this.startRadio.Text = "Start radio";
        	// 
        	// timergrid
        	// 
        	this.timergrid.AllowUserToOrderColumns = true;
        	this.timergrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
        	this.timergrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        	dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
        	dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
        	dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial Narrow", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
        	dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
        	dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
        	dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
        	this.timergrid.DefaultCellStyle = dataGridViewCellStyle2;
        	this.timergrid.Location = new System.Drawing.Point(8, 101);
        	this.timergrid.Margin = new System.Windows.Forms.Padding(2);
        	this.timergrid.Name = "timergrid";
        	this.timergrid.RowHeadersVisible = false;
        	this.timergrid.RowTemplate.Height = 24;
        	this.timergrid.Size = new System.Drawing.Size(714, 312);
        	this.timergrid.TabIndex = 1;
        	// 
        	// TimerPortButton
        	// 
        	this.TimerPortButton.BackColor = System.Drawing.Color.Lime;
        	this.TimerPortButton.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.TimerPortButton.Location = new System.Drawing.Point(156, 8);
        	this.TimerPortButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        	this.TimerPortButton.Name = "TimerPortButton";
        	this.TimerPortButton.Size = new System.Drawing.Size(156, 72);
        	this.TimerPortButton.TabIndex = 4;
        	this.TimerPortButton.TabStop = false;
        	this.TimerPortButton.Text = "Timer Stopped";
        	this.TimerPortButton.UseVisualStyleBackColor = false;
        	this.TimerPortButton.Click += new System.EventHandler(this.TimerPortButtonClick);
        	// 
        	// triggerStopButton
        	// 
        	this.triggerStopButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.triggerStopButton.Location = new System.Drawing.Point(568, 8);
        	this.triggerStopButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        	this.triggerStopButton.Name = "triggerStopButton";
        	this.triggerStopButton.Size = new System.Drawing.Size(136, 32);
        	this.triggerStopButton.TabIndex = 22;
        	this.triggerStopButton.TabStop = false;
        	this.triggerStopButton.Text = "Trigger Stop(F6)";
        	this.triggerStopButton.UseVisualStyleBackColor = true;
        	this.triggerStopButton.Click += new System.EventHandler(this.TriggerStopButtonClick);
        	// 
        	// stopResetButton
        	// 
        	this.stopResetButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.stopResetButton.Location = new System.Drawing.Point(568, 46);
        	this.stopResetButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        	this.stopResetButton.Name = "stopResetButton";
        	this.stopResetButton.Size = new System.Drawing.Size(136, 33);
        	this.stopResetButton.TabIndex = 12;
        	this.stopResetButton.TabStop = false;
        	this.stopResetButton.Text = "Reset Stop(F4)";
        	this.stopResetButton.UseVisualStyleBackColor = true;
        	this.stopResetButton.Click += new System.EventHandler(this.StopResetButtonClick);
        	// 
        	// triggerStartButton
        	// 
        	this.triggerStartButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.triggerStartButton.Location = new System.Drawing.Point(426, 7);
        	this.triggerStartButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        	this.triggerStartButton.Name = "triggerStartButton";
        	this.triggerStartButton.Size = new System.Drawing.Size(136, 32);
        	this.triggerStartButton.TabIndex = 21;
        	this.triggerStartButton.TabStop = false;
        	this.triggerStartButton.Text = "Trigger Start(F5)";
        	this.triggerStartButton.UseVisualStyleBackColor = true;
        	this.triggerStartButton.Click += new System.EventHandler(this.TriggerStartButtonClick);
        	// 
        	// startResetButton
        	// 
        	this.startResetButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.startResetButton.Location = new System.Drawing.Point(426, 46);
        	this.startResetButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        	this.startResetButton.Name = "startResetButton";
        	this.startResetButton.Size = new System.Drawing.Size(136, 33);
        	this.startResetButton.TabIndex = 10;
        	this.startResetButton.TabStop = false;
        	this.startResetButton.Text = "Reset Start(F3)";
        	this.startResetButton.UseVisualStyleBackColor = true;
        	this.startResetButton.Click += new System.EventHandler(this.StartResetButtonClick);
        	// 
        	// tabControl1
        	// 
        	this.tabControl1.Controls.Add(this.timingTabPage);
        	this.tabControl1.Controls.Add(this.scoringTabPage);
        	this.tabControl1.Controls.Add(this.logTabPage);
        	this.tabControl1.Controls.Add(this.HeatsTabPage);
        	this.tabControl1.Controls.Add(this.COMPortsTabPage);
        	this.tabControl1.Controls.Add(this.registrationTabPage);
        	this.tabControl1.Controls.Add(this.configurationTabPage);
        	this.tabControl1.Controls.Add(this.classesTabPage);
        	this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.tabControl1.Location = new System.Drawing.Point(0, 0);
        	this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
        	this.tabControl1.Name = "tabControl1";
        	this.tabControl1.SelectedIndex = 0;
        	this.tabControl1.Size = new System.Drawing.Size(732, 434);
        	this.tabControl1.TabIndex = 29;
        	this.tabControl1.TabStop = false;
        	this.tabControl1.Deselecting += new System.Windows.Forms.TabControlCancelEventHandler(this.TabControl1Deselecting);
        	// 
        	// classesTabPage
        	// 
        	this.classesTabPage.Controls.Add(this.addClassButton);
        	this.classesTabPage.Controls.Add(this.classDataGridView);
        	this.classesTabPage.Location = new System.Drawing.Point(4, 22);
        	this.classesTabPage.Margin = new System.Windows.Forms.Padding(2);
        	this.classesTabPage.Name = "classesTabPage";
        	this.classesTabPage.Size = new System.Drawing.Size(724, 408);
        	this.classesTabPage.TabIndex = 7;
        	this.classesTabPage.Text = "Classes";
        	this.classesTabPage.UseVisualStyleBackColor = true;
        	// 
        	// addClassButton
        	// 
        	this.addClassButton.Location = new System.Drawing.Point(7, 11);
        	this.addClassButton.Margin = new System.Windows.Forms.Padding(2);
        	this.addClassButton.Name = "addClassButton";
        	this.addClassButton.Size = new System.Drawing.Size(120, 37);
        	this.addClassButton.TabIndex = 1;
        	this.addClassButton.Text = "Add class";
        	this.addClassButton.UseVisualStyleBackColor = true;
        	this.addClassButton.Click += new System.EventHandler(this.AddClassButtonClick);
        	// 
        	// classDataGridView
        	// 
        	this.classDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
        	this.classDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        	this.classDataGridView.Location = new System.Drawing.Point(7, 54);
        	this.classDataGridView.Margin = new System.Windows.Forms.Padding(2);
        	this.classDataGridView.Name = "classDataGridView";
        	this.classDataGridView.RowTemplate.Height = 24;
        	this.classDataGridView.Size = new System.Drawing.Size(712, 353);
        	this.classDataGridView.TabIndex = 0;
        	// 
        	// MainForm
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(732, 434);
        	this.Controls.Add(this.tabControl1);
        	this.KeyPreview = true;
        	this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        	this.Name = "MainForm";
        	this.Text = "M&J Timing";
        	this.configurationTabPage.ResumeLayout(false);
        	((System.ComponentModel.ISupportInitialize)(this.configurationDataGridView)).EndInit();
        	this.registrationTabPage.ResumeLayout(false);
        	this.registrationTabPage.PerformLayout();
        	((System.ComponentModel.ISupportInitialize)(this.drivers)).EndInit();
        	this.COMPortsTabPage.ResumeLayout(false);
        	this.HeatsTabPage.ResumeLayout(false);
        	this.logTabPage.ResumeLayout(false);
        	this.scoringTabPage.ResumeLayout(false);
        	this.scoringTabPage.PerformLayout();
        	this.groupBox2.ResumeLayout(false);
        	this.groupBox2.PerformLayout();
        	this.groupBox1.ResumeLayout(false);
        	this.timingTabPage.ResumeLayout(false);
        	((System.ComponentModel.ISupportInitialize)(this.timergrid)).EndInit();
        	this.tabControl1.ResumeLayout(false);
        	this.classesTabPage.ResumeLayout(false);
        	((System.ComponentModel.ISupportInitialize)(this.classDataGridView)).EndInit();
        	this.ResumeLayout(false);

        }

    private System.Windows.Forms.Button FindButton;
        private System.Windows.Forms.TextBox findText;
        private System.Windows.Forms.Button MSRImportButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox OfficialRunsTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton set1PlusSet2RadioButton;
        private System.Windows.Forms.RadioButton bestRunRadioButton;
        private System.Windows.Forms.RadioButton set2RadioButton;
        private System.Windows.Forms.RadioButton set1RadioButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button addClassButton;
        private System.Windows.Forms.TabPage classesTabPage;
        private System.Windows.Forms.DataGridView classDataGridView;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.CheckedListBox scoreModifiers;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox titleTextBox;
        private System.Windows.Forms.Button ScoreButton;
        private System.Windows.Forms.TextBox Day2TextBox;
        private System.Windows.Forms.TextBox Day1TextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView configurationDataGridView;
        private System.Windows.Forms.TabPage configurationTabPage;
        private System.Windows.Forms.DataGridView drivers;
        private System.Windows.Forms.Button clearNotesButton;
        private System.Windows.Forms.Button addDriverButton;
        private System.Windows.Forms.Button unregButton;
        private System.Windows.Forms.TabPage registrationTabPage;
        private System.Windows.Forms.RichTextBox COMPortsTextBox;
        private System.Windows.Forms.TabPage COMPortsTabPage;
        private System.Windows.Forms.RichTextBox HeatsTextBox;
        private System.Windows.Forms.TabPage HeatsTabPage;
        private System.Windows.Forms.Label startRadio;
        private System.Windows.Forms.Label stopRadio;
        private System.Windows.Forms.Label startAlign;
        private System.Windows.Forms.Label stopAlign;
        private System.Windows.Forms.CheckedListBox scoringList;
        private System.Windows.Forms.RichTextBox scoresTextBox;
        private System.Windows.Forms.TabPage timingTabPage;
        private System.Windows.Forms.TabPage logTabPage;
        private System.Windows.Forms.TabPage scoringTabPage;
        private timingGrid.TimingGrid timergrid;
        private System.Windows.Forms.Button triggerStopButton;
        private System.Windows.Forms.Button triggerStartButton;
        private System.Windows.Forms.Button stopResetButton;
        private System.Windows.Forms.Button startResetButton;
        private System.Windows.Forms.Button TimerPortButton;
        private System.Windows.Forms.RichTextBox msgTextBox;
        private System.Windows.Forms.Button DeleteDriversButton;
        private System.Windows.Forms.CheckBox concat_checkbox;
        private System.Windows.Forms.ComboBox SetComboBox;


    }

}
