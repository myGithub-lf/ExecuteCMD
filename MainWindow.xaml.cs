using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExecuteCMD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static StringBuilder cmdOutput = null;
        private static StringBuilder cmdErrorOutput = null;
        private static string baseDir = "";

        private string lastWorkFolder = "";
        private string lastHttpPort = "";
        private string lastHttpsPort = "";
        private string lastShutdownPort = "";

        private ArrayList dbNameList = new ArrayList();
        private ArrayList dbPathList = new ArrayList();
        private ArrayList appserverList = new ArrayList();


        public MainWindow()
        {
            InitializeComponent();
            baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            selectAllAppserver.IsEnabled = false;
            if (this.dbList.Items == null || this.dbList.Items.IsEmpty)
            {
                selectAllDB.IsEnabled = false;
            }

            instancetype.Text = "Prod";
            oldOEDLC.Text = @"C:\Progress\OpenEdge11.7";
            oldOEPort.Text = "20955";
            newOEDLC.Text = @"C:\Progress\OpenEdge12.2";
            newOEPort.Text = "20957";

            if (oldOEDLC.Text != "" && oldOEPort.Text != "" && newOEDLC.Text != "" && newOEPort.Text != "")
            {
                EnabledAll();
            }
            else
            {
                DisabledAll();
            }
        }

        private void KillTasksAndServices(object sender, RoutedEventArgs e)
        {
            KillServiceCommand();
        }

        private void StartServices(object sender, RoutedEventArgs e)
        {
            StartServiceCommand();
        }

        private void LeaveOldOEDLC(object sender, RoutedEventArgs e)
        {
            if (oldOEDLC.Text != "")
            {
                if (!Directory.Exists(oldOEDLC.Text))
                {
                    MessageBoxResult result = MessageBox.Show("Please input a correct directory");
                    /* Refocus */
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                                                new Action(() =>
                                                {
                                                    oldOEDLC.Focus();
                                                }));
                    /* Reset the cursor position */
                    oldOEDLC.Select(0, oldOEDLC.Text.Length);
                    return;
                }
            }
            if (oldOEDLC.Text != "" && oldOEPort.Text != "" && newOEDLC.Text != "" && newOEPort.Text != "")
            {
                EnabledAll();
            }
            else
            {
                DisabledAll();
            }
        }

        private void LeaveOldOEPort(object sender, RoutedEventArgs e)
        {
            if (oldOEPort.Text != "")
            {
                /* Verify that the port is digit */
                if (!Regex.IsMatch(oldOEPort.Text, @"^\d+$"))
                {
                    MessageBoxResult result = MessageBox.Show("Please input a correct port");
                    /* Refocus */
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                                                new Action(() =>
                                                {
                                                    oldOEPort.Focus();
                                                }));
                    /* Reset the cursor position */
                    oldOEPort.Select(0, oldOEPort.Text.Length);
                    return;
                }
            }
            if (oldOEDLC.Text != "" && oldOEPort.Text != "" && newOEDLC.Text != "" && newOEPort.Text != "")
            {
                EnabledAll();
            }
            else
            {
                DisabledAll();
            }
        }

        private void LeaveNewOEDLC(object sender, RoutedEventArgs e)
        {
            if (newOEDLC.Text != "")
            {
                if (!Directory.Exists(newOEDLC.Text))
                {
                    MessageBoxResult result = MessageBox.Show("Please input a correct directory");
                    /* Refocus */
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                                                new Action(() =>
                                                {
                                                    newOEDLC.Focus();
                                                }));
                    /* Reset the cursor position */
                    newOEDLC.Select(0, newOEDLC.Text.Length);
                    return;
                }
            }
            if (oldOEDLC.Text != "" && oldOEPort.Text != "" && newOEDLC.Text != "" && newOEPort.Text != "")
            {
                EnabledAll();
            }
            else
            {
                DisabledAll();
            }
        }

        private void LeaveNewOEPort(object sender, RoutedEventArgs e)
        {
            if (newOEPort.Text != "")
            {
                /* Verify that the port is digit */
                if (!Regex.IsMatch(newOEPort.Text, @"^\d+$"))
                {
                    MessageBoxResult result = MessageBox.Show("Please input a correct port");
                    /* Refocus */
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                                                new Action(() =>
                                                {
                                                    newOEPort.Focus();
                                                }));
                    /* Reset the cursor position */
                    newOEPort.Select(0, newOEPort.Text.Length);
                    return;
                }
            }
            if (oldOEDLC.Text != "" && oldOEPort.Text != "" && newOEDLC.Text != "" && newOEPort.Text != "")
            {
                EnabledAll();
            }
            else
            {
                DisabledAll();
            }
        }

        private void GetDBList(object sender, RoutedEventArgs e)
        {
            int dbNumber;
            int dbPathNum;
            DataBase[] dataBase;

            string output = Execute(GetAllDBCommand(oldOEDLC.Text, oldOEPort.Text), "", true, false);
            ProcessingGetAllDBCommandExecutionResults(output);
            //缺少错误情况处理，暂不处理

            dbNumber = dbNameList.Count;
            dbPathNum = dbPathList.Count;
            if (dbNumber != 0 && dbPathNum != 0 && dbNumber == dbPathNum)
            {
                dataBase = new DataBase[dbNumber];
                for (int i = 0; i < dbNumber; i++)
                {
                    dataBase[i] = new DataBase { DBId = i, IsCheck = false, DBName = dbNameList[i].ToString(), DBPath = dbPathList[i].ToString(), Upgraded = CheckIfDbUpgraded(dbPathList[i].ToString()), Status = GetDbStatus(dbNameList[i].ToString()) };
                }
            }
            else
            {
                MessageBox.Show("The number of dbNames and dbPaths does not match");
                return;
            }

            dbList.ItemsSource = dataBase;
            if (this.dbList.Items == null || this.dbList.Items.IsEmpty)
            {
                selectAllDB.IsEnabled = false;
            }
            else
            {
                selectAllDB.IsEnabled = true;
            }
        }

        private bool CheckIfDbUpgraded(string dbPath)
        {
            //执行命令
            string result = Execute(GetDBVersionCommand(newOEDLC.Text, dbPath), "", true, false);
            string tempFileName = CreateFile(baseDir);
            try
            {
                if (!result.Equals(""))
                {
                    File.WriteAllText(tempFileName, result);
                    string alLines = File.ReadAllText(tempFileName);
                    if (!alLines.Equals("") && alLines.Contains("Database is already in version 12 format. (5122)"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            DeleteFile(tempFileName);
            return false;
        }

        private string GetDBVersionCommand(string oe12_2DLC, string dbPath)
        {
            return "call echo y|" + oe12_2DLC + @"\bin\proutil " + dbPath + " -C conv1112";
        }

        private string GetDbStatus(string dbName)
        {
            //执行命令dbman -query -all -status started -port 20931
            string result = Execute(GetAllDBStatusCommand(newOEDLC.Text, newOEPort.Text), "", true, false);
            string tempFileName = CreateFile(baseDir);
            try
            {
                if (!result.Equals(""))
                {
                    File.WriteAllText(tempFileName,result);
                    string alLines = File.ReadAllText(tempFileName);
                    if (!alLines.Equals("") && alLines.Contains("Database Name: " + dbName))
                    {
                        return "started";
                    }
                    else
                    {
                        return "not started";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            DeleteFile(tempFileName);
            return "not started";
        }

        private string GetAllDBStatusCommand(string oe12_2DLC, string port)
        {
            return "call " + oe12_2DLC + @"\bin\dbman -query -all -status started -port " + port;
        }

        private void SelectOneDB(object sender, RoutedEventArgs e)
        {
            int count = 0;
            CheckBox checkBox = (CheckBox)sender;
            int id = Convert.ToInt32(checkBox.ToolTip);
            if (this.dbList.Items != null || this.dbList.Items.IsEmpty == false)
            {
                List<DataBase> dbs = this.dbList.Items.Cast<DataBase>().ToList();
                DataBase dataBase = null;
                try
                {
                    dataBase = dbs.Where(a => a.DBId == id).SingleOrDefault();
                    if (dataBase == null)
                    {
                        MessageBox.Show(id + "Nothing can be selected");
                    }
                    else
                    {
                        if (checkBox.IsChecked == true)
                        {
                            dataBase.IsCheck = true;
                            foreach (DataBase db in dbs)
                            {
                                if (db.IsCheck == true)
                                {
                                    count++;
                                }
                            }
                            if (count == dbs.Count)
                            {
                                selectAllDB.IsChecked = true;
                            }
                        }
                        else
                        {
                            dataBase.IsCheck = false;
                            if (selectAllDB.IsChecked == true)
                            {
                                selectAllDB.IsChecked = false;
                            }
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Contains multiple same DB");
                }
            }
            else
            {
                MessageBox.Show("Nothing can be selected");
            }
        }

        private void SelectAllDB(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (this.dbList.Items != null || this.dbList.Items.IsEmpty == false)
            {
                List<DataBase> dbs = this.dbList.Items.Cast<DataBase>().ToList();
                if (checkBox.IsChecked == true)
                {
                    foreach (DataBase db in dbs)
                    {
                        db.IsCheck = true;
                    }
                }
                else
                {
                    foreach (DataBase db in dbs)
                    {
                        db.IsCheck = false;
                    }
                }
            }
            else
            {
                MessageBox.Show("Nothing can be selected");
            }
        }

        private void UpgradeDB(object sender, RoutedEventArgs e)
        {
            string command;
            if (this.dbList.Items != null || this.dbList.Items.IsEmpty == false)
            {
                List<DataBase> dbs = this.dbList.Items.Cast<DataBase>().ToList();
                foreach (DataBase db in dbs)
                {
                    if (db.IsCheck && !db.Upgraded)
                    {
                        command = MigrateDBCommand(newOEDLC.Text, System.IO.Path.Combine(oldOEDLC.Text, @"properties\conmgr.properties"), db.DBName);
                        if (command.Equals(""))
                        {
                            continue;
                        }
                        else
                        {
                            Execute(command, "", true, false);
                            int commandsCount = 0;

                            //缺少错误处理，（不知可能出现哪些错误）暂不处理
                            Execute(UpgradeDBCommand(newOEDLC.Text, db.DBPath)[0], "", true, false);
                            if (!cmdErrorOutput.ToString().Equals(""))
                            {
                                //撤销之前的操作，即
                                MessageBox.Show(cmdErrorOutput.ToString());
                                continue;
                            }
                            else if (cmdErrorOutput.ToString().Equals(""))
                            {
                                commandsCount++;

                                Execute(UpgradeDBCommand(newOEDLC.Text, db.DBPath)[1], "", true, false);
                                if (!cmdErrorOutput.ToString().Equals(""))
                                {
                                    //撤销之前的操作，即
                                    MessageBox.Show(cmdErrorOutput.ToString());
                                    continue;
                                }
                                else if (cmdErrorOutput.ToString().Equals(""))
                                {
                                    commandsCount++;

                                    Execute(UpgradeDBCommand(newOEDLC.Text, db.DBPath)[2], "", true, false);
                                    if (!cmdErrorOutput.ToString().Equals(""))
                                    {
                                        //撤销之前的操作，即
                                        MessageBox.Show(cmdErrorOutput.ToString());
                                        continue;
                                    }
                                    else if (cmdErrorOutput.ToString().Equals(""))
                                    {
                                        commandsCount++;

                                        Execute(UpgradeDBCommand(newOEDLC.Text, db.DBPath)[3], "", true, false);
                                        if (!cmdErrorOutput.ToString().Equals(""))
                                        {
                                            //撤销之前的操作，即
                                            MessageBox.Show(cmdErrorOutput.ToString());
                                            continue;
                                        }
                                        else if (cmdErrorOutput.ToString().Equals(""))
                                        {
                                            commandsCount++;
                                        }
                                    }
                                }
                            }

                            if (commandsCount == 4)
                            {
                                db.Upgraded = true;
                            }
                            else
                            {
                                //升级db出错，需要自动处理错误吗？是，则处理错误并重新升级，否则清手动处理错误，错误内容在文件xxxx
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Nothing can be selected");
            }
        }

        private void StopDB(object sender, RoutedEventArgs e)
        {
            if (this.dbList.Items != null || this.dbList.Items.IsEmpty == false)
            {
                List<DataBase> dbs = this.dbList.Items.Cast<DataBase>().ToList();
                foreach (DataBase db in dbs)
                {
                    if (db.IsCheck && db.Status.Equals("started") && db.Upgraded)
                    {
                        Execute(StopDBOn12_2Command(newOEDLC.Text, newOEPort.Text, db.DBName), "", true, false);
                        if (cmdErrorOutput.ToString().Equals(""))
                        {
                            db.Status = "not started";
                        }
                        else
                        {
                            MessageBox.Show("An error occurred at stop DB " + db.DBName + " : " + cmdErrorOutput.ToString());
                            continue;
                        }
                    }
                }
            }
        }

        private void RestartDB(object sender, RoutedEventArgs e)
        {
            if (this.dbList.Items != null || this.dbList.Items.IsEmpty == false)
            {
                List<DataBase> dbs = this.dbList.Items.Cast<DataBase>().ToList();
                foreach (DataBase db in dbs)
                {
                    if (db.IsCheck && !db.Status.Equals("started") && db.Upgraded)
                    {
                        Execute(StartDBOn12_2Command(newOEDLC.Text, newOEPort.Text, db.DBName), "", true, false);
                        if (cmdErrorOutput.ToString().Equals(""))
                        {
                            db.Status = "started";
                        }
                        else
                        {
                            MessageBox.Show("An error occurred at start DB " + db.DBName + " : " + cmdErrorOutput.ToString());
                            continue;
                        }
                    }
                }
            }
        }

        private void GetAppserverList(object sender, RoutedEventArgs e)
        {
            int appsNum;
            Appserver[] appArray;

            string output = Execute(GetAllAppServerCommand(oldOEDLC.Text, oldOEPort.Text), "", true, false);
            ProcessingGetAllAppServerCommandExecutionResults(output);
            //缺少错误处理，（不知可能出现哪些错误）暂不处理

            appsNum = appserverList.Count;
            if (appsNum != 0)
            {
                appArray = new Appserver[appsNum];
                for (int i = 0; i < appsNum; i++)
                {
                    appArray[i] = new Appserver
                    {
                        AppId = i,
                        IsCheck = false,
                        IsEnable = true,
                        Upgraded = false,
                        Status = "Not Running",
                        InstanceType = "Prod",
                        UbrokerName = appserverList[i].ToString(),
                        RelatedDBPath = GetAppserverRelatedDB(appserverList[i].ToString()),
                    };
                }

                for (int i = 0; i < appArray.Length; i++)
                {
                    //判断文件是否存在，该文件用于存储ubroker|pasname|uid|pwd|workFolder, 在点击migrate app时写入
                    if (File.Exists(baseDir + @"\app-pas.txt"))
                    {
                        foreach (string line in File.ReadLines(baseDir + @"\app-pas.txt").Where(a => a.Contains(appserverList[i].ToString())))
                        {
                            appArray[i].PasName = line.Split("|")[1];
                            appArray[i].UserName = line.Split("|")[2];
                            appArray[i].Password = line.Split("|")[3];
                            appArray[i].WorkFolder = line.Split("|")[4];
                            appArray[i].InstancePathName = appArray[i].PasName;
                            appArray[i].Upgraded = CheckIfAppserverUpgraded(appArray[i].PasName);
                            appArray[i].Status = GetAppserverStatus(appArray[i].PasName);
                            appArray[i].HttpPort = GetAppserverHttpPort(appArray[i].PasName).ToString();
                            appArray[i].HttpsPort = GetAppserverHttpsPort(appArray[i].PasName).ToString();
                            appArray[i].ShutdownPort = GetAppserverShutdownPort(appArray[i].PasName).ToString();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("The number of dbNames and dbPaths does not match");
                return;
            }
            appServerList.ItemsSource = appArray;
            AutoWidth(appServerList);
        }

        private string GetAppserverRelatedDB(string ubroker)
        {
            string value = "";
            StreamReader sr = null;
            StreamReader sr2;
            string file = oldOEDLC.Text + @"\properties\ubroker.properties";
            try
            {
                if (File.Exists(file))
                {
                    sr = new StreamReader(file);
                    string str = "";
                    bool start = false;
                    while ((str = sr.ReadLine()) != null)
                    {
                        if (str.Contains("[UBroker.AS." + ubroker + "]"))
                        {
                            start = true;
                            continue;
                        }
                        if (start == true)
                        {
                            if (str.Contains("srvrStartupParam"))
                            {
                                value = str.Substring(str.IndexOf("-pf") + 4, (str.IndexOf(".pf") + 3) - (str.IndexOf("-pf") + 4));
                                break;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if (File.Exists(value))
                    {
                        sr2 = new StreamReader(value);
                        string str2 = "";
                        while ((str2 = sr2.ReadLine()) != null)
                        {
                            if (str2.Contains("-db"))
                            {
                                value = str2.Substring(4);
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("No files found for appserver " + ubroker + ".");
                    }
                }
                else
                {
                    MessageBox.Show("No files found for appservers.");
                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sr.Close();
            }
            return value;
        }

        private bool CheckIfAppserverUpgraded(string pasName)
        {
            //执行命令pasman instances
            string result = Execute(CheckAppserverCommand(newOEDLC.Text), "", true, false);
            string tempFileName = CreateFile(baseDir);
            try
            {
                if (!result.Equals(""))
                {
                    File.WriteAllText(tempFileName, result);
                    string alLines = File.ReadAllText(tempFileName);
                    if (!alLines.Equals("") && alLines.Contains(pasName))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            DeleteFile(tempFileName);
            return false;
        }

        private string CheckAppserverCommand(string oe12_2DLC)
        {
            return "call " + oe12_2DLC + @"\bin\pasman instances";
        }

        private string GetAppserverStatus(string pasName)
        {
            //执行命令
            string result = Execute(GetAppserverStatusCommand(newOEDLC.Text, pasName), "", true, false);

            if (!result.Equals(""))
            {
                if (result.Equals("1"))
                {
                    return "Running";
                }
                else
                {
                    return "Not Running";
                }
            }

            return "Not Running";
        }

        private string GetAppserverStatusCommand(string oe12_2DLC, string pasName)
        {
            return "call " + oe12_2DLC + @"\bin\pasman env running -I " + pasName;
        }

        private int GetAppserverHttpPort(string pasName)
        {
            //执行命令
            string result = Execute(GetAppserverHttpPortCommand(newOEDLC.Text, pasName), "", true, false);

            if (!result.Equals(""))
            {
                try{
                    
                    return Convert.ToInt32(result.Trim());
                }
                catch
                {
                    return -1;
                }
            }

            return -1;
        }

        private string GetAppserverHttpPortCommand(string oe12_2DLC, string pasName)
        {
            return "call " + oe12_2DLC + @"\bin\pasman env http -I " + pasName;
        }

        private int GetAppserverHttpsPort(string pasName)
        {
            //执行命令
            string result = Execute(GetAppserverHttpsPortCommand(newOEDLC.Text, pasName), "", true, false);

            if (!result.Equals(""))
            {
                try
                {

                    return Convert.ToInt32(result.Trim());
                }
                catch
                {
                    return -1;
                }
            }

            return -1;
        }

        private string GetAppserverHttpsPortCommand(string oe12_2DLC, string pasName)
        {
            return "call " + oe12_2DLC + @"\bin\pasman env https -I " + pasName;
        }

        private int GetAppserverShutdownPort(string pasName)
        {
            //执行命令
            string result = Execute(GetAppserverShutdownPortCommand(newOEDLC.Text, pasName), "", true, false);

            if (!result.Equals(""))
            {
                try
                {

                    return Convert.ToInt32(result.Trim());
                }
                catch
                {
                    return -1;
                }
            }

            return -1;
        }

        private string GetAppserverShutdownPortCommand(string oe12_2DLC, string pasName)
        {
            return "call " + oe12_2DLC + @"\bin\pasman env shut -I " + pasName;
        }

        private void MoveAppserver(object sender, RoutedEventArgs e)
        {
            if (this.appServerList.Items != null || this.appServerList.Items.IsEmpty == false)
            {
                List<Appserver> apps = this.appServerList.Items.Cast<Appserver>().ToList();
                foreach (Appserver app in apps)
                {
                    if (app.IsCheck && !app.Upgraded)
                    {
                        int commandsCount = 0;
                        
                        Execute(MigrateAppserverCommand(oldOEDLC.Text, newOEDLC.Text, app.WorkFolder, app.HttpPort, app.HttpsPort, app.ShutdownPort, app.UserName, app.Password, app.InstanceType, app.UbrokerName, app.PasName)[0], app.WorkFolder, true, false);
                        if (!cmdErrorOutput.ToString().Equals("") && cmdErrorOutput.ToString().Contains("alias is in use"))
                        {
                            //撤销之前的操作，即删掉work folder中的实例，修改DLC/properties/pasmgr.properties，DLC/servers/pasoe/conf/instances.windows（需要先停掉12.2的adminserver）
                            MessageBox.Show(cmdErrorOutput.ToString());
                            continue;
                        }
                        else if (cmdErrorOutput.ToString().Equals(""))
                        {
                            commandsCount++;

                            Execute(MigrateAppserverCommand(oldOEDLC.Text, newOEDLC.Text, app.WorkFolder, app.HttpPort, app.HttpsPort, app.ShutdownPort, app.UserName, app.Password, app.InstanceType, app.UbrokerName, app.PasName)[1], app.WorkFolder, true, false);
                            if (!cmdErrorOutput.ToString().Equals(""))
                            {
                                //撤销之前的操作，即删掉work folder中的实例，修改DLC/properties/pasmgr.properties，DLC/servers/pasoe/conf/instances.windows（需要先停掉12.2的adminserver）
                                MessageBox.Show(cmdErrorOutput.ToString());
                                continue;
                            }
                            else if (cmdErrorOutput.ToString().Equals(""))
                            {
                                commandsCount++;

                                Execute(MigrateAppserverCommand(oldOEDLC.Text, newOEDLC.Text, app.WorkFolder, app.HttpPort, app.HttpsPort, app.ShutdownPort, app.UserName, app.Password, app.InstanceType, app.UbrokerName, app.PasName)[2], app.WorkFolder, true, false);
                                if (!cmdErrorOutput.ToString().Equals(""))
                                {
                                    //撤销之前的操作，即删掉work folder中的实例，修改DLC/properties/pasmgr.properties，DLC/servers/pasoe/conf/instances.windows（需要先停掉12.2的adminserver）
                                    MessageBox.Show(cmdErrorOutput.ToString());
                                    continue;
                                }
                                else if (cmdErrorOutput.ToString().Equals(""))
                                {
                                    UpdateOemergeFile(app.PasName + "." + app.UbrokerName + ".oemerge");
                                    commandsCount++;

                                    Execute(MigrateAppserverCommand(oldOEDLC.Text, newOEDLC.Text, app.WorkFolder, app.HttpPort, app.HttpsPort, app.ShutdownPort, app.UserName, app.Password, app.InstanceType, app.UbrokerName, app.PasName)[3], app.WorkFolder, true, false);
                                    if (!cmdErrorOutput.ToString().Equals(""))
                                    {
                                        //撤销之前的操作，即删掉work folder中的实例，修改DLC/properties/pasmgr.properties，DLC/servers/pasoe/conf/instances.windows（需要先停掉12.2的adminserver）
                                        MessageBox.Show(cmdErrorOutput.ToString());
                                        continue;
                                    }
                                    else if (cmdErrorOutput.ToString().Equals(""))
                                    {
                                        commandsCount++;
                                    }
                                }
                            }
                        }

                        if (commandsCount == 4)
                        {
                            app.Upgraded = true;
                        }
                        else
                        {
                            //迁移appserver出错，需要自动处理错误吗？是，则处理错误并重新迁移（并在错误文件中标识改错误已处理，若仍未处理成功，则提示用户手动处理），此过程中将重启您的openedge service；否则请手动处理错误，错误内容在文件xxxx
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Nothing can be selected");
            }
        }

        private void StopAppServerAndDBOnOldOE(object sender, RoutedEventArgs e)
        {
            ArrayList appserverTempList = new ArrayList();
            ArrayList dbNameTempList = new ArrayList();

            //获取执行结果
            string output = Execute(GetAllAppServerCommand(oldOEDLC.Text, oldOEPort.Text), "", true, false);

            //在当前执行程序所在目录下，创建临时文件
            string tempFileName = CreateFile(baseDir);

            StreamWriter sw = null;
            StreamReader sr = null;
            try
            {
                //根据临时文件创建输出流
                sw = new StreamWriter(tempFileName);
                //将执行结果写入输出流，即写入临时文件
                sw.WriteLine(output);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (sw != null)
                {
                    //关闭输出流
                    sw.Close();
                }
            }
            try
            {
                //根据临时文件创建输入流，临时文件的内容是前一步写入的
                sr = new StreamReader(tempFileName);
                //读取每一行，若包含 "Searching for" 则获取并存入arraylist集合
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("Searching for"))
                    {
                        if (line.Trim().Split(" ").Length >= 3)
                        {
                            if ((line.Trim().Split(" ")[0] + " " + line.Trim().Split(" ")[1]) == "Searching for")
                            {
                                if (!line.Trim().Split(" ")[2].Equals("asbroker1") &&
                                    !line.Trim().Split(" ")[2].Equals("bpsbroker1") &&
                                    !line.Trim().Split(" ")[2].Equals("esbbroker1") &&
                                    !line.Trim().Split(" ")[2].Equals("icfrepos") &&
                                    !line.Trim().Split(" ")[2].Equals("restbroker1"))
                                {
                                    appserverTempList.Add(line.Trim().Split(" ")[2]);
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (sr != null)
                {
                    //关闭输入流
                    sr.Close();
                }
            }
            //如果临时文件存在，则删除
            DeleteFile(tempFileName);
            //stop All Appserver
            foreach (object ubroker in appserverTempList)
            {
                Execute(StopAllAppserverCommand(oldOEDLC.Text, oldOEPort.Text, ubroker.ToString()), "", false, true);
            }

            //获取所有db
            string dbOutput = Execute(GetAllDBCommand(oldOEDLC.Text, oldOEPort.Text), "", true, false);

            //在当前执行程序所在目录下，创建临时文件
            tempFileName = CreateFile(baseDir);

            sw = null;
            sr = null;
            try
            {
                //根据临时文件创建输出流
                sw = new StreamWriter(tempFileName);
                //将执行结果写入输出流，即写入临时文件
                sw.WriteLine(dbOutput);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (sw != null)
                {
                    //关闭输出流
                    sw.Close();
                }
            }
            try
            {
                //根据临时文件创建输入流，临时文件的内容是前一步写入的
                sr = new StreamReader(tempFileName);
                //读取每一行，若包含 "Database Name:"或"database path:" 则获取并存入arraylist集合
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("Database Name:"))
                    {
                        if (line.Trim().Split(" ").Length >= 3)
                        {
                            if ((line.Trim().Split(" ")[0] + " " + line.Trim().Split(" ")[1]) == "Database Name:")
                            {
                                dbNameTempList.Add(line.Trim().Split(" ")[2]);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (sr != null)
                {
                    //关闭输入流
                    sr.Close();
                }
            }
            //如果临时文件存在，则删除
            DeleteFile(tempFileName);
            //stop all db
            foreach (object dbName in dbNameTempList)
            {
                Execute(StopAllDBCommand(oldOEDLC.Text, oldOEPort.Text, dbName.ToString()), "", false, true);
            }

        }

        private void StopPAS(object sender, RoutedEventArgs e)
        {
            if (this.appServerList.Items != null || this.appServerList.Items.IsEmpty == false)
            {
                List<Appserver> apps = this.appServerList.Items.Cast<Appserver>().ToList();
                foreach (Appserver app in apps)
                {
                    if (app.IsCheck && app.Status.Equals("Running") && app.Upgraded)
                    {
                        Execute(StopPasOn12_2Command(newOEDLC.Text, app.PasName), "", true, false);
                        if (cmdErrorOutput.ToString().Equals(""))
                        {
                            app.Status = "Not Running";
                        }
                        else
                        {
                            MessageBox.Show("An error occurred at stop PAS " + app.PasName + " : " + cmdErrorOutput.ToString());
                            continue;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Nothing can be selected");
            }
        }

        private void RestartPAS(object sender, RoutedEventArgs e)
        {
            if (this.appServerList.Items != null || this.appServerList.Items.IsEmpty == false)
            {
                List<Appserver> apps = this.appServerList.Items.Cast<Appserver>().ToList();
                foreach (Appserver app in apps)
                {
                    if (app.IsCheck && app.Status.Equals("Not Running") && app.Upgraded)
                    {
                        Execute(StartPasOn12_2Command(newOEDLC.Text, app.PasName), "", true, false);
                        if (cmdErrorOutput.ToString().Equals(""))
                        {
                            app.Status = "Not Running";
                        }
                        else
                        {
                            MessageBox.Show("An error occurred at start PAS " + app.PasName + " : " + cmdErrorOutput.ToString());
                            continue;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Nothing can be selected");
            }
        }

        private void AppServerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.appServerList.SelectedItem != null)
            {
                Appserver appserver = (Appserver)this.appServerList.SelectedItem;
                if (this.appServerList.Items != null || this.appServerList.Items.IsEmpty == false)
                {
                    List<Appserver> apps = this.appServerList.Items.Cast<Appserver>().ToList();

                    /*set other unselected items to disabled*/
                    foreach (Appserver app in apps)
                    {
                        if (!(app.Equals(appserver)))
                        {
                            app.IsEnable = false;
                            ListViewItem listViewItem = (ListViewItem)appServerList.ItemContainerGenerator.ContainerFromItem(app);
                            listViewItem.IsSelected = false;
                            listViewItem.IsEnabled = false;
                        }
                    }

                    /*set window value*/
                    ubrokername.Text = appserver.UbrokerName;
                    if (appserver.IsCheck)
                    {
                        instancepathname.Text = appserver.InstancePathName;
                        pasname.Text = appserver.PasName;

                        workfolder.Text = appserver.WorkFolder;
                        httpport.Text = appserver.HttpPort;
                        httpsport.Text = appserver.HttpsPort;
                        shutdownport.Text = appserver.ShutdownPort;
                        username.Text = appserver.UserName;
                        pwd.Text = appserver.Password;
                    }
                    else
                    {
                        instancepathname.Text = ubrokername.Text;
                        pasname.Text = ubrokername.Text;

                        appserver.InstancePathName = appserver.UbrokerName;
                        appserver.PasName = appserver.UbrokerName;
                    }


                }
            }
        }

        private void LeaveWorkDir(object sender, RoutedEventArgs e)
        {
            if (this.appServerList.SelectedItem != null)
            {
                Appserver appserver = (Appserver)this.appServerList.SelectedItem;
                if (workfolder.Text != "")
                {
                    if (Directory.Exists(workfolder.Text))
                    {
                        appserver.WorkFolder = workfolder.Text;
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("Please input a correct directory");
                        /* Refocus */
                        this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                                                    new Action(() =>
                                                    {
                                                        workfolder.Focus();
                                                    }));
                        /* Reset the cursor position */
                        workfolder.Select(0, workfolder.Text.Length);
                        return;
                    }
                }
            }
        }

        private void LeaveHttpPort(object sender, RoutedEventArgs e)
        {
            bool valid = true;
            if (this.appServerList.SelectedItem != null)
            {
                Appserver appserver = (Appserver)this.appServerList.SelectedItem;
                List<Appserver> apps = this.appServerList.Items.Cast<Appserver>().ToList();

                if (httpport.Text != "")
                {
                    /* Verify that the port is digit */
                    if (Regex.IsMatch(httpport.Text, @"^\d+$"))
                    {
                        /* Verify that the port is in use */
                        if (httpport.Text == httpsport.Text)
                        {
                            MessageBox.Show("The port number is the same as httpsPort");
                            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                                                    new Action(() =>
                                                    {
                                                        httpport.Focus();
                                                    }));
                            httpport.Select(0, httpport.Text.Length);
                            valid = false;
                            return;
                        }
                        if (httpport.Text == shutdownport.Text)
                        {
                            MessageBox.Show("The port number is the same as shutdownPort");
                            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                                                    new Action(() =>
                                                    {
                                                        httpport.Focus();
                                                    }));
                            httpport.Select(0, httpport.Text.Length);
                            valid = false;
                            return;
                        }

                        foreach (Appserver app in apps)
                        {
                            if (!app.UbrokerName.Equals(appserver.UbrokerName))
                            {
                                if (app.HttpPort != null && app.HttpsPort != null && app.ShutdownPort != null)
                                {
                                    if (!app.HttpPort.Equals("") && !app.HttpsPort.Equals("") && !app.ShutdownPort.Equals(""))
                                    {
                                        if (app.HttpPort.Equals(httpport.Text) || app.HttpsPort.Equals(httpport.Text) || app.ShutdownPort.Equals(httpport.Text))
                                        {
                                            MessageBox.Show("The port number is in use");
                                            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                                                                    new Action(() =>
                                                                    {
                                                                        httpport.Focus();
                                                                    }));
                                            httpport.Select(0, httpport.Text.Length);
                                            valid = false;
                                            return;
                                        }
                                    }
                                }
                            }
                        }

                        if (valid == true)
                        {
                            appserver.HttpPort = httpport.Text;
                        }
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("Please input a correct port");
                        /* Refocus */
                        this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                                                    new Action(() =>
                                                    {
                                                        httpport.Focus();
                                                    }));
                        /* Reset the cursor position */
                        httpport.Select(0, httpport.Text.Length);
                        return;
                    }
                }
            }
        }

        private void LeaveHttpsPort(object sender, RoutedEventArgs e)
        {
            bool valid = true;
            if (this.appServerList.SelectedItem != null)
            {
                Appserver appserver = (Appserver)this.appServerList.SelectedItem;
                List<Appserver> apps = this.appServerList.Items.Cast<Appserver>().ToList();

                if (httpsport.Text != "")
                {
                    /* Verify that the port is digit */
                    if (Regex.IsMatch(httpsport.Text, @"^\d+$"))
                    {
                        /* Verify that the port is in use */
                        if (httpsport.Text == httpport.Text)
                        {
                            MessageBox.Show("The port number is the same as httpPort");
                            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                                                    new Action(() =>
                                                    {
                                                        httpsport.Focus();
                                                    }));
                            httpsport.Select(0, httpsport.Text.Length);
                            valid = false;
                            return;
                        }
                        if (httpsport.Text == shutdownport.Text)
                        {
                            MessageBox.Show("The port number is the same as shutdownPort");
                            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                                                    new Action(() =>
                                                    {
                                                        httpsport.Focus();
                                                    }));
                            httpsport.Select(0, httpsport.Text.Length);
                            valid = false;
                            return;
                        }

                        foreach (Appserver app in apps)
                        {
                            if (!app.UbrokerName.Equals(appserver.UbrokerName))
                            {
                                if (app.HttpPort != null && app.HttpsPort != null && app.ShutdownPort != null)
                                {
                                    if (!app.HttpPort.Equals("") && !app.HttpsPort.Equals("") && !app.ShutdownPort.Equals(""))
                                    {
                                        if (app.HttpPort.Equals(httpsport.Text) || app.HttpsPort.Equals(httpsport.Text) || app.ShutdownPort.Equals(httpsport.Text))
                                        {
                                            MessageBox.Show("The port number is in use");
                                            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                                                                    new Action(() =>
                                                                    {
                                                                        httpsport.Focus();
                                                                    }));
                                            httpsport.Select(0, httpsport.Text.Length);
                                            valid = false;
                                            return;
                                        }
                                    }
                                }
                            }
                        }

                        if (valid == true)
                        {
                            appserver.HttpsPort = httpsport.Text;
                        }
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("Please input a correct port");
                        /* Refocus */
                        this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                                                    new Action(() =>
                                                    {
                                                        httpsport.Focus();
                                                    }));
                        /* Reset the cursor position */
                        httpsport.Select(0, httpsport.Text.Length);
                        return;
                    }
                }
            }
        }

        private void LeaveShutdownPort(object sender, RoutedEventArgs e)
        {
            bool valid = true;
            if (this.appServerList.SelectedItem != null)
            {
                Appserver appserver = (Appserver)this.appServerList.SelectedItem;
                List<Appserver> apps = this.appServerList.Items.Cast<Appserver>().ToList();

                if (shutdownport.Text != "")
                {
                    /* Verify that the port is digit */
                    if (Regex.IsMatch(shutdownport.Text, @"^\d+$"))
                    {
                        /* Verify that the port is in use */
                        if (shutdownport.Text == httpsport.Text)
                        {
                            MessageBox.Show("The port number is the same as httpsPort");
                            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                                                    new Action(() =>
                                                    {
                                                        shutdownport.Focus();
                                                    }));
                            shutdownport.Select(0, shutdownport.Text.Length);
                            valid = false;
                            return;
                        }
                        if (shutdownport.Text == httpport.Text)
                        {
                            MessageBox.Show("The port number is the same as httpPort");
                            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                                                    new Action(() =>
                                                    {
                                                        shutdownport.Focus();
                                                    }));
                            shutdownport.Select(0, shutdownport.Text.Length);
                            valid = false;
                            return;
                        }
                        foreach (Appserver app in apps)
                        {
                            if (!app.UbrokerName.Equals(appserver.UbrokerName))
                            {
                                if (app.HttpPort != null && app.HttpsPort != null && app.ShutdownPort != null)
                                {
                                    if (!app.HttpPort.Equals("") && !app.HttpsPort.Equals("") && !app.ShutdownPort.Equals(""))
                                    {
                                        if (app.HttpPort.Equals(shutdownport.Text) || app.HttpsPort.Equals(shutdownport.Text) || app.ShutdownPort.Equals(shutdownport.Text))
                                        {
                                            MessageBox.Show("The port number is in use");
                                            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                                                                    new Action(() =>
                                                                    {
                                                                        shutdownport.Focus();
                                                                    }));
                                            shutdownport.Select(0, shutdownport.Text.Length);
                                            valid = false;
                                            return;
                                        }
                                    }
                                }
                            }
                        }

                        if (valid == true)
                        {
                            appserver.ShutdownPort = shutdownport.Text;
                        }
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("Please input a correct port");
                        /* Refocus */
                        this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                                                    new Action(() =>
                                                    {
                                                        shutdownport.Focus();
                                                    }));
                        /* Reset the cursor position */
                        shutdownport.Select(0, shutdownport.Text.Length);
                        return;
                    }
                }
            }
        }

        private void LeaveUsername(object sender, RoutedEventArgs e)
        {
            if (this.appServerList.SelectedItem != null)
            {
                Appserver appserver = (Appserver)this.appServerList.SelectedItem;
                if (username.Text != "")
                {
                    appserver.UserName = username.Text;
                }
            }
        }

        private void LeavePwd(object sender, RoutedEventArgs e)
        {
            if (this.appServerList.SelectedItem != null)
            {
                Appserver appserver = (Appserver)this.appServerList.SelectedItem;
                if (pwd.Text != "")
                {
                    appserver.Password = pwd.Text;
                }
            }
        }

        private void LeaveInstancePathname(object sender, RoutedEventArgs e)
        {
            if (this.appServerList.SelectedItem != null)
            {
                Appserver appserver = (Appserver)this.appServerList.SelectedItem;
                appserver.InstancePathName = instancepathname.Text;
                pasname.Text = instancepathname.Text;
                appserver.PasName = appserver.InstancePathName;
            }
        }

        private void LeavePasName(object sender, RoutedEventArgs e)
        {
            if (this.appServerList.SelectedItem != null)
            {
                Appserver appserver = (Appserver)this.appServerList.SelectedItem;
                appserver.PasName = pasname.Text;
                instancepathname.Text = pasname.Text;
                appserver.InstancePathName = appserver.PasName;
            }
        }

        /* auto fill field value (workfolder、httpport、httpsport、shutdownport) */
        private void AutoFill(object sender, RoutedEventArgs e)
        {
            int tempPort = 0;

            if (this.appServerList.SelectedItem != null)
            {
                Appserver appserver = (Appserver)this.appServerList.SelectedItem;
                List<Appserver> apps = this.appServerList.Items.Cast<Appserver>().ToList();

                if (lastWorkFolder != "")
                {
                    if (appserver.IsCheck)
                    {
                        workfolder.Text = appserver.WorkFolder;
                    }
                    else
                    {
                        workfolder.Text = lastWorkFolder;
                    }
                }
                else
                {
                    workfolder.Text = @"C:\tempFolder";
                }

                if (lastHttpPort != "")
                {
                    if (appserver.IsCheck)
                    {
                        httpport.Text = appserver.HttpPort;
                    }
                    else
                    {
                        httpport.Text = lastHttpPort;
                    }
                }
                else
                {
                    httpport.Text = "1000";
                }

                if (lastHttpsPort != "")
                {
                    if (appserver.IsCheck)
                    {
                        httpsport.Text = appserver.HttpsPort;
                    }
                    else
                    {
                        httpsport.Text = lastHttpsPort;
                    }
                }
                else
                {
                    httpsport.Text = "1001";
                }

                if (lastShutdownPort != "")
                {
                    if (appserver.IsCheck)
                    {
                        shutdownport.Text = appserver.ShutdownPort;
                    }
                    else
                    {
                        shutdownport.Text = lastShutdownPort;
                    }
                }
                else
                {
                    shutdownport.Text = "1002";
                }


                /*verify that work folder is a directory*/
                if (workfolder.Text != "")
                {
                    if (Directory.Exists(workfolder.Text))
                    {
                        appserver.WorkFolder = workfolder.Text;
                    }
                    else
                    {
                        Directory.CreateDirectory(workfolder.Text);
                    }
                }

                /*Verify that the port is in use, if true, Automatically add 10 based on the previous port*/
                /*verify httpPort*/
                bool valid = false;
                while (!valid)
                {
                    if (httpport.Text == httpsport.Text || httpport.Text == shutdownport.Text)
                    {
                        valid = false;
                    }
                    else
                    {
                        foreach (Appserver app in apps)
                        {
                            if (!app.UbrokerName.Equals(appserver.UbrokerName))
                            {
                                if (app.HttpPort != null && app.HttpsPort != null && app.ShutdownPort != null)
                                {
                                    if (!app.HttpPort.Equals("") && !app.HttpsPort.Equals("") && !app.ShutdownPort.Equals(""))
                                    {
                                        if (app.HttpPort.Equals(httpport.Text) || app.HttpsPort.Equals(httpport.Text) || app.ShutdownPort.Equals(httpport.Text))
                                        {
                                            valid = false;
                                            break;
                                        }
                                        else
                                        {
                                            valid = true;
                                        }
                                    }
                                    else
                                    {
                                        //
                                    }
                                }
                                else
                                {
                                    valid = true;
                                }
                            }
                            else
                            {
                                if (app.IsCheck)
                                {
                                    valid = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (valid == false)
                    {
                        /*Automatically add 10 based on the previous port #*/
                        try
                        {
                            tempPort = Convert.ToInt32(httpport.Text);
                            tempPort += 10;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        httpport.Text = tempPort.ToString();
                    }
                }


                /*verify httpsPort*/
                valid = false;
                while (!valid)
                {
                    if (httpsport.Text == httpport.Text || httpsport.Text == shutdownport.Text)
                    {
                        valid = false;
                    }
                    else
                    {
                        foreach (Appserver app in apps)
                        {
                            if (!app.UbrokerName.Equals(appserver.UbrokerName))
                            {
                                if (app.HttpPort != null && app.HttpsPort != null && app.ShutdownPort != null)
                                {
                                    if (!app.HttpPort.Equals("") && !app.HttpsPort.Equals("") && !app.ShutdownPort.Equals(""))
                                    {
                                        if (app.HttpPort.Equals(httpsport.Text) || app.HttpsPort.Equals(httpsport.Text) || app.ShutdownPort.Equals(httpsport.Text))
                                        {
                                            valid = false;
                                            break;
                                        }
                                        else
                                        {
                                            valid = true;
                                        }
                                    }
                                }
                                else
                                {
                                    valid = true;
                                }
                            }
                            else
                            {
                                if (app.IsCheck)
                                {
                                    valid = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (valid == false)
                    {
                        /*Automatically add 10 based on the previous port #*/
                        try
                        {
                            tempPort = Convert.ToInt32(httpsport.Text);
                            tempPort += 10;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        httpsport.Text = tempPort.ToString();
                    }
                }

                /*verify shutdownPort*/
                valid = false;
                while (!valid)
                {
                    if (shutdownport.Text == httpport.Text || shutdownport.Text == httpsport.Text)
                    {
                        valid = false;
                    }
                    else
                    {
                        foreach (Appserver app in apps)
                        {
                            if (!app.UbrokerName.Equals(appserver.UbrokerName))
                            {
                                if (app.HttpPort != null && app.HttpsPort != null && app.ShutdownPort != null)
                                {
                                    if (!app.HttpPort.Equals("") && !app.HttpsPort.Equals("") && !app.ShutdownPort.Equals(""))
                                    {
                                        if (app.HttpPort.Equals(shutdownport.Text) || app.HttpsPort.Equals(shutdownport.Text) || app.ShutdownPort.Equals(shutdownport.Text))
                                        {
                                            valid = false;
                                            break;
                                        }
                                        else
                                        {
                                            valid = true;
                                        }
                                    }
                                }
                                else
                                {
                                    valid = true;
                                }
                            }
                            else
                            {
                                if (app.IsCheck)
                                {
                                    valid = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (valid == false)
                    {
                        /*Automatically add 10 based on the previous port #*/
                        try
                        {
                            tempPort = Convert.ToInt32(shutdownport.Text);
                            tempPort += 10;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        shutdownport.Text = tempPort.ToString();
                    }
                }

            }
        }

        /* save a pas instance configuration*/
        private void SaveApp(object sender, RoutedEventArgs e)
        {
            if (this.appServerList.SelectedItem != null)
            {
                Appserver appserver = (Appserver)this.appServerList.SelectedItem;

                if (workfolder.Text.Equals(""))
                {
                    MessageBox.Show("Work Folder is blank");
                    workfolder.Focus();
                    return;
                }
                if (httpport.Text.Equals(""))
                {
                    MessageBox.Show("httpport is blank");
                    httpport.Focus();
                    return;
                }
                else
                {
                    if (!(Regex.IsMatch(httpport.Text, @"^\d+$")))
                    {
                        MessageBox.Show("httpport is not digit");
                        httpport.Focus();
                        return;
                    }
                }
                if (httpsport.Text.Equals(""))
                {
                    MessageBox.Show("httpsport is blank");
                    httpsport.Focus();
                    return;
                }
                else
                {
                    if (!(Regex.IsMatch(httpsport.Text, @"^\d+$")))
                    {
                        MessageBox.Show("httpsport is not digit");
                        httpsport.Focus();
                        return;
                    }
                }
                if (shutdownport.Text.Equals(""))
                {
                    MessageBox.Show("shutdownport is blank");
                    shutdownport.Focus();
                    return;
                }
                else
                {
                    if (!(Regex.IsMatch(shutdownport.Text, @"^\d+$")))
                    {
                        MessageBox.Show("shutdownport is not digit");
                        shutdownport.Focus();
                        return;
                    }
                }
                if (ubrokername.Text.Equals(""))
                {
                    MessageBox.Show("ubrokername is blank");
                    ubrokername.Focus();
                    return;
                }
                if (instancetype.Text.Equals(""))
                {
                    MessageBox.Show("instancetype is blank");
                    instancetype.Focus();
                    return;
                }
                if (username.Text.Equals(""))
                {
                    MessageBox.Show("username is blank");
                    username.Focus();
                    return;
                }
                if (pwd.Text.Equals(""))
                {
                    MessageBox.Show("pwd is blank");
                    pwd.Focus();
                    return;
                }
                if (instancepathname.Text.Equals(""))
                {
                    MessageBox.Show("instancepathname is blank");
                    instancepathname.Focus();
                    return;
                }
                if (pasname.Text.Equals(""))
                {
                    MessageBox.Show("pasname is blank");
                    pasname.Focus();
                    return;
                }

                if (workfolder.Text != "" && httpport.Text != "" && httpsport.Text != "" && shutdownport.Text != "" && ubrokername.Text != "" && instancetype.Text != "" && username.Text != "" && pwd.Text != "" && instancepathname.Text != "" && pasname.Text != "")
                {
                    appserver.WorkFolder = workfolder.Text;
                    appserver.HttpPort = httpport.Text;
                    appserver.HttpsPort = httpsport.Text;
                    appserver.ShutdownPort = shutdownport.Text;
                    //appserver.UbrokerName = ubrokername.Text;
                    appserver.UserName = username.Text;
                    appserver.Password = pwd.Text;
                    appserver.InstancePathName = instancepathname.Text;
                    appserver.PasName = pasname.Text;

                }

                appserver.IsCheck = true;
                List<Appserver> apps = this.appServerList.Items.Cast<Appserver>().ToList();
                foreach (Appserver app in apps)
                {
                    ListViewItem listViewItem = (ListViewItem)appServerList.ItemContainerGenerator.ContainerFromItem(app);
                    listViewItem.IsSelected = false;
                    listViewItem.IsEnabled = true;
                }


                lastWorkFolder = workfolder.Text;
                lastHttpPort = httpport.Text;
                lastHttpsPort = httpsport.Text;
                lastShutdownPort = shutdownport.Text;

                workfolder.Text = "";
                httpport.Text = "";
                httpsport.Text = "";
                shutdownport.Text = "";
                ubrokername.Text = "";
                username.Text = "";
                pwd.Text = "";
                instancepathname.Text = "";
                pasname.Text = "";
            }
            else
            {
                MessageBox.Show("Please select an item");
            }
        }

        /* remove a pas instance configuration */
        private void RemoveApp(object sender, RoutedEventArgs e)
        {
            if (this.appServerList.SelectedItem != null)
            {
                /* Clear Window value*/
                workfolder.Text = "";
                httpport.Text = "";
                httpsport.Text = "";
                shutdownport.Text = "";
                ubrokername.Text = "";
                username.Text = "";
                pwd.Text = "";
                instancepathname.Text = "";
                pasname.Text = "";


                Appserver appserver = (Appserver)this.appServerList.SelectedItem;
                if (appserver.IsCheck)
                {
                    /* Marked as not added to config.ini file */
                    appserver.IsCheck = false;

                    /* Clear Old value */
                    appserver.WorkFolder = "";
                    appserver.HttpPort = "";
                    appserver.HttpsPort = "";
                    appserver.ShutdownPort = "";
                    appserver.UserName = "";
                    appserver.Password = "";
                    appserver.InstancePathName = "";
                    appserver.PasName = "";
                }

                /* make other unselected item selectable */
                List<Appserver> apps = this.appServerList.Items.Cast<Appserver>().ToList();
                foreach (Appserver app in apps)
                {
                    ListViewItem listViewItem = (ListViewItem)appServerList.ItemContainerGenerator.ContainerFromItem(app);
                    listViewItem.IsSelected = false;
                    listViewItem.IsEnabled = true;
                }
            }
        }

        /* Empty screen value */
        private void CancelApp(object sender, RoutedEventArgs e)
        {
            if (this.appServerList.SelectedItem != null)
            {
                /* Clear Window value*/
                workfolder.Text = "";
                httpport.Text = "";
                httpsport.Text = "";
                shutdownport.Text = "";
                ubrokername.Text = "";
                username.Text = "";
                pwd.Text = "";
                instancepathname.Text = "";
                pasname.Text = "";

                /* make other unselected item selectable */
                List<Appserver> apps = this.appServerList.Items.Cast<Appserver>().ToList();
                foreach (Appserver app in apps)
                {
                    ListViewItem listViewItem = (ListViewItem)appServerList.ItemContainerGenerator.ContainerFromItem(app);
                    listViewItem.IsSelected = false;
                    listViewItem.IsEnabled = true;
                }
            }
        }



        private void DisabledAll()
        {
            workfolder.IsEnabled = false;
            httpport.IsEnabled = false;
            httpsport.IsEnabled = false;
            shutdownport.IsEnabled = false;
            ubrokername.IsEnabled = false;
            instancetype.IsEnabled = false;
            username.IsEnabled = false;
            pwd.IsEnabled = false;
            instancepathname.IsEnabled = false;
            pasname.IsEnabled = false;
            getAppserver.IsEnabled = false;
            getDB.IsEnabled = false;
            upgradeDB.IsEnabled = false;
            upgradeApp.IsEnabled = false;
            killTasksAndServices.IsEnabled = false;
            startSerices.IsEnabled = false;
            stopAll.IsEnabled = false;
            stopPas.IsEnabled = false;
            restartPas.IsEnabled = false;
            stopDB.IsEnabled = false;
            startDB.IsEnabled = false;
            selectAllDB.IsEnabled = false;
            dbList.IsEnabled = false;
            appServerList.IsEnabled = false;
        }
        private void EnabledAll()
        {
            workfolder.IsEnabled = true;
            httpport.IsEnabled = true;
            httpsport.IsEnabled = true;
            shutdownport.IsEnabled = true;
            ubrokername.IsEnabled = true;
            instancetype.IsEnabled = true;
            username.IsEnabled = true;
            pwd.IsEnabled = true;
            instancepathname.IsEnabled = true;
            pasname.IsEnabled = true;
            getAppserver.IsEnabled = true;
            getDB.IsEnabled = true;
            upgradeDB.IsEnabled = true;
            upgradeApp.IsEnabled = true;
            killTasksAndServices.IsEnabled = true;
            startSerices.IsEnabled = true;
            stopAll.IsEnabled = true;
            stopPas.IsEnabled = true;
            restartPas.IsEnabled = true;
            stopDB.IsEnabled = true;
            startDB.IsEnabled = true;
            selectAllDB.IsEnabled = true;
            dbList.IsEnabled = true;
            appServerList.IsEnabled = true;
        }

        /*  make list view to auto set column width*/
        private void AutoWidth(ListView listView)
        {
            GridView gv = listView.View as GridView;
            if (gv != null)
            {
                foreach (GridViewColumn gvc in gv.Columns)
                {
                    gvc.Width = gvc.ActualWidth;
                    gvc.Width = Double.NaN;
                }
            }
        }


        private string GetAllDBCommand(string oldOeDLC, string oldOePort)
        {
            return "call " + oldOeDLC + "\\bin\\dbman -query -all -port " + oldOePort;
        }
        private void ProcessingGetAllDBCommandExecutionResults(string executionResult)
        {
            //清空集合
            dbNameList.Clear();
            dbPathList.Clear();

            //在当前执行程序所在目录下，创建临时文件
            string tempFileName = CreateFile(baseDir);

            StreamWriter sw = null;
            StreamReader sr = null;
            try
            {
                //根据临时文件创建输出流
                sw = new StreamWriter(tempFileName);
                //将执行结果写入输出流，即写入临时文件
                sw.WriteLine(executionResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (sw != null)
                {
                    //关闭输出流
                    sw.Close();
                }
            }
            try
            {
                //根据临时文件创建输入流，临时文件的内容是前一步写入的
                sr = new StreamReader(tempFileName);
                //读取每一行，若包含 "Database Name:"或"database path:" 则获取并存入arraylist集合
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("Database Name:"))
                    {
                        if (line.Trim().Split(" ").Length >= 3)
                        {
                            if ((line.Trim().Split(" ")[0] + " " + line.Trim().Split(" ")[1]) == "Database Name:")
                            {
                                dbNameList.Add(line.Trim().Split(" ")[2]);
                            }
                        }

                    }

                    if (line.Contains("database path:"))
                    {
                        if (line.Trim().Split(" ").Length >= 3)
                        {
                            if ((line.Trim().Split(" ")[0] + " " + line.Trim().Split(" ")[1]) == "database path:")
                            {
                                dbPathList.Add(line.Trim().Split(" ")[2]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (sr != null)
                {
                    //关闭输入流
                    sr.Close();
                }
            }
            //如果临时文件存在，则删除
            DeleteFile(tempFileName);
        }

        private string GetAllAppServerCommand(string oldOeDLC, string oldOePort)
        {
            return "call " + oldOeDLC + "\\bin\\asbman -q -all -port " + oldOePort;
        }
        private void ProcessingGetAllAppServerCommandExecutionResults(string executionResult)
        {
            //在当前执行程序所在目录下，创建临时文件
            string tempFileName = CreateFile(baseDir);

            StreamWriter sw = null;
            StreamReader sr = null;
            try
            {
                //根据临时文件创建输出流
                sw = new StreamWriter(tempFileName);
                //将执行结果写入输出流，即写入临时文件
                sw.WriteLine(executionResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (sw != null)
                {
                    //关闭输出流
                    sw.Close();
                }
            }
            try
            {
                //根据临时文件创建输入流，临时文件的内容是前一步写入的
                sr = new StreamReader(tempFileName);
                //读取每一行，若包含 "Searching for" 则获取并存入arraylist集合
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("Searching for"))
                    {
                        if (line.Trim().Split(" ").Length >= 3)
                        {
                            if ((line.Trim().Split(" ")[0] + " " + line.Trim().Split(" ")[1]) == "Searching for")
                            {
                                if (!line.Trim().Split(" ")[2].Equals("asbroker1") &&
                                    !line.Trim().Split(" ")[2].Equals("bpsbroker1") &&
                                    !line.Trim().Split(" ")[2].Equals("esbbroker1") &&
                                    !line.Trim().Split(" ")[2].Equals("icfrepos") &&
                                    !line.Trim().Split(" ")[2].Equals("restbroker1"))
                                {
                                    appserverList.Add(line.Trim().Split(" ")[2]);
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (sr != null)
                {
                    //关闭输入流
                    sr.Close();
                }
            }
            //如果临时文件存在，则删除
            DeleteFile(tempFileName);
        }

        private void KillServiceCommand()
        {
            Execute("tasklist|findstr /i _proapsv.exe && taskkill /f /t /im _proapsv.exe", "", true, false);
            Execute("tasklist|findstr /i _mprosrv.exe && taskkill /f /t /im _mprosrv.exe", "", true, false);
            Execute("tasklist|findstr /i _mproapsv.exe && taskkill /f /t /im _mproapsv.exe", "", true, false);
            Execute("tasklist|findstr /i _mprshut.exe && taskkill /f /t /im _mprshut.exe", "", true, false);
            Execute("net stop AdminService11.7(64-bit)", "", true, false);
            Execute("net stop AdminService12.2(64-bit)", "", true, false);
            Execute("net stop fathom_12.2", "", true, false);
            Execute("tasklist|findstr /i java.exe && taskkill /f /im java.exe", "", true, false);
        }

        private void StartServiceCommand()
        {
            Execute("net start AdminService11.7(64-bit)", "", true, false);
            Execute("net start AdminService12.2(64-bit)", "", true, false);
            Execute("net start fathom_12.2", "", true, false);
        }

        private string StopDBOn12_2Command(string oe12_2DLC, string oe12_2Port, string dbName)
        {
            return "call " + oe12_2DLC + @"\bin\dbman -stop -name " + dbName + " -port " + oe12_2Port;
        }

        private string StartDBOn12_2Command(string oe12_2DLC, string oe12_2Port, string dbName)
        {
            return "call " + oe12_2DLC + @"\bin\dbman -start -name " + dbName + " -port " + oe12_2Port;
        }

        private string StopPasOn12_2Command(string oe12_2DLC, string pasName)
        {
            return "call " + oe12_2DLC + @"\bin\pasman stop -I " + pasName + " -F";
        }

        private string StartPasOn12_2Command(string oe12_2DLC, string pasName)
        {
            return "call " + oe12_2DLC + @"\bin\pasman start -I " + pasName + " -F";
        }

        private string StopAllDBCommand(string oldOeDLC, string oldOePort, string dbName)
        {
            return "call " + oldOeDLC + @"\bin\dbman -stop -name " + dbName + " -port " + oldOePort;
        }

        private string StopAllAppserverCommand(string oldOeDLC, string oldOePort, string ubrokerName)
        {
            return "call " + oldOeDLC + @"\bin\asbman -kill -name " + ubrokerName + " -port " + oldOePort;
        }

        private string MigrateDBCommand(string oe12_2DLC, string conmgr_properties, string dbName)
        {
            string command = "";
            string conmgrTemp_properties = System.IO.Path.Combine(baseDir, "conmgrTemp.properties");
            if (!File.Exists(conmgrTemp_properties))
            {
                File.Create(conmgrTemp_properties);
            }
            StreamReader sr = null;
            StreamWriter sw = null;
            try
            {
                //判断conmgr_properties是否是文件且存在，存在读取所有行并付给字符串（File.ReadAllText(conmgr_properties)）
                if (File.Exists(conmgr_properties))
                {
                    //File.Copy(conmgr_properties, System.IO.Path.Combine(baseDir, System.IO.Path.GetFileName(conmgr_properties)));
                    string allLinesStr = File.ReadAllText(conmgr_properties);

                    //判断字符串中是否包含[configuration.dbName.defaultconfiguration] [database.dbName] [servergroup.dbName.defaultconfiguration.defaultservergroup], 若存在则读取每一行，并写入临时文件conmgrTemp.properties
                    if (allLinesStr.Contains("[configuration." + dbName + ".defaultconfiguration]") && allLinesStr.Contains("[database." + dbName + "]") && allLinesStr.Contains("[servergroup." + dbName + ".defaultconfiguration.defaultservergroup]"))
                    {
                        //读取conmgr_properties每一行
                        sr = new StreamReader(conmgr_properties);
                        sw = new StreamWriter(conmgrTemp_properties);
                        bool start_config = false;
                        bool start_database = false;
                        bool start_servergroup = false;
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Contains("[configuration." + dbName + ".defaultconfiguration]"))
                            {
                                sw.WriteLine(line);
                                start_config = true;
                                continue;
                            }

                            if (start_config)
                            {
                                if (line.Contains("[configuration.") && !line.Contains("[configuration." + dbName + ".defaultconfiguration]"))
                                {
                                    start_config = false;
                                    continue;
                                }

                                if (line.Contains("[database.") && !line.Contains("[database." + dbName + "]"))
                                {
                                    start_config = false;
                                    continue;
                                }

                                sw.WriteLine(line);
                                continue;
                            }

                            if (line.Contains("[database." + dbName + "]"))
                            {
                                sw.WriteLine(line);
                                start_database = true;
                                continue;
                            }

                            if (start_database)
                            {
                                if (line.Contains("[database.") && !line.Contains("[database." + dbName + "]"))
                                {
                                    start_database = false;
                                    continue;
                                }

                                if (line.Contains("[servergroup.") && !line.Contains("[servergroup." + dbName + ".defaultconfiguration.defaultservergroup]"))
                                {
                                    start_database = false;
                                    continue;
                                }

                                if (line.Contains("[environment]"))
                                {
                                    start_database = false;
                                    continue;
                                }

                                sw.WriteLine(line);
                                continue;
                            }

                            if (line.Contains("[servergroup." + dbName + ".defaultconfiguration.defaultservergroup]"))
                            {
                                sw.WriteLine(line);
                                start_servergroup = true;
                                continue;
                            }

                            if (start_servergroup)
                            {
                                if (line.Contains("[servergroup.") && !line.Contains("[servergroup." + dbName + ".defaultconfiguration.defaultservergroup]"))
                                {
                                    start_servergroup = false;
                                    continue;
                                }

                                sw.WriteLine(line);
                                continue;
                            }
                        }
                        start_config = false;
                        start_database = false;
                        start_servergroup = false;
                        command = "call " + oe12_2DLC + @"\bin\mergeprop -type database -action update -delta " + conmgrTemp_properties + " -nobackup";
                    }
                    else
                    {
                        MessageBox.Show("DB " + dbName + " does not exist and cannot be migrated");
                        command = "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
                if (sr != null)
                {
                    sr.Close();
                }
            }

            return command;
        }

        private string[] UpgradeDBCommand(string oe12_2DLC, string dbPath)
        {
            string[] commands =
            {
                "call " + oe12_2DLC + @"\bin\117dbutils\117_dbutil.exe " + dbPath + " -C truncate bi",
                "call " + oe12_2DLC + @"\bin\117dbutils\117_dbutil.exe -C probkup " + dbPath + " " + dbPath + "_117.bak",
                "call echo y|" + oe12_2DLC + @"\bin\proutil " + dbPath + " -C conv1112",
                "call " + oe12_2DLC + @"\bin\probkup " + dbPath + " " + dbPath +".bk"
            };
            return commands;
        }

        private string[] MigrateAppserverCommand(string oldOeDLC, string oe12_2DLC, string workDir, string nextHttpPort, string nextHttpsPort, string nextShutdownPort, string uid, string pwd, string instanceType, string ubrokerName, string pasName)
        {
            string oemergeFile = pasName + "." + ubrokerName + ".oemerge";

            string[] commands =
            {
                "call " + oe12_2DLC + @"\bin\pasman create -v -p " + nextHttpPort +" -P " + nextHttpsPort + " -s " + nextShutdownPort + " -m " + uid + ":" + pwd + " -Z " + instanceType + " " + pasName,
                "call " + oe12_2DLC + @"\bin\pasman test -I " + pasName,
                "call " + oe12_2DLC + @"\bin\paspropconv --ubrokerPropsFile " + oldOeDLC + @"\properties\ubroker.properties --ubrokerName UBroker.AS." + ubrokerName + " --pasoeAppName " + pasName,
                "call " + oe12_2DLC + @"\bin\pasman oeprop  -I " + pasName + " -f " + oemergeFile
            };
            return commands;
        }

        private void UpdateOemergeFile(string oemergeFile)
        {
            try
            {
                string[] lines = File.ReadAllLines(oemergeFile);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("updated PROPATH"))
                    {
                        lines[i] = lines[i].Replace("OpenEdge11.7", "OpenEdge12.2");
                    }

                    if (lines[i].Contains("PROPATH="))
                    {
                        lines[i] = lines[i].Replace("OpenEdge11.7", "OpenEdge12.2");
                    }

                    if (lines[i].Contains("sessionConnectProc="))
                    {
                        lines[i] += "pasconnect.p";
                    }

                    if (lines[i].Contains("sessionDisconnProc="))
                    {
                        lines[i] += "pasdisconnect.p";
                    }
                }
                File.WriteAllLines(oemergeFile, lines);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 执行单条命令
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="workdir">工作目录</param>
        /// <param name="createWindow">是否显示执行窗口</param>
        /// <param name="showErrorMsg">是否显示error输出</param>
        /// <returns></returns>
        private string Execute(string command, string workdir, bool createWindow, bool showErrorMsg)
        {
            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "cmd.exe";
            processStartInfo.Arguments = "/C" + command;
            processStartInfo.UseShellExecute = false;
            //processStartInfo.Verb = "RunAs"; //以管理员方式启动
            processStartInfo.RedirectStandardInput = false;
            processStartInfo.CreateNoWindow = createWindow;
            processStartInfo.StandardOutputEncoding = Encoding.UTF8;
            if (!workdir.Equals(""))
            {
                processStartInfo.WorkingDirectory = workdir;
            }

            processStartInfo.RedirectStandardOutput = true;
            //监听输出数据，注册事件处理程序
            process.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
            cmdOutput = new StringBuilder();

            processStartInfo.RedirectStandardError = true;
            //监听错误输出数据
            process.ErrorDataReceived += new DataReceivedEventHandler(CmdErrorDataHandler);
            cmdErrorOutput = new StringBuilder();

            //设置startInfo属性
            process.StartInfo = processStartInfo;

            try
            {
                if (process.Start()) //开始进程
                {
                    //开始监听输出数据
                    process.BeginOutputReadLine();
                    //开始监听错误输出数据
                    process.BeginErrorReadLine();

                    process.WaitForExit();

                    if (cmdErrorOutput.ToString() != "" && showErrorMsg)
                    {
                        MessageBox.Show(cmdErrorOutput.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (process != null)
                {
                    process.Close();
                }
            }
            return cmdOutput.ToString();
        }

        /// <summary>
        /// 可执行多条命令，如果有error跳过剩下的命令返回error message，否则返回所有dos命令的输出
        /// </summary>
        /// <param name="dosCommands">dos 命令数组</param>
        /// <returns>如果有error跳过剩下的命令返回error message，否则返回所有dos命令的输出</returns>
        public string Execute(string[] dosCommands, string workDir, bool createWindow)
        {
            //string output = "";
            //string outputError = "";
            StreamWriter sw = null;

            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "CMD.exe";
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.CreateNoWindow = createWindow;

            if (!workDir.Equals(""))
            {
                processStartInfo.WorkingDirectory = workDir;
            }

            processStartInfo.RedirectStandardOutput = true;
            //监听输出数据，注册事件处理程序
            process.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
            cmdOutput = new StringBuilder();

            processStartInfo.RedirectStandardError = true;
            //监听错误输出数据
            process.ErrorDataReceived += new DataReceivedEventHandler(CmdErrorDataHandler);
            cmdErrorOutput = new StringBuilder();

            //设置startInfo属性
            process.StartInfo = processStartInfo;
            //启动进程
            process.Start();
            //开始监听输出数据
            process.BeginOutputReadLine();
            //开始监听错误输出数据
            process.BeginErrorReadLine();

            try
            {
                sw = process.StandardInput;
                sw.AutoFlush = true;
                foreach (string command in dosCommands)
                {

                    MessageBox.Show(command);
                    sw.WriteLine(command);
                    sw.Flush();
                    //outputError = process.StandardError.ReadToEnd();
                    System.Threading.Thread.Sleep(1000);
                    if (cmdErrorOutput.ToString() != "")
                    {
                        MessageBox.Show(cmdErrorOutput.ToString());
                        break;
                    }
                }
                sw.WriteLine("exit");
                process.WaitForExit();

                //output = process.StandardOutput.ReadToEnd();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {


                if (sw != null)
                {
                    sw.Close();
                }
                if (process != null)
                {
                    process.Close();
                }
            }

            if (cmdErrorOutput.ToString() != "")
            {
                return cmdErrorOutput.ToString();
            }
            else
            {
                return cmdOutput.ToString();
            }
        }

        /// <summary>
        /// 编写OutputDataReceived事件处理程序
        /// </summary>
        /// <param name="sendingProcess"></param>
        /// <param name="outLine"></param>
        private static void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            // Collect the net view command output.
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                // Add the text to the collected output.
                cmdOutput.Append(Environment.NewLine + "  " + outLine.Data);
            }
        }

        private static void CmdErrorDataHandler(object sendingProcess, DataReceivedEventArgs errLine)
        {
            // Write the error text to the string

            if (!String.IsNullOrEmpty(errLine.Data))
            {
                cmdErrorOutput.Append(Environment.NewLine + "  " + errLine.Data);
            }
        }

        private string CreateFile(string currPath)
        {
            //检查是否存在文件夹
            string subPath = currPath + @"\temp\";
            if (false == System.IO.Directory.Exists(subPath))
            {
                //创建pic文件夹
                System.IO.Directory.CreateDirectory(subPath);
            }
            //确认创建文件夹是否成功，如果不成功，则直接在当前目录保存
            if (false == System.IO.Directory.Exists(subPath))
            {
                return currPath + "/" + "executeResult.txt";
            }
            else
            {
                return subPath + "executeResult.txt";
            }
        }

        private void DeleteFile(string file)
        {
            //判断文件是否存在，如果存在，则删除
            if (System.IO.File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }
}
