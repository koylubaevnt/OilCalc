namespace OilCalc
{
    partial class frmTest
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.tcTest = new System.Windows.Forms.TabControl();
            this.tpRoundRules = new System.Windows.Forms.TabPage();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.lResult = new System.Windows.Forms.Label();
            this.lDimension = new System.Windows.Forms.Label();
            this.lValue = new System.Windows.Forms.Label();
            this.tbDimension = new System.Windows.Forms.TextBox();
            this.tbValue = new System.Windows.Forms.TextBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.tbMethods = new System.Windows.Forms.TabPage();
            this.tbInfo = new System.Windows.Forms.TextBox();
            this.tbStep = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbEndValue = new System.Windows.Forms.TextBox();
            this.tbStartValue = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbRound = new System.Windows.Forms.TextBox();
            this.btCycle = new System.Windows.Forms.Button();
            this.tbResultCalc = new System.Windows.Forms.TextBox();
            this.lTemperature = new System.Windows.Forms.Label();
            this.lDensity = new System.Windows.Forms.Label();
            this.tbTemperature = new System.Windows.Forms.TextBox();
            this.tbDensity = new System.Windows.Forms.TextBox();
            this.btnCalc = new System.Windows.Forms.Button();
            this.cbMethod = new System.Windows.Forms.ComboBox();
            this.btnCalcCheck = new System.Windows.Forms.Button();
            this.tcTest.SuspendLayout();
            this.tpRoundRules.SuspendLayout();
            this.tbMethods.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcTest
            // 
            this.tcTest.Controls.Add(this.tpRoundRules);
            this.tcTest.Controls.Add(this.tbMethods);
            this.tcTest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcTest.Location = new System.Drawing.Point(0, 0);
            this.tcTest.Name = "tcTest";
            this.tcTest.SelectedIndex = 0;
            this.tcTest.Size = new System.Drawing.Size(588, 413);
            this.tcTest.TabIndex = 0;
            // 
            // tpRoundRules
            // 
            this.tpRoundRules.Controls.Add(this.btCycle);
            this.tpRoundRules.Controls.Add(this.label4);
            this.tpRoundRules.Controls.Add(this.tbRound);
            this.tpRoundRules.Controls.Add(this.tbStep);
            this.tpRoundRules.Controls.Add(this.label1);
            this.tpRoundRules.Controls.Add(this.label2);
            this.tpRoundRules.Controls.Add(this.label3);
            this.tpRoundRules.Controls.Add(this.tbEndValue);
            this.tpRoundRules.Controls.Add(this.tbStartValue);
            this.tpRoundRules.Controls.Add(this.tbInfo);
            this.tpRoundRules.Controls.Add(this.tbResult);
            this.tpRoundRules.Controls.Add(this.lResult);
            this.tpRoundRules.Controls.Add(this.lDimension);
            this.tpRoundRules.Controls.Add(this.lValue);
            this.tpRoundRules.Controls.Add(this.tbDimension);
            this.tpRoundRules.Controls.Add(this.tbValue);
            this.tpRoundRules.Controls.Add(this.btnTest);
            this.tpRoundRules.Location = new System.Drawing.Point(4, 22);
            this.tpRoundRules.Name = "tpRoundRules";
            this.tpRoundRules.Padding = new System.Windows.Forms.Padding(3);
            this.tpRoundRules.Size = new System.Drawing.Size(580, 387);
            this.tpRoundRules.TabIndex = 0;
            this.tpRoundRules.Text = "Округление";
            this.tpRoundRules.UseVisualStyleBackColor = true;
            // 
            // tbResult
            // 
            this.tbResult.Location = new System.Drawing.Point(82, 70);
            this.tbResult.Name = "tbResult";
            this.tbResult.Size = new System.Drawing.Size(100, 20);
            this.tbResult.TabIndex = 9;
            // 
            // lResult
            // 
            this.lResult.AutoSize = true;
            this.lResult.Location = new System.Drawing.Point(7, 73);
            this.lResult.Name = "lResult";
            this.lResult.Size = new System.Drawing.Size(59, 13);
            this.lResult.TabIndex = 8;
            this.lResult.Text = "Результат";
            // 
            // lDimension
            // 
            this.lDimension.AutoSize = true;
            this.lDimension.Location = new System.Drawing.Point(7, 46);
            this.lDimension.Name = "lDimension";
            this.lDimension.Size = new System.Drawing.Size(67, 13);
            this.lDimension.TabIndex = 7;
            this.lDimension.Text = "Округление";
            // 
            // lValue
            // 
            this.lValue.AutoSize = true;
            this.lValue.Location = new System.Drawing.Point(7, 19);
            this.lValue.Name = "lValue";
            this.lValue.Size = new System.Drawing.Size(55, 13);
            this.lValue.TabIndex = 6;
            this.lValue.Text = "Значение";
            // 
            // tbDimension
            // 
            this.tbDimension.Location = new System.Drawing.Point(82, 43);
            this.tbDimension.Name = "tbDimension";
            this.tbDimension.Size = new System.Drawing.Size(100, 20);
            this.tbDimension.TabIndex = 5;
            // 
            // tbValue
            // 
            this.tbValue.Location = new System.Drawing.Point(82, 16);
            this.tbValue.Name = "tbValue";
            this.tbValue.Size = new System.Drawing.Size(100, 20);
            this.tbValue.TabIndex = 4;
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(6, 103);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(100, 23);
            this.btnTest.TabIndex = 3;
            this.btnTest.Text = "Тест";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // tbMethods
            // 
            this.tbMethods.Controls.Add(this.btnCalcCheck);
            this.tbMethods.Controls.Add(this.cbMethod);
            this.tbMethods.Controls.Add(this.tbResultCalc);
            this.tbMethods.Controls.Add(this.lTemperature);
            this.tbMethods.Controls.Add(this.lDensity);
            this.tbMethods.Controls.Add(this.tbTemperature);
            this.tbMethods.Controls.Add(this.tbDensity);
            this.tbMethods.Controls.Add(this.btnCalc);
            this.tbMethods.Location = new System.Drawing.Point(4, 22);
            this.tbMethods.Name = "tbMethods";
            this.tbMethods.Padding = new System.Windows.Forms.Padding(3);
            this.tbMethods.Size = new System.Drawing.Size(580, 387);
            this.tbMethods.TabIndex = 1;
            this.tbMethods.Text = "Методы";
            this.tbMethods.UseVisualStyleBackColor = true;
            // 
            // tbInfo
            // 
            this.tbInfo.Location = new System.Drawing.Point(7, 133);
            this.tbInfo.Multiline = true;
            this.tbInfo.Name = "tbInfo";
            this.tbInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbInfo.Size = new System.Drawing.Size(567, 246);
            this.tbInfo.TabIndex = 10;
            // 
            // tbStep
            // 
            this.tbStep.Location = new System.Drawing.Point(321, 73);
            this.tbStep.Name = "tbStep";
            this.tbStep.Size = new System.Drawing.Size(100, 20);
            this.tbStep.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(202, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Шаг";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(202, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Конечное значение";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(202, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Начальное значение";
            // 
            // tbEndValue
            // 
            this.tbEndValue.Location = new System.Drawing.Point(321, 46);
            this.tbEndValue.Name = "tbEndValue";
            this.tbEndValue.Size = new System.Drawing.Size(100, 20);
            this.tbEndValue.TabIndex = 12;
            // 
            // tbStartValue
            // 
            this.tbStartValue.Location = new System.Drawing.Point(321, 19);
            this.tbStartValue.Name = "tbStartValue";
            this.tbStartValue.Size = new System.Drawing.Size(100, 20);
            this.tbStartValue.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(204, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Округление";
            // 
            // tbRound
            // 
            this.tbRound.Location = new System.Drawing.Point(321, 99);
            this.tbRound.Name = "tbRound";
            this.tbRound.Size = new System.Drawing.Size(100, 20);
            this.tbRound.TabIndex = 17;
            // 
            // btCycle
            // 
            this.btCycle.Location = new System.Drawing.Point(472, 96);
            this.btCycle.Name = "btCycle";
            this.btCycle.Size = new System.Drawing.Size(100, 23);
            this.btCycle.TabIndex = 19;
            this.btCycle.Text = "Тест 2";
            this.btCycle.UseVisualStyleBackColor = true;
            this.btCycle.Click += new System.EventHandler(this.btCycle_Click);
            // 
            // tbResultCalc
            // 
            this.tbResultCalc.Location = new System.Drawing.Point(7, 129);
            this.tbResultCalc.Multiline = true;
            this.tbResultCalc.Name = "tbResultCalc";
            this.tbResultCalc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbResultCalc.Size = new System.Drawing.Size(567, 246);
            this.tbResultCalc.TabIndex = 16;
            // 
            // lTemperature
            // 
            this.lTemperature.AutoSize = true;
            this.lTemperature.Location = new System.Drawing.Point(6, 76);
            this.lTemperature.Name = "lTemperature";
            this.lTemperature.Size = new System.Drawing.Size(74, 13);
            this.lTemperature.TabIndex = 15;
            this.lTemperature.Text = "Температура";
            // 
            // lDensity
            // 
            this.lDensity.AutoSize = true;
            this.lDensity.Location = new System.Drawing.Point(6, 49);
            this.lDensity.Name = "lDensity";
            this.lDensity.Size = new System.Drawing.Size(61, 13);
            this.lDensity.TabIndex = 14;
            this.lDensity.Text = "Плотность";
            // 
            // tbTemperature
            // 
            this.tbTemperature.Location = new System.Drawing.Point(81, 73);
            this.tbTemperature.Name = "tbTemperature";
            this.tbTemperature.Size = new System.Drawing.Size(100, 20);
            this.tbTemperature.TabIndex = 13;
            // 
            // tbDensity
            // 
            this.tbDensity.Location = new System.Drawing.Point(81, 46);
            this.tbDensity.Name = "tbDensity";
            this.tbDensity.Size = new System.Drawing.Size(100, 20);
            this.tbDensity.TabIndex = 12;
            // 
            // btnCalc
            // 
            this.btnCalc.Location = new System.Drawing.Point(6, 99);
            this.btnCalc.Name = "btnCalc";
            this.btnCalc.Size = new System.Drawing.Size(100, 23);
            this.btnCalc.TabIndex = 11;
            this.btnCalc.Text = "Расчет";
            this.btnCalc.UseVisualStyleBackColor = true;
            this.btnCalc.Click += new System.EventHandler(this.btnCalc_Click);
            // 
            // cbMethod
            // 
            this.cbMethod.FormattingEnabled = true;
            this.cbMethod.Location = new System.Drawing.Point(9, 17);
            this.cbMethod.Name = "cbMethod";
            this.cbMethod.Size = new System.Drawing.Size(172, 21);
            this.cbMethod.TabIndex = 17;
            // 
            // btnCalcCheck
            // 
            this.btnCalcCheck.Location = new System.Drawing.Point(112, 99);
            this.btnCalcCheck.Name = "btnCalcCheck";
            this.btnCalcCheck.Size = new System.Drawing.Size(100, 23);
            this.btnCalcCheck.TabIndex = 18;
            this.btnCalcCheck.Text = "Расчет тестов";
            this.btnCalcCheck.UseVisualStyleBackColor = true;
            this.btnCalcCheck.Click += new System.EventHandler(this.btnCalcCheck_Click);
            // 
            // frmTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(588, 413);
            this.Controls.Add(this.tcTest);
            this.Name = "frmTest";
            this.Text = "Тестовая форма";
            this.Load += new System.EventHandler(this.frmTest_Load);
            this.tcTest.ResumeLayout(false);
            this.tpRoundRules.ResumeLayout(false);
            this.tpRoundRules.PerformLayout();
            this.tbMethods.ResumeLayout(false);
            this.tbMethods.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tcTest;
        private System.Windows.Forms.TabPage tpRoundRules;
        private System.Windows.Forms.TabPage tbMethods;
        private System.Windows.Forms.TextBox tbDimension;
        private System.Windows.Forms.TextBox tbValue;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.TextBox tbResult;
        private System.Windows.Forms.Label lResult;
        private System.Windows.Forms.Label lDimension;
        private System.Windows.Forms.Label lValue;
        private System.Windows.Forms.TextBox tbInfo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbRound;
        private System.Windows.Forms.TextBox tbStep;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbEndValue;
        private System.Windows.Forms.TextBox tbStartValue;
        private System.Windows.Forms.Button btCycle;
        private System.Windows.Forms.TextBox tbResultCalc;
        private System.Windows.Forms.Label lTemperature;
        private System.Windows.Forms.Label lDensity;
        private System.Windows.Forms.TextBox tbTemperature;
        private System.Windows.Forms.TextBox tbDensity;
        private System.Windows.Forms.Button btnCalc;
        private System.Windows.Forms.ComboBox cbMethod;
        private System.Windows.Forms.Button btnCalcCheck;


    }
}