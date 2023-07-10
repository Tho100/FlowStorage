using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MySql.Data.MySqlClient;
using System.Threading;

using FlowSERVER1.Authentication;
using FlowSERVER1.AlertForms;
using FlowSERVER1.Global;

namespace FlowSERVER1 {
    public partial class SignInForm : Form {

        public static SignInForm instance;

        private readonly SignUpForm mainForm;

        private readonly HomePage accessHomePage = HomePage.instance;

        private readonly MySqlConnection con = ConnectionModel.con;

        private string returnedAuth0 {get; set; }
        private string returnedAuth1 {get; set; }
        private string CurrentLang {get; set; } = null;
        private string custEmail {get; set; }
        private string custUsername { get; set; }
        private string inputGetEmail {get; set; }
        private int attemptCurr {get; set; } = 0;
        private HomePage _form {get; set; } =  HomePage.instance;

        public SignInForm(SignUpForm mainForm) {
            InitializeComponent();

            this.mainForm = mainForm;
                
            instance = this;
        }

        public void setupAutoLogin(String _custUsername) {

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";

            if (!Directory.Exists(appDataPath)) {

                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);
                using (StreamWriter _performWrite = File.CreateText(appDataPath + "\\CUST_DATAS.txt")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(_custUsername));
                }
                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            else {
                Directory.Delete(appDataPath, true);
                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);
                using (StreamWriter _performWrite = File.CreateText(appDataPath + "\\CUST_DATAS.txt")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(_custUsername));
                }
                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
        }

        private string returnAuthValues(string whichColumn) {

            var concludeValue = new List<string>();

            string checkPasswordQuery = $"SELECT {whichColumn} FROM information WHERE CUST_EMAIL = @email";
            using (var command = new MySqlCommand(checkPasswordQuery, con)) {
                command.Parameters.AddWithValue("@email", inputGetEmail);
                using (var readerPass = command.ExecuteReader()) {
                    while (readerPass.Read()) {
                        concludeValue.Add(readerPass.GetString(0));
                    }
                }
            }

            return concludeValue.FirstOrDefault() ?? string.Empty;
        }

        private async void setupRedundane() {

            var flowLayout = accessHomePage.flwLayoutHome;
            var garbageButton = accessHomePage.btnGarbageImage;
            var itsEmptyHereLabel = accessHomePage.lblEmptyHere;

            const string selectUserQuery = "SELECT CUST_USERNAME FROM information WHERE CUST_EMAIL = @email";
            const string selectEmailQuery = "SELECT CUST_EMAIL FROM information WHERE CUST_USERNAME = @username";

            using (var command = new MySqlCommand(selectUserQuery, con)) {
                command.Parameters.AddWithValue("@email", inputGetEmail);
                using(MySqlDataReader readerUsername = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while(await readerUsername.ReadAsync()) {
                        custUsername = readerUsername.GetString(0);
                    }
                }
            }

            using (var emailCommand = new MySqlCommand(selectEmailQuery, con)) {
                emailCommand.Parameters.AddWithValue("@username", custUsername);
                using(MySqlDataReader reader = (MySqlDataReader) await emailCommand.ExecuteReaderAsync()) {
                    while(await reader.ReadAsync()) {
                        if(reader.GetString(0) != null) {
                            custEmail = reader.GetString(0);
                        }
                    }
                }
            }

            flowLayout.Controls.Clear();
            accessHomePage.lstFoldersPage.Items.Clear();
            Globals.custUsername = custUsername;
            Globals.custEmail = custEmail;

            garbageButton.Visible = itsEmptyHereLabel.Visible = label4.Visible = false;

            buildGreetingLabel();

            if(flowLayout.Controls.Count == 0) {
                buildEmptyBody();
            }

        }

        private void buildEmptyBody() {
            accessHomePage.lblEmptyHere.Visible = true;
            accessHomePage.btnGarbageImage.Visible = true;
        }

        private async Task generateUserFolder(String userName) {

            string[] itemFolder = { "Home", "Shared To Me", "Shared Files" };
            _form.lstFoldersPage.Items.AddRange(itemFolder);
            _form.lstFoldersPage.SelectedIndex = 0;

            List<string> updatesTitle = new List<string>();

            const string getTitles = "SELECT DISTINCT FOLDER_TITLE FROM folder_upload_info WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(getTitles, ConnectionModel.con)) {
                command.Parameters.AddWithValue("@username", userName);
                using (MySqlDataReader fold_Reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await fold_Reader.ReadAsync()) {
                        updatesTitle.Add(EncryptionModel.Decrypt(fold_Reader.GetString(0)));
                    }
                }
            }

            foreach (string titleEach in updatesTitle) {
                _form.lstFoldersPage.Items.Add(titleEach);
            }
        }

        /// <summary>
        /// 
        /// Verify user input authentication and process
        /// sign in verification and load user data
        /// 
        /// </summary>
        /// <returns>verifyStatus</returns>
        private async Task<bool> verifyAuthentication() {

            var _getEmail = guna2TextBox1.Text;
            inputGetEmail = _getEmail;

            var inputAuth0 = guna2TextBox2.Text;
            var inputAuth1 = guna2TextBox4.Text;

            bool authenticationSuccessful = false;

            try {

                if (!string.IsNullOrEmpty(returnAuthValues("CUST_PASSWORD"))) {
                    returnedAuth0 = returnAuthValues("CUST_PASSWORD");
                    if (!string.IsNullOrEmpty(returnAuthValues("CUST_PIN"))) {
                        returnedAuth1 = returnAuthValues("CUST_PIN");
                    }
                }

            }
            catch (Exception) {
                label4.Visible = true;
            }

            if (EncryptionModel.computeAuthCase(inputAuth0) == returnedAuth0 &&
                EncryptionModel.computeAuthCase(inputAuth1) == returnedAuth1) {

                authenticationSuccessful = true;

                setupRedundane();

                this.Close();

                Thread _retrievalAlertForm = new Thread(() => new RetrievalAlert("Connecting to your account...", "login").ShowDialog());
                _retrievalAlertForm.Start();

                await getCurrentLang();
                setupUILanguage(CurrentLang);
                buildGreetingLabel();

                try {

                    HomePage.instance.lblItemCountText.Text = HomePage.instance.flwLayoutHome.Controls.Count.ToString();

                    GlobalsData.fileTypeValuesSharedToMe.Clear();
                    GlobalsData.fileTypeValuesSharedToOthers.Clear();

                    await generateUserData();

                    RetrievalAlert retrievalAlertForm = Application.OpenForms.OfType<RetrievalAlert>().FirstOrDefault();
                    retrievalAlertForm?.Close();

                    if (guna2CheckBox2.Checked) {
                        setupAutoLogin(Globals.custUsername);
                    }

                }  catch (Exception) {
                    MessageBox.Show(
                        "An error occurred. Check your internet connection and try again.",
                        "Flowstorage",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            else {
                closeFormOnLimit();
            }

            return authenticationSuccessful;
        }

        /// <summary>
        /// 
        /// Login failed for 5 attempts then close the form.
        /// 
        /// </summary>
        private void closeFormOnLimit() {
            label4.Visible = true;
            if (attemptCurr == 5) {
                this.Close();
            }
        }

        /// <summary>
        /// 
        /// Generate user data into HomePage 
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task generateUserData() {

            await generateUserFolder(custUsername);
        }

        private void HomePage_HomePageClosed(object sender, FormClosedEventArgs e) {
            if (Application.OpenForms.Count >= 1) {
                Application.Exit(); 
            }
        }

        private void showHomePage() {
            HomePage.instance.FormClosed += HomePage_HomePageClosed;
            HomePage.instance.Show();
            mainForm.Hide();
            Close();
        }

        private void label4_Click(object sender, EventArgs e) {

        }

        /// <summary>
        /// 
        /// Perform authentication and fetch user data on login
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void guna2Button2_Click(object sender, EventArgs e) {

            try {
                
                attemptCurr++;

                var verifyAuthenticaton = await verifyAuthentication();
                if(verifyAuthenticaton) {

                    int calculatePercentageUsage = 0;
                    if (int.TryParse(HomePage.instance.lblItemCountText.Text, out int getCurrentCount) && int.TryParse(await getAccountTypeNumber(), out int getLimitedValue)) {
                        if (getLimitedValue == 0) {
                            HomePage.instance.lblUsagePercentage.Text = "0%";
                        }
                        else {
                            calculatePercentageUsage = (getCurrentCount * 100) / getLimitedValue;
                            HomePage.instance.lblUsagePercentage.Text = calculatePercentageUsage.ToString() + "%";
                        }
                        HomePage.instance.lblLimitUploadText.Text = await getAccountTypeNumber();
                        HomePage.instance.progressBarUsageStorage.Value = calculatePercentageUsage;
                    }

                    showHomePage();

                }

            }
            catch (Exception) {
                new CustomAlert(title: "Failed to sign-in to your account", subheader: "Are you connected to the internet?").Show();
            }

        }

        private async Task<string> getAccountTypeNumber() {

            string accountType = "";

            const string querySelectType = "SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(querySelectType, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                accountType = Convert.ToString(await command.ExecuteScalarAsync());
            }

            Globals.accountType = accountType;
            return Globals.uploadFileLimit[accountType].ToString();
        }

        /// <summary>
        /// Setup UI languages for labels
        /// </summary>
        /// <param name="_custLang"></param>
        private void setupUILanguage(String _custLang) {
            var Form_1 = HomePage.instance;
            if (_custLang == "MY") {

                Form_1.label2.Text = "Kiraan Item";
                Form_1.lblUpload.Text = "Muat-Naik";
                Form_1.btnUploadFile.Text = "Muat-Naik Fail";
                Form_1.btnUploadFolder.Text = "Muat-Naik Folder";
                Form_1.btnCreateDirectory.Text = "Buat Direktori";
                Form_1.btnFileSharing.Text = "Perkongsian Fail";
                Form_1.btnFileSharing.Size = new Size(159, 47);
                Form_1.lblEssentials.Text = "Kepentingan";
            }

            if (_custLang == "US") {

                Form_1.label2.Text = "Item Count";
                Form_1.lblUpload.Text = "Upload";
                Form_1.btnUploadFile.Text = "Upload File";
                Form_1.btnUploadFolder.Text = "Upload Folder";
                Form_1.btnCreateDirectory.Text = "Create Directory";
                Form_1.btnFileSharing.Text = "File Sharing";
                Form_1.btnFileSharing.Size = new Size(125, 47);
                Form_1.lblEssentials.Text = "Essentials";
            }

            if (_custLang == "GER") {
                Form_1.lblUpload.Text = "Hochladen";
                Form_1.label2.Text = "Stückzahl";
                Form_1.btnUploadFile.Text = "Datei hochladen";
                Form_1.btnUploadFolder.Text = "Ordner hochladen";
                Form_1.btnCreateDirectory.Text = "Verzeichnis erstellen";
                Form_1.btnFileSharing.Text = "Datenaustausch";
                Form_1.btnFileSharing.Size = new Size(159, 47);
                Form_1.lblEssentials.Text = "Essentials";
            }

            if (_custLang == "JAP") {
                Form_1.lblUpload.Text = "アップロード";
                Form_1.label2.Text = "アイテム数";
                Form_1.btnUploadFile.Text = "ファイルをアップロードする";
                Form_1.btnUploadFolder.Text = "フォルダのアップロード";
                Form_1.btnCreateDirectory.Text = "ディレクトリの作成";
                Form_1.btnFileSharing.Text = "ファイル共有";
                Form_1.btnFileSharing.Size = new Size(125, 47);
                Form_1.lblEssentials.Text = "必需品";
            }

            if (_custLang == "ESP") {
                Form_1.lblUpload.Text = "Subir";
                Form_1.label2.Text = "Recuento de elementos";
                Form_1.btnUploadFile.Text = "Subir archivo";
                Form_1.btnUploadFolder.Text = "Cargar carpeta";
                Form_1.btnCreateDirectory.Text = "Crear directorio";
                Form_1.btnFileSharing.Text = "Compartición de archivos";
                Form_1.btnFileSharing.Size = new Size(125, 47);
                Form_1.lblEssentials.Text = "Esenciales";
            }

            if (_custLang == "FRE") {
                Form_1.lblUpload.Text = "Télécharger";
                Form_1.label2.Text = "Nombre d'éléments";
                Form_1.btnUploadFile.Text = "Téléverser un fichier";
                Form_1.btnUploadFolder.Text = "Télécharger le dossier";
                Form_1.btnCreateDirectory.Text = "Créer le répertoire";
                Form_1.btnFileSharing.Text = "Partage de fichiers";
                Form_1.btnFileSharing.Size = new Size(125, 47);
                Form_1.lblEssentials.Text = "Essentiel";
                Form_1.btnSettings.Text = "Paramètres";
            }

            if (_custLang == "POR") {
                Form_1.lblUpload.Text = "Carregar";
                Form_1.label2.Text = "Contagem de itens";
                Form_1.btnUploadFile.Text = "Subir arquivo";
                Form_1.btnUploadFolder.Text = "Carregar Pasta";
                Form_1.btnCreateDirectory.Text = "Criar diretório";
                Form_1.btnFileSharing.Text = "Compartilhamento de arquivos";
                Form_1.btnFileSharing.Size = new Size(125, 47);
                Form_1.lblEssentials.Text = "Essenciais";
            }

            if (_custLang == "CHI") {
                Form_1.lblUpload.Text = "上传";
                Form_1.label2.Text = "物品数量";
                Form_1.btnUploadFile.Text = "上传文件";
                Form_1.btnUploadFolder.Text = "上传文件夹";
                Form_1.btnCreateDirectory.Text = "创建目录";
                Form_1.btnFileSharing.Text = "文件共享";
                Form_1.btnFileSharing.Size = new Size(125, 47);
                Form_1.lblEssentials.Text = "要点";
            }

            if(_custLang == "RUS") {
                Form_1.lblUpload.Text = "Загрузить";
                Form_1.label2.Text = "Количество предметов";
                Form_1.btnUploadFile.Text = "Загрузить файл";
                Form_1.btnUploadFolder.Text = "Загрузить папку";
                Form_1.btnCreateDirectory.Text = "Создать каталог";
                Form_1.btnFileSharing.Text = "Общий доступ к файлам";
                Form_1.btnFileSharing.Size = new Size(125, 47);
                Form_1.lblEssentials.Text = "Основные";
            }

            if(_custLang == "DUT") {
                Form_1.lblUpload.Text = "Uploaden";
                Form_1.label2.Text = "Aantal artikelen";
                Form_1.btnUploadFile.Text = "Bestand uploaden";
                Form_1.btnUploadFolder.Text = "Map uploaden";
                Form_1.btnCreateDirectory.Text = "Directory aanmaken";
                Form_1.btnFileSharing.Text = "Bestanden delen";
                Form_1.btnFileSharing.Size = new Size(125, 47);
                Form_1.lblEssentials.Text = "Essentials";
            }
        }

        /// <summary>
        /// Retrieve user current language
        /// </summary>
        private async Task getCurrentLang() {

            const string _selectLang = "SELECT CUST_LANG FROM lang_info WHERE CUST_USERNAME = @username";
            using (var command = new MySqlCommand(_selectLang, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                using (MySqlDataReader readLang = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    if (await readLang.ReadAsync()) {
                        CurrentLang = readLang.GetString(0);
                    }
                }
            }

        }

        private void buildGreetingLabel() {

            HomePage form = HomePage.instance;
            var lab1 = form.lblGreetingText;

            DateTime now = DateTime.Now;
            int hours = now.Hour;
            string greeting = "";

            if (hours >= 1 && hours <= 12) {
                if (CurrentLang == "US") {
                    greeting = "Good Morning, " + Globals.custUsername;
                }
                else if (CurrentLang == "MY") {
                    greeting = "Selemat Pagi, " + Globals.custUsername;
                }
                else if (CurrentLang == "GER") {
                    greeting = "Guten Morgen, " + Globals.custUsername ;
                }
                else if (CurrentLang == "JAP") {
                    greeting = "おはよう " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "ESP") {
                    greeting = "Buen día " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "FRE") {
                    greeting = "Bonjour " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "POR") {
                    greeting = "Bom dia " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "CHI") {
                    greeting = "早上好 " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "RUS") {
                    greeting = "Доброе утро " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "DUT") {
                    greeting = "Goedemorgen " + Globals.custUsername + " :)";
                }
            }

            else if (hours >= 12 && hours <= 16) {
                if (CurrentLang == "US") {
                    greeting = "Good Afternoon, " + Globals.custUsername;
                }
                else if (CurrentLang == "MY") {
                    greeting = "Selamat Petang, " + Globals.custUsername;
                }
                else if (CurrentLang == "GER") {
                    greeting = "Guten Tag, " + Globals.custUsername;
                }
                else if (CurrentLang == "JAP") {
                    greeting = "こんにちは, " + Globals.custUsername;
                }
                else if (CurrentLang == "ESP") {
                    greeting = "Buenas tardes " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "FRE") {
                    greeting = "Bon après-midi " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "POR") {
                    greeting = "Boa tarde " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "CHI") {
                    greeting = "下午好 " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "RUS") {
                    greeting = "Добрый день " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "DUT") {
                    greeting = "Goedemiddag " + Globals.custUsername + " :)";
                }

            }
             else if (hours >= 16 && hours <= 21) {
                if (hours == 20 || hours == 21) {
                    if (CurrentLang == "US") {
                        greeting = "Good Late Evening, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "MY") {
                        greeting = "Selamat Lewat-Petang, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "GER") {
                        greeting = "Guten späten Abend, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "JAP") {
                        greeting = "こんばんは " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "ESP") {
                        greeting = "buenas tardes " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "FRE") {
                        greeting = "bonne soirée " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "POR") {
                        greeting = "Boa noite " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "CHI") {
                        greeting = "晚上好 " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "RUS") {
                        greeting = "Добрый день " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "DUT") {
                        greeting = "Goedeavond " + Globals.custUsername + " :)";
                    }

                }
                else {
                    if (CurrentLang == "US") {
                        greeting = "Good Evening, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "MY") {
                        greeting = "Selamat Petang, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "GER") {
                        greeting = "Guten Abend, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "JAP") {
                        greeting = "こんばんは " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "ESP") {
                        greeting = "Buenas terdes " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "FRE") {
                        greeting = "bonne soirée " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "POR") {
                        greeting = "Boa noite " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "CHI") {
                        greeting = "晚上好 " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "RUS") {
                        greeting = "Добрый вечер " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "DUT") {
                        greeting = "Goedeavond " + Globals.custUsername + " :)";
                    }
                }

            }
            else if (hours >= 21 && hours <= 24) {
                if (CurrentLang == "US") {
                    greeting = "Good Night, " + Globals.custUsername;
                }
                else if (CurrentLang == "MY") {
                    greeting = "Selamat Malam, " + Globals.custUsername;
                }
                else if (CurrentLang == "GER") {
                    greeting = "Guten Nacth, " + Globals.custUsername;
                }
                else if (CurrentLang == "JAP") {
                    greeting = "おやすみ " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "ESP") {
                    greeting = "Buenas noches " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "FRE") {
                    greeting = "bonne nuit " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "POR") {
                    greeting = "Boa noite " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "CHI") {
                    greeting = "晚安 " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "RUS") {
                    greeting = "Спокойной ночи " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "DUT") {
                    greeting = "Welterusten " + Globals.custUsername + " :)";
                }

            }
            lab1.Text = greeting;
        }

        private void LogIN_Load(object sender, EventArgs e) {

        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
            guna2TextBox2.PasswordChar = '*';
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
            guna2TextBox2.PasswordChar = '\0';
        }

        private void guna2CheckBox2_CheckedChanged(object sender, EventArgs e) {

        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e) {
            if (System.Text.RegularExpressions.Regex.IsMatch(guna2TextBox4.Text, "[^0-9]")) {
                guna2TextBox4.Text = guna2TextBox4.Text.Remove(guna2TextBox4.Text.Length - 1);
            }
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e) {

        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void label3_Click_1(object sender, EventArgs e) {

        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e) {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e) {

        }

        private void label5_Click(object sender, EventArgs e) {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {

            ValidateRecoveryEmail _showPasswordRecovery = new ValidateRecoveryEmail();
            _showPasswordRecovery.Show();
            
            this.Close();

        }
        private void guna2Button4_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
