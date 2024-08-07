using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Newtonsoft.Json;
using System.Net.Http;
//using System.Collections.Generic;

using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using System.Threading;
using System.Net;
using System.ServiceProcess;
using Nethereum.Web3;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using System.Numerics;
using System.Security.Policy;

namespace WindowsResourceProvider
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.Shown += new EventHandler(MainForm_Shown);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
            
            
        }
        private void MainForm_Shown(object sender, EventArgs e)
        {
            // Code to be executed after the form is loaded and shown
            //MessageBox.Show("The form has been loaded and displayed!");
            RestoreTextBoxValue();
            bool installed = Properties.Settings.Default.installed;
            if (installed == false)
            {
                //Task.Run(async () => await Install());
                Install();
                SaveInstalled(true);
                //MessageBox.Show("Installation Complete. Enter your private key and press launch");
            }

            publicAddressTextBox.Text = GetPublicAddressFromPrivateKey(privateKeytextBox.Text);
            LaunchBacalhau();
            Launch();
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 10000; // 10 seconds
            timer.Tick += async (s, ee) => await GetData();
            timer.Start();
            //GetData();

            // Add your custom code here
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Check if the process is not null and is still running
            //if (myProcess != null && !myProcess.HasExited)
            //{
            //    // Attempt to close the process gracefully
            //    myProcess.CloseMainWindow();
            //    myProcess.Close();

            //    // Optionally, forcefully terminate the process if it doesn't close
            //    // myProcess.Kill();
            //}
            try
            {
                process.Kill();
            }
            catch (Exception)
            {

                //throw;
            }
            try
            {
                process2.Kill();
            }
            catch (Exception)
            {

                //throw;
            }
            SaveTextBoxValue();
            Process alpine_process = new Process();

            alpine_process.StartInfo.FileName = "wsl";
            alpine_process.StartInfo.Arguments = "-t Alpine";
            alpine_process.StartInfo.CreateNoWindow = true;
            alpine_process.Start();
            alpine_process.WaitForExit();
        }
        private void launch_button_Click(object sender, EventArgs e)
        {
            publicAddressTextBox.Text = GetPublicAddressFromPrivateKey(privateKeytextBox.Text);
            LaunchBacalhau();
            Launch();
            GetData();

            // Command to be executed
            //Launch();

            //process.WaitForExit();
        }
        Process process = new Process();
        Process process2 = new Process();
        Process process3 = new Process();

        private void Launch()
        {
            string launch_cmd = File.ReadAllText("launch.txt");
            string pk = "export WEB3_PRIVATE_KEY=" + privateKeytextBox.Text + ";"; // export WEB3_PRIVATE_KEY=0x0093a705313ee3e02b69fd58758ac485644c20ba4a5db0cb5c17cb8835e992b8;";
            string command = "wsl -d Alpine " + pk + launch_cmd.Replace("\n",";") ;

            // Create a new process
           
            // Configure the process using the StartInfo properties
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c {command}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

            // Subscribe to the OutputDataReceived event to read the output asynchronously
            process.OutputDataReceived += (s, args) =>
            {
                if (args.Data != null)
                {
                    // Update the textBox2.Text property in a thread-safe manner
                    this.Invoke((MethodInvoker)delegate
                    {
                        textBox1.AppendText(args.Data + Environment.NewLine);
                    });
                }
            };

            process.Start();
            // Begin asynchronous read operation
            process.BeginOutputReadLine();
        }
        private void LaunchBacalhau()
        {
            //string launch_cmd = File.ReadAllText("launch.txt");
            //string pk = "export WEB3_PRIVATE_KEY=0x0093a705313ee3e02b69fd58758ac485644c20ba4a5db0cb5c17cb8835e992b8;";
            //string command = "wsl -d Alpine   cd ~/lilypad; export LOG_LEVEL=debug; ./stack bacalhau-node";
            string command = "wsl -d Alpine   export BACALHAU_SERVE_IPFS_PATH=/tmp/lilypad/data/ipfs; export LOG_LEVEL=debug; bacalhau serve --node-type compute,requester --job-selection-accept-networked";


            // Create a new process

            // Configure the process using the StartInfo properties
            process2.StartInfo.FileName = "cmd.exe";
            process2.StartInfo.Arguments = $"/c {command}";
            process2.StartInfo.UseShellExecute = false;
            process2.StartInfo.RedirectStandardOutput = true;
            process2.StartInfo.RedirectStandardError = true;
            process2.StartInfo.CreateNoWindow = true;

            // Subscribe to the OutputDataReceived event to read the output asynchronously
            process2.OutputDataReceived += (s, args) =>
            {
                if (args.Data != null)
                {
                    // Update the textBox2.Text property in a thread-safe manner
                    this.Invoke((MethodInvoker)delegate
                    {
                        textBox2.AppendText(args.Data + Environment.NewLine);
                    });
                }
            };

            process2.Start();
            // Begin asynchronous read operation
            process2.BeginOutputReadLine();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private  void install_Click(object sender, EventArgs e)
        {


            //Task.Run(async  ()=> await Install());
            Install();
        }
        private async Task Install()
        {
            installingLabel1.Visible = true;
            progressBar1.Visible = true;
            //// Command to be executed
            //string command = "wsl -d Alpine";
            //privateKeytextBox.Text ="";
            //publicAddressTextBox.Text = "";
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            var privateKey = ecKey.GetPrivateKeyAsBytes();
            var privateKeyHex = BitConverter.ToString(privateKey).Replace("-", "");
            privateKeytextBox.Text = privateKeyHex;
            publicAddressTextBox.Text = GetPublicAddressFromPrivateKey(privateKeyHex);

            SaveTextBoxValue();
            SaveInstalled(false);

            installingLabel1.Visible =  true;
            progressBar1.Visible = true;
            Process process = new Process();

            //if (!File.Exists("DockerDesktopInstaller.exe")) { 
            //    process.StartInfo.FileName = "curl";
            //    process.StartInfo.Arguments = "-Lo DockerDesktopInstaller.exe \"https://desktop.docker.com/win/stable/Docker%20Desktop%20Installer.exe\"";
            //    process.Start();
            //    process.WaitForExit();
            //}
            progressBar1.Value = 10;
            if (!IsDockerInstalled())
            {
                InstallDocker();
            }
                

            //process.StartInfo.FileName = "DockerDesktopInstaller.exe";
            //process.StartInfo.Arguments = "install";
            //process.Start();
            //process.WaitForExit();

            progressBar1.Value = 70;
            process.OutputDataReceived += (s, args) =>
            {
                if (args.Data != null)
                {
                    // Update the textBox2.Text property in a thread-safe manner
                    this.Invoke((MethodInvoker)delegate
                    {
                        textBox2.AppendText(args.Data + Environment.NewLine);
                    });
                }
            };
            process.StartInfo.FileName = "Alpine";
            process.StartInfo.Arguments = "clean";
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;

            process.Start();

            using (StreamWriter sw = process.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine("Y");
                }
            }
            process.WaitForExit();
            //Thread.Sleep(1000);
            progressBar1.Value = 80;
            process.StartInfo.FileName = "Alpine";
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            using (StreamWriter sw = process.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine("Y");
                }
            }
            process.WaitForExit();

            string install_cmd = File.ReadAllText("install.txt");
            //string pk = "export WEB3_PRIVATE_KEY=" + privateKeytextBox.Text + ";"; // export WEB3_PRIVATE_KEY=0x0093a705313ee3e02b69fd58758ac485644c20ba4a5db0cb5c17cb8835e992b8;";
            string command = "wsl -d Alpine " + install_cmd.Replace("\n", ";");

            // Create a new process

            progressBar1.Value = 90;
            // Configure the process using the StartInfo properties
            process3.OutputDataReceived += (s, args) =>
            {
                if (args.Data != null)
                {
                    // Update the textBox2.Text property in a thread-safe manner
                    this.Invoke((MethodInvoker)delegate
                    {
                        textBox1.AppendText(args.Data + Environment.NewLine);
                    });
                }
            };
            process3.StartInfo.FileName = "cmd.exe";
            process3.StartInfo.Arguments = $"/c {command}";
            process3.StartInfo.RedirectStandardInput = true;
            process3.StartInfo.RedirectStandardOutput = true;
            process3.StartInfo.RedirectStandardError = true;
            process3.StartInfo.UseShellExecute = false;
            process3.StartInfo.CreateNoWindow = true;

            //process3.StartInfo.RedirectStandardOutput = true;
            //process3.StartInfo.CreateNoWindow = true;
            //process3.OutputDataReceived += (s, args) =>
            //{
            //    if (args.Data != null)
            //    {
            //        // Update the textBox2.Text property in a thread-safe manner
            //        this.Invoke((MethodInvoker)delegate
            //        {
            //            textBox3.AppendText(args.Data + Environment.NewLine);
            //        });
            //    }
            //};

            process3.Start();
            process3.WaitForExit();
            progressBar1.Value = 100;
            installingLabel1.Visible = false;
            progressBar1.Visible = false;
            //MessageBox.Show("done");

            // Read the output (if any).
            //string result = process.StandardOutput.ReadToEnd();
            //Console.WriteLine(result);
            //textBox3.Text = result;
            //process.WaitForExit();
        }
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void SaveInstalled(bool b)
        {
            Properties.Settings.Default.installed = b;
            Properties.Settings.Default.Save();
        }
        //private void RestoreTextBoxValue()
        //{
        //    installed = Properties.Settings.Default.installed;
        //}

        private void SaveTextBoxValue()
        {
            Properties.Settings.Default.TextBoxValue = privateKeytextBox.Text;
            Properties.Settings.Default.Save();
        }
        private void RestoreTextBoxValue()
        {
            privateKeytextBox.Text = Properties.Settings.Default.TextBoxValue;
        }

        // If you have a specific method to reset the form, call SaveTextBoxValue there as well
        private void ResetForm()
        {
            // Your reset logic here
            SaveTextBoxValue();
            // Optionally, immediately restore the value if needed
            RestoreTextBoxValue();
        }
        public static string GetPublicAddressFromPrivateKey(string privateKey)
        {
            // Ensure the private key is in the correct format
            if (!privateKey.StartsWith("0x"))
            {
                privateKey = "0x" + privateKey;
            }
            if(privateKey.Length < 3)
            {
                return "";
            }

            // Utilize Nethereum to convert the private key to a public address
            var account = new Nethereum.Web3.Accounts.Account(privateKey);

            // Return the public address
            return account.Address;
        }
        public async Task GetData()
        {
            //InitializeComponent();
            //Load += async (sender, e) => await 
            LPlabel.Text = (await GetTokenBalanceAsync(publicAddressTextBox.Text)).ToString();
            Ethlabel.Text = (await GetEthBalanceAsync(publicAddressTextBox.Text)).ToString();
            PointsLabel.Text =  (await FetchAndDisplayData(publicAddressTextBox.Text)).ToString();

            //();
        }

        private async Task<string> FetchAndDisplayData(string walletAddress)
        {
            //Thread.Sleep(10000);
            //string walletAddress = publicAddressTextBox.Text;// "0xF7A8907B0EECAd6054BDb2A89E6A729bc95601cF"; // Example wallet address
            var leaderboardEntry = await GetLeaderboardEntryForWallet(walletAddress);
            if (leaderboardEntry != null)// && Form1.ActiveForm !=null)
            {
                
                //Form1.ActiveForm.Text = $"Resource Provider Rank: {leaderboardEntry.Rank}, Energy: {leaderboardEntry.Energy}, Points: {leaderboardEntry.Points}";
                return leaderboardEntry.Points;
                //yourLabel.Text = $"Rank: {leaderboardEntry.Rank}, Energy: {leaderboardEntry.Energy}, Points: {leaderboardEntry.Points}";
            }
            else
            {
                return "";
                //Form1.ActiveForm.Text = $"Resource Provider  Wallet not found.";
            }
        }

        private async Task<LeaderboardEntry> GetLeaderboardEntryForWallet(string walletAddress)
        {
            string url = "https://api-testnet.lilypad.tech/metrics-dashboard/leaderboard";
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetStringAsync(url);
                    var leaderboardEntries = JsonConvert.DeserializeObject<List<LeaderboardEntry>>(response);
                    return leaderboardEntries.Find(entry => entry.Wallet.Equals(walletAddress, StringComparison.OrdinalIgnoreCase));
                }
                catch (Exception ex)
                {
                    //MessageBox.Show($"Failed to fetch or parse data: {ex.Message}");
                    return null;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            privateKeytextBox.UseSystemPasswordChar = !privateKeytextBox.UseSystemPasswordChar;
        }
        static bool IsDockerInstalled()
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process())
                {
                    process.StartInfo = processStartInfo;
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(output) && output.Contains("Docker version"))
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                // Handle exceptions if needed (e.g., ProcessStartInfo exceptions)
            }

            return false;
        }
        private  void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            Console.Write("\rProgress: {0}%", e.ProgressPercentage);
        }

        private static void DownloadFileCompletedCallback(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Console.WriteLine("\nDownload error: " + e.Error.Message);
            }
            else
            {
                Console.WriteLine("\nDownload completed successfully.");
            }
        }
        private async Task InstallDocker ()
        {
            string installerUrl = "https://desktop.docker.com/win/stable/Docker%20Desktop%20Installer.exe";//https://desktop.docker.com/win/stable/Docker%20Desktop%20Installer.exe";
            string installerPath = Path.Combine(Path.GetTempPath(), "Docker Desktop Installer.exe");

            // Step 1: Download Docker Desktop Installer
            Console.WriteLine("Downloading Docker Desktop Installer...");
            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Add("User-Agent: Other");
                //webClient.DownloadFile(installerUrl, installerPath);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
                webClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(DownloadFileCompletedCallback);
                webClient.DownloadFileAsync(new Uri(installerUrl), installerPath);

                // Wait until the download is complete
                while (webClient.IsBusy)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
            progressBar1.Value = 20;
            // Step 2: Install Docker Desktop silently
            Console.WriteLine("Installing Docker Desktop...");
            ProcessStartInfo installProcess = new ProcessStartInfo
            {
                FileName = installerPath,
                Arguments = "install --quiet",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            progressBar1.Value = 30;
            using (Process process = Process.Start(installProcess))
            {
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                Console.WriteLine("Installation Output: " + output);
                Console.WriteLine("Installation Error: " + error);
            }
            progressBar1.Value = 40;
            // Step 3: Start Docker Desktop service
            Console.WriteLine("Starting Docker service...");
            try
            {
                ServiceController dockerService = new ServiceController("com.docker.service");
                if (dockerService.Status == ServiceControllerStatus.Stopped)
                {
                    dockerService.Start();
                    dockerService.WaitForStatus(ServiceControllerStatus.Running);
                }
                Console.WriteLine("Docker service started successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error starting Docker service: " + ex.Message);
            }
            progressBar1.Value = 50;
            // Step 4: Verify Docker installation
            Console.WriteLine("Verifying Docker installation...");
            ProcessStartInfo verifyProcess = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "--version",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(verifyProcess))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(output) && output.Contains("Docker version"))
                {
                    Console.WriteLine("Docker is installed. Version: " + output);
                }
                else
                {
                    Console.WriteLine("Docker installation failed or Docker service is not running.");
                    Console.WriteLine("Error: " + error);
                }
            }
            progressBar1.Value = 60;

            // Clean up installer file
            if (File.Exists(installerPath))
            {
                File.Delete(installerPath);
            }

            Console.WriteLine("Process completed.");
        }
        private readonly string _infuraUrl = "https://sepolia-rollup.arbitrum.io/rpc"; // Example Infura URL
        string erc20ABI = "[{\"constant\":true,\"inputs\":[{\"name\":\"_owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"name\":\"balance\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"}]";

        // Example ERC20 token contract address
        private readonly string _tokenContractAddress = "0x0352485f8a3cB6d305875FaC0C40ef01e0C06535";

        public async Task<decimal> GetTokenBalanceAsync(string userAddress)
        {
            var web3 = new Web3(_infuraUrl);

            // Example ERC20 ABI (used to call standard ERC20 functions)
            var contract = web3.Eth.GetContract(erc20ABI, _tokenContractAddress);

            // Get balance function
            var balanceOfFunction = contract.GetFunction("balanceOf");

            // Call balanceOf function to get balance
            var balance = await balanceOfFunction.CallAsync<BigInteger>(userAddress);

            // Convert balance from BigInteger to decimal
            decimal balanceDecimal = UnitConversion.Convert.FromWei(balance);
            //web3.Eth.GetBalance(userAddress)
            return balanceDecimal;
        }
        public async Task<decimal> GetEthBalanceAsync(string userAddress)
        {
            var web3 = new Web3(_infuraUrl);

            // Get the balance in Ether (wei format)
            var balanceWei = await web3.Eth.GetBalance.SendRequestAsync(userAddress);

            // Convert balance from wei to ether
            decimal balanceEth = Web3.Convert.FromWei(balanceWei.Value);

            return balanceEth;
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenUrl("https://leaderboard.lilypad.tech/leaderboard");
        }
        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true // UseShellExecute is set to true to open the URL in the default browser.
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //OpenUrl("https://www.alchemy.com/faucets/arbitrum-sepolia");
            OpenUrl("https://faucets.chain.link/arbitrum-sepolia");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenUrl("https://faucet-testnet.lilypad.tech/");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(publicAddressTextBox.Text);

        }
    }

    public class LeaderboardEntry
    {
        public string Rank { get; set; }
        public string Wallet { get; set; }
        public string Energy { get; set; }
        public string Points { get; set; }
    }

    //public partial class Form1 : Form
    //{
       
    //}
}
