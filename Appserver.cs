using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ExecuteCMD
{
    class Appserver : INotifyPropertyChanged
    {
        private int appId;
        private bool isCheck;
        private bool isEnable;
        private bool upgraded;
        private string status;

        public int AppId
        {
            get { return appId; }
            set
            {
                appId = value;
                OnPropertyChanged("AppId");
            }
        }
        public bool IsCheck
        {
            get { return isCheck; }
            set
            {
                isCheck = value;
                OnPropertyChanged("IsCheck");
            }
        }

        public bool IsEnable
        {
            get { return isEnable; }
            set
            {
                isEnable = value;
                OnPropertyChanged("IsEnable");
            }
        }

        public bool Upgraded
        {
            get { return upgraded; }
            set
            {
                upgraded = value;
                OnPropertyChanged("Upgraded");
            }
        }
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        public string UbrokerName { get; set; }

        public string RelatedDBPath { get; set; }

        public string WorkFolder { get; set; }

        public string HttpPort { get; set; }

        public string HttpsPort { get; set; }

        public string ShutdownPort { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string InstanceType { get; set; }

        public string InstancePathName { get; set; }

        public string PasName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
