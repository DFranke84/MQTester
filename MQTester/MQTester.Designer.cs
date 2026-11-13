namespace MQTester
{
    partial class MQTester
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Connection
        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.Label lblEnvironment;
        private System.Windows.Forms.ComboBox cboEnvironment;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblQueueManager;
        private System.Windows.Forms.TextBox txtQueueManager;
        private System.Windows.Forms.Label lblChannel;
        private System.Windows.Forms.TextBox txtChannel;
        private System.Windows.Forms.Label lblQueue;
        private System.Windows.Forms.ComboBox cboQueue;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Button btnQueueDepth;
        private System.Windows.Forms.Label lblDepth;

        // Send
        private System.Windows.Forms.GroupBox grpSend;
        private System.Windows.Forms.Button btnLoadFile;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.RadioButton rbEachLine;
        private System.Windows.Forms.RadioButton rbWholeText;
        private System.Windows.Forms.Label lblDelay;
        private System.Windows.Forms.NumericUpDown nudDelay;
        private System.Windows.Forms.Label lblRepeat;
        private System.Windows.Forms.NumericUpDown nudRepeat;
        private System.Windows.Forms.Label lblPersistence;
        private System.Windows.Forms.ComboBox cboPersistence;
        private System.Windows.Forms.Label lblExpiry;
        private System.Windows.Forms.NumericUpDown nudExpiry;
        private System.Windows.Forms.Label lblReplyTo;
        private System.Windows.Forms.ComboBox cboReplyTo;
        private System.Windows.Forms.Label lblCorrelId;
        private System.Windows.Forms.TextBox txtCorrelId;
        private System.Windows.Forms.Button btnNewCorrelId;
        private System.Windows.Forms.CheckBox chkNewCorrelPerMsg;
        private System.Windows.Forms.Label lblMsgId;
        private System.Windows.Forms.TextBox txtMsgId;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label lblReplyWait;
        private System.Windows.Forms.NumericUpDown nudReplyWait;
        private System.Windows.Forms.ComboBox cboReplyMatch;
        private System.Windows.Forms.Button btnSendAndWait;

        // Read
        private System.Windows.Forms.GroupBox grpRead;
        private System.Windows.Forms.RadioButton rbBrowse;
        private System.Windows.Forms.RadioButton rbGet;
        private System.Windows.Forms.Label lblReadCount;
        private System.Windows.Forms.NumericUpDown nudReadCount;
        private System.Windows.Forms.Label lblReadCountHint;
        private System.Windows.Forms.Label lblMatchCorrelId;
        private System.Windows.Forms.TextBox txtMatchCorrelId;
        private System.Windows.Forms.Label lblMatchMsgId;
        private System.Windows.Forms.TextBox txtMatchMsgId;
        private System.Windows.Forms.Label lblReadWait;
        private System.Windows.Forms.NumericUpDown nudReadWait;
        private System.Windows.Forms.Label lblReadWaitHint;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.Button btnPurge;

        // Message + log
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Label lblLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolTip toolTip;

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
            components = new System.ComponentModel.Container();
            grpConnection = new GroupBox();
            lblEnvironment = new Label();
            cboEnvironment = new ComboBox();
            lblHost = new Label();
            txtHost = new TextBox();
            lblPort = new Label();
            txtPort = new TextBox();
            lblQueueManager = new Label();
            txtQueueManager = new TextBox();
            lblChannel = new Label();
            txtChannel = new TextBox();
            lblQueue = new Label();
            cboQueue = new ComboBox();
            lblUsername = new Label();
            txtUsername = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            btnTestConnection = new Button();
            btnQueueDepth = new Button();
            lblDepth = new Label();
            grpSend = new GroupBox();
            btnLoadFile = new Button();
            btnClear = new Button();
            rbEachLine = new RadioButton();
            rbWholeText = new RadioButton();
            lblDelay = new Label();
            nudDelay = new NumericUpDown();
            lblRepeat = new Label();
            nudRepeat = new NumericUpDown();
            lblPersistence = new Label();
            cboPersistence = new ComboBox();
            lblExpiry = new Label();
            nudExpiry = new NumericUpDown();
            lblReplyTo = new Label();
            cboReplyTo = new ComboBox();
            lblCorrelId = new Label();
            txtCorrelId = new TextBox();
            btnNewCorrelId = new Button();
            chkNewCorrelPerMsg = new CheckBox();
            lblMsgId = new Label();
            txtMsgId = new TextBox();
            btnSend = new Button();
            lblReplyWait = new Label();
            nudReplyWait = new NumericUpDown();
            cboReplyMatch = new ComboBox();
            btnSendAndWait = new Button();
            grpRead = new GroupBox();
            rbBrowse = new RadioButton();
            rbGet = new RadioButton();
            lblReadCount = new Label();
            nudReadCount = new NumericUpDown();
            lblReadCountHint = new Label();
            lblMatchCorrelId = new Label();
            txtMatchCorrelId = new TextBox();
            lblMatchMsgId = new Label();
            txtMatchMsgId = new TextBox();
            lblReadWait = new Label();
            nudReadWait = new NumericUpDown();
            lblReadWaitHint = new Label();
            btnRead = new Button();
            btnPurge = new Button();
            lblMessage = new Label();
            btnCancel = new Button();
            txtMessage = new TextBox();
            lblLog = new Label();
            txtLog = new TextBox();
            statusStrip = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            progressBar = new ToolStripProgressBar();
            openFileDialog = new OpenFileDialog();
            toolTip = new ToolTip(components);
            grpConnection.SuspendLayout();
            grpSend.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudDelay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudRepeat).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudExpiry).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudReplyWait).BeginInit();
            grpRead.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudReadCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudReadWait).BeginInit();
            statusStrip.SuspendLayout();
            SuspendLayout();
            //
            // grpConnection
            //
            grpConnection.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpConnection.Controls.Add(lblEnvironment);
            grpConnection.Controls.Add(cboEnvironment);
            grpConnection.Controls.Add(lblHost);
            grpConnection.Controls.Add(txtHost);
            grpConnection.Controls.Add(lblPort);
            grpConnection.Controls.Add(txtPort);
            grpConnection.Controls.Add(lblQueueManager);
            grpConnection.Controls.Add(txtQueueManager);
            grpConnection.Controls.Add(lblChannel);
            grpConnection.Controls.Add(txtChannel);
            grpConnection.Controls.Add(lblQueue);
            grpConnection.Controls.Add(cboQueue);
            grpConnection.Controls.Add(lblUsername);
            grpConnection.Controls.Add(txtUsername);
            grpConnection.Controls.Add(lblPassword);
            grpConnection.Controls.Add(txtPassword);
            grpConnection.Controls.Add(btnTestConnection);
            grpConnection.Controls.Add(btnQueueDepth);
            grpConnection.Controls.Add(lblDepth);
            grpConnection.Location = new Point(12, 8);
            grpConnection.Name = "grpConnection";
            grpConnection.Size = new Size(1176, 118);
            grpConnection.TabIndex = 0;
            grpConnection.TabStop = false;
            grpConnection.Text = "Connection";
            //
            // lblEnvironment
            //
            lblEnvironment.AutoSize = true;
            lblEnvironment.Location = new Point(10, 27);
            lblEnvironment.Name = "lblEnvironment";
            lblEnvironment.Text = "Environment:";
            //
            // cboEnvironment
            //
            cboEnvironment.DropDownStyle = ComboBoxStyle.DropDownList;
            cboEnvironment.Location = new Point(110, 24);
            cboEnvironment.Name = "cboEnvironment";
            cboEnvironment.Size = new Size(260, 23);
            cboEnvironment.TabIndex = 0;
            cboEnvironment.SelectedIndexChanged += cboEnvironment_SelectedIndexChanged;
            //
            // lblHost
            //
            lblHost.AutoSize = true;
            lblHost.Location = new Point(10, 57);
            lblHost.Name = "lblHost";
            lblHost.Text = "Host:";
            //
            // txtHost
            //
            txtHost.Location = new Point(110, 54);
            txtHost.Name = "txtHost";
            txtHost.Size = new Size(260, 23);
            txtHost.TabIndex = 1;
            //
            // lblPort
            //
            lblPort.AutoSize = true;
            lblPort.Location = new Point(10, 87);
            lblPort.Name = "lblPort";
            lblPort.Text = "Port:";
            //
            // txtPort
            //
            txtPort.Location = new Point(110, 84);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(80, 23);
            txtPort.TabIndex = 2;
            //
            // lblQueueManager
            //
            lblQueueManager.AutoSize = true;
            lblQueueManager.Location = new Point(390, 27);
            lblQueueManager.Name = "lblQueueManager";
            lblQueueManager.Text = "Queue Manager:";
            //
            // txtQueueManager
            //
            txtQueueManager.Location = new Point(500, 24);
            txtQueueManager.Name = "txtQueueManager";
            txtQueueManager.Size = new Size(220, 23);
            txtQueueManager.TabIndex = 3;
            //
            // lblChannel
            //
            lblChannel.AutoSize = true;
            lblChannel.Location = new Point(390, 57);
            lblChannel.Name = "lblChannel";
            lblChannel.Text = "Channel:";
            //
            // txtChannel
            //
            txtChannel.Location = new Point(500, 54);
            txtChannel.Name = "txtChannel";
            txtChannel.Size = new Size(220, 23);
            txtChannel.TabIndex = 4;
            //
            // lblQueue
            //
            lblQueue.AutoSize = true;
            lblQueue.Location = new Point(390, 87);
            lblQueue.Name = "lblQueue";
            lblQueue.Text = "Queue:";
            //
            // cboQueue
            //
            cboQueue.Location = new Point(500, 84);
            cboQueue.Name = "cboQueue";
            cboQueue.Size = new Size(220, 23);
            cboQueue.TabIndex = 5;
            //
            // lblUsername
            //
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(740, 27);
            lblUsername.Name = "lblUsername";
            lblUsername.Text = "Username:";
            //
            // txtUsername
            //
            txtUsername.Location = new Point(820, 24);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(160, 23);
            txtUsername.TabIndex = 6;
            //
            // lblPassword
            //
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(740, 57);
            lblPassword.Name = "lblPassword";
            lblPassword.Text = "Password:";
            //
            // txtPassword
            //
            txtPassword.Location = new Point(820, 54);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(160, 23);
            txtPassword.TabIndex = 7;
            //
            // btnTestConnection
            //
            btnTestConnection.Location = new Point(1000, 22);
            btnTestConnection.Name = "btnTestConnection";
            btnTestConnection.Size = new Size(165, 27);
            btnTestConnection.TabIndex = 8;
            btnTestConnection.Text = "Test Connection";
            btnTestConnection.UseVisualStyleBackColor = true;
            btnTestConnection.Click += btnTestConnection_Click;
            //
            // btnQueueDepth
            //
            btnQueueDepth.Location = new Point(1000, 52);
            btnQueueDepth.Name = "btnQueueDepth";
            btnQueueDepth.Size = new Size(165, 27);
            btnQueueDepth.TabIndex = 9;
            btnQueueDepth.Text = "Queue Depth";
            btnQueueDepth.UseVisualStyleBackColor = true;
            btnQueueDepth.Click += btnQueueDepth_Click;
            //
            // lblDepth
            //
            lblDepth.AutoSize = true;
            lblDepth.Location = new Point(1000, 88);
            lblDepth.Name = "lblDepth";
            lblDepth.Text = "Depth: -";
            //
            // grpSend
            //
            grpSend.Controls.Add(btnLoadFile);
            grpSend.Controls.Add(btnClear);
            grpSend.Controls.Add(rbEachLine);
            grpSend.Controls.Add(rbWholeText);
            grpSend.Controls.Add(lblDelay);
            grpSend.Controls.Add(nudDelay);
            grpSend.Controls.Add(lblRepeat);
            grpSend.Controls.Add(nudRepeat);
            grpSend.Controls.Add(lblPersistence);
            grpSend.Controls.Add(cboPersistence);
            grpSend.Controls.Add(lblExpiry);
            grpSend.Controls.Add(nudExpiry);
            grpSend.Controls.Add(lblReplyTo);
            grpSend.Controls.Add(cboReplyTo);
            grpSend.Controls.Add(lblCorrelId);
            grpSend.Controls.Add(txtCorrelId);
            grpSend.Controls.Add(btnNewCorrelId);
            grpSend.Controls.Add(chkNewCorrelPerMsg);
            grpSend.Controls.Add(lblMsgId);
            grpSend.Controls.Add(txtMsgId);
            grpSend.Controls.Add(btnSend);
            grpSend.Controls.Add(lblReplyWait);
            grpSend.Controls.Add(nudReplyWait);
            grpSend.Controls.Add(cboReplyMatch);
            grpSend.Controls.Add(btnSendAndWait);
            grpSend.Location = new Point(12, 132);
            grpSend.Name = "grpSend";
            grpSend.Size = new Size(700, 228);
            grpSend.TabIndex = 1;
            grpSend.TabStop = false;
            grpSend.Text = "Send";
            //
            // btnLoadFile
            //
            btnLoadFile.Location = new Point(10, 20);
            btnLoadFile.Name = "btnLoadFile";
            btnLoadFile.Size = new Size(110, 27);
            btnLoadFile.TabIndex = 0;
            btnLoadFile.Text = "Load File...";
            btnLoadFile.UseVisualStyleBackColor = true;
            btnLoadFile.Click += btnLoadFile_Click;
            //
            // btnClear
            //
            btnClear.Location = new Point(126, 20);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(80, 27);
            btnClear.TabIndex = 1;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            //
            // rbEachLine
            //
            rbEachLine.AutoSize = true;
            rbEachLine.Checked = true;
            rbEachLine.Location = new Point(225, 24);
            rbEachLine.Name = "rbEachLine";
            rbEachLine.TabIndex = 2;
            rbEachLine.TabStop = true;
            rbEachLine.Text = "Each line = one message";
            rbEachLine.UseVisualStyleBackColor = true;
            //
            // rbWholeText
            //
            rbWholeText.AutoSize = true;
            rbWholeText.Location = new Point(420, 24);
            rbWholeText.Name = "rbWholeText";
            rbWholeText.TabIndex = 3;
            rbWholeText.Text = "Whole text = one message";
            rbWholeText.UseVisualStyleBackColor = true;
            //
            // lblDelay
            //
            lblDelay.AutoSize = true;
            lblDelay.Location = new Point(10, 61);
            lblDelay.Name = "lblDelay";
            lblDelay.Text = "Delay (ms):";
            //
            // nudDelay
            //
            nudDelay.Location = new Point(110, 58);
            nudDelay.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            nudDelay.Name = "nudDelay";
            nudDelay.Size = new Size(80, 23);
            nudDelay.TabIndex = 4;
            //
            // lblRepeat
            //
            lblRepeat.AutoSize = true;
            lblRepeat.Location = new Point(205, 61);
            lblRepeat.Name = "lblRepeat";
            lblRepeat.Text = "Repeat:";
            //
            // nudRepeat
            //
            nudRepeat.Location = new Point(265, 58);
            nudRepeat.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            nudRepeat.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudRepeat.Name = "nudRepeat";
            nudRepeat.Size = new Size(70, 23);
            nudRepeat.TabIndex = 5;
            nudRepeat.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            // lblPersistence
            //
            lblPersistence.AutoSize = true;
            lblPersistence.Location = new Point(350, 61);
            lblPersistence.Name = "lblPersistence";
            lblPersistence.Text = "Persistence:";
            //
            // cboPersistence
            //
            cboPersistence.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPersistence.Items.AddRange(new object[] { "Queue default", "Persistent", "Not persistent" });
            cboPersistence.Location = new Point(440, 58);
            cboPersistence.Name = "cboPersistence";
            cboPersistence.Size = new Size(150, 23);
            cboPersistence.TabIndex = 6;
            //
            // lblExpiry
            //
            lblExpiry.AutoSize = true;
            lblExpiry.Location = new Point(10, 93);
            lblExpiry.Name = "lblExpiry";
            lblExpiry.Text = "Expiry (sec):";
            //
            // nudExpiry
            //
            nudExpiry.Location = new Point(110, 90);
            nudExpiry.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            nudExpiry.Name = "nudExpiry";
            nudExpiry.Size = new Size(80, 23);
            nudExpiry.TabIndex = 7;
            //
            // lblReplyTo
            //
            lblReplyTo.AutoSize = true;
            lblReplyTo.Location = new Point(205, 93);
            lblReplyTo.Name = "lblReplyTo";
            lblReplyTo.Text = "Reply-to:";
            //
            // cboReplyTo
            //
            cboReplyTo.Location = new Point(265, 90);
            cboReplyTo.Name = "cboReplyTo";
            cboReplyTo.Size = new Size(325, 23);
            cboReplyTo.TabIndex = 8;
            //
            // lblCorrelId
            //
            lblCorrelId.AutoSize = true;
            lblCorrelId.Location = new Point(10, 125);
            lblCorrelId.Name = "lblCorrelId";
            lblCorrelId.Text = "CorrelId:";
            //
            // txtCorrelId
            //
            txtCorrelId.Font = new Font("Consolas", 9F);
            txtCorrelId.Location = new Point(110, 122);
            txtCorrelId.Name = "txtCorrelId";
            txtCorrelId.Size = new Size(380, 22);
            txtCorrelId.TabIndex = 9;
            //
            // btnNewCorrelId
            //
            btnNewCorrelId.Location = new Point(496, 120);
            btnNewCorrelId.Name = "btnNewCorrelId";
            btnNewCorrelId.Size = new Size(60, 26);
            btnNewCorrelId.TabIndex = 10;
            btnNewCorrelId.Text = "New";
            btnNewCorrelId.UseVisualStyleBackColor = true;
            btnNewCorrelId.Click += btnNewCorrelId_Click;
            //
            // chkNewCorrelPerMsg
            //
            chkNewCorrelPerMsg.AutoSize = true;
            chkNewCorrelPerMsg.Location = new Point(565, 124);
            chkNewCorrelPerMsg.Name = "chkNewCorrelPerMsg";
            chkNewCorrelPerMsg.TabIndex = 11;
            chkNewCorrelPerMsg.Text = "New per msg";
            chkNewCorrelPerMsg.UseVisualStyleBackColor = true;
            //
            // lblMsgId
            //
            lblMsgId.AutoSize = true;
            lblMsgId.Location = new Point(10, 157);
            lblMsgId.Name = "lblMsgId";
            lblMsgId.Text = "MsgId:";
            //
            // txtMsgId
            //
            txtMsgId.Font = new Font("Consolas", 9F);
            txtMsgId.Location = new Point(110, 154);
            txtMsgId.Name = "txtMsgId";
            txtMsgId.Size = new Size(380, 22);
            txtMsgId.TabIndex = 12;
            //
            // btnSend
            //
            btnSend.Location = new Point(10, 186);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(150, 30);
            btnSend.TabIndex = 13;
            btnSend.Text = "Send Messages";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            //
            // lblReplyWait
            //
            lblReplyWait.AutoSize = true;
            lblReplyWait.Location = new Point(180, 194);
            lblReplyWait.Name = "lblReplyWait";
            lblReplyWait.Text = "Reply wait (s):";
            //
            // nudReplyWait
            //
            nudReplyWait.Location = new Point(275, 190);
            nudReplyWait.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            nudReplyWait.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudReplyWait.Name = "nudReplyWait";
            nudReplyWait.Size = new Size(60, 23);
            nudReplyWait.TabIndex = 14;
            nudReplyWait.Value = new decimal(new int[] { 30, 0, 0, 0 });
            //
            // cboReplyMatch
            //
            cboReplyMatch.DropDownStyle = ComboBoxStyle.DropDownList;
            cboReplyMatch.Items.AddRange(new object[] { "Reply CorrelId = my MsgId", "Reply CorrelId = my CorrelId" });
            cboReplyMatch.Location = new Point(345, 190);
            cboReplyMatch.Name = "cboReplyMatch";
            cboReplyMatch.Size = new Size(170, 23);
            cboReplyMatch.TabIndex = 15;
            //
            // btnSendAndWait
            //
            btnSendAndWait.Location = new Point(522, 186);
            btnSendAndWait.Name = "btnSendAndWait";
            btnSendAndWait.Size = new Size(168, 30);
            btnSendAndWait.TabIndex = 16;
            btnSendAndWait.Text = "Send && Wait for Reply";
            btnSendAndWait.UseVisualStyleBackColor = true;
            btnSendAndWait.Click += btnSendAndWait_Click;
            //
            // grpRead
            //
            grpRead.Controls.Add(rbBrowse);
            grpRead.Controls.Add(rbGet);
            grpRead.Controls.Add(lblReadCount);
            grpRead.Controls.Add(nudReadCount);
            grpRead.Controls.Add(lblReadCountHint);
            grpRead.Controls.Add(lblMatchCorrelId);
            grpRead.Controls.Add(txtMatchCorrelId);
            grpRead.Controls.Add(lblMatchMsgId);
            grpRead.Controls.Add(txtMatchMsgId);
            grpRead.Controls.Add(lblReadWait);
            grpRead.Controls.Add(nudReadWait);
            grpRead.Controls.Add(lblReadWaitHint);
            grpRead.Controls.Add(btnRead);
            grpRead.Controls.Add(btnPurge);
            grpRead.Location = new Point(724, 132);
            grpRead.Name = "grpRead";
            grpRead.Size = new Size(464, 228);
            grpRead.TabIndex = 2;
            grpRead.TabStop = false;
            grpRead.Text = "Read";
            //
            // rbBrowse
            //
            rbBrowse.AutoSize = true;
            rbBrowse.Checked = true;
            rbBrowse.Location = new Point(10, 24);
            rbBrowse.Name = "rbBrowse";
            rbBrowse.TabIndex = 0;
            rbBrowse.TabStop = true;
            rbBrowse.Text = "Browse (leave on queue)";
            rbBrowse.UseVisualStyleBackColor = true;
            //
            // rbGet
            //
            rbGet.AutoSize = true;
            rbGet.Location = new Point(200, 24);
            rbGet.Name = "rbGet";
            rbGet.TabIndex = 1;
            rbGet.Text = "Get (remove from queue)";
            rbGet.UseVisualStyleBackColor = true;
            //
            // lblReadCount
            //
            lblReadCount.AutoSize = true;
            lblReadCount.Location = new Point(10, 61);
            lblReadCount.Name = "lblReadCount";
            lblReadCount.Text = "Max messages:";
            //
            // nudReadCount
            //
            nudReadCount.Location = new Point(120, 58);
            nudReadCount.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            nudReadCount.Name = "nudReadCount";
            nudReadCount.Size = new Size(80, 23);
            nudReadCount.TabIndex = 2;
            nudReadCount.Value = new decimal(new int[] { 10, 0, 0, 0 });
            //
            // lblReadCountHint
            //
            lblReadCountHint.AutoSize = true;
            lblReadCountHint.ForeColor = SystemColors.GrayText;
            lblReadCountHint.Location = new Point(206, 61);
            lblReadCountHint.Name = "lblReadCountHint";
            lblReadCountHint.Text = "0 = all";
            //
            // lblMatchCorrelId
            //
            lblMatchCorrelId.AutoSize = true;
            lblMatchCorrelId.Location = new Point(10, 93);
            lblMatchCorrelId.Name = "lblMatchCorrelId";
            lblMatchCorrelId.Text = "Match CorrelId:";
            //
            // txtMatchCorrelId
            //
            txtMatchCorrelId.Font = new Font("Consolas", 9F);
            txtMatchCorrelId.Location = new Point(120, 90);
            txtMatchCorrelId.Name = "txtMatchCorrelId";
            txtMatchCorrelId.Size = new Size(330, 22);
            txtMatchCorrelId.TabIndex = 3;
            //
            // lblMatchMsgId
            //
            lblMatchMsgId.AutoSize = true;
            lblMatchMsgId.Location = new Point(10, 125);
            lblMatchMsgId.Name = "lblMatchMsgId";
            lblMatchMsgId.Text = "Match MsgId:";
            //
            // txtMatchMsgId
            //
            txtMatchMsgId.Font = new Font("Consolas", 9F);
            txtMatchMsgId.Location = new Point(120, 122);
            txtMatchMsgId.Name = "txtMatchMsgId";
            txtMatchMsgId.Size = new Size(330, 22);
            txtMatchMsgId.TabIndex = 4;
            //
            // lblReadWait
            //
            lblReadWait.AutoSize = true;
            lblReadWait.Location = new Point(10, 157);
            lblReadWait.Name = "lblReadWait";
            lblReadWait.Text = "Wait (sec):";
            //
            // nudReadWait
            //
            nudReadWait.Location = new Point(120, 154);
            nudReadWait.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            nudReadWait.Name = "nudReadWait";
            nudReadWait.Size = new Size(80, 23);
            nudReadWait.TabIndex = 5;
            //
            // lblReadWaitHint
            //
            lblReadWaitHint.AutoSize = true;
            lblReadWaitHint.ForeColor = SystemColors.GrayText;
            lblReadWaitHint.Location = new Point(206, 157);
            lblReadWaitHint.Name = "lblReadWaitHint";
            lblReadWaitHint.Text = "for the first message (0 = don't wait)";
            //
            // btnRead
            //
            btnRead.Location = new Point(10, 186);
            btnRead.Name = "btnRead";
            btnRead.Size = new Size(150, 30);
            btnRead.TabIndex = 6;
            btnRead.Text = "Read Messages";
            btnRead.UseVisualStyleBackColor = true;
            btnRead.Click += btnRead_Click;
            //
            // btnPurge
            //
            btnPurge.Location = new Point(300, 186);
            btnPurge.Name = "btnPurge";
            btnPurge.Size = new Size(150, 30);
            btnPurge.TabIndex = 7;
            btnPurge.Text = "Purge Queue...";
            btnPurge.UseVisualStyleBackColor = true;
            btnPurge.Click += btnPurge_Click;
            //
            // lblMessage
            //
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(12, 372);
            lblMessage.Name = "lblMessage";
            lblMessage.Text = "Message(s) to send - paste, type or load a file. In \"each line\" mode, blank lines and lines starting with -- are skipped.";
            //
            // btnCancel
            //
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.Enabled = false;
            btnCancel.Location = new Point(1068, 366);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(120, 27);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            //
            // txtMessage
            //
            txtMessage.AcceptsReturn = true;
            txtMessage.AcceptsTab = true;
            txtMessage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtMessage.Font = new Font("Consolas", 9.75F);
            txtMessage.Location = new Point(12, 398);
            txtMessage.MaxLength = 0;
            txtMessage.Multiline = true;
            txtMessage.Name = "txtMessage";
            txtMessage.ScrollBars = ScrollBars.Both;
            txtMessage.Size = new Size(1176, 270);
            txtMessage.TabIndex = 4;
            txtMessage.WordWrap = false;
            //
            // lblLog
            //
            lblLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblLog.AutoSize = true;
            lblLog.Location = new Point(12, 674);
            lblLog.Name = "lblLog";
            lblLog.Text = "Log:";
            //
            // txtLog
            //
            txtLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.Font = new Font("Consolas", 9F);
            txtLog.Location = new Point(12, 692);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Both;
            txtLog.Size = new Size(1176, 138);
            txtLog.TabIndex = 5;
            txtLog.WordWrap = false;
            //
            // statusStrip
            //
            statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus, progressBar });
            statusStrip.Location = new Point(0, 838);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(1200, 22);
            statusStrip.TabIndex = 6;
            //
            // lblStatus
            //
            lblStatus.Name = "lblStatus";
            lblStatus.Spring = true;
            lblStatus.Text = "Ready";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            //
            // progressBar
            //
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(200, 16);
            progressBar.Visible = false;
            //
            // openFileDialog
            //
            openFileDialog.Filter = "Text files (*.txt;*.xml;*.json;*.csv)|*.txt;*.xml;*.json;*.csv|All files (*.*)|*.*";
            //
            // MQTester
            //
            ClientSize = new Size(1200, 860);
            Controls.Add(grpConnection);
            Controls.Add(grpSend);
            Controls.Add(grpRead);
            Controls.Add(lblMessage);
            Controls.Add(btnCancel);
            Controls.Add(txtMessage);
            Controls.Add(lblLog);
            Controls.Add(txtLog);
            Controls.Add(statusStrip);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MinimumSize = new Size(1216, 700);
            Name = "MQTester";
            Text = "IBM MQ Tester";
            FormClosing += MQTester_FormClosing;
            grpConnection.ResumeLayout(false);
            grpConnection.PerformLayout();
            grpSend.ResumeLayout(false);
            grpSend.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudDelay).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudRepeat).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudExpiry).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudReplyWait).EndInit();
            grpRead.ResumeLayout(false);
            grpRead.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudReadCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudReadWait).EndInit();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
