namespace CryptoPriceAppUI
{
    partial class MainForm
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

        private void InitializeComponent()
        {
            comboBoxPair = new ComboBox();
            btnGetPrices = new Button();
            listBoxPrices = new ListBox();
            btnSubscribe = new Button();
            btnUnsubscribe = new Button();
            lblStatus = new Label();
            SuspendLayout();
            // 
            // comboBoxPair
            // 
            comboBoxPair.FormattingEnabled = true;
            comboBoxPair.Location = new Point(12, 12);
            comboBoxPair.Name = "comboBoxPair";
            comboBoxPair.Size = new Size(121, 23);
            comboBoxPair.TabIndex = 0;
            comboBoxPair.SelectedIndexChanged += ComboBoxPair_SelectedIndexChanged;
            // 
            // btnGetPrices
            // 
            btnGetPrices.Location = new Point(139, 10);
            btnGetPrices.Name = "btnGetPrices";
            btnGetPrices.Size = new Size(75, 23);
            btnGetPrices.TabIndex = 1;
            btnGetPrices.Text = "Get Prices";
            btnGetPrices.UseVisualStyleBackColor = true;
            btnGetPrices.Click += BtnGetPrices_Click;
            // 
            // listBoxPrices
            // 
            listBoxPrices.FormattingEnabled = true;
            listBoxPrices.Location = new Point(12, 39);
            listBoxPrices.Name = "listBoxPrices";
            listBoxPrices.Size = new Size(202, 154);
            listBoxPrices.TabIndex = 2;
            // 
            // btnSubscribe
            // 
            btnSubscribe.Location = new Point(220, 10);
            btnSubscribe.Name = "btnSubscribe";
            btnSubscribe.Size = new Size(82, 23);
            btnSubscribe.TabIndex = 3;
            btnSubscribe.Text = "Subscribe";
            btnSubscribe.UseVisualStyleBackColor = true;
            btnSubscribe.Click += BtnSubscribe_Click;
            // 
            // btnUnsubscribe
            // 
            btnUnsubscribe.Location = new Point(220, 39);
            btnUnsubscribe.Name = "btnUnsubscribe";
            btnUnsubscribe.Size = new Size(82, 23);
            btnUnsubscribe.TabIndex = 0;
            btnUnsubscribe.Text = "Unsubscribe";
            btnUnsubscribe.Click += BtnUnsubscribe_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.Red;
            lblStatus.Location = new Point(10, 200);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 15);
            lblStatus.TabIndex = 4;
            // 
            // MainForm
            // 
            ClientSize = new Size(314, 224);
            Controls.Add(btnUnsubscribe);
            Controls.Add(btnSubscribe);
            Controls.Add(listBoxPrices);
            Controls.Add(btnGetPrices);
            Controls.Add(comboBoxPair);
            Controls.Add(lblStatus);
            Name = "MainForm";
            Text = "Crypto Price Checker";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxPair;
        private System.Windows.Forms.Button btnGetPrices;
        private System.Windows.Forms.ListBox listBoxPrices;
        private System.Windows.Forms.Button btnSubscribe;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnUnsubscribe;
    }
}
