using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Text.RegularExpressions;

namespace FlowSERVER1 {
    public partial class confirmPas : Form {
        private int attemptCurr = 0;        
        private MySqlConnection con = ConnectionModel.con;
        private MySqlCommand command = ConnectionModel.command;
        public confirmPas(String _custEmail = "") {
            InitializeComponent();
            label3.Text = _custEmail;

            try {

                List<Control> _buttonList = new List<Control> 
                {
                    guna2Button1,guna2Button2,guna2Button3,guna2Button4,guna2Button5,
                    guna2Button6,guna2Button7,guna2Button8,guna2Button9,guna2Button11,
                    guna2Button12,guna2Button13
                };

                String _retrieveCust = EncryptionModel.Decrypt(getInformation("CUST_PASSWORD"), "0123456789085746");
                String _asterisk = new string('*',_retrieveCust.Length-5);
                String _setupCustFirst = _retrieveCust.Remove(1,_retrieveCust.Length-5).Insert(1,_asterisk);

                Random _setupRandom = new Random();
                int _indexButtonVal = _setupRandom.Next(0,11);
                _buttonList[_indexButtonVal].Text = _setupCustFirst;
                _buttonList[_indexButtonVal].Click += (sender_e,e_args) => {
                    attemptCurr = 0;
                    resetPas _showPasswordRecovery = new resetPas(label3.Text);
                    _showPasswordRecovery.Show();
                    this.Close();
                };

                List<String> _randomlyGeneratedKey = new List<String>();

                for(int i=0; i<12; i++) {
                    var _randomString = GenerateString(_retrieveCust.Length);
                    var _addAst = new string('*', _randomString.Length - 5);
                    var _rearrangeString = _randomString.Remove(1, _randomString.Length - 5).Insert(1, _addAst);
                    _randomlyGeneratedKey.Add(_rearrangeString);
                }

                for(int i=0; i<12; i++) {
                    if(_buttonList[i] != _buttonList[_indexButtonVal]) {
                        _buttonList[i].Text = _randomlyGeneratedKey[i];
                    }
                }

            } catch (Exception) {
                MessageBox.Show("An error ocurred.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        private static Random random = new Random();
        public static string GenerateString(int length) {
            const string chars = "abcdefghijklmnopqrstuvwxyz.@-1234567890";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        private string getInformation(String _columnName) {

            List<String> _retrievedValues = new List<string>();

            String _selectValueQuery = "SELECT " + _columnName + " FROM information WHERE CUST_EMAIL = @email";
            command = new MySqlCommand(_selectValueQuery,con);
            command.Parameters.AddWithValue("@email",label3.Text);

            MySqlDataReader _readValue = command.ExecuteReader();
            if(_readValue.Read()) {
                _retrievedValues.Add(_readValue.GetString(0));
            }
            _readValue.Close();

            return _retrievedValues[0];
        }
        private void getAttemptCurr() {
            if(attemptCurr == 3) {
                attemptCurr = 0;

                Thread _failedRecoveryForm = new Thread(() => new failedPas().ShowDialog());
                _failedRecoveryForm.Start();

                Application.Exit();

            }
        }

        private void confirmPas_Load(object sender, EventArgs e) {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button10_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button8_Click(object sender, EventArgs e) {
            attemptCurr++;
            getAttemptCurr();
        }

        private void guna2Button7_Click(object sender, EventArgs e) {
            attemptCurr++;
            getAttemptCurr();
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            attemptCurr++;
            getAttemptCurr();
        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            attemptCurr++;
            getAttemptCurr();
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            attemptCurr++;
            getAttemptCurr();
        }

        private void guna2Button9_Click(object sender, EventArgs e) {
            attemptCurr++;
            getAttemptCurr();
        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            attemptCurr++;
            getAttemptCurr();
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            attemptCurr++;
            getAttemptCurr();
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            attemptCurr++;
            getAttemptCurr();
        }

        private void guna2Button12_Click(object sender, EventArgs e) {
            attemptCurr++;
            getAttemptCurr();
        }

        private void guna2Button13_Click(object sender, EventArgs e) {
            attemptCurr++;
            getAttemptCurr();
        }

        private void guna2Button11_Click(object sender, EventArgs e) {
            attemptCurr++;
            getAttemptCurr();
        }

        private void label2_Click(object sender, EventArgs e) {

        }
    }
}
