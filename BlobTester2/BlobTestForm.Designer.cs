
namespace BlobTester
{
    partial class BlobTestForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BlobTestForm));
            this.bsPayload = new System.Windows.Forms.BindingSource(this.components);
            this.bsBlobFiles = new System.Windows.Forms.BindingSource(this.components);
            this.bsBlobItems = new System.Windows.Forms.BindingSource(this.components);
            this.bsVehicleInQueue = new System.Windows.Forms.BindingSource(this.components);
            this.bindingSource2 = new System.Windows.Forms.BindingSource(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.timerShowQueueLen = new System.Windows.Forms.Timer(this.components);
            this.gbSelection = new System.Windows.Forms.GroupBox();
            this.cbDistributeLoop = new System.Windows.Forms.CheckBox();
            this.cbDetailedDistributeLogging = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbTestConti = new System.Windows.Forms.RadioButton();
            this.rbProdConti = new System.Windows.Forms.RadioButton();
            this.tbFilename = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.bOpenFile = new System.Windows.Forms.Button();
            this.cbAppend = new System.Windows.Forms.CheckBox();
            this.cbUseTimestampNow = new System.Windows.Forms.CheckBox();
            this.lbVehiclelabel = new System.Windows.Forms.Label();
            this.lbVehicle = new System.Windows.Forms.TextBox();
            this.bViewData = new System.Windows.Forms.Button();
            this.bDistribute = new System.Windows.Forms.Button();
            this.lbBlobQueue = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.spDays = new System.Windows.Forms.NumericUpDown();
            this.lbSince = new System.Windows.Forms.Label();
            this.dtSinceTime = new System.Windows.Forms.DateTimePicker();
            this.cbSince = new System.Windows.Forms.CheckBox();
            this.cbDetailedContiLogging = new System.Windows.Forms.CheckBox();
            this.dtSince = new System.Windows.Forms.DateTimePicker();
            this.cbUseRecentDate = new System.Windows.Forms.CheckBox();
            this.rbTestBlob = new System.Windows.Forms.RadioButton();
            this.tbBusFilter = new System.Windows.Forms.TextBox();
            this.rbProdBlob = new System.Windows.Forms.RadioButton();
            this.cbDownload = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tbFeedback2 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.gcFiles = new DevExpress.XtraGrid.GridControl();
            this.gridViewFiles = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridView4 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.bnFiles = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorCountItem1 = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveFirstItem1 = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem1 = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem1 = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem1 = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem1 = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcPayload = new DevExpress.XtraGrid.GridControl();
            this.gvPayload = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridView3 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.bindingNavigator1 = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.gcTirePressure = new DevExpress.XtraGrid.GridControl();
            this.gvTirePressure = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridView5 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lbVoortgang = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.gcVehiclesAndSensors = new DevExpress.XtraGrid.GridControl();
            this.bsVehiclesAndSensors = new System.Windows.Forms.BindingSource(this.components);
            this.gvVehiclesAndSensors = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.bindingNavigator2 = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorAddNewItem1 = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorCountItem2 = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorDeleteItem1 = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveFirstItem2 = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem2 = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem2 = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem2 = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem2 = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.bsPayload)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsBlobFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsBlobItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsVehicleInQueue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource2)).BeginInit();
            this.gbSelection.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spDays)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bnFiles)).BeginInit();
            this.bnFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcPayload)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvPayload)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator1)).BeginInit();
            this.bindingNavigator1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcTirePressure)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvTirePressure)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcVehiclesAndSensors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsVehiclesAndSensors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvVehiclesAndSensors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator2)).BeginInit();
            this.bindingNavigator2.SuspendLayout();
            this.SuspendLayout();
            // 
            // bsPayload
            // 
            this.bsPayload.CurrentChanged += new System.EventHandler(this.bsPayLoad_CurrentChanged);
            // 
            // bsBlobFiles
            // 
            this.bsBlobFiles.CurrentChanged += new System.EventHandler(this.bindingSource4_CurrentChanged);
            // 
            // bsBlobItems
            // 
            this.bsBlobItems.CurrentChanged += new System.EventHandler(this.bsBlobItems_CurrentChanged);
            // 
            // bsVehicleInQueue
            // 
            this.bsVehicleInQueue.CurrentChanged += new System.EventHandler(this.bsVehicleInQueue_CurrentChanged);
            // 
            // bindingSource2
            // 
            this.bindingSource2.CurrentChanged += new System.EventHandler(this.bindingSource2_CurrentChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // timerShowQueueLen
            // 
            this.timerShowQueueLen.Enabled = true;
            this.timerShowQueueLen.Interval = 10000;
            this.timerShowQueueLen.Tick += new System.EventHandler(this.timerShowQueueLen_Tick);
            // 
            // gbSelection
            // 
            this.gbSelection.Controls.Add(this.cbDistributeLoop);
            this.gbSelection.Controls.Add(this.cbDetailedDistributeLogging);
            this.gbSelection.Controls.Add(this.groupBox1);
            this.gbSelection.Controls.Add(this.bViewData);
            this.gbSelection.Controls.Add(this.bDistribute);
            this.gbSelection.Controls.Add(this.lbBlobQueue);
            this.gbSelection.Controls.Add(this.groupBox2);
            this.gbSelection.Controls.Add(this.button1);
            this.gbSelection.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbSelection.Location = new System.Drawing.Point(0, 0);
            this.gbSelection.Name = "gbSelection";
            this.gbSelection.Size = new System.Drawing.Size(1573, 119);
            this.gbSelection.TabIndex = 3;
            this.gbSelection.TabStop = false;
            this.gbSelection.Text = "Selection and Actions";
            this.gbSelection.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // cbDistributeLoop
            // 
            this.cbDistributeLoop.AutoSize = true;
            this.cbDistributeLoop.Location = new System.Drawing.Point(627, 77);
            this.cbDistributeLoop.Name = "cbDistributeLoop";
            this.cbDistributeLoop.Size = new System.Drawing.Size(50, 17);
            this.cbDistributeLoop.TabIndex = 26;
            this.cbDistributeLoop.Text = "Loop";
            this.cbDistributeLoop.UseVisualStyleBackColor = true;
            // 
            // cbDetailedDistributeLogging
            // 
            this.cbDetailedDistributeLogging.AutoSize = true;
            this.cbDetailedDistributeLogging.Location = new System.Drawing.Point(468, 77);
            this.cbDetailedDistributeLogging.Name = "cbDetailedDistributeLogging";
            this.cbDetailedDistributeLogging.Size = new System.Drawing.Size(153, 17);
            this.cbDetailedDistributeLogging.TabIndex = 25;
            this.cbDetailedDistributeLogging.Text = "Detailed Distribute Logging";
            this.cbDetailedDistributeLogging.UseVisualStyleBackColor = true;
            this.cbDetailedDistributeLogging.CheckedChanged += new System.EventHandler(this.cbDetailedDistributeLogging_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.tbFilename);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.bOpenFile);
            this.groupBox1.Controls.Add(this.cbAppend);
            this.groupBox1.Controls.Add(this.cbUseTimestampNow);
            this.groupBox1.Controls.Add(this.lbVehiclelabel);
            this.groupBox1.Controls.Add(this.lbVehicle);
            this.groupBox1.Location = new System.Drawing.Point(699, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(731, 85);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Debugging";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(542, 65);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(29, 23);
            this.button4.TabIndex = 16;
            this.button4.Text = "...";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbTestConti);
            this.groupBox3.Controls.Add(this.rbProdConti);
            this.groupBox3.Location = new System.Drawing.Point(287, 43);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(114, 37);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "ContiWeb";
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // rbTestConti
            // 
            this.rbTestConti.AutoSize = true;
            this.rbTestConti.Checked = true;
            this.rbTestConti.Location = new System.Drawing.Point(6, 13);
            this.rbTestConti.Name = "rbTestConti";
            this.rbTestConti.Size = new System.Drawing.Size(46, 17);
            this.rbTestConti.TabIndex = 3;
            this.rbTestConti.TabStop = true;
            this.rbTestConti.Text = "Test";
            this.rbTestConti.UseVisualStyleBackColor = true;
            this.rbTestConti.CheckedChanged += new System.EventHandler(this.rbTestConti_CheckedChanged);
            // 
            // rbProdConti
            // 
            this.rbProdConti.AutoSize = true;
            this.rbProdConti.Location = new System.Drawing.Point(58, 13);
            this.rbProdConti.Name = "rbProdConti";
            this.rbProdConti.Size = new System.Drawing.Size(47, 17);
            this.rbProdConti.TabIndex = 4;
            this.rbProdConti.TabStop = true;
            this.rbProdConti.Text = "Prod";
            this.rbProdConti.UseVisualStyleBackColor = true;
            this.rbProdConti.CheckedChanged += new System.EventHandler(this.rbTestConti_CheckedChanged);
            // 
            // tbFilename
            // 
            this.tbFilename.Location = new System.Drawing.Point(11, 26);
            this.tbFilename.Name = "tbFilename";
            this.tbFilename.Size = new System.Drawing.Size(525, 20);
            this.tbFilename.TabIndex = 3;
            this.tbFilename.Text = "C:\\Klanten\\Arriva\\Blob3\\Blob3\\bin\\Debug\\arriva-nl\\cloud-fms\\9718_cloudfms1-2_2021" +
    "1022113721_2234217_1634903244358.json";
            this.tbFilename.TextChanged += new System.EventHandler(this.tbFilename_TextChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(663, 15);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(57, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "ReadFile";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // bOpenFile
            // 
            this.bOpenFile.Location = new System.Drawing.Point(542, 26);
            this.bOpenFile.Name = "bOpenFile";
            this.bOpenFile.Size = new System.Drawing.Size(29, 23);
            this.bOpenFile.TabIndex = 5;
            this.bOpenFile.Text = "...";
            this.bOpenFile.UseVisualStyleBackColor = true;
            this.bOpenFile.Click += new System.EventHandler(this.bOpenFile_Click);
            // 
            // cbAppend
            // 
            this.cbAppend.AutoSize = true;
            this.cbAppend.Location = new System.Drawing.Point(590, 19);
            this.cbAppend.Name = "cbAppend";
            this.cbAppend.Size = new System.Drawing.Size(63, 17);
            this.cbAppend.TabIndex = 16;
            this.cbAppend.Text = "Append";
            this.cbAppend.UseVisualStyleBackColor = true;
            this.cbAppend.CheckedChanged += new System.EventHandler(this.cbAppend_CheckedChanged);
            // 
            // cbUseTimestampNow
            // 
            this.cbUseTimestampNow.AutoSize = true;
            this.cbUseTimestampNow.Location = new System.Drawing.Point(590, 63);
            this.cbUseTimestampNow.Name = "cbUseTimestampNow";
            this.cbUseTimestampNow.Size = new System.Drawing.Size(126, 17);
            this.cbUseTimestampNow.TabIndex = 9;
            this.cbUseTimestampNow.Text = "Use Timestamp \'now\'";
            this.cbUseTimestampNow.UseVisualStyleBackColor = true;
            this.cbUseTimestampNow.CheckedChanged += new System.EventHandler(this.cbUseTimestampNow_CheckedChanged);
            // 
            // lbVehiclelabel
            // 
            this.lbVehiclelabel.AutoSize = true;
            this.lbVehiclelabel.Location = new System.Drawing.Point(8, 54);
            this.lbVehiclelabel.Name = "lbVehiclelabel";
            this.lbVehiclelabel.Size = new System.Drawing.Size(89, 13);
            this.lbVehiclelabel.TabIndex = 10;
            this.lbVehiclelabel.Text = "Vehicle / CCU-ID";
            this.lbVehiclelabel.Click += new System.EventHandler(this.lbVehiclelabel_Click);
            // 
            // lbVehicle
            // 
            this.lbVehicle.Location = new System.Drawing.Point(103, 51);
            this.lbVehicle.Name = "lbVehicle";
            this.lbVehicle.ReadOnly = true;
            this.lbVehicle.Size = new System.Drawing.Size(178, 20);
            this.lbVehicle.TabIndex = 12;
            this.lbVehicle.TextChanged += new System.EventHandler(this.lbVehicle_TextChanged);
            // 
            // bViewData
            // 
            this.bViewData.Location = new System.Drawing.Point(88, 74);
            this.bViewData.Name = "bViewData";
            this.bViewData.Size = new System.Drawing.Size(196, 23);
            this.bViewData.TabIndex = 23;
            this.bViewData.Text = "View Blob Data (and download)";
            this.bViewData.UseVisualStyleBackColor = true;
            this.bViewData.Click += new System.EventHandler(this.bViewData_Click);
            // 
            // bDistribute
            // 
            this.bDistribute.Location = new System.Drawing.Point(12, 74);
            this.bDistribute.Name = "bDistribute";
            this.bDistribute.Size = new System.Drawing.Size(70, 23);
            this.bDistribute.TabIndex = 21;
            this.bDistribute.Text = "Distribute";
            this.bDistribute.UseVisualStyleBackColor = true;
            this.bDistribute.Click += new System.EventHandler(this.bDistribute_Click);
            // 
            // lbBlobQueue
            // 
            this.lbBlobQueue.AutoSize = true;
            this.lbBlobQueue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbBlobQueue.Location = new System.Drawing.Point(6, 102);
            this.lbBlobQueue.Name = "lbBlobQueue";
            this.lbBlobQueue.Size = new System.Drawing.Size(99, 13);
            this.lbBlobQueue.TabIndex = 18;
            this.lbBlobQueue.Text = "<Conti update info>";
            this.lbBlobQueue.Click += new System.EventHandler(this.lbBlobQueue_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.spDays);
            this.groupBox2.Controls.Add(this.lbSince);
            this.groupBox2.Controls.Add(this.dtSinceTime);
            this.groupBox2.Controls.Add(this.cbSince);
            this.groupBox2.Controls.Add(this.cbDetailedContiLogging);
            this.groupBox2.Controls.Add(this.dtSince);
            this.groupBox2.Controls.Add(this.cbUseRecentDate);
            this.groupBox2.Controls.Add(this.rbTestBlob);
            this.groupBox2.Controls.Add(this.tbBusFilter);
            this.groupBox2.Controls.Add(this.rbProdBlob);
            this.groupBox2.Controls.Add(this.cbDownload);
            this.groupBox2.Location = new System.Drawing.Point(12, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(585, 56);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "BlobStorage";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(323, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "days";
            // 
            // spDays
            // 
            this.spDays.Location = new System.Drawing.Point(279, 30);
            this.spDays.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.spDays.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spDays.Name = "spDays";
            this.spDays.Size = new System.Drawing.Size(39, 20);
            this.spDays.TabIndex = 21;
            this.spDays.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lbSince
            // 
            this.lbSince.AutoSize = true;
            this.lbSince.Location = new System.Drawing.Point(170, 15);
            this.lbSince.Name = "lbSince";
            this.lbSince.Size = new System.Drawing.Size(44, 13);
            this.lbSince.TabIndex = 20;
            this.lbSince.Text = "<since>";
            // 
            // dtSinceTime
            // 
            this.dtSinceTime.Enabled = false;
            this.dtSinceTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtSinceTime.Location = new System.Drawing.Point(202, 30);
            this.dtSinceTime.Name = "dtSinceTime";
            this.dtSinceTime.Size = new System.Drawing.Size(70, 20);
            this.dtSinceTime.TabIndex = 12;
            this.dtSinceTime.ValueChanged += new System.EventHandler(this.dtSinceTime_ValueChanged);
            // 
            // cbSince
            // 
            this.cbSince.AutoSize = true;
            this.cbSince.Location = new System.Drawing.Point(58, 33);
            this.cbSince.Name = "cbSince";
            this.cbSince.Size = new System.Drawing.Size(53, 17);
            this.cbSince.TabIndex = 11;
            this.cbSince.Text = "Since";
            this.cbSince.UseVisualStyleBackColor = true;
            this.cbSince.CheckedChanged += new System.EventHandler(this.cbSince_CheckedChanged);
            // 
            // cbDetailedContiLogging
            // 
            this.cbDetailedContiLogging.AutoSize = true;
            this.cbDetailedContiLogging.Location = new System.Drawing.Point(449, 32);
            this.cbDetailedContiLogging.Name = "cbDetailedContiLogging";
            this.cbDetailedContiLogging.Size = new System.Drawing.Size(130, 17);
            this.cbDetailedContiLogging.TabIndex = 19;
            this.cbDetailedContiLogging.Text = "Detailed ContiLogging";
            this.cbDetailedContiLogging.UseVisualStyleBackColor = true;
            this.cbDetailedContiLogging.CheckedChanged += new System.EventHandler(this.cbDetailedContiLogging_CheckedChanged);
            // 
            // dtSince
            // 
            this.dtSince.Enabled = false;
            this.dtSince.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtSince.Location = new System.Drawing.Point(117, 30);
            this.dtSince.Name = "dtSince";
            this.dtSince.Size = new System.Drawing.Size(79, 20);
            this.dtSince.TabIndex = 10;
            this.dtSince.ValueChanged += new System.EventHandler(this.dtSince_ValueChanged);
            // 
            // cbUseRecentDate
            // 
            this.cbUseRecentDate.AutoSize = true;
            this.cbUseRecentDate.Location = new System.Drawing.Point(58, 14);
            this.cbUseRecentDate.Name = "cbUseRecentDate";
            this.cbUseRecentDate.Size = new System.Drawing.Size(118, 17);
            this.cbUseRecentDate.TabIndex = 8;
            this.cbUseRecentDate.Text = "Only recent records";
            this.cbUseRecentDate.UseVisualStyleBackColor = true;
            this.cbUseRecentDate.CheckedChanged += new System.EventHandler(this.cbUseRecentDate_CheckedChanged);
            // 
            // rbTestBlob
            // 
            this.rbTestBlob.AutoSize = true;
            this.rbTestBlob.Checked = true;
            this.rbTestBlob.Location = new System.Drawing.Point(3, 17);
            this.rbTestBlob.Name = "rbTestBlob";
            this.rbTestBlob.Size = new System.Drawing.Size(46, 17);
            this.rbTestBlob.TabIndex = 1;
            this.rbTestBlob.TabStop = true;
            this.rbTestBlob.Text = "Test";
            this.rbTestBlob.UseVisualStyleBackColor = true;
            this.rbTestBlob.CheckedChanged += new System.EventHandler(this.rbProd_CheckedChanged);
            // 
            // tbBusFilter
            // 
            this.tbBusFilter.Location = new System.Drawing.Point(539, 9);
            this.tbBusFilter.Name = "tbBusFilter";
            this.tbBusFilter.Size = new System.Drawing.Size(42, 20);
            this.tbBusFilter.TabIndex = 9;
            this.tbBusFilter.TextChanged += new System.EventHandler(this.tbBusFilter_TextChanged);
            // 
            // rbProdBlob
            // 
            this.rbProdBlob.AutoSize = true;
            this.rbProdBlob.Location = new System.Drawing.Point(3, 34);
            this.rbProdBlob.Name = "rbProdBlob";
            this.rbProdBlob.Size = new System.Drawing.Size(47, 17);
            this.rbProdBlob.TabIndex = 2;
            this.rbProdBlob.TabStop = true;
            this.rbProdBlob.Text = "Prod";
            this.rbProdBlob.UseVisualStyleBackColor = true;
            this.rbProdBlob.CheckedChanged += new System.EventHandler(this.rbProd_CheckedChanged);
            // 
            // cbDownload
            // 
            this.cbDownload.AutoSize = true;
            this.cbDownload.Enabled = false;
            this.cbDownload.Location = new System.Drawing.Point(340, 11);
            this.cbDownload.Name = "cbDownload";
            this.cbDownload.Size = new System.Drawing.Size(193, 17);
            this.cbDownload.TabIndex = 6;
            this.cbDownload.Text = "Download to File (specify 1 vehicle)";
            this.cbDownload.UseVisualStyleBackColor = true;
            this.cbDownload.CheckedChanged += new System.EventHandler(this.cbDownload_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(295, 74);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(167, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Get and Send to Continental";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.bGetBlobs_Click);
            // 
            // tbFeedback2
            // 
            this.tbFeedback2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbFeedback2.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbFeedback2.Location = new System.Drawing.Point(0, 0);
            this.tbFeedback2.Multiline = true;
            this.tbFeedback2.Name = "tbFeedback2";
            this.tbFeedback2.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbFeedback2.Size = new System.Drawing.Size(1559, 162);
            this.tbFeedback2.TabIndex = 2;
            this.tbFeedback2.WordWrap = false;
            this.tbFeedback2.TextChanged += new System.EventHandler(this.tbFeedback2_TextChanged);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(1460, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "Clear";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // gcFiles
            // 
            this.gcFiles.DataSource = this.bsBlobFiles;
            this.gcFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcFiles.Location = new System.Drawing.Point(0, 25);
            this.gcFiles.MainView = this.gridViewFiles;
            this.gcFiles.Name = "gcFiles";
            this.gcFiles.Size = new System.Drawing.Size(516, 349);
            this.gcFiles.TabIndex = 3;
            this.gcFiles.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewFiles,
            this.gridView4,
            this.gridView1});
            this.gcFiles.Click += new System.EventHandler(this.gridControlFiles_Click);
            // 
            // gridViewFiles
            // 
            this.gridViewFiles.Appearance.ViewCaption.Options.UseTextOptions = true;
            this.gridViewFiles.Appearance.ViewCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.gridViewFiles.GridControl = this.gcFiles;
            this.gridViewFiles.Name = "gridViewFiles";
            this.gridViewFiles.OptionsView.ShowAutoFilterRow = true;
            this.gridViewFiles.OptionsView.ShowViewCaption = true;
            this.gridViewFiles.ViewCaption = "BlobItems / BlobFiles";
            // 
            // gridView4
            // 
            this.gridView4.GridControl = this.gcFiles;
            this.gridView4.Name = "gridView4";
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gcFiles;
            this.gridView1.Name = "gridView1";
            // 
            // bnFiles
            // 
            this.bnFiles.AddNewItem = this.bindingNavigatorAddNewItem;
            this.bnFiles.BindingSource = this.bsBlobItems;
            this.bnFiles.CountItem = this.bindingNavigatorCountItem1;
            this.bnFiles.DeleteItem = this.bindingNavigatorDeleteItem;
            this.bnFiles.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem1,
            this.bindingNavigatorMovePreviousItem1,
            this.bindingNavigatorSeparator2,
            this.bindingNavigatorPositionItem1,
            this.bindingNavigatorCountItem1,
            this.bindingNavigatorSeparator3,
            this.bindingNavigatorMoveNextItem1,
            this.bindingNavigatorMoveLastItem1,
            this.bindingNavigatorSeparator4,
            this.bindingNavigatorAddNewItem,
            this.bindingNavigatorDeleteItem});
            this.bnFiles.Location = new System.Drawing.Point(0, 0);
            this.bnFiles.MoveFirstItem = this.bindingNavigatorMoveFirstItem1;
            this.bnFiles.MoveLastItem = this.bindingNavigatorMoveLastItem1;
            this.bnFiles.MoveNextItem = this.bindingNavigatorMoveNextItem1;
            this.bnFiles.MovePreviousItem = this.bindingNavigatorMovePreviousItem1;
            this.bnFiles.Name = "bnFiles";
            this.bnFiles.PositionItem = this.bindingNavigatorPositionItem1;
            this.bnFiles.Size = new System.Drawing.Size(516, 25);
            this.bnFiles.TabIndex = 17;
            this.bnFiles.Text = "bindingNavigator4";
            this.bnFiles.RefreshItems += new System.EventHandler(this.bindingNavigator4_RefreshItems);
            // 
            // bindingNavigatorAddNewItem
            // 
            this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem.Image")));
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorAddNewItem.Text = "Add new";
            this.bindingNavigatorAddNewItem.Click += new System.EventHandler(this.bindingNavigatorAddNewItem_Click);
            // 
            // bindingNavigatorCountItem1
            // 
            this.bindingNavigatorCountItem1.Name = "bindingNavigatorCountItem1";
            this.bindingNavigatorCountItem1.Size = new System.Drawing.Size(35, 22);
            this.bindingNavigatorCountItem1.Text = "of {0}";
            this.bindingNavigatorCountItem1.ToolTipText = "Total number of items";
            this.bindingNavigatorCountItem1.Click += new System.EventHandler(this.bindingNavigatorCountItem1_Click);
            // 
            // bindingNavigatorDeleteItem
            // 
            this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem.Image")));
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorDeleteItem.Text = "Delete";
            this.bindingNavigatorDeleteItem.Click += new System.EventHandler(this.bindingNavigatorDeleteItem_Click);
            // 
            // bindingNavigatorMoveFirstItem1
            // 
            this.bindingNavigatorMoveFirstItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem1.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem1.Image")));
            this.bindingNavigatorMoveFirstItem1.Name = "bindingNavigatorMoveFirstItem1";
            this.bindingNavigatorMoveFirstItem1.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem1.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveFirstItem1.Text = "Move first";
            this.bindingNavigatorMoveFirstItem1.Click += new System.EventHandler(this.bindingNavigatorMoveFirstItem1_Click);
            // 
            // bindingNavigatorMovePreviousItem1
            // 
            this.bindingNavigatorMovePreviousItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem1.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem1.Image")));
            this.bindingNavigatorMovePreviousItem1.Name = "bindingNavigatorMovePreviousItem1";
            this.bindingNavigatorMovePreviousItem1.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem1.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMovePreviousItem1.Text = "Move previous";
            this.bindingNavigatorMovePreviousItem1.Click += new System.EventHandler(this.bindingNavigatorMovePreviousItem1_Click);
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 25);
            this.bindingNavigatorSeparator2.Click += new System.EventHandler(this.bindingNavigatorSeparator2_Click);
            // 
            // bindingNavigatorPositionItem1
            // 
            this.bindingNavigatorPositionItem1.AccessibleName = "Position";
            this.bindingNavigatorPositionItem1.AutoSize = false;
            this.bindingNavigatorPositionItem1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.bindingNavigatorPositionItem1.Name = "bindingNavigatorPositionItem1";
            this.bindingNavigatorPositionItem1.Size = new System.Drawing.Size(50, 23);
            this.bindingNavigatorPositionItem1.Text = "0";
            this.bindingNavigatorPositionItem1.ToolTipText = "Current position";
            this.bindingNavigatorPositionItem1.Click += new System.EventHandler(this.bindingNavigatorPositionItem1_Click);
            // 
            // bindingNavigatorSeparator3
            // 
            this.bindingNavigatorSeparator3.Name = "bindingNavigatorSeparator3";
            this.bindingNavigatorSeparator3.Size = new System.Drawing.Size(6, 25);
            this.bindingNavigatorSeparator3.Click += new System.EventHandler(this.bindingNavigatorSeparator3_Click);
            // 
            // bindingNavigatorMoveNextItem1
            // 
            this.bindingNavigatorMoveNextItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem1.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem1.Image")));
            this.bindingNavigatorMoveNextItem1.Name = "bindingNavigatorMoveNextItem1";
            this.bindingNavigatorMoveNextItem1.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem1.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveNextItem1.Text = "Move next";
            this.bindingNavigatorMoveNextItem1.Click += new System.EventHandler(this.bindingNavigatorMoveNextItem1_Click);
            // 
            // bindingNavigatorMoveLastItem1
            // 
            this.bindingNavigatorMoveLastItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem1.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem1.Image")));
            this.bindingNavigatorMoveLastItem1.Name = "bindingNavigatorMoveLastItem1";
            this.bindingNavigatorMoveLastItem1.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem1.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveLastItem1.Text = "Move last";
            this.bindingNavigatorMoveLastItem1.Click += new System.EventHandler(this.bindingNavigatorMoveLastItem1_Click);
            // 
            // bindingNavigatorSeparator4
            // 
            this.bindingNavigatorSeparator4.Name = "bindingNavigatorSeparator4";
            this.bindingNavigatorSeparator4.Size = new System.Drawing.Size(6, 25);
            this.bindingNavigatorSeparator4.Click += new System.EventHandler(this.bindingNavigatorSeparator4_Click);
            // 
            // gridView2
            // 
            this.gridView2.Name = "gridView2";
            // 
            // gcPayload
            // 
            this.gcPayload.DataSource = this.bsPayload;
            this.gcPayload.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcPayload.Location = new System.Drawing.Point(0, 25);
            this.gcPayload.MainView = this.gvPayload;
            this.gcPayload.Name = "gcPayload";
            this.gcPayload.Size = new System.Drawing.Size(1039, 138);
            this.gcPayload.TabIndex = 4;
            this.gcPayload.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvPayload,
            this.gridView3});
            this.gcPayload.Click += new System.EventHandler(this.gcPayload_Click);
            // 
            // gvPayload
            // 
            this.gvPayload.Appearance.ViewCaption.Options.UseTextOptions = true;
            this.gvPayload.Appearance.ViewCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.gvPayload.GridControl = this.gcPayload;
            this.gvPayload.Name = "gvPayload";
            this.gvPayload.OptionsView.ColumnAutoWidth = false;
            this.gvPayload.OptionsView.ShowViewCaption = true;
            this.gvPayload.ViewCaption = "Payload";
            // 
            // gridView3
            // 
            this.gridView3.GridControl = this.gcPayload;
            this.gridView3.Name = "gridView3";
            // 
            // bindingNavigator1
            // 
            this.bindingNavigator1.AddNewItem = null;
            this.bindingNavigator1.BindingSource = this.bsPayload;
            this.bindingNavigator1.CountItem = this.bindingNavigatorCountItem;
            this.bindingNavigator1.DeleteItem = null;
            this.bindingNavigator1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem});
            this.bindingNavigator1.Location = new System.Drawing.Point(0, 0);
            this.bindingNavigator1.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bindingNavigator1.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bindingNavigator1.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bindingNavigator1.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bindingNavigator1.Name = "bindingNavigator1";
            this.bindingNavigator1.PositionItem = this.bindingNavigatorPositionItem;
            this.bindingNavigator1.Size = new System.Drawing.Size(1039, 25);
            this.bindingNavigator1.TabIndex = 3;
            this.bindingNavigator1.Text = "bindingNavigator1";
            this.bindingNavigator1.RefreshItems += new System.EventHandler(this.bindingNavigator1_RefreshItems);
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(35, 22);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
            this.bindingNavigatorCountItem.Click += new System.EventHandler(this.bindingNavigatorCountItem_Click);
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveFirstItem.Text = "Move first";
            this.bindingNavigatorMoveFirstItem.Click += new System.EventHandler(this.bindingNavigatorMoveFirstItem_Click);
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMovePreviousItem.Text = "Move previous";
            this.bindingNavigatorMovePreviousItem.Click += new System.EventHandler(this.bindingNavigatorMovePreviousItem_Click);
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 25);
            this.bindingNavigatorSeparator.Click += new System.EventHandler(this.bindingNavigatorSeparator_Click);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "Position";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 23);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "Current position";
            this.bindingNavigatorPositionItem.Click += new System.EventHandler(this.bindingNavigatorPositionItem_Click);
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 25);
            this.bindingNavigatorSeparator1.Click += new System.EventHandler(this.bindingNavigatorSeparator1_Click);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveNextItem.Text = "Move next";
            this.bindingNavigatorMoveNextItem.Click += new System.EventHandler(this.bindingNavigatorMoveNextItem_Click);
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveLastItem.Text = "Move last";
            this.bindingNavigatorMoveLastItem.Click += new System.EventHandler(this.bindingNavigatorMoveLastItem_Click);
            // 
            // gcTirePressure
            // 
            this.gcTirePressure.DataSource = this.bindingSource2;
            this.gcTirePressure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcTirePressure.Location = new System.Drawing.Point(0, 0);
            this.gcTirePressure.MainView = this.gvTirePressure;
            this.gcTirePressure.Name = "gcTirePressure";
            this.gcTirePressure.Size = new System.Drawing.Size(1039, 207);
            this.gcTirePressure.TabIndex = 6;
            this.gcTirePressure.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvTirePressure,
            this.gridView5});
            this.gcTirePressure.Click += new System.EventHandler(this.gridControl2_Click);
            // 
            // gvTirePressure
            // 
            this.gvTirePressure.Appearance.ViewCaption.Options.UseTextOptions = true;
            this.gvTirePressure.Appearance.ViewCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.gvTirePressure.GridControl = this.gcTirePressure;
            this.gvTirePressure.Name = "gvTirePressure";
            this.gvTirePressure.OptionsView.ShowViewCaption = true;
            this.gvTirePressure.ViewCaption = "Payload Details (FEF4 , FC42)";
            // 
            // gridView5
            // 
            this.gridView5.GridControl = this.gcTirePressure;
            this.gridView5.Name = "gridView5";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
            this.splitContainer1.Panel2.Controls.Add(this.button3);
            this.splitContainer1.Panel2.Controls.Add(this.tbFeedback2);
            this.splitContainer1.Size = new System.Drawing.Size(1559, 540);
            this.splitContainer1.SplitterDistance = 374;
            this.splitContainer1.TabIndex = 4;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.gcFiles);
            this.splitContainer2.Panel1.Controls.Add(this.bnFiles);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(1559, 374);
            this.splitContainer2.SplitterDistance = 516;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.gcPayload);
            this.splitContainer3.Panel1.Controls.Add(this.bindingNavigator1);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.gcTirePressure);
            this.splitContainer3.Size = new System.Drawing.Size(1039, 374);
            this.splitContainer3.SplitterDistance = 163;
            this.splitContainer3.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbVoortgang});
            this.statusStrip1.Location = new System.Drawing.Point(0, 140);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1559, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lbVoortgang
            // 
            this.lbVoortgang.Name = "lbVoortgang";
            this.lbVoortgang.Size = new System.Drawing.Size(68, 17);
            this.lbVoortgang.Text = "<Progress>";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 119);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1573, 572);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1565, 546);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Azure Blob";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.gcVehiclesAndSensors);
            this.tabPage2.Controls.Add(this.bindingNavigator2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1565, 546);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Vehicles and Sensors";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // gcVehiclesAndSensors
            // 
            this.gcVehiclesAndSensors.DataSource = this.bsVehiclesAndSensors;
            this.gcVehiclesAndSensors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcVehiclesAndSensors.Location = new System.Drawing.Point(3, 28);
            this.gcVehiclesAndSensors.MainView = this.gvVehiclesAndSensors;
            this.gcVehiclesAndSensors.Name = "gcVehiclesAndSensors";
            this.gcVehiclesAndSensors.Size = new System.Drawing.Size(1559, 515);
            this.gcVehiclesAndSensors.TabIndex = 0;
            this.gcVehiclesAndSensors.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvVehiclesAndSensors});
            this.gcVehiclesAndSensors.Click += new System.EventHandler(this.gcVehiclesAndSensors_Click);
            // 
            // gvVehiclesAndSensors
            // 
            this.gvVehiclesAndSensors.GridControl = this.gcVehiclesAndSensors;
            this.gvVehiclesAndSensors.Name = "gvVehiclesAndSensors";
            this.gvVehiclesAndSensors.OptionsView.ColumnAutoWidth = false;
            this.gvVehiclesAndSensors.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gvVehiclesAndSensors_CustomDrawCell);
            // 
            // bindingNavigator2
            // 
            this.bindingNavigator2.AddNewItem = this.bindingNavigatorAddNewItem1;
            this.bindingNavigator2.BindingSource = this.bsVehiclesAndSensors;
            this.bindingNavigator2.CountItem = this.bindingNavigatorCountItem2;
            this.bindingNavigator2.DeleteItem = this.bindingNavigatorDeleteItem1;
            this.bindingNavigator2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem2,
            this.bindingNavigatorMovePreviousItem2,
            this.bindingNavigatorSeparator5,
            this.bindingNavigatorPositionItem2,
            this.bindingNavigatorCountItem2,
            this.bindingNavigatorSeparator6,
            this.bindingNavigatorMoveNextItem2,
            this.bindingNavigatorMoveLastItem2,
            this.bindingNavigatorSeparator7,
            this.bindingNavigatorAddNewItem1,
            this.bindingNavigatorDeleteItem1,
            this.toolStripLabel1,
            this.toolStripButtonRefresh});
            this.bindingNavigator2.Location = new System.Drawing.Point(3, 3);
            this.bindingNavigator2.MoveFirstItem = this.bindingNavigatorMoveFirstItem2;
            this.bindingNavigator2.MoveLastItem = this.bindingNavigatorMoveLastItem2;
            this.bindingNavigator2.MoveNextItem = this.bindingNavigatorMoveNextItem2;
            this.bindingNavigator2.MovePreviousItem = this.bindingNavigatorMovePreviousItem2;
            this.bindingNavigator2.Name = "bindingNavigator2";
            this.bindingNavigator2.PositionItem = this.bindingNavigatorPositionItem2;
            this.bindingNavigator2.Size = new System.Drawing.Size(1559, 25);
            this.bindingNavigator2.TabIndex = 1;
            this.bindingNavigator2.Text = "bindingNavigator2";
            // 
            // bindingNavigatorAddNewItem1
            // 
            this.bindingNavigatorAddNewItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorAddNewItem1.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem1.Image")));
            this.bindingNavigatorAddNewItem1.Name = "bindingNavigatorAddNewItem1";
            this.bindingNavigatorAddNewItem1.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem1.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorAddNewItem1.Text = "Add new";
            // 
            // bindingNavigatorCountItem2
            // 
            this.bindingNavigatorCountItem2.Name = "bindingNavigatorCountItem2";
            this.bindingNavigatorCountItem2.Size = new System.Drawing.Size(35, 22);
            this.bindingNavigatorCountItem2.Text = "of {0}";
            this.bindingNavigatorCountItem2.ToolTipText = "Total number of items";
            // 
            // bindingNavigatorDeleteItem1
            // 
            this.bindingNavigatorDeleteItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorDeleteItem1.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem1.Image")));
            this.bindingNavigatorDeleteItem1.Name = "bindingNavigatorDeleteItem1";
            this.bindingNavigatorDeleteItem1.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem1.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorDeleteItem1.Text = "Delete";
            // 
            // bindingNavigatorMoveFirstItem2
            // 
            this.bindingNavigatorMoveFirstItem2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem2.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem2.Image")));
            this.bindingNavigatorMoveFirstItem2.Name = "bindingNavigatorMoveFirstItem2";
            this.bindingNavigatorMoveFirstItem2.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem2.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveFirstItem2.Text = "Move first";
            // 
            // bindingNavigatorMovePreviousItem2
            // 
            this.bindingNavigatorMovePreviousItem2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem2.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem2.Image")));
            this.bindingNavigatorMovePreviousItem2.Name = "bindingNavigatorMovePreviousItem2";
            this.bindingNavigatorMovePreviousItem2.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem2.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMovePreviousItem2.Text = "Move previous";
            // 
            // bindingNavigatorSeparator5
            // 
            this.bindingNavigatorSeparator5.Name = "bindingNavigatorSeparator5";
            this.bindingNavigatorSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorPositionItem2
            // 
            this.bindingNavigatorPositionItem2.AccessibleName = "Position";
            this.bindingNavigatorPositionItem2.AutoSize = false;
            this.bindingNavigatorPositionItem2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.bindingNavigatorPositionItem2.Name = "bindingNavigatorPositionItem2";
            this.bindingNavigatorPositionItem2.Size = new System.Drawing.Size(50, 23);
            this.bindingNavigatorPositionItem2.Text = "0";
            this.bindingNavigatorPositionItem2.ToolTipText = "Current position";
            // 
            // bindingNavigatorSeparator6
            // 
            this.bindingNavigatorSeparator6.Name = "bindingNavigatorSeparator6";
            this.bindingNavigatorSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorMoveNextItem2
            // 
            this.bindingNavigatorMoveNextItem2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem2.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem2.Image")));
            this.bindingNavigatorMoveNextItem2.Name = "bindingNavigatorMoveNextItem2";
            this.bindingNavigatorMoveNextItem2.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem2.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveNextItem2.Text = "Move next";
            // 
            // bindingNavigatorMoveLastItem2
            // 
            this.bindingNavigatorMoveLastItem2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem2.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem2.Image")));
            this.bindingNavigatorMoveLastItem2.Name = "bindingNavigatorMoveLastItem2";
            this.bindingNavigatorMoveLastItem2.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem2.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveLastItem2.Text = "Move last";
            // 
            // bindingNavigatorSeparator7
            // 
            this.bindingNavigatorSeparator7.Name = "bindingNavigatorSeparator7";
            this.bindingNavigatorSeparator7.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripLabel1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripLabel1.Image")));
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(16, 22);
            this.toolStripLabel1.Text = "xls";
            this.toolStripLabel1.Click += new System.EventHandler(this.toolStripLabel1_Click_1);
            // 
            // toolStripButtonRefresh
            // 
            this.toolStripButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRefresh.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRefresh.Image")));
            this.toolStripButtonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRefresh.Text = "toolStripButton1";
            this.toolStripButtonRefresh.Click += new System.EventHandler(this.toolStripButtonRefresh_Click);
            // 
            // BlobTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1573, 691);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.gbSelection);
            this.Name = "BlobTestForm";
            this.Text = "Azure Blob and Continental Tire Pressure";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.bsPayload)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsBlobFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsBlobItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsVehicleInQueue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource2)).EndInit();
            this.gbSelection.ResumeLayout(false);
            this.gbSelection.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spDays)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bnFiles)).EndInit();
            this.bnFiles.ResumeLayout(false);
            this.bnFiles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcPayload)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvPayload)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator1)).EndInit();
            this.bindingNavigator1.ResumeLayout(false);
            this.bindingNavigator1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcTirePressure)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvTirePressure)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView5)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcVehiclesAndSensors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsVehiclesAndSensors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvVehiclesAndSensors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator2)).EndInit();
            this.bindingNavigator2.ResumeLayout(false);
            this.bindingNavigator2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.BindingSource bsPayload;
        private System.Windows.Forms.BindingSource bindingSource2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.BindingSource bsBlobFiles;
        private System.Windows.Forms.Timer timerShowQueueLen;
        private System.Windows.Forms.BindingSource bsVehicleInQueue;
        private System.Windows.Forms.BindingSource bsBlobItems;
        private System.Windows.Forms.GroupBox gbSelection;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button bViewData;
        private System.Windows.Forms.Button bDistribute;
        private System.Windows.Forms.CheckBox cbDetailedContiLogging;
        private System.Windows.Forms.Label lbBlobQueue;
        private System.Windows.Forms.CheckBox cbAppend;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbTestConti;
        private System.Windows.Forms.RadioButton rbProdConti;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DateTimePicker dtSinceTime;
        private System.Windows.Forms.CheckBox cbSince;
        private System.Windows.Forms.DateTimePicker dtSince;
        private System.Windows.Forms.CheckBox cbUseRecentDate;
        private System.Windows.Forms.RadioButton rbTestBlob;
        private System.Windows.Forms.TextBox tbBusFilter;
        private System.Windows.Forms.RadioButton rbProdBlob;
        private System.Windows.Forms.CheckBox cbDownload;
        private System.Windows.Forms.TextBox lbVehicle;
        private System.Windows.Forms.Label lbVehiclelabel;
        private System.Windows.Forms.CheckBox cbUseTimestampNow;
        private System.Windows.Forms.Button bOpenFile;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox tbFilename;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbFeedback2;
        private System.Windows.Forms.Button button3;
        private DevExpress.XtraGrid.GridControl gcFiles;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewFiles;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView4;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private System.Windows.Forms.BindingNavigator bnFiles;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem1;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem1;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator3;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem1;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator4;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
        private DevExpress.XtraGrid.GridControl gcPayload;
        private DevExpress.XtraGrid.Views.Grid.GridView gvPayload;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView3;
        private System.Windows.Forms.BindingNavigator bindingNavigator1;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private DevExpress.XtraGrid.GridControl gcTirePressure;
        private DevExpress.XtraGrid.Views.Grid.GridView gvTirePressure;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView5;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lbVoortgang;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private DevExpress.XtraGrid.GridControl gcVehiclesAndSensors;
        private DevExpress.XtraGrid.Views.Grid.GridView gvVehiclesAndSensors;
        private System.Windows.Forms.BindingSource bsVehiclesAndSensors;
        private System.Windows.Forms.BindingNavigator bindingNavigator2;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem1;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem2;
        private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem2;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem2;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator5;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem2;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator6;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem2;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem2;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator7;
        private System.Windows.Forms.Label lbSince;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown spDays;
        private System.Windows.Forms.CheckBox cbDetailedDistributeLogging;
        private System.Windows.Forms.CheckBox cbDistributeLoop;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
    }
}

