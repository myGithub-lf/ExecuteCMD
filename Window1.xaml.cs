using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ExecuteCMD
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public CommandBase ExitCommand
        {
            get
            {
                return new CommandBase(OnClick);
            }
        }

        private void OnClick()
        {
            //MyIE.MessageHook += MyIE_MessageHook;
            //MyIE.Navigate("about:blank");
            WebBrowser wb = (WebBrowser)MyIE.Content;
            //mshtml.HTMLDocumentClass document = (mshtml.HTMLDocumentClass)wb.Document;
            MessageBox.Show(System.IO.Path.Combine(System.IO.Path.Combine(@"C:\openedge12_2", "Node01"), "conf"));
        }

        private IntPtr MyIE_MessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 130) //代表请求关闭“网页标签”
            {
                //do something
                MessageBox.Show("close web");
            }
            return hwnd;
        }

        public Window1()
        {
            InitializeComponent();
        }
    }

    public class CommandBase : ICommand
    {
        private readonly Action<object> _commandpara;
        private readonly Action _command;
        private readonly Func<bool> _canExecute;

        public CommandBase(Action command, Func<bool> canExecute = null)
        {
            if (command == null)
            {
                throw new ArgumentNullException();
            }
            _canExecute = canExecute;
            _command = command;
        }

        public CommandBase(Action<object> commandpara, Func<bool> canExecute = null)
        {
            if (commandpara == null)
            {
                throw new ArgumentNullException();
            }
            _canExecute = canExecute;
            _commandpara = commandpara;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (parameter != null)
            {
                _commandpara(parameter);
            }
            else
            {
                if (_command != null)
                {
                    _command();
                }
                else if (_commandpara != null)
                {
                    _commandpara(null);
                }
            }
        }
    }
}
